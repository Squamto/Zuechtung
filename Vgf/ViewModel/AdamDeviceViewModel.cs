// -----------------------------------------------------------------------
// <copyright file="AdamDeviceViewModel.cs" company="IB Hermann">
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
    using Model.Smartlink;

    /// <summary>
    /// Defines the adam device view model.
    /// </summary>
    public class AdamDeviceViewModel : BaseViewModel
    {
        protected PowerModel powerModel;
        /// <summary>
        /// Initializes a new instance of the <see cref="AdamDeviceViewModel"/> class.
        /// </summary>
        public AdamDeviceViewModel(PowerModel powerModel)
        {
            this.powerModel = powerModel;
            this.Values = new List<StringValueViewModel>();
            this.Values.Add(new StringValueViewModel(0));
            this.Values.Add(new StringValueViewModel(1));
            this.Values.Add(new StringValueViewModel(2));
            this.Values.Add(new StringValueViewModel(3));
            this.Values.Add(new StringValueViewModel(4));
            this.Values.Add(new StringValueViewModel(5));
            this.Values.Add(new StringValueViewModel(6));
            this.EnableExeutionLog(Global.LogInfo);
        }

        public List<StringValueViewModel> Values { get; }

        public StringValueViewModel Value1 => this.Values[0];
        public StringValueViewModel Value2 => this.Values[1];
        public StringValueViewModel Value3 => this.Values[2];
        public StringValueViewModel Value4 => this.Values[3];
        public StringValueViewModel Value5 => this.Values[4];
        public StringValueViewModel Value6 => this.Values[5];
        public StringValueViewModel Value7 => this.Values[6];

    }
}
