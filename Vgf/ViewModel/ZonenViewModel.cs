// -----------------------------------------------------------------------
// <copyright file="ZonenViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System.Linq;
    using System.Windows.Controls;
    using Config;
    using Framework.ViewModel;
    using Model;
    using Model.FG;
    using Model.Power;

    /// <summary>
    /// Defines the zonen view model.
    /// </summary>
    public class ZonenViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZonenViewModel"/> class.
        /// </summary>
        public ZonenViewModel(FgChannels channels, PowerModel powerModel)
        {
            this.Channels = channels;
            this.EnableExeutionLog(Global.LogInfo);
            this.Zone1 = new ZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone1), powerModel);
            this.Zone2 = new ZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone2), powerModel);
            this.Zone3 = new ZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone3), powerModel);
            this.Zone4 = new ZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone4), powerModel);
            this.Zone5 = new ZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone5), powerModel);
            this.Zone6 = new ZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone6), powerModel);
            this.Zone7 = new ZoneViewModel(this.Channels.Channels.First(o => o.Zone == ZoneNames.Zone7), powerModel);
        }

        public FgChannels Channels { get; }
        
        public ZoneViewModel Zone1 { get; }

        public ZoneViewModel Zone2 { get; }

        public ZoneViewModel Zone3 { get; }

        public ZoneViewModel Zone4 { get; }

        public ZoneViewModel Zone5 { get; }

        public ZoneViewModel Zone6 { get; }

        public ZoneViewModel Zone7 { get; }
    }
}
