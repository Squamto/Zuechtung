// -----------------------------------------------------------------------
// <copyright file="FgChannel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.FG
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web;
    using Config;
    using Framework.Helper;
    using Model.Power;
    using Model.Smartlink;

    public class FgChannel
    {
        private double currentSetpoint;
        private double currentTemperature;
        private double safetyTemperature;
        private double currentPower;
        private double currentTemperatureDifference;
        private double lastTemperatureDifference;
        private readonly Simulation? simu;
        private readonly SmartlinkModel smartlinkModel;
        private readonly bool useHardwareMainTemperature;
        private readonly bool useHardwareSafetyTemperature;
        private readonly PowerModel powerModel;
        private readonly MainModel mainModel;
        private DigitalFilter filterCurrentTemperature;
        private DigitalFilter filterSetpointGradient;

        // Konfig Daten
        private double pidKInWattPerGrad;
        private double pidK2InWattPerGrad;
        private double pidTnInSeconds;
        private double pidIPartAllowedTemperatureDifference;
        private double alarmTemperature;
        private double cutoffTemperature;
        private int channelMainTemperature;
        private int channelSafetyTemperature;
        private int powerSlot;
        private int PowerChannel;
        private double maxPowerOutput;
        private double maxPowerOutputTemperature;
        private double maxPowerOutputWhen0Grad;
        private double prefactorInWPerGrad;
        private double setpointIncreaseIPartAllowed;
        private double lastSetpoint;

        public FgChannel(ZoneNames name, MainModel mainModel)
        {
            this.mainModel = mainModel;
            this.smartlinkModel = mainModel.SmartlinkModel;
            this.powerModel = mainModel.PowerModel;
            this.mainModel.IsConnectededChanged += this.OnMainModelIsConnectededChanged;
            this.Steps = [];
            this.Zone = name;
            this.useHardwareMainTemperature = this.smartlinkModel.NeedSimulation(this.channelMainTemperature);
            this.useHardwareSafetyTemperature = this.smartlinkModel.NeedSimulation(this.channelSafetyTemperature);
            if (!this.useHardwareMainTemperature || !this.useHardwareSafetyTemperature)
            {
                switch (this.Zone)
                {
                    case ZoneNames.Zone1:
                        this.simu = new Simulation(1, 120, 20, 75);
                        break;
                    case ZoneNames.Zone2:
                        this.simu = new Simulation(2, 150, 20, 70);
                        break;
                    case ZoneNames.Zone3:
                        this.simu = new Simulation(3, 150, 20, 80);
                        break;
                    case ZoneNames.Zone4:
                        this.simu = new Simulation(4, 150, 20, 60);
                        break;
                    case ZoneNames.Zone5:
                        this.simu = new Simulation(5, 150, 20, 50);
                        break;
                    case ZoneNames.Zone6:
                        this.simu = new Simulation(6, 150, 20, 40);
                        break;
                    case ZoneNames.Zone7:
                        this.simu = new Simulation(7, 150, 20, 30);
                        break;
                }
            }
        }

        public event EventHandler<double>? CurrentSetpointChanged;

        public event EventHandler<double>? CurrentTemperatureChanged;

        public event EventHandler<double>? SafetyTemperatureChanged;

        public event EventHandler<double>? CurrentPowerChanged;

        public event EventHandler<double>? CurrentTemperatureDifferenceChanged;

        public event EventHandler<bool>? TemperatureAlarmOccured;

        public event EventHandler<bool>? TemperatureShutoffOccured;

        public List<StepChannel> Steps { get; private set; }

        public ZoneNames Zone { get; }

        public string Name => Zone.ToString();

        public double CurrentSetpoint
        {
            get => this.currentSetpoint;
            private set
            {
                this.currentSetpoint = value;
                this.CurrentSetpointChanged?.Invoke(this, value);
            }
        }

        public double CurrentTemperature
        {
            get => this.currentTemperature;
            private set
            {
                double fT = this.filterCurrentTemperature.Step(value);
                this.currentTemperature = fT;
                this.CurrentTemperatureChanged?.Invoke(this, fT);
            }
        }

        public double SafetyTemperature
        {
            get => this.safetyTemperature;
            private set
            {
                this.safetyTemperature = value;
                this.SafetyTemperatureChanged?.Invoke(this, value);
            }
        }

        public double CurrentPower
        {
            get => this.currentPower;
            private set
            {
                if (this.powerModel.IsOutputSetToZero)
                {
                    this.currentPower = 0.0;
                }
                else
                {
                    this.currentPower = value;
                }
                this.CurrentPowerChanged?.Invoke(this, this.currentPower);
            }
        }

        public double CurrentTemperatureDifference
        {
            get => this.currentTemperatureDifference;
            private set
            {
                this.currentTemperatureDifference = value;
                this.CurrentTemperatureDifferenceChanged?.Invoke(this, value);
            }
        }

        public double MaxP { get; private set; }

        public double IRegler { get; private set; }

        public double IPuffer { get; private set; }

        public double PRegler { get; private set; }

        public double Vorregler { get; private set; }

        public bool IsAlarm => (this.CurrentTemperature > this.alarmTemperature) || (this.SafetyTemperature > this.alarmTemperature);

        public bool IsAbschalt => (this.CurrentTemperature > this.cutoffTemperature) || (this.SafetyTemperature > this.cutoffTemperature);

        public double ManualSetpoint { get; set; }

        public bool IsManualSetpointMode { get; set; }

        public void Start(List<StepChannel> steps)
        {
            this.Steps = steps;
        }

        public void ReadTemperature()
        {
            if (this.useHardwareMainTemperature)
            {
                this.CurrentTemperature = this.smartlinkModel.Meas(this.channelMainTemperature);
            }
            else
            {
                this.CurrentTemperature = this.simu.ReadTemperature();
            }
            if (this.useHardwareSafetyTemperature)
            {
                this.SafetyTemperature = this.smartlinkModel.Meas(this.channelSafetyTemperature);
            }
            else
            {
                this.SafetyTemperature = this.simu.ReadTemperature(); 
            }
            if (this.powerModel.IsExistComPort)
            {
                this.powerModel.WritePower(this.powerSlot, this.PowerChannel, this.CurrentPower);
            }
            else
            {
                this.simu?.WritePower(this.CurrentPower);
            }
        }

        public void Cycle(int step, int cycle)
        {
            if (this.IsManualSetpointMode)
            {
                this.CurrentSetpoint = this.ManualSetpoint;
            }
            else
            {
                this.CurrentSetpoint = this.CalculateCurrentSetpoint(step, cycle);
            }
            this.CurrentPower = this.Pid();

            if (this.IsAlarm)
            {
                this.TemperatureAlarmOccured?.Invoke(this, this.IsAlarm);
                Global.UserMsg($"Alarmtemeratur in {this.Zone} überschritten!");
            }
            if (this.IsAbschalt)
            {
                this.TemperatureShutoffOccured?.Invoke(this, true);
                Global.UserMsg($"Abschalttempertaur in {this.Zone} überschritten! Programm wird gestoppt! Leistung wird ausgeschaltet!");
            }
        }

        public void SetConfigValues()
        {
            this.filterCurrentTemperature = new DigitalFilter(Conf.I.GetValueAsDouble(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.FilterCoeffCurrentTemperature, this.Zone)), true);
            this.filterSetpointGradient = new DigitalFilter(Conf.I.GetValueAsDouble(ConfigNames.ValDevicePid(AreaPidConfig.FilterCoeffSetpointGradient, this.Zone)), true);
            this.pidKInWattPerGrad = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePid(AreaPidConfig.PidKInWattPerGrad, this.Zone));
            this.pidK2InWattPerGrad = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePid(AreaPidConfig.PidK2InWattPerGrad, this.Zone));
            this.pidTnInSeconds = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePid(AreaPidConfig.PidTnInSeconds, this.Zone));
            this.pidIPartAllowedTemperatureDifference = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePid(AreaPidConfig.IPartAllowedTemperatureDifference, this.Zone));
            this.setpointIncreaseIPartAllowed = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePid(AreaPidConfig.SetpointIncreaseIPartAllowed, this.Zone));
            this.alarmTemperature = Conf.I.GetValueAsDouble(ConfigNames.ValDeviceZone(AreaZoneConfig.AlarmTemperature, this.Zone));
            this.cutoffTemperature = Conf.I.GetValueAsDouble(ConfigNames.ValDeviceZone(AreaZoneConfig.CutoffTemperature, this.Zone));
            this.channelMainTemperature = Conf.I.GetValueAsInt(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.ChannelMainTemperature, this.Zone));
            this.channelSafetyTemperature = Conf.I.GetValueAsInt(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.ChannelSafetyTemperature, this.Zone));
            this.powerSlot = Conf.I.GetValueAsInt(ConfigNames.ValDevicePower(AreaPowerConfig.PowerSlot, this.Zone));
            this.PowerChannel = Conf.I.GetValueAsInt(ConfigNames.ValDevicePower(AreaPowerConfig.PowerChannel, this.Zone));
            this.maxPowerOutput = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MaxPowerOutput, this.Zone));
            this.maxPowerOutputTemperature = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MaxPowerOutputTemperature, this.Zone));
            this.maxPowerOutputWhen0Grad = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.MaxPowerOutputWhen0Grad, this.Zone));
            this.prefactorInWPerGrad = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePid(AreaPidConfig.PrefactorInWPerGrad, this.Zone));
        }

        public string LogString()
        {
            string rv =
                this.CurrentSetpoint.ToString("F5") + "\t" +
                this.CurrentTemperature.ToString("F5") + "\t" +
                this.CurrentPower.ToString("F5") + "\t";
            return rv;
        }

        internal void PowerOff()
        {
            this.powerModel.SetAllManualOff();
            this.CurrentSetpoint = 0.0;
            this.CurrentPower = 0.0;
        }

        private double Pid()
        {
            // Regler
            // Diff
            this.CurrentTemperatureDifference = this.CurrentTemperature - this.CurrentSetpoint;
            double diff2Val = this.CurrentTemperatureDifference * this.CurrentTemperatureDifference;
            if (this.CurrentTemperatureDifference < 0)
            {
                diff2Val = -diff2Val;
            }

            // Max Power
            double mMaxPower = (this.maxPowerOutput - this.maxPowerOutputWhen0Grad) / this.maxPowerOutputTemperature;
            double mP = (this.CurrentTemperature * mMaxPower) + this.maxPowerOutputWhen0Grad;
            mP = Math.Min(mP, this.maxPowerOutput);
            this.MaxP = Math.Round((mP) * 100.0) / 100.0;


            // PID
            double setpointGradient = 0.0;
            if (this.lastSetpoint > 0)
            {
                setpointGradient = Math.Abs(this.filterSetpointGradient.Step(this.CurrentSetpoint - this.lastSetpoint));
            }
            Debug.Write($"\t{setpointGradient}");
            if (Math.Abs(this.CurrentTemperatureDifference) < Math.Abs(this.pidIPartAllowedTemperatureDifference))
            {
                if (setpointGradient < this.setpointIncreaseIPartAllowed)
                {
                    Debug.Write($" I");
                    double mw = -(this.lastTemperatureDifference + this.CurrentTemperatureDifference) / 2.0;
                    double mwTime = mw;
                    double iPartRoh = mwTime / this.pidTnInSeconds;
                    this.IRegler = Math.Round(iPartRoh * 100000.0) / 100000.0;
                    double ip = Math.Round((this.IPuffer + this.IRegler) * 100000.0) / 100000.0;
                    ip = Math.Max(ip, 0);
                    this.IPuffer = ip;
                }
                Debug.Write($" N");
            }
            else
            {
                if (this.currentTemperatureDifference < 0.0)
                {
                    this.IPuffer = 0.0;
                }
            }
            this.lastSetpoint = this.CurrentSetpoint;

            this.PRegler = -((this.pidKInWattPerGrad * this.CurrentTemperatureDifference) + (this.pidK2InWattPerGrad * diff2Val));
            this.Vorregler = this.prefactorInWPerGrad * this.CurrentSetpoint;

            // Outout power (W)
            double stellwert = Math.Round((this.IPuffer + this.PRegler + this.Vorregler) * 1000.0) / 1000.0;
            stellwert = Math.Min(Math.Max(stellwert, 0.0), this.MaxP);

            // I-Puffer
            this.IPuffer = Math.Min(Math.Max(this.IPuffer, -this.MaxP), this.MaxP);

            // Save last temperature difference
            this.lastTemperatureDifference = this.CurrentTemperatureDifference;

            // Result = power (W)
            return stellwert;
        }

        private double CalculateCurrentSetpoint(int step, int cycle)
        {
            double n = this.Steps[step].TargetTemp;
            double m = (this.Steps[step + 1].TargetTemp - n) / this.Steps[step].Cycles;
            double rv = m * cycle + n;
            return rv;
        }

        private void OnMainModelIsConnectededChanged(object? sender, bool e)
        {
            this.filterCurrentTemperature.SetOldValue(this.smartlinkModel.Meas(this.channelMainTemperature));
        }
    }
}
