// -----------------------------------------------------------------------
// <copyright file="SmartlinkViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Config;
    using Framework.ViewModel;
    using Model;
    using Model.FG;
    using Model.Smartlink;

    /// <summary>
    /// Defines the smartlink view model.
    /// </summary>
    public class SmartlinkViewModel : BaseViewModel
    {
        private SmartlinkModel smartlinkModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartlinkViewModel"/> class.
        /// </summary>
        public SmartlinkViewModel(SmartlinkModel smartlinkModel)
        {
            this.smartlinkModel = smartlinkModel;
            this.Smartlink1 = new SmartlinkDeviceViewModel(smartlinkModel);
            this.Smartlink2 = new SmartlinkDeviceViewModel(smartlinkModel);
            this.Smartlink3 = new SmartlinkDeviceViewModel(smartlinkModel);
            Task.Factory.StartNew(this.BackgroundTask);
            this.EnableExeutionLog(Global.LogInfo);
        }

        public SmartlinkDeviceViewModel Smartlink1 { get; }

        public SmartlinkDeviceViewModel Smartlink2 { get; }

        public SmartlinkDeviceViewModel Smartlink3 { get; }

        private void BackgroundTask()
        {
            while (true)
            {
                for (int i = 1; i < 19; i++)
                {
                    double t = this.smartlinkModel.Meas(i);
                    switch (i)
                    {
                        case 1: this.Smartlink1.Temperature1 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 2: this.Smartlink1.Temperature2 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 3: this.Smartlink1.Temperature3 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 4: this.Smartlink1.Temperature4 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 5: this.Smartlink1.Temperature5 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 6: this.Smartlink1.Temperature6 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 7: this.Smartlink2.Temperature1 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 8: this.Smartlink2.Temperature2 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 9: this.Smartlink2.Temperature3 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 10: this.Smartlink2.Temperature4 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 11: this.Smartlink2.Temperature5 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 12: this.Smartlink2.Temperature6 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 13: this.Smartlink3.Temperature1 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 14: this.Smartlink3.Temperature2 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 15: this.Smartlink3.Temperature3 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 16: this.Smartlink3.Temperature4 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 17: this.Smartlink3.Temperature5 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                        case 18: this.Smartlink3.Temperature6 = t.ToString("F4", CultureInfo.InvariantCulture); break;
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
