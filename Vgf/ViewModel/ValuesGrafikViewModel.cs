// -----------------------------------------------------------------------
// <copyright file="ValuesGrafikViewModel.cs" company="IB Hermann">
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
    using OxyPlot.Legends;
    using OxyPlot;

    /// <summary>
    /// Defines the values grafik view model.
    /// </summary>
    public class ValuesGrafikViewModel : BaseViewModel
    {
        protected LinearAxis yAxis;
        protected LinearAxis xAxis;
        protected LineSeries lineSeriesZone1;
        protected LineSeries lineSeriesZone2;
        protected LineSeries lineSeriesZone3;
        protected LineSeries lineSeriesZone4;
        protected LineSeries lineSeriesZone5;
        protected LineSeries lineSeriesZone6;
        protected LineSeries lineSeriesZone7;
        protected MainModel mainModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuesGrafikViewModel"/> class.
        /// </summary>
        public ValuesGrafikViewModel(MainModel mainModel, string headline)
        {
            this.mainModel = mainModel;
            this.mainModel.Channels.ControlStateChanged += this.OnChannelsControlStateChanged;
            this.Channels = this.mainModel.Channels;
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

            this.yAxis = new LinearAxis { Position = AxisPosition.Left };
            this.xAxis = new LinearAxis { Position = AxisPosition.Bottom };
            this.PlotViewModel.Axes.Add(this.yAxis);
            this.PlotViewModel.Axes.Add(this.xAxis);
            this.lineSeriesZone1 = new LineSeries() { Title = "Z1", Color = OxyColors.Red};
            this.lineSeriesZone2 = new LineSeries() { Title = "Z2", Color = OxyColors.Brown };
            this.lineSeriesZone3 = new LineSeries() { Title = "Z3", Color = OxyColors.DarkViolet };
            this.lineSeriesZone4 = new LineSeries() { Title = "Z4", Color = OxyColors.Turquoise };
            this.lineSeriesZone5 = new LineSeries() { Title = "Z5", Color = OxyColors.Green };
            this.lineSeriesZone6 = new LineSeries() { Title = "Z6", Color = OxyColors.Blue };
            this.lineSeriesZone7 = new LineSeries() { Title = "Z7", Color = OxyColors.Black };
            this.PlotViewModel.Series.Add(this.lineSeriesZone1);
            this.PlotViewModel.Series.Add(this.lineSeriesZone2);
            this.PlotViewModel.Series.Add(this.lineSeriesZone3);
            this.PlotViewModel.Series.Add(this.lineSeriesZone4);
            this.PlotViewModel.Series.Add(this.lineSeriesZone5);
            this.PlotViewModel.Series.Add(this.lineSeriesZone6);
            this.PlotViewModel.Series.Add(this.lineSeriesZone7);
            this.IsShowZone1 = true;
            this.IsShowZone2 = true;
            this.IsShowZone3 = true;
            this.IsShowZone4 = true;
            this.IsShowZone5 = true;
            this.IsShowZone6 = true;
            this.IsShowZone7 = true;
            this.NextPlotCommand = new RelayCommand(() => this.NextPlotCommandOccured?.Invoke(this, EventArgs.Empty), (o) => this.mainModel.Channels.ControlState == ControlStates.Stop, Global.UserMsg);
            this.PreviusPlotCommand = new RelayCommand(() => this.PreviousCommandOccured?.Invoke(this, EventArgs.Empty), (o) => this.mainModel.Channels.ControlState == ControlStates.Stop, Global.UserMsg);
            this.AutoZoomCommand = new RelayCommand(this.AutoZoom, (o) => true, Global.UserMsg);
            this.EnableExeutionLog(Global.LogInfo);
        }

        public event EventHandler NextPlotCommandOccured;
        public event EventHandler PreviousCommandOccured;

        public FgChannels Channels { get; }

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
                this.Set(value);
                this.lineSeriesZone1.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        public bool IsShowZone2
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.lineSeriesZone2.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        public bool IsShowZone3
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.lineSeriesZone3.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        public bool IsShowZone4
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.lineSeriesZone4.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        public bool IsShowZone5
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.lineSeriesZone5.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        public bool IsShowZone6
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.lineSeriesZone6.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        public bool IsShowZone7
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.lineSeriesZone7.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(true);
            }
        }

        public RelayCommand NextPlotCommand {  get; }
        public RelayCommand PreviusPlotCommand { get; }
        public RelayCommand AutoZoomCommand { get; }

        public void ShowLogdata(int valueRow)
        {
            this.yAxis.Minimum = double.NaN;
            this.yAxis.Maximum = double.NaN;
            this.xAxis.Minimum = double.NaN;
            this.xAxis.Maximum = double.NaN;
            int currentCycle = 0;
            this.lineSeriesZone1.Points.Clear();
            this.lineSeriesZone2.Points.Clear();
            this.lineSeriesZone3.Points.Clear();
            this.lineSeriesZone4.Points.Clear();
            this.lineSeriesZone5.Points.Clear();
            this.lineSeriesZone6.Points.Clear();
            this.lineSeriesZone7.Points.Clear();
            this.PlotViewModel.InvalidatePlot(true);
            List<Tuple<double, double, double, double, double, double, double>> values = this.mainModel.Sampler.GetValueTable(valueRow);
            foreach (Tuple<double, double, double, double, double, double, double> val in values)
            {
                currentCycle++;
                this.lineSeriesZone1.Points.Add(new DataPoint(currentCycle, val.Item1));
                this.lineSeriesZone2.Points.Add(new DataPoint(currentCycle, val.Item2));
                this.lineSeriesZone3.Points.Add(new DataPoint(currentCycle, val.Item3));
                this.lineSeriesZone4.Points.Add(new DataPoint(currentCycle, val.Item4));
                this.lineSeriesZone5.Points.Add(new DataPoint(currentCycle, val.Item5));
                this.lineSeriesZone6.Points.Add(new DataPoint(currentCycle, val.Item6));
                this.lineSeriesZone7.Points.Add(new DataPoint(currentCycle, val.Item7));
            }
            this.IsShowZone1 = true;
            this.IsShowZone2 = true;
            this.IsShowZone3 = true;
            this.IsShowZone4 = true;
            this.IsShowZone5 = true;
            this.IsShowZone6 = true;
            this.IsShowZone7 = true;
        }

        private void AutoZoom()
        {
            this.yAxis.Minimum = double.NaN;
            this.yAxis.Maximum = double.NaN;
            this.xAxis.Minimum = double.NaN;
            this.xAxis.Maximum = double.NaN;
        }

        private void OnChannelsControlStateChanged(object? sender, ControlStates e)
        {
            BaseViewModel.RequeryCommands();
        }

    }
}
