// -----------------------------------------------------------------------
// <copyright file="CurrentValuesGrafikViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using Model;
    using Model.FG;
    using OxyPlot;

    /// <summary>
    /// Defines the current values grafik view model.
    /// </summary>
    public class CurrentValuesGrafikViewModel : ValuesGrafikViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentValuesGrafikViewModel"/> class.
        /// </summary>
        public CurrentValuesGrafikViewModel(MainModel mainModel, GraphDataSource graphDataSource)
            : base(mainModel, graphDataSource, "Ist Temperaturen in °C")
        { }

    }
}
