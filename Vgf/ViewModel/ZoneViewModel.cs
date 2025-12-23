// -----------------------------------------------------------------------
// <copyright file="ZoneViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System.Globalization;
    using System.Windows.Media;
    using Framework.ViewModel;
    using Model;
    using Model.FG;
    using Model.Power;

    /// <summary>
    /// Defines the zone view model.
    /// </summary>
    public class ZoneViewModel : BaseViewModel
    {
        private PowerModel powerModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneViewModel"/> class.
        /// </summary>
        public ZoneViewModel(FgChannel channel, PowerModel powerModel)
        {
            this.Channel = channel;
            this.powerModel = powerModel;
            this.Channel.CurrentSetpointChanged += this.OnChannelCurrentSetpointChanged;
            this.Channel.CurrentTemperatureChanged += this.OnChannelCurrentTemperatureChanged;
            this.Channel.SafetyTemperatureChanged += this.OnChannelSafetyTemperatureChanged;
            this.Channel.CurrentPowerChanged += this.OnChannelCurrentPowerChanged;
            this.Channel.TemperatureAlarmOccured += this.OnChannelTemperatureAlarmOccured;
            this.Channel.CurrentTemperatureDifferenceChanged += this.OnChannelCurrentTemperatureDifferenceChanged;
            this.SetDefaultColors();
            this.EnableExeutionLog(Global.LogInfo);
        }

        public FgChannel Channel { get; }

        public string Name => this.Channel.Name;

        public string Setpoint
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Temperature
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string SafetyTemperature
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Difference
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Power
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string NameColor
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string SetpointColor
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string TemepratureColor
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string SafetyTemperatureColor
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string DifferenceColor
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string PowerColor
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public void SetDefaultColors()
        {
            this.NameColor = Colors.Blue.ToString();
            this.SetpointColor = Colors.Wheat.ToString();
            this.TemepratureColor = Colors.Lime.ToString();
            this.SafetyTemperatureColor = Colors.LightGreen.ToString();
            this.DifferenceColor = Colors.LightGray.ToString();
            this.PowerColor = Colors.LightSkyBlue.ToString();
        }

        public void SetAlarmColors()
        {
            this.NameColor = Colors.Red.ToString();
            this.SetpointColor = Colors.Red.ToString();
            this.TemepratureColor = Colors.Red.ToString();
            this.SafetyTemperatureColor = Colors.Red.ToString();
            this.DifferenceColor = Colors.Red.ToString();
            this.PowerColor = Colors.Red.ToString();
        }

        private void OnChannelSafetyTemperatureChanged(object sender, double e)
        {
            this.SafetyTemperature = e.ToString("F1", CultureInfo.InvariantCulture);
        }

        private void OnChannelCurrentSetpointChanged(object sender, double e)
        {
            if (this.Channel.IsManualSetpointMode)
            {
                this.Setpoint = e.ToString("F1", CultureInfo.InvariantCulture) + " M";
            }
            else
            {
                this.Setpoint = e.ToString("F1", CultureInfo.InvariantCulture);
            }
        }

        private void OnChannelCurrentTemperatureChanged(object sender, double e)
        {
            this.Temperature = e.ToString("F1", CultureInfo.InvariantCulture);
        }
        private void OnChannelCurrentPowerChanged(object sender, double e)
        {
            int zone = (int)this.Channel.Zone;
            if (this.powerModel.IsManualValues[zone])
            {
                this.Power = this.powerModel.GetPSoll(zone).ToString("F1", CultureInfo.InvariantCulture) + " M";
            }
            else
            {
                this.Power = e.ToString("F1", CultureInfo.InvariantCulture);
            }

        }
        private void OnChannelCurrentTemperatureDifferenceChanged(object sender, double e)
        {
            this.Difference = e.ToString("F2", CultureInfo.InvariantCulture);
        }

        private void OnChannelTemperatureAlarmOccured(object sender, bool e)
        {
            this.SetAlarmColors();
        }
    }
}
