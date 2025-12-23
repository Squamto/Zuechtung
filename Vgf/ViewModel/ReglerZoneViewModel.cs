// -----------------------------------------------------------------------
// <copyright file="ReglerZoneViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System;
    using System.Globalization;
    using System.Security.Policy;
    using System.Windows.Media;
    using Framework.ViewModel;
    using Model;
    using Model.FG;
    using Model.Power;

    /// <summary>
    /// Defines the regler zone view model.
    /// </summary>
    public class ReglerZoneViewModel : BaseViewModel
    {
        private PowerModel powerModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneViewModel"/> class.
        /// </summary>
        public ReglerZoneViewModel(FgChannel channel, PowerModel powerModel)
        {
            this.Channel = channel;
            this.powerModel = powerModel;
            this.ZoneNr = (((int)channel.Zone) + 1).ToString();
            this.ManualTemperature = "0.0";
            this.Channel.CurrentPowerChanged += this.OnChannelCurrentPowerChanged;
            this.EnableExeutionLog(Global.LogInfo);
        }

        public FgChannel Channel { get; }

        public string ZoneNr
        {
            get => this.Get<string>();
            set => this.Set(value);
        }
        public bool IsManualTemperature
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.Channel.IsManualSetpointMode = value;
                this.WriteManualTemperature();
            }
        }
        
        public string ManualTemperature
        {
            get => this.Get<string>();
            set
            {
                this.Set(value);
                this.WriteManualTemperature();
            }
        }

        public string Difference
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string PAnteil
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string ITerm
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string IPuffer
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string MaxU
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Stellfaktor
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Stellwert
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string StellWertMittel
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        private void WriteManualTemperature()
        {
            try
            {
                double sp = double.Parse(this.ManualTemperature, CultureInfo.InvariantCulture);
                if ((sp < 0) || (sp > 1300))
                {
                    Global.UserMsg("Bitte geben Sie eine Zahl zwischen 0.0 und 1300.0 ein.");
                    return;
                }
                this.Channel.ManualSetpoint = sp;
            }
            catch
            {
                Global.UserMsg("Bitte geben Sie eine Zahl zwischen 0.0 und 1300.0 ein.");
            }
        }

        private void OnChannelCurrentPowerChanged(object? sender, double e)
        {
            this.Difference = this.Channel.CurrentTemperatureDifference.ToString("F2", CultureInfo.InvariantCulture);
            this.PAnteil = this.Channel.PRegler.ToString("F2", CultureInfo.InvariantCulture);
            this.ITerm = this.Channel.IRegler.ToString("F4", CultureInfo.InvariantCulture);
            this.IPuffer = this.Channel.IPuffer.ToString("F4", CultureInfo.InvariantCulture);
            this.MaxU = this.Channel.MaxP.ToString("F2", CultureInfo.InvariantCulture);
            this.Stellfaktor = this.Channel.Vorregler.ToString("F2", CultureInfo.InvariantCulture);
            this.StellWertMittel = (this.Channel.IPuffer + this.Channel.Vorregler).ToString("F2", CultureInfo.InvariantCulture);
            int zone = (int)this.Channel.Zone;
            if (this.powerModel.IsManualValues[zone])
            {
                this.Stellwert = this.powerModel.GetPSoll(zone).ToString("F2", CultureInfo.InvariantCulture) + " M";
            }
            else
            {
                this.Stellwert = this.Channel.CurrentPower.ToString("F2", CultureInfo.InvariantCulture);
            }

        }
    }
}
