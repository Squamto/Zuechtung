// -----------------------------------------------------------------------
// <copyright file="PowerModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.Power
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO.Ports;
    using System.Threading;
    using Config;
    using Model.SerialPort;

    public class PowerModel
    {
        private readonly int port;
        private readonly SerialPortReconnector serialPort;
        private readonly double[] uSoll;
        private readonly double[] pSoll;
        private double[] uOutFaktor;
        private double[] uOutOffset;
        private double[] pOutR;
        private bool isAlarm;
        private bool isExternalAlarm;

        public PowerModel()
        {
            this.port = Conf.I.GetValueAsInt(ConfigNames.ValDevicePower(AreaPowerConfig.ADAMPort));
            this.uSoll = new double[7];
            this.pSoll = new double[7];
            this.IsManualValues = new bool[7];
            if (this.IsExistComPort)
            {
                this.serialPort = new SerialPortReconnector(port);
                this.serialPort.SerialPortAlarmOccured += this.OnSerialPortAlarmOccured;
                this.serialPort.CompleteReinitNeeded += this.OnSerialPortCompleteReinitNeeded;
            }
            this.UIMeasurement = new UIMeasurement(this.serialPort);
            this.SetConfigValues();
        }

        public event EventHandler CompleteReinitNeeded;

        public event EventHandler ConfigParameterChanged;

        public bool IsExistComPort => this.port > 0;

        public UIMeasurement UIMeasurement { get; }

        public bool[] IsManualValues { get; }

        public bool IsOutputSetToZero => this.isAlarm || this.isExternalAlarm;

        public void Init()
        {
            if (this.serialPort != null)
            {
                try
                {
                    this.serialPort.Open();
                    this.serialPort.WriteLine("$01M");
                    string read = this.serialPort.ReadLine();
                    if (!read.Contains("!015000"))
                    {
                        Global.UserMsg($"Serial port {this.serialPort.PortName} fail response to command \"$01M\" Response:{read}");
                        return;
                    }

                    //this.ConfigOutputFormat(0, 0);
                    //this.ConfigOutputFormat(0, 1);
                    //this.ConfigOutputFormat(0, 2);
                    //this.ConfigOutputFormat(0, 3);
                    //this.ConfigOutputFormat(1, 0);
                    //this.ConfigOutputFormat(1, 1);
                    //this.ConfigOutputFormat(1, 2);
                }
                catch (Exception exception)
                {
                    Global.UserMsg(exception);
                }
            }
        }

        public void SetConfigValues()
        {
            this.uOutFaktor = new double[7]
            {
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUFaktor,ZoneNames.Zone1)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUFaktor,ZoneNames.Zone2)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUFaktor,ZoneNames.Zone3)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUFaktor,ZoneNames.Zone4)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUFaktor,ZoneNames.Zone5)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUFaktor,ZoneNames.Zone6)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUFaktor,ZoneNames.Zone7)),
            };
            this.uOutOffset = new double[7]
            {
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUOffset,ZoneNames.Zone1)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUOffset,ZoneNames.Zone2)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUOffset,ZoneNames.Zone3)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUOffset,ZoneNames.Zone4)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUOffset,ZoneNames.Zone5)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUOffset,ZoneNames.Zone6)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.PowerUOffset,ZoneNames.Zone7)),
            };
            this.pOutR = new double[7]
            {
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.Wiederstand,ZoneNames.Zone1)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.Wiederstand,ZoneNames.Zone2)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.Wiederstand,ZoneNames.Zone3)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.Wiederstand,ZoneNames.Zone4)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.Wiederstand,ZoneNames.Zone5)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.Wiederstand,ZoneNames.Zone6)),
                Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.Wiederstand,ZoneNames.Zone7)),
            };
            this.UIMeasurement.SetConfigValues();
            this.ConfigParameterChanged?.Invoke(this, new EventArgs());
        }

        public double GetPSoll(int channel)
        {
            if ((channel >= 0) && (channel < 7))
            {
                return this.pSoll[channel];
            }
            return 0.0;
        }

        public double GetUSoll(int channel)
        {
            if ((channel >= 0) && (channel < 7))
            {
                return this.uSoll[channel];
            }
            return 0.0;
        }

        public void SetIsManual(int channel, double value, bool isManual)
        {
            this.IsManualValues[channel] = isManual;
            this.pSoll[channel] = value;
        }

        public void SetAllManualOff()
        {
            for (int i = 0; i < this.IsManualValues.Length; i++)
            {
                this.IsManualValues[i] = false;
            }
        }

        public void WritePower(int slot, int channel, double power)
        {
            if ((this.serialPort != null) && (!this.serialPort.CheckWaitForReconnect()))
            {
                try
                {
                    // Analog Data Out
                    // Sendet einen digitalen Wert vom Hostcomputer an einen angegebenen Kanal eines angegebenen Steckplatzes in einem angegebenen ADAM System
                    // zur Ausgabe als analoges Signal.
                    // Nach dem Empfang gibt das analoge Ausgabemodul im angegebenen Steckplatz ein analoges Signal aus, das dem empfangenen digitalen Wert entspricht.
                    //
                    // #aaSiCj(data)(cr)
                    //
                    // # is a delimiter character.
                    // aa(range 00 - FF) represents the 2 - character hexadecimal Modbus network address of the ADAM system.
                    // SiCj identifies the I/ O slot i(i : 0 to 7) and the channel j(j: 0 to 3) of the analog output module that is to output an analog signal.
                    // (data) is a digital value incoming to the module, which corresponds to the desired analog output value (always in engineering units)
                    // to be output from the module.
                    // The analog value output will depend on the module’s range configuration.
                    // Format for Output Range 0..10V: Minimum Value:00.000  MaximumValue:10.000
                    // (cr) is the terminating character, carriage return (0Dh)
                    int zone = PowerModel.APos(slot, channel);
                    double powerOut = power;
                    if (this.IsManualValues[zone])
                    {
                        powerOut = this.pSoll[zone];
                    }
                    else
                    {
                        this.pSoll[zone] = power;
                    }
                    if (this.isAlarm || this.isExternalAlarm)
                    {
                        powerOut = 0.0;
                        this.pSoll[zone] = 0.0;
                    }

                    // U = SQRT(P * R) => P = U * U / R => U = R * I, P = U * I
                    double voltage = Math.Sqrt(powerOut * this.pOutR[zone]);
                    this.uSoll[zone] = voltage;
                    voltage = voltage * this.uOutFaktor[zone] + this.uOutOffset[zone];
                    string command = $"#0{slot + 1}S0C{channel}{voltage.ToString("F3", CultureInfo.InvariantCulture).PadLeft(6, '0')}";
                    this.serialPort.WriteLine(command);
                    string read = this.serialPort.ReadLine();
                    this.CheckResponse(read, ">");
                }
                catch (Exception exception)
                {
                    Global.UserMsg(exception);
                }
            }
        }

        private void ConfigOutputFormat(int slot, int channel)
        {
            // $aaSiCjArrff
            // Configuration
            // Sets the output range, data format and slew rate for a specified channel of a specified analog output module in a specified system.
            // Syntax: $aaSiCjArrff(cr)
            //$ is a delimiter character.
            // aa(range 00 - FF) represents the 2 - character hexadecimal address of the ADAM system you want to configure.
            // SiCj identifies the I / O slot i(i: 0 to 7) and the channel j(j: 0 to 3) of the module you want to configure.
            // A is I / O module configuration command.
            // rr represents the 2 - character hexadecimal code of the output range. 32 means 0..10V
            // ff is a hexadecimal number that equals the 8 - bit parameter representing the status of data format and slew rate.
            // Bits 0 and 1 represent data format (00 Engineering Units)
            // Bits 2, 3, 4, 5 represent slew rate.
            // Bit 7 is the integration time (0 -> 50ms)
            // The other bits are not used and are set to 0.
            // (cr) is the terminating character, carriage return (0Dh)
            string command = $"$01S{slot}C{channel}A3200";
            this.serialPort.WriteLine(command);
            string read = this.serialPort.ReadLine();
            this.CheckResponse(read, "!01");
            Thread.Sleep(20);
        }

        static public int APos(int slot, int channel)
        {
            if (slot == 1)
            {
                return channel + 4;
            }
            return channel;
        }

        static public Tuple<int, int> Sc(int apos)
        {
            if (apos < 4)
            {
                return new Tuple<int, int>( 0, apos);
            }
            return new Tuple<int, int>(1, apos-4);
        }

        public void SetAlarm(bool isAlarm)
        {
            this.isExternalAlarm = isAlarm;
        }

        public void InjectError()
        {
            this.serialPort.ErrorInjection();
        }

        internal bool CheckResponse(string response, string expected)
        {
            if (response.Contains(expected))
            {
                return true;
            }
            else
            {
                Global.UserMsg($"Serial port {this.serialPort?.PortName} fail response: Expected:{expected} Response:{response}");
                return false;
            }
        }

        private void OnSerialPortAlarmOccured(object? sender, bool e)
        {
            this.isAlarm = e;
        }

        private void OnSerialPortCompleteReinitNeeded(object? sender, EventArgs e)
        {
            this.CompleteReinitNeeded?.Invoke(sender, e);
        }
    }
}
