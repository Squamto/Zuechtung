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
    using System.Collections.Specialized;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Timers;


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
        protected GraphDataSource graphDataSource;
        protected bool shouldUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuesGrafikViewModel"/> class.
        /// </summary>
        public ValuesGrafikViewModel(MainModel mainModel, GraphDataSource graphDataSource, string headline)
        {
            this.graphDataSource = graphDataSource;
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
            this.lineSeriesZone1 = new LineSeries() { ItemsSource = graphDataSource.dataCollections[0], Title = "Z1", Color = OxyColors.Red };
            this.lineSeriesZone2 = new LineSeries() { ItemsSource = graphDataSource.dataCollections[1], Title = "Z2", Color = OxyColors.Brown };
            this.lineSeriesZone3 = new LineSeries() { ItemsSource = graphDataSource.dataCollections[2], Title = "Z3", Color = OxyColors.DarkViolet };
            this.lineSeriesZone4 = new LineSeries() { ItemsSource = graphDataSource.dataCollections[3], Title = "Z4", Color = OxyColors.Turquoise };
            this.lineSeriesZone5 = new LineSeries() { ItemsSource = graphDataSource.dataCollections[4], Title = "Z5", Color = OxyColors.Green };
            this.lineSeriesZone6 = new LineSeries() { ItemsSource = graphDataSource.dataCollections[5], Title = "Z6", Color = OxyColors.Blue };
            this.lineSeriesZone7 = new LineSeries() { ItemsSource = graphDataSource.dataCollections[6], Title = "Z7", Color = OxyColors.Black };
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

            this.graphDataSource.OnDataChanged += delegate { this.shouldUpdate = true; };
            this.mainModel.Channels.CurrentCycleChanged +=(object? sender, int e) => {
                if(e % 3 == 0 && this.shouldUpdate)
                    this.AutoZoom();
            };

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
            set {
                this.Set(value);
                this.lineSeriesZone1.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(false);
            }
        }

        public bool IsShowZone2
        {
            get => this.Get<bool>();
            set {
                this.Set(value);
                this.lineSeriesZone2.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(false);
            }
        }

        public bool IsShowZone3
        {
            get => this.Get<bool>();
            set {
                this.Set(value);
                this.lineSeriesZone3.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(false);
            }
        }

        public bool IsShowZone4
        {
            get => this.Get<bool>();
            set {
                this.Set(value);
                this.lineSeriesZone4.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(false);
            }
        }

        public bool IsShowZone5
        {
            get => this.Get<bool>();
            set {
                this.Set(value);
                this.lineSeriesZone5.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(false);
            }
        }

        public bool IsShowZone6
        {
            get => this.Get<bool>();
            set {
                this.Set(value);
                this.lineSeriesZone6.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(false);
            }
        }

        public bool IsShowZone7
        {
            get => this.Get<bool>();
            set {
                this.Set(value);
                this.lineSeriesZone7.IsVisible = value;
                this.PlotViewModel.InvalidatePlot(false);
            }
        }

        public RelayCommand NextPlotCommand { get; }
        public RelayCommand PreviusPlotCommand { get; }
        public RelayCommand AutoZoomCommand { get; }

        public void AutoZoom()
        {
            this.yAxis.Minimum = double.NaN;
            this.yAxis.Maximum = double.NaN;
            this.xAxis.Minimum = double.NaN;
            this.xAxis.Maximum = double.NaN;
            this.xAxis.Reset();
            this.yAxis.Reset();
            this.PlotViewModel.InvalidatePlot(true);
        }

        private void OnChannelsControlStateChanged(object? sender, ControlStates e)
        {
            BaseViewModel.RequeryCommands();
        }
    }

    public class GraphDataSource
    {
        public List<DataPoint>[] dataCollections;

        public event Action OnDataChanged = delegate { };

        public void InvokeOnDataChanged() => this.OnDataChanged();

        public GraphDataSource(int count = 7)
        {
            List<List<DataPoint>> list = new(count);
            for(int i = 0; i < count; i++)
                list.Add(new());
            this.dataCollections = list.ToArray();
        }

        public void Clear()
        {
            foreach(var collection in dataCollections)
                collection.Clear();
            this.InvokeOnDataChanged();
        }

        public void LoadLogData(int valueRow, MainModel mainModel)
        {
            this.Clear();

            var values = mainModel.Sampler.GetValueTable(valueRow);

            int currentCycle = 0;
            foreach(ITuple val in values) {
                if(val.Length != this.dataCollections.Length)
                    throw new Exception("Invalid Logfile loaded");

                currentCycle++;
                for(int i = 0; i < val.Length; i++)
                    this.dataCollections[i].Add(new DataPoint(currentCycle, (double)val[i]));
            }
            this.InvokeOnDataChanged();
        }

        public void LoadLogDataForZones(int index, MainModel mainModel)
        {
            this.Clear();

            // TODO: Eventuell das Logfile nur einmal zentral parsen?
            for(int i = 0; i <  this.dataCollections.Length; i++) {
                var values = mainModel.Sampler.GetAllValuesFromZone(i);

                int currentCycle = 0;
                foreach(ITuple val in values) {
                    if(val.Length >= index)
                        throw new Exception("Invalid Logfile loaded");

                    currentCycle++;
                    this.dataCollections[i].Add(new DataPoint(currentCycle, (double)val[index]));
                }
            }
            this.InvokeOnDataChanged();
        }
    }
}
