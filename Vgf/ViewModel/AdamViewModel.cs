// -----------------------------------------------------------------------
// <copyright file="AdamViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System;
    using System.Globalization;
    using Config;
    using Framework.ViewModel;
    using Model;
    using Model.Power;

    /// <summary>
    /// Defines the adam view model.
    /// </summary>
    public class AdamViewModel : BaseViewModel
    {
        private PowerModel powerModel;
        private double[] rValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdamViewModel"/> class.
        /// </summary>
        public AdamViewModel(PowerModel powerModel)
        {
            this.powerModel = powerModel;
            this.USoll = new AdamDeviceViewModel(powerModel);
            this.UIst = new AdamDeviceViewModel(powerModel);
            this.ISoll = new AdamDeviceViewModel(powerModel);
            this.IIst = new AdamDeviceViewModel(powerModel);
            this.RConf = new AdamDeviceViewModel(powerModel);
            this.PManual = new AdamManualViewModel(powerModel);
            this.PIst = new AdamDeviceViewModel(powerModel);
            this.powerModel.UIMeasurement.NewUIValuesReceived += this.OnPowerModelNewUIValuesReceived;
            this.rValues = new double[7];
            this.SetConfigValues();
            this.powerModel.ConfigParameterChanged += this.OnPowerModelConfigParameterChanged;
            this.EnableExeutionLog(Global.LogInfo);
        }

        public AdamDeviceViewModel USoll { get; }

        public AdamDeviceViewModel UIst { get; }

        public AdamDeviceViewModel ISoll { get; }

        public AdamDeviceViewModel IIst { get; }

        public AdamDeviceViewModel RConf { get; }


        public AdamManualViewModel PManual { get; }

        public AdamDeviceViewModel PIst { get; }

        public void SetConfigValues()
        {
            for (int i = 0; i < 7; i++)
            {
                this.rValues[i] = Conf.I.GetValueAsDouble(ConfigNames.ValDevicePower(AreaPowerConfig.Wiederstand, ((ZoneNames)i)));
                this.RConf.Values[i].Value = rValues[i].ToString(CultureInfo.InvariantCulture);
            }
        }

        private void OnPowerModelNewUIValuesReceived(object? sender, EventArgs e)
        {
            for (int i = 0; i < this.UIst.Values.Count; i++)
            {
                double uSoll = this.powerModel.GetUSoll(i);
                double uIst = this.powerModel.UIMeasurement.GetUIst(i);
                double iSoll = uSoll / rValues[i];
                double iIst = this.powerModel.UIMeasurement.GetIIst(i);
                this.USoll.Values[i].Value = uSoll.ToString("F3", CultureInfo.InvariantCulture);
                this.UIst.Values[i].Value = uIst.ToString("F3", CultureInfo.InvariantCulture);
                this.ISoll.Values[i].Value = iSoll.ToString("F3", CultureInfo.InvariantCulture);
                this.IIst.Values[i].Value = iIst.ToString("F3", CultureInfo.InvariantCulture);
                if (!this.PManual.IsManualValues[i].Value)
                {
                    this.PManual.Values[i].Value = (iSoll * uSoll).ToString("F3", CultureInfo.InvariantCulture);
                }
                this.PIst.Values[i].Value = (iIst * uIst).ToString("F3", CultureInfo.InvariantCulture);
            }
        }
        private void OnPowerModelConfigParameterChanged(object? sender, EventArgs e)
        {
            this.SetConfigValues();
        }
    }
}
