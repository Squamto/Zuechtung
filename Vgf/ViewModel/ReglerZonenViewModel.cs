// -----------------------------------------------------------------------
// <copyright file="ReglerZonenViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Controls;
    using Config;
    using Framework.ViewModel;
    using Model;
    using Model.FG;
    using Model.Power;

    /// <summary>
    /// Defines the regler zonen view model.
    /// </summary>
    public class ReglerZonenViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReglerZonenViewModel"/> class.
        /// </summary>
        public ReglerZonenViewModel(FgChannels channels, PowerModel powerModel)
        {
            this.Channels = channels;
            this.EnableExeutionLog(Global.LogInfo);
            this.Zone1 = new ReglerZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone1), powerModel);
            this.Zone2 = new ReglerZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone2), powerModel);
            this.Zone3 = new ReglerZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone3), powerModel);
            this.Zone4 = new ReglerZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone4), powerModel);
            this.Zone5 = new ReglerZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone5), powerModel);
            this.Zone6 = new ReglerZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone6), powerModel);
            this.Zone7 = new ReglerZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone7), powerModel);
            this.SetAllManualCommand = new RelayCommand(this.SetAllManual, (o) => true, Global.UserMsg);
        }

        public FgChannels Channels { get; }
        
        public ReglerZoneViewModel Zone1 { get; }

        public ReglerZoneViewModel Zone2 { get; }

        public ReglerZoneViewModel Zone3 { get; }

        public ReglerZoneViewModel Zone4 { get; }

        public ReglerZoneViewModel Zone5 { get; }

        public ReglerZoneViewModel Zone6 { get; }

        public ReglerZoneViewModel Zone7 { get; }

        public double AllManualTemperature
        {
            get => this.Get<double>();
            set => this.Set(value);
        }

        public RelayCommand SetAllManualCommand { get; }

        private void SetAllManual()
        {
            this.Zone1.IsManualTemperature = true;
            this.Zone2.IsManualTemperature = true; 
            this.Zone3.IsManualTemperature = true;   
            this.Zone4.IsManualTemperature = true; 
            this.Zone5.IsManualTemperature = true;  
            this.Zone6.IsManualTemperature = true;
            this.Zone7.IsManualTemperature = true;
            this.Zone1.ManualTemperature = this.AllManualTemperature.ToString("F1", CultureInfo.InvariantCulture);
            this.Zone2.ManualTemperature = this.AllManualTemperature.ToString("F1", CultureInfo.InvariantCulture);
            this.Zone3.ManualTemperature = this.AllManualTemperature.ToString("F1", CultureInfo.InvariantCulture);
            this.Zone4.ManualTemperature = this.AllManualTemperature.ToString("F1", CultureInfo.InvariantCulture);
            this.Zone5.ManualTemperature = this.AllManualTemperature.ToString("F1", CultureInfo.InvariantCulture);
            this.Zone6.ManualTemperature = this.AllManualTemperature.ToString("F1", CultureInfo.InvariantCulture);
            this.Zone7.ManualTemperature = this.AllManualTemperature.ToString("F1", CultureInfo.InvariantCulture);
        }
    }
}
