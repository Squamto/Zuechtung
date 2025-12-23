// -----------------------------------------------------------------------
// <copyright file="FgChannels.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model.FG
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Config;
    using Framework;
    using Model.Log;
    using Model.Power;
    using Model.Smartlink;
    using static System.Net.Mime.MediaTypeNames;

    public class FgChannels
    {
        private ControlStates controlState;
        private int currentStep;
        private int currentCycle;
        private readonly SmartlinkModel smartlinkModel;
        private readonly PowerModel powerModel;
        private DateTime timeStart;
        private DateTime timeEnd;
        private DateTime timeStartPause;
        private TimeSpan pauseTime;
        private string fileName;
        private SamplerFile samplerFile;
        private SamplerRun samplerRun;

        public FgChannels(MainModel mainmodel)
        {
            this.smartlinkModel = mainmodel.SmartlinkModel;
            this.powerModel = mainmodel.PowerModel;
            this.Channels = [];
            this.ControlState = ControlStates.Stop;
            this.samplerFile = new SamplerFile();
            this.samplerRun = new SamplerRun();
            this.Channels.Add(new FgChannel(ZoneNames.Zone1, mainmodel));
            this.Channels.Add(new FgChannel(ZoneNames.Zone2, mainmodel));
            this.Channels.Add(new FgChannel(ZoneNames.Zone3, mainmodel));
            this.Channels.Add(new FgChannel(ZoneNames.Zone4, mainmodel));
            this.Channels.Add(new FgChannel(ZoneNames.Zone5, mainmodel));
            this.Channels.Add(new FgChannel(ZoneNames.Zone6, mainmodel));
            this.Channels.Add(new FgChannel(ZoneNames.Zone7, mainmodel));
            this.SetConfigValues();
            foreach (FgChannel channel in this.Channels)
            {
                channel.TemperatureShutoffOccured += this.OnChannelTemperatureShutoffOccured;
            }
            this.Steps = [];
        }

        public event EventHandler? StepsChanged;

        public event EventHandler<int>? CurrentStepChanged;

        public event EventHandler<int>? CurrentCycleChanged;

        public event EventHandler<ControlStates>? ControlStateChanged;

        public List<FgChannel> Channels { get; }

        public List<StepChannels> Steps { get; private set; }

        public ControlStates ControlState
        {
            get => this.controlState;
            private set
            {
                this.controlState = value;
                this.ControlStateChanged?.Invoke(this, value);
            }
        }

        public int CurrentStep
        {
            get => this.currentStep;
            private set
            {
                this.currentStep = value;
                this.CurrentStepChanged?.Invoke(this, value);
            }
        }

        public int CurrentCycle
        {
            get => this.currentCycle;
            private set
            {
                this.currentCycle = value;
                this.CurrentCycleChanged?.Invoke(this, value);
            }
        }

        public ISampler Sampler
        {
            get
            {
                if (this.ControlState == ControlStates.Stop)
                {
                    return this.samplerFile;
                }
                else
                {
                    return this.samplerRun;
                }
            }
        }

        public void Start()
        {
            this.ControlState = ControlStates.Run;
            this.timeStart = DateTime.Now;
            this.timeEnd = this.timeStart;
            string logPath = Conf.I.GetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LogTemperaturePowerPath));
            this.fileName = Path.Combine(
                logPath,
                    "TemPow_" +
                    timeStart.Year.ToString().PadLeft(2, '0') +
                    timeStart.Month.ToString().PadLeft(2, '0') +
                    timeStart.Day.ToString().PadLeft(2, '0') +
                    timeStart.Hour.ToString().PadLeft(2, '0') +
                    timeStart.Minute.ToString().PadLeft(2, '0') +
                    timeStart.Second.ToString().PadLeft(2, '0') +
                    ".txt");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            this.samplerRun.CurrentLog.Clear();
            this.timeStart = DateTime.Now; 
            this.CurrentCycle = 0;
            this.CurrentStep = 0;
            foreach (FgChannel channel in this.Channels)
            {
                channel.Steps.Clear();
            }
            foreach (StepChannels step in this.Steps)
            {
                this.AddStepToChannel(step, ZoneNames.Zone1);
                this.AddStepToChannel(step, ZoneNames.Zone2);
                this.AddStepToChannel(step, ZoneNames.Zone3);
                this.AddStepToChannel(step, ZoneNames.Zone4);
                this.AddStepToChannel(step, ZoneNames.Zone5);
                this.AddStepToChannel(step, ZoneNames.Zone6);
                this.AddStepToChannel(step, ZoneNames.Zone7);
                this.timeEnd += TimeSpan.FromSeconds(step.Cycles);
            }
            this.timeStartPause = DateTime.MaxValue;
            this.pauseTime = TimeSpan.FromSeconds(0);
        }

        public void Stop()
        {
            this.ControlState = ControlStates.RequestStop;
        }

        public void StopAndHold()
        {
            this.ControlState = ControlStates.RequestStopAndHold;
        }

        public void Pause()
        {
            if (this.ControlState == ControlStates.Run)
            {
                this.ControlState = ControlStates.Pause;
                this.timeStartPause = DateTime.Now;
            }
        }

        public void Further()
        {
            if (this.ControlState == ControlStates.Pause)
            {
                this.timeEnd = this.timeStart;
                foreach (StepChannels step in this.Steps)
                {
                    this.timeEnd += TimeSpan.FromSeconds(step.Cycles);
                }
                this.pauseTime += (DateTime.Now - this.timeStartPause);
                this.timeEnd += this.pauseTime;
                this.ControlState = ControlStates.Run;
            }
        }

        public void SetConfigValues()
        {
            foreach (FgChannel channel in this.Channels)
            {
                channel.SetConfigValues();
            }
        }


        public void StepsRefreshed()
        {
            this.StepsChanged?.Invoke(this, EventArgs.Empty);
        }

        public TimeSpan GetRunTime()
        {
            return (DateTime.Now - this.timeStart);
        }

        public TimeSpan GetTimeToEnd()
        {
            return (this.timeEnd - DateTime.Now);
        }

        private void AddStepToChannel(StepChannels step, ZoneNames name)
        {
            FgChannel fgChannel = this.Channels.First(o => o.Zone == name);
            StepChannel stepChannel = new(step.Cycles, step.TargetTemps[name]);
            fgChannel.Steps.Add(stepChannel);
        }

        public void Cycle()
        {
            try
            {
                foreach (FgChannel channel in this.Channels)
                {
                    channel.ReadTemperature();
                }
                if (this.powerModel.IsExistComPort)
                {
                    this.powerModel.UIMeasurement.ReadUI();
                }

                switch (this.ControlState)
                {
                    case ControlStates.None:
                        break;
                    case ControlStates.Stop:
                        break;
                    case ControlStates.RequestStop:
                        foreach (FgChannel channel in this.Channels)
                        {
                            channel.PowerOff();
                        }
                        this.ControlState = ControlStates.Stop;
                        this.samplerFile.Refresh();
                        break;
                    case ControlStates.RequestStopAndHold:
                        this.samplerFile.Refresh();
                        this.ControlState = ControlStates.Stop;
                        break;
                    case ControlStates.Pause:
                        this.CurrentCycle = this.CurrentCycle;
                        this.Sample();
                        break;
                    case ControlStates.Run:
                        if ((this.Steps.Count < 2) || (this.CurrentStep < 0) || (this.CurrentStep > this.Steps.Count - 2))
                        {
                            this.ControlState = ControlStates.RequestStop;
                            return;
                        }

                        if (this.CurrentCycle < this.Steps[this.CurrentStep].Cycles)
                        {
                            if (this.ControlState == ControlStates.Run)
                            {
                                this.CurrentCycle++;
                            }
                        }
                        else
                        {
                            if (this.CurrentStep < this.Steps.Count - 2)
                            {
                                this.CurrentStep++;
                                this.CurrentCycle = 0;
                            }
                            else
                            {
                                this.ControlState = ControlStates.RequestStop;
                            }
                        }
                        this.Sample();
                        break;
                }
            }
            catch (Exception exception)
            {
                Global.UserMsg(exception);
            }
        }

        private void Sample()
        {
            string logText = $"{DateTime.Now}\t";
            foreach (FgChannel channel in this.Channels)
            {
                channel.Cycle(this.CurrentStep, this.CurrentCycle);
                logText += channel.LogString();
            }
            logText += "\r\n";
            File.AppendAllText(this.fileName, logText);
            this.samplerRun.CurrentLog.Add(logText);
            Debug.WriteLine("");
        }

        private void OnChannelTemperatureShutoffOccured(object sender, bool e)
        {
            this.Stop();
        }
    }
}
