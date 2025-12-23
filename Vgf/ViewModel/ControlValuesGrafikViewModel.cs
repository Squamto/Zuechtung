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
        public ControlValuesGrafikViewModel(MainModel mainModel)
            : base(mainModel, "Temperaturführung in °C")
        {
            this.Channels.StepsChanged += this.OnChannelsStepsChanged;
        }

        private void OnChannelsStepsChanged(object sender, EventArgs e)
        {
            this.lineSeriesZone1.Points.Clear();
            this.lineSeriesZone2.Points.Clear();
            this.lineSeriesZone3.Points.Clear();
            this.lineSeriesZone4.Points.Clear();
            this.lineSeriesZone5.Points.Clear();
            this.lineSeriesZone6.Points.Clear();
            this.lineSeriesZone7.Points.Clear();
            int curentCycle = 0;
            foreach (StepChannels step in this.Channels.Steps)
            {
                this.lineSeriesZone1.Points.Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone1]));
                this.lineSeriesZone2.Points.Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone2]));
                this.lineSeriesZone3.Points.Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone3]));
                this.lineSeriesZone4.Points.Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone4]));
                this.lineSeriesZone5.Points.Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone5]));
                this.lineSeriesZone6.Points.Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone6]));
                this.lineSeriesZone7.Points.Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone7]));
                curentCycle += step.Cycles;
            }
            this.xAxis.Minimum = 0.0;
            this.xAxis.Maximum = curentCycle;
            this.yAxis.Minimum = 0.0;
            this.yAxis.Maximum = 1000.0;
            this.yAxis.Reset();
            this.xAxis.Reset();
            this.PlotViewModel.InvalidatePlot(true);
        }
    }
}
