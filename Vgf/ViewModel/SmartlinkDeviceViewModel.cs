// -----------------------------------------------------------------------
// <copyright file="SmartlinkDeviceViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System.Windows.Media;
    using Framework.ViewModel;
    using Model;
    using Model.FG;
    using Model.Smartlink;

    /// <summary>
    /// Defines the smartlink device view model.
    /// </summary>
    public class SmartlinkDeviceViewModel : BaseViewModel
    {
        private SmartlinkModel smartlink;
        /// <summary>
        /// Initializes a new instance of the <see cref="SmartlinkDeviceViewModel"/> class.
        /// </summary>
        public SmartlinkDeviceViewModel(SmartlinkModel smartlinks)
        {
            this.smartlink = smartlinks;
            this.EnableExeutionLog(Global.LogInfo);
        }

        public string Temperature1
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Temperature2
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Temperature3
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Temperature4
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Temperature5
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string Temperature6
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

    }
}
