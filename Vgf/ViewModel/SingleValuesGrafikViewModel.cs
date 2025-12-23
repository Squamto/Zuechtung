// -----------------------------------------------------------------------
// <copyright file="SingleValuesGrafikViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using Framework.ViewModel;
    using Model;
    using Model.FG;
    using OxyPlot.Series;
    using OxyPlot.Axes;
    using OxyPlot;
    using OxyPlot.Legends;
    using OxyPlot.Annotations;

    /// <summary>
    /// Defines the single values grafik view model.
    /// </summary>
    public class SingleValuesGrafikViewModel : BaseViewModel
    {
        protected LinearAxis yAxisTemperature;
        protected LinearAxis yAxisPower;
        protected LinearAxis xAxis;
        protected LineSeries lineSeriesSetTemperatur;
        protected LineSeries lineSeriesActualTemperature;
        protected LineSeries lineSeriesPower;
        private int currentCycle;
        private MainModel mainModel;
        private bool stopGraph;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuesGrafikViewModel"/> class.
        /// </summary>
        public SingleValuesGrafikViewModel(MainModel mainModel, string headline)
        {
            this.mainModel = mainModel;
            this.mainModel.Channels.ControlStateChanged += this.OnChannelsControlStateChanged;
            this.Channels = this.mainModel.Channels;
            this.Channels.CurrentCycleChanged += this.OnChannelsCurrentCycleChanged;
            this.Channels.CurrentStepChanged += this.OnChannelsCurrentStepChanged;
            this.PlotViewModel = new ViewResolvingPlotModel { Title = headline };
            this.PlotViewModel.Legends.Add(new Legend
            {
                LegendBackground = OxyColor.FromAColor(220, OxyColors.White),
                LegendBorder = OxyColors.Black,
                LegendBorderThickness = 1.0,
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.TopLeft,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendLineSpacing = 8,
                LegendMaxWidth = 1000,
                LegendFontSize = 10
            });

            this.yAxisTemperature = new LinearAxis { Position = AxisPosition.Left, Key=nameof(this.yAxisTemperature) };
            this.yAxisPower = new LinearAxis { Position = AxisPosition.Right, Key = nameof(this.yAxisPower) };
            this.xAxis = new LinearAxis { Position = AxisPosition.Bottom, Key = nameof(this.xAxis) };
            this.PlotViewModel.Axes.Add(this.yAxisTemperature);
            this.PlotViewModel.Axes.Add(this.yAxisPower);
            this.PlotViewModel.Axes.Add(this.xAxis);
            this.lineSeriesSetTemperatur = new LineSeries();
            this.lineSeriesSetTemperatur.YAxisKey = this.yAxisTemperature.Key;
            this.lineSeriesSetTemperatur.RenderInLegend = true;
            this.lineSeriesSetTemperatur.Title = "FG";
            this.lineSeriesActualTemperature = new LineSeries();
            this.lineSeriesActualTemperature.YAxisKey = this.yAxisTemperature.Key;
            this.lineSeriesActualTemperature.RenderInLegend = true;
            this.lineSeriesActualTemperature.Title = "Temp";
            this.lineSeriesPower = new LineSeries();
            this.lineSeriesPower.YAxisKey = this.yAxisPower.Key;
            this.lineSeriesPower.Title = "Power";
            this.lineSeriesPower.Color = OxyColors.Brown;
            this.PlotViewModel.Series.Add(this.lineSeriesSetTemperatur);
            this.PlotViewModel.Series.Add(this.lineSeriesActualTemperature);
            this.PlotViewModel.Series.Add(this.lineSeriesPower);
            this.NextPlotCommand = new RelayCommand(() => this.NextPlotCommandOccured?.Invoke(this, EventArgs.Empty), (o) => this.mainModel.Channels.ControlState == ControlStates.Stop, Global.UserMsg);
            this.PreviusPlotCommand = new RelayCommand(() => this.PreviousCommandOccured?.Invoke(this, EventArgs.Empty), (o) => this.mainModel.Channels.ControlState == ControlStates.Stop, Global.UserMsg);
            this.AutoZoomCommand = new RelayCommand(this.AutoZoom, (o) => true, Global.UserMsg);
            this.ClearAllZoneShow();
            this.Set(true, nameof(this.IsShowZone1));
            this.CurrentZone = 1;
            this.EnableExeutionLog(Global.LogInfo);
        }

        public event EventHandler NextPlotCommandOccured;
        public event EventHandler PreviousCommandOccured;

        public FgChannels Channels { get; }

        public int CurrentZone { get; private set; }

        public ViewResolvingPlotModel PlotViewModel
        {
            get => this.Get<ViewResolvingPlotModel>();
            set => this.Set(value);
        }

        public bool IsShowZone1
        {
            get => this.Get<bool>();
            set
            {
                if (value)
                {
                    this.ClearAllZoneShow();
                    this.ShowLogdataWithRefresh(1);
                }
                this.Set(value);
            }
        }

        public bool IsShowZone2
        {
            get => this.Get<bool>();
            set
            {
                if (value)
                {
                    this.ClearAllZoneShow();
                    this.ShowLogdataWithRefresh(2);
                }
                this.Set(value);
            }
        }

        public bool IsShowZone3
        {
            get => this.Get<bool>();
            set
            {
                if (value)
                {
                    this.ClearAllZoneShow();
                    this.ShowLogdataWithRefresh(3);
                }
                this.Set(value);
            }
        }

        public bool IsShowZone4
        {
            get => this.Get<bool>();
            set
            {
                if (value)
                {
                    this.ClearAllZoneShow();
                    this.ShowLogdataWithRefresh(4);
                }
                this.Set(value);
            }
        }

        public bool IsShowZone5
        {
            get => this.Get<bool>();
            set
            {
                if (value)
                {
                    this.ClearAllZoneShow();
                    this.ShowLogdataWithRefresh(5);
                }
                this.Set(value);
            }
        }

        public bool IsShowZone6
        {
            get => this.Get<bool>();
            set
            {
                if (value)
                {
                    this.ClearAllZoneShow();
                    this.ShowLogdataWithRefresh(6);
                }
                this.Set(value);
            }
        }

        public bool IsShowZone7
        {
            get => this.Get<bool>();
            set
            {
                if (value)
                {
                    this.ClearAllZoneShow();
                    this.ShowLogdataWithRefresh(7);
                }
                this.Set(value);
            }
        }

        public RelayCommand NextPlotCommand { get; }
        public RelayCommand PreviusPlotCommand { get; }
        public RelayCommand AutoZoomCommand { get; }

        public void ShowLogdata(int zone)
        {
            try
            {
                this.xAxis.Minimum = double.NaN;
                this.xAxis.Maximum = double.NaN;
                this.yAxisTemperature.Minimum = double.NaN;
                this.yAxisTemperature.Maximum = double.NaN;
                this.yAxisPower.Minimum = double.NaN;
                this.yAxisPower.Maximum = double.NaN;
                this.yAxisPower.Maximum = double.NaN;
                this.stopGraph = true;
                this.CurrentZone = zone;
                this.currentCycle = 0;
                this.lineSeriesSetTemperatur.Points.Clear();
                this.lineSeriesActualTemperature.Points.Clear();
                this.lineSeriesPower.Points.Clear();
                this.PlotViewModel.InvalidatePlot(true);
                List<Tuple<double, double, double>> values = this.mainModel.Sampler.GetAllValuesFromZone(zone);
                foreach (Tuple<double, double, double> val in values)
                {
                    this.currentCycle++;
                    this.lineSeriesSetTemperatur.Points.Add(new DataPoint(this.currentCycle, val.Item1));
                    this.lineSeriesActualTemperature.Points.Add(new DataPoint(this.currentCycle, val.Item2));
                    this.lineSeriesPower.Points.Add(new DataPoint(this.currentCycle, val.Item3));
                }
                this.PlotViewModel.InvalidatePlot(true);
            }
            finally
            {
                this.stopGraph = false;           
            }
        }

        private void ShowLogdataWithRefresh(int zone)
        {
            this.mainModel.Sampler.Refresh();
            this.ShowLogdata(zone);
        }

        private void ClearAllZoneShow()
        {
            this.Set(false, nameof(this.IsShowZone1));
            this.Set(false, nameof(this.IsShowZone2));
            this.Set(false, nameof(this.IsShowZone3));
            this.Set(false, nameof(this.IsShowZone4));
            this.Set(false, nameof(this.IsShowZone5));
            this.Set(false, nameof(this.IsShowZone6));
            this.Set(false, nameof(this.IsShowZone7));
        }

        private void AutoZoom()
        {
            this.yAxisPower.Minimum = double.NaN;
            this.yAxisPower.Maximum = double.NaN;
            this.yAxisTemperature.Minimum = double.NaN;
            this.yAxisTemperature.Maximum = double.NaN;
            this.xAxis.Minimum = double.NaN;
            this.xAxis.Maximum = double.NaN;
        }

        private void OnChannelsCurrentStepChanged(object sender, int e)
        {
            if (e == 0)
            {
                this.currentCycle = 0;
                this.lineSeriesSetTemperatur.Points.Clear();
                this.lineSeriesActualTemperature.Points.Clear();
                this.lineSeriesPower.Points.Clear();
                int i = 0;
                foreach (StepChannels step in this.Channels.Steps)
                {
                    i += step.Cycles;
                }

                this.xAxis.Minimum = 0.0;
                this.xAxis.Maximum = i;
                this.yAxisTemperature.Minimum = 0.0;
                this.yAxisTemperature.Maximum = 1000.0;
                this.yAxisTemperature.Reset();
                this.yAxisPower.Minimum = 0.0;
                this.yAxisPower.Maximum = 10.0;
                this.yAxisPower.Reset();
                this.xAxis.Reset();
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        private void OnChannelsCurrentCycleChanged(object sender, int e)
        {
            if (this.stopGraph)
            {
                return;
            }

            this.currentCycle++;
            foreach (FgChannel channel in this.Channels.Channels)
            {
                this.lineSeriesSetTemperatur.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[this.CurrentZone-1].CurrentSetpoint));
                this.lineSeriesActualTemperature.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[this.CurrentZone-1].CurrentTemperature));
                this.lineSeriesPower.Points.Add(new DataPoint(this.currentCycle, this.Channels.Channels[this.CurrentZone - 1].CurrentPower));
            }
            this.PlotViewModel.InvalidatePlot(true);
        }

        private void OnChannelsControlStateChanged(object? sender, ControlStates e)
        {
            BaseViewModel.RequeryCommands();
        }
    }
}
