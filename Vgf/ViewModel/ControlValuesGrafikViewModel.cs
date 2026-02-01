// -----------------------------------------------------------------------
// <copyright file="ControlValuesGrafikViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System;
    using Model.FG;
    using OxyPlot;
    using Config;
    using Model;
    using System.Diagnostics;

    /// <summary>
    /// Defines the control values grafik view model.
    /// </summary>
    public class ControlValuesGrafikViewModel : ValuesGrafikViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlViewModel"/> class.
        /// </summary>
        public ControlValuesGrafikViewModel(MainModel mainModel, GraphDataSource graphDataSource)
            : base(mainModel, graphDataSource, "Temperaturführung in °C")
        { }
    }
}
