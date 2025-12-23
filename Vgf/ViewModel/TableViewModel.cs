// -----------------------------------------------------------------------
// <copyright file="TableViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf
{
    using Framework.ViewModel;
    using Model;
    using Vgf.ViewModel;

    /// <summary>
    /// Defines the table view model.
    /// </summary>
    public class TableViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewModel"/> class.
        /// </summary>
        public TableViewModel(
            MainModel mainModel, 
            ZonenViewModel zonenViewModel, 
            ReglerZonenViewModel? reglerZonenViewModel, 
            ControlViewModel controlViewModel, 
            ConfigViewModel configViewModel,
            SmartlinkViewModel smartlinkViewModel,
            AdamViewModel adamViewModel)
        {
            this.MainModel = mainModel;
            this.ZonenViewModel = zonenViewModel;
            this.ReglerZonenViewModel = reglerZonenViewModel;
            this.ControlViewModel = controlViewModel;
            this.ConfigViewModel = configViewModel;
            this.ControlValuesGrafikViewModel = new ControlValuesGrafikViewModel(this.MainModel);
            this.CurrentValuesGrafikViewModel = new CurrentValuesGrafikViewModel(this.MainModel);
            this.SingleValuesGrafikViewModel = new SingleValuesGrafikViewModel(this.MainModel, "Einzelwerte der Zone");
            this.SmartlinkViewModel = smartlinkViewModel;
            this.AdamViewModel = adamViewModel;
        }

        public MainModel MainModel { get; }

        public ZonenViewModel ZonenViewModel { get; }

        public ReglerZonenViewModel? ReglerZonenViewModel { get; }

        public ControlViewModel ControlViewModel { get; }

        public ConfigViewModel ConfigViewModel { get; }

        public SmartlinkViewModel SmartlinkViewModel { get; }
        
        public AdamViewModel? AdamViewModel { get; }

        public ControlValuesGrafikViewModel ControlValuesGrafikViewModel { get; }

        public CurrentValuesGrafikViewModel CurrentValuesGrafikViewModel { get; }

        public SingleValuesGrafikViewModel SingleValuesGrafikViewModel { get; }

        public bool Tab1Selected
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }

        public bool Tab2Selected
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }

        public bool Tab3Selected
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }

        public bool Tab4Selected
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }
    }
}
