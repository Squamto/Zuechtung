// -----------------------------------------------------------------------
// <copyright file="MainModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Config;
    using Framework;
    using Framework.Helper;
    using Model.FG;
    using Model.Log;
    using Model.Power;
    using Model.Smartlink;

    public class MainModel
    {
        public DateTime nextCycle;
        private bool isInitialized;
        private bool isReinitRunning;
        private bool requestStop;
        private bool isLoopRunning;
        private bool isConnected;

        public MainModel()
        {
            this.SmartlinkModel = new SmartlinkModel();
            this.PowerModel = new PowerModel();
            this.Channels = new FgChannels(this);
            this.nextCycle = DateTime.MaxValue;
            foreach (NameValueItem item in Conf.I.Values)
            {
                Global.LogInfo(LogCategories.Config, item.Name, item.Value);
            }
            this.SmartlinkModel.AlarmOccured += this.OnSmartlinkModelAlarmOccured;
            this.SmartlinkModel.CompleteReinitNeeded += this.OnCompleteReinitNeeded;
            this.PowerModel.CompleteReinitNeeded += this.OnCompleteReinitNeeded;
            Task.Factory.StartNew(this.Run);
        }

        public event EventHandler<bool> IsConnectededChanged;

        public SmartlinkModel SmartlinkModel { get; }

        public PowerModel PowerModel { get; }

        public FgChannels Channels { get; }

        public ISampler Sampler => this.Channels.Sampler;

        private void Run()
        {
            Thread.Sleep(100);
            this.requestStop = false;
            if (!this.isInitialized)
            {
                try
                {
                    this.isInitialized = true;
                    this.SmartlinkModel.Init();
                    this.PowerModel.Init();
                }
                catch (Exception exception)
                {
                    Global.UserMsg(exception);
                }
            }
            this.isLoopRunning = true;
            this.nextCycle = DateTime.Now;
            while (this.isLoopRunning)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    TimeSpan delta = now - this.nextCycle;
                    if(delta >= TimeSpan.Zero)
                    {
                        if(delta > TimeSpan.FromSeconds(2)) {
                            Global.LogInfo(LogCategories.Always, "TimeControl", $"Big time Difference detected (nextCycle: {this.nextCycle}, delta: {delta}), resetting next Cycle to now + 1s");
                            Global.UserMsg(
                                "Es wurde eine große Zeitdifferenz zum letzten Cycle festgestellt. " +
                                "Bitte stellen Sie sicher, dass der Rechner nicht überlastet ist und nicht in den Standby-Modus wechselt"
                                );
                            this.nextCycle = now;
                        }
                        this.nextCycle = this.nextCycle.AddSeconds(1);
                        this.Channels.Cycle();
                    }
                    if (this.requestStop)
                    {
                        this.isConnected = false;
                        this.IsConnectededChanged?.Invoke(this, this.isConnected);
                        this.isLoopRunning = false;
                    }
                    if(!this.isConnected && this.SmartlinkModel.IsConnectd)
                    {
                        this.isConnected = true;
                        this.IsConnectededChanged?.Invoke(this, this.isConnected);
                    }
                }
                catch (Exception exception)
                {
                    Global.UserMsg(exception);
                }
            }
        }

        public void RefreshConfigValues()
        {
            this.Channels.SetConfigValues();
            this.PowerModel.SetConfigValues();
        }

        private void ReinitInBackground()
        {
            try
            {
                this.requestStop = true;
                this.SmartlinkModel.Smartlink1?.Stop();
                this.SmartlinkModel.Smartlink2?.Stop();
                this.SmartlinkModel.Smartlink3?.Stop();
                while (this.SmartlinkModel.Smartlink1.IsRunning
                    && this.SmartlinkModel.Smartlink2.IsRunning
                    && this.SmartlinkModel.Smartlink3.IsRunning
                    && this.isLoopRunning)
                {
                    Thread.Sleep(1000);
                }
                this.isInitialized = false;
                Thread.Sleep(10000);
                Task.Factory.StartNew(this.Run);
            }
            catch (Exception exception)
            {
                Global.UserMsg(new Exception("Schwerer Fehler!!! Reinitialisierung aller Komponenten, konnte nicht erfolgreich durchgeführt werden!", exception));
            }
            finally
            {
                this.isReinitRunning = false;
            }
        }

        private void OnSmartlinkModelAlarmOccured(object? sender, bool e)
        {
            this.PowerModel.SetAlarm(e);
        }

        private void OnCompleteReinitNeeded(object? sender, EventArgs e)
        {
            if (this.isReinitRunning)
            {
                return;
            }
            this.isReinitRunning = true;
            Task.Factory.StartNew(this.ReinitInBackground);
        }
    }
}
