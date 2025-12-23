// -----------------------------------------------------------------------
// <copyright file="SmartlinkModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Threading.Channels;
using System.Windows.Documents;
using Config;
using Model.SerialPort;

namespace Model.Smartlink
{
    public class SmartlinkModel
    {
        private readonly int port1;
        private readonly int port2;
        private readonly int port3;
        private bool isSM1Alarm;
        private bool isSM2Alarm;
        private bool isSM3Alarm;

        public SmartlinkModel()
        {
            this.port1 = Conf.I.GetValueAsInt(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.Smartlink1Exist));
            this.port2 = Conf.I.GetValueAsInt(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.Smartlink2Exist));
            this.port3 = Conf.I.GetValueAsInt(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.Smartlink3Exist));
            if (this.port1 > 0)
            {
                this.Smartlink1 = new Smartlink(this.port1, Conf.I.GetValue(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.Smartlink1ThermoelementTypes)).Split(','));
                this.Smartlink1.AlarmOccured += this.OnSmartlink1AlarmOccured;
                this.Smartlink1.CompleteReinitNeeded += this.OnCompleteReinitNeeded;
            }
            if (this.port2 > 0)
            {
                this.Smartlink2 = new Smartlink(this.port2, Conf.I.GetValue(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.Smartlink2ThermoelementTypes)).Split(','));
                this.Smartlink2.AlarmOccured += this.OnSmartlink2AlarmOccured;
                this.Smartlink2.CompleteReinitNeeded += this.OnCompleteReinitNeeded;
            }
            if (this.port3 > 0)
            {
                this.Smartlink3 = new Smartlink(this.port3, Conf.I.GetValue(ConfigNames.ValDeviceSmartlink(AreaSmartlinkConfig.Smartlink3ThermoelementTypes)).Split(','));
                this.Smartlink3.AlarmOccured += this.OnSmartlink3AlarmOccured;
                this.Smartlink3.CompleteReinitNeeded += this.OnCompleteReinitNeeded;
            }
        }

        public event EventHandler<bool> AlarmOccured;

        public event EventHandler CompleteReinitNeeded;

        public Smartlink? Smartlink1 { get; }

        public Smartlink? Smartlink2 { get; }

        public Smartlink? Smartlink3 { get; }

        public bool IsConnectd =>
            ((this.Smartlink1 == null) || this.Smartlink1.IsInitialized)
            && ((this.Smartlink2 == null) || this.Smartlink2.IsInitialized)
            && ((this.Smartlink3 == null) || this.Smartlink3.IsInitialized);

        public void Init()
        {
            try
            {
                this.Smartlink1?.Init();
                this.Smartlink2?.Init();
                this.Smartlink3?.Init();
                this.Smartlink1?.Start();
                this.Smartlink2?.Start();
                this.Smartlink3?.Start();
            }
            catch (Exception exception)
            {
                Global.UserMsg(exception);
            }
        }

        public double Meas(int channel)
        {
            Smartlink? smartlink = this.GetSmartlinkFromChannel(channel);
            if (smartlink == null)
            {
                return 0.0;
            }

            int channelSm = channel - ((this.GetSmartlinkNumberFromChannel(channel) - 1) * 6);
            double rv = smartlink.GetValue(channelSm);
            return rv;
        }


        public bool NeedSimulation(int channel)
        {
            bool useHardware = Conf.I.GetValueAsInt(ConfigNames.ValDeviceSmartlink(this.GetSmartlinkExistFromChannel(channel))) > 0;
            return useHardware;
        }

        private int GetSmartlinkNumberFromChannel(int channel)
        {
            int num = (channel - 1) / 6;
            int numberOfSmartlink = num + 1;
            return numberOfSmartlink;
        }


        int te = 0;
        private Smartlink? GetSmartlinkFromChannel(int channel)
        {
            switch (this.GetSmartlinkNumberFromChannel(channel))
            {
                case 1:
                    return this.Smartlink1;
                case 2:
                    return this.Smartlink2;
                case 3:
                    return this.Smartlink3;
                default:
                    break;
            }
            return null;
        }

        private AreaSmartlinkConfig GetSmartlinkExistFromChannel(int channel)
        {
            int number = this.GetSmartlinkNumberFromChannel(channel);
            AreaSmartlinkConfig rv = AreaSmartlinkConfig.Smartlink1Exist;
            switch (number)
            {
                case 1:
                    rv = AreaSmartlinkConfig.Smartlink1Exist;
                    break;
                case 2:
                    rv = AreaSmartlinkConfig.Smartlink2Exist;
                    break;
                case 3:
                    rv = AreaSmartlinkConfig.Smartlink3Exist;
                    break;
            };
            return rv;
        }

        private void CheckAlarm()
        {
            if (this.isSM1Alarm || this.isSM2Alarm || this.isSM3Alarm)
            {
                this.AlarmOccured?.Invoke(this, true);
            }
            else
            {
                this.AlarmOccured?.Invoke(this, false);
            }
        }

        private void OnSmartlink1AlarmOccured(object? sender, bool e)
        {
            this.isSM1Alarm = e;
            this.CheckAlarm();
        }

        private void OnSmartlink2AlarmOccured(object? sender, bool e)
        {
            this.isSM2Alarm = e;
            this.CheckAlarm();
        }

        private void OnSmartlink3AlarmOccured(object? sender, bool e)
        {
            this.isSM3Alarm = e;
            this.CheckAlarm();
        }

        private void OnCompleteReinitNeeded(object? sender, EventArgs e)
        {
            this.CompleteReinitNeeded?.Invoke(sender, e);
        }
    }
}
