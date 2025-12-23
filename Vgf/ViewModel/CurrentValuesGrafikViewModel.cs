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
        private int currentCycle;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentValuesGrafikViewModel"/> class.
        /// </summary>
        public CurrentValuesGrafikViewModel(MainModel mainModel)
            : base(mainModel, "Ist Temperaturen in °C")
        {
            this.mainModel = mainModel;
            this.Channels.CurrentCycleChanged += this.OnChannelsCurrentCycleChanged;
            this.Channels.CurrentStepChanged += this.OnChannelsCurrentStepChanged;
        }

        private void OnChannelsCurrentStepChanged(object sender, int e)
        {
            if (e == 0)
            {
                this.currentCycle = 0;
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
                    curentCycle += step.Cycles;
                }
                this.xAxis.Minimum =  0.0;
                this.xAxis.Maximum = curentCycle;
                this.yAxis.Minimum = 0.0;
                this.yAxis.Maximum = 1000.0;
                this.yAxis.Reset();
                this.xAxis.Reset();
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        private void OnChannelsCurrentCycleChanged(object sender, int e)
        {
            this.currentCycle++;
            foreach (FgChannel channel in this.Channels.Channels)
            {
                this.lineSeriesZone1.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[0].CurrentTemperature));
                this.lineSeriesZone2.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[1].CurrentTemperature));
                this.lineSeriesZone3.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[2].CurrentTemperature));
                this.lineSeriesZone4.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[3].CurrentTemperature));
                this.lineSeriesZone5.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[4].CurrentTemperature));
                this.lineSeriesZone6.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[5].CurrentTemperature));
                this.lineSeriesZone7.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[6].CurrentTemperature));
            }
            this.PlotViewModel.InvalidatePlot(true);
        }
    }
}
