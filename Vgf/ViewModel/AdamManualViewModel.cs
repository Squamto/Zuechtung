// -----------------------------------------------------------------------
// <copyright file="AdamManualViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System.Globalization;
    using Model;
    using Model.Power;

    /// <summary>
    /// Defines the adam manual view model.
    /// </summary>
    public class AdamManualViewModel : AdamDeviceViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdamManualViewModel"/> class.
        /// </summary>
        public AdamManualViewModel(PowerModel powerModel)
            :base(powerModel)
        {
            this.IsManualValues = new List<BoolValueViewModel>();
            this.IsManualValues.Add(new BoolValueViewModel(0));
            this.IsManualValues.Add(new BoolValueViewModel(1));
            this.IsManualValues.Add(new BoolValueViewModel(2));
            this.IsManualValues.Add(new BoolValueViewModel(3));
            this.IsManualValues.Add(new BoolValueViewModel(4));
            this.IsManualValues.Add(new BoolValueViewModel(5));
            this.IsManualValues.Add(new BoolValueViewModel(6));

            for (int i = 0; i < 7; i++)
            {
                this.Values[i].ValueChanged += this.OnAdamManualViewModeValueChanged;
                this.IsManualValues[i].ValueChanged += this.OnAdamManualViewModeValueChanged;
                this.Values[i].Value = "0.0";

            }
            this.EnableExeutionLog(Global.LogInfo);
        }

        public List<BoolValueViewModel> IsManualValues { get; }

        public BoolValueViewModel IsManual1 => this.IsManualValues[0];
        public BoolValueViewModel IsManual2 => this.IsManualValues[1];
        public BoolValueViewModel IsManual3 => this.IsManualValues[2];
        public BoolValueViewModel IsManual4 => this.IsManualValues[3];
        public BoolValueViewModel IsManual5 => this.IsManualValues[4];
        public BoolValueViewModel IsManual6 => this.IsManualValues[5];
        public BoolValueViewModel IsManual7 => this.IsManualValues[6];

        private void OnAdamManualViewModeValueChanged(object? sender, int e)
        {
            this.powerModel.SetIsManual(e, double.Parse(this.Values[e].Value, CultureInfo.InvariantCulture), this.IsManualValues[e].Value);
        }
    }
}
