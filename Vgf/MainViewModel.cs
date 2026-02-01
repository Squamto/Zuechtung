// -----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf
{
    using System;
    using System.Text.Json.Serialization;
    using Config;
    using Framework;
    using Framework.ViewModel;
    using Microsoft.CSharp.RuntimeBinder;
    using Model;
    using Model.FG;
    using Model.Log;
    using OxyPlot;
    using Vgf.ViewModel;

    /// <summary>
    /// Defines the main view model.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            try
            {
                // Log for start
                Global.LogInfo(LogCategories.Always, "ApplicationStart", Global.ApplicationTitle + " gestartet.  *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***");

                // Init message box for messages and dialogs.
                this.MessageBoxViewModel = new MessageBoxViewModel();
                this.MessageBoxViewModel.Medium("Ok", string.Empty, string.Empty, string.Empty);
                this.MessageBoxViewModel.EnableExeutionLog(Global.LogInfo);
                Global.UserMsgAction = (msg) => this.MessageBoxViewModel.ShowMessage(msg);
                Global.UserDialogFunction = (msg) => this.MessageBoxViewModel.ShowDialog(msg);

                if (Conf.I.Init())
                {
                    Global.UserMsg("First start of Application in this environement. File configdata.xml createted!");
                }
                this.MainModel = new MainModel();
                this.ZonenViewModel = new ZonenViewModel(this.MainModel.Channels, this.MainModel.PowerModel);
                this.ReglerZonenViewModel = new ReglerZonenViewModel(this.MainModel.Channels, this.MainModel.PowerModel);
                this.ControlViewModel = new ControlViewModel(this.MainModel);
                this.ConfigViewModel = new ConfigViewModel(this.MainModel);
                this.ConfigViewModel.FileName = Conf.I.CurrentFileName;
                this.ConfigViewModel.FillData();
                this.SmartlinkViewModel = new SmartlinkViewModel(this.MainModel.SmartlinkModel);
                this.AdamViewModel = new AdamViewModel(this.MainModel.PowerModel);

                this.CurrentTemperaturesGraphData = new GraphDataSource();
                this.CurrentSetPointsGraphData = new GraphDataSource();
                this.CurrentPowerGraphData = new GraphDataSource();
                this.ControlValuesGraphData = new GraphDataSource();

                this.MainModel.Channels.CurrentCycleChanged += this.OnChannelsCurrentCycleChanged;
                this.MainModel.Channels.CurrentStepChanged += this.OnChannelsCurrentStepChanged;
                this.MainModel.Channels.StepsChanged += this.OnChannelsStepsChanged;

                this.Table1 = new TableViewModel(this.MainModel, CurrentTemperaturesGraphData, CurrentSetPointsGraphData, CurrentPowerGraphData, ControlValuesGraphData, this.ZonenViewModel, this.ReglerZonenViewModel, this.ControlViewModel, this.ConfigViewModel, this.SmartlinkViewModel, this.AdamViewModel);
                this.Table2 = new TableViewModel(this.MainModel, CurrentTemperaturesGraphData, CurrentSetPointsGraphData, CurrentPowerGraphData, ControlValuesGraphData, this.ZonenViewModel, this.ReglerZonenViewModel, this.ControlViewModel, this.ConfigViewModel, this.SmartlinkViewModel, this.AdamViewModel);
                this.Table3 = new TableViewModel(this.MainModel, CurrentTemperaturesGraphData, CurrentSetPointsGraphData, CurrentPowerGraphData, ControlValuesGraphData, this.ZonenViewModel, this.ReglerZonenViewModel, this.ControlViewModel, this.ConfigViewModel, this.SmartlinkViewModel, this.AdamViewModel);
                this.Table4 = new TableViewModel(this.MainModel, CurrentTemperaturesGraphData, CurrentSetPointsGraphData, CurrentPowerGraphData, ControlValuesGraphData, this.ZonenViewModel, this.ReglerZonenViewModel, this.ControlViewModel, this.ConfigViewModel, this.SmartlinkViewModel, this.AdamViewModel);
                this.Table1.ControlValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table1.ControlValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;
                this.Table1.CurrentValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table1.CurrentValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;
                this.Table1.SingleValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table1.SingleValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;

                this.Table2.ControlValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table2.ControlValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;
                this.Table2.CurrentValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table2.CurrentValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;
                this.Table2.SingleValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table2.SingleValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;

                this.Table3.ControlValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table3.ControlValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;
                this.Table3.CurrentValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table3.CurrentValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;
                this.Table3.SingleValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table3.SingleValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;

                this.Table4.ControlValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table4.ControlValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;
                this.Table4.CurrentValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table4.CurrentValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;
                this.Table4.SingleValuesGrafikViewModel.NextPlotCommandOccured += this.OnControlValuesGrafikViewModelNextPlotCommandOccured;
                this.Table4.SingleValuesGrafikViewModel.PreviousCommandOccured += this.OnControlValuesGrafikViewModelPreviousCommandOccured;

                this.Table1.Tab1Selected = true;
                this.Table2.Tab2Selected = true;
                this.Table3.Tab3Selected = true;
                this.Table4.Tab4Selected = true;
                this.MainModel.Channels.ControlStateChanged += this.OnChannelsControlStateChanged;
                this.NextPlotCommand = new RelayCommand(this.NextPlot, (o) => (this.MainModel.Channels.ControlState == ControlStates.Stop), Global.UserMsg);
                this.PreviousPlotCommand = new RelayCommand(this.PreviuosPlot, (o) => (this.MainModel.Channels.ControlState == ControlStates.Stop), Global.UserMsg);
                this.ExitCommand = new RelayCommand(() => Environment.Exit(0), (o) => (this.MainModel.Channels.ControlState == ControlStates.Stop), Global.UserMsg);
                this.MainModel.Sampler.Refresh();
                this.ShowLogData();
                this.ControlViewModel.StartTimeAbsolut = this.MainModel.Sampler.LogStart;
                this.ControlViewModel.EndTimeAbsolut = this.MainModel.Sampler.LogEnd;
                this.ControlViewModel.CurrentTime = this.MainModel.Sampler.LogDuration;
                this.ControlViewModel.CurrentTimUntilEnd = "-";
                this.ControlViewModel.Init();
            }
            catch (RuntimeBinderException exception)
            {
                Global.UserMsg(exception);
            }
        }

        public MainModel? MainModel { get; }

        public GraphDataSource? CurrentTemperaturesGraphData { get; }
        public GraphDataSource? CurrentSetPointsGraphData { get; }
        public GraphDataSource? CurrentPowerGraphData { get; }

        public GraphDataSource? ControlValuesGraphData { get; }

        public ZonenViewModel? ZonenViewModel { get; }

        public ReglerZonenViewModel? ReglerZonenViewModel { get; }

        public ControlViewModel? ControlViewModel { get; }

        public ConfigViewModel? ConfigViewModel { get; }

        public SmartlinkViewModel? SmartlinkViewModel { get; }

        public AdamViewModel? AdamViewModel { get; }

        public TableViewModel? Table1 { get; }

        public TableViewModel? Table2 { get; }

        public TableViewModel? Table3 { get; }

        public TableViewModel? Table4 { get; }

        public MessageBoxViewModel? MessageBoxViewModel { get; }

        public RelayCommand NextPlotCommand { get; }

        public RelayCommand PreviousPlotCommand { get; }

        public RelayCommand ExitCommand { get; }

        private void NextPlot()
        {
            // TODO: remove
            {
                var rng = new Random();
                double value = 100.0;
                for(int i = 0; i < 200000; i++) {
                    value += (rng.NextDouble() - 0.5);
                    foreach(var collection in CurrentTemperaturesGraphData.dataCollections)
                        collection.Add(new DataPoint(i, value));
                    foreach(var collection in CurrentSetPointsGraphData.dataCollections)
                        collection.Add(new DataPoint(i, value));
                    foreach(var collection in CurrentPowerGraphData.dataCollections)
                        collection.Add(new DataPoint(i, value));
                }
            }
            //this.MainModel.Sampler.ReadNextTemperaturLog();
            //this.ControlViewModel.StartTimeAbsolut = this.MainModel.Sampler.LogStart;
            //this.ControlViewModel.EndTimeAbsolut = this.MainModel.Sampler.LogEnd;
            //this.ControlViewModel.CurrentTime = this.MainModel.Sampler.LogDuration;
            //this.ControlViewModel.CurrentTimUntilEnd = "-";
            //this.ShowLogData();
        }

        private void PreviuosPlot()
        {
            this.MainModel.Sampler.ReadPreviuosTemperaturLog();
            this.ControlViewModel.StartTimeAbsolut = this.MainModel.Sampler.LogStart;
            this.ControlViewModel.EndTimeAbsolut = this.MainModel.Sampler.LogEnd;
            this.ControlViewModel.CurrentTime = this.MainModel.Sampler.LogDuration;
            this.ControlViewModel.CurrentTimUntilEnd = "-";
            this.ShowLogData();
        }

        private void ShowLogData()
        {
            this.MainModel.Sampler.Refresh();
            this.CurrentPowerGraphData?.LoadLogData(SamplerFile.STARTTABPOWER, this.MainModel);
            this.CurrentTemperaturesGraphData?.LoadLogData(SamplerFile.STARTTABTEMPERATURES, this.MainModel);
            this.ControlValuesGraphData?.LoadLogData(SamplerFile.STARTTABSETVALUES, this.MainModel);

            this.Table1.ControlValuesGrafikViewModel.AutoZoom();
            this.Table2.ControlValuesGrafikViewModel.AutoZoom();
            this.Table3.ControlValuesGrafikViewModel.AutoZoom();
            this.Table4.ControlValuesGrafikViewModel.AutoZoom();

            this.Table1.CurrentValuesGrafikViewModel.AutoZoom();
            this.Table2.CurrentValuesGrafikViewModel.AutoZoom();
            this.Table3.CurrentValuesGrafikViewModel.AutoZoom();
            this.Table4.CurrentValuesGrafikViewModel.AutoZoom();

            this.Table1.SingleValuesGrafikViewModel.AutoZoom();
            this.Table2.SingleValuesGrafikViewModel.AutoZoom();
            this.Table3.SingleValuesGrafikViewModel.AutoZoom();
            this.Table4.SingleValuesGrafikViewModel.AutoZoom();
        }

        private void OnChannelsControlStateChanged(object? sender, ControlStates e)
        {
            BaseViewModel.RequeryCommands();
        }

        private void OnControlValuesGrafikViewModelPreviousCommandOccured(object? sender, EventArgs e)
        {
            this.PreviuosPlot();
        }

        private void OnControlValuesGrafikViewModelNextPlotCommandOccured(object? sender, EventArgs e)
        {
            this.NextPlot();
        }

        private void OnChannelsCurrentCycleChanged(object? sender, int cycles)
        {
            List<FgChannel> Channels = this.MainModel.Channels.Channels;

            for (int i = 0; i < Channels.Count; i++) {
                this.CurrentTemperaturesGraphData.dataCollections[i].Add(new DataPoint(this.CurrentTemperaturesGraphData.dataCollections[i].Count, Channels[i].CurrentTemperature));
                this.CurrentSetPointsGraphData.dataCollections[i].Add(new DataPoint(this.CurrentSetPointsGraphData.dataCollections[i].Count, Channels[i].CurrentSetpoint));
                this.CurrentPowerGraphData.dataCollections[i].Add(new DataPoint(this.CurrentPowerGraphData.dataCollections[i].Count, Channels[i].CurrentPower));
            }

            int refreshInterval = Conf.I.GetValueAsInt(ConfigNames.ValDeviceBase(AreaBaseConfig.GraphRefreshInterval));
            if(refreshInterval == 0 || cycles % refreshInterval == 0) {
                this.CurrentTemperaturesGraphData.InvokeOnDataChanged();
                this.CurrentSetPointsGraphData.InvokeOnDataChanged();
                this.CurrentPowerGraphData.InvokeOnDataChanged();
            }
        }

        private void OnChannelsCurrentStepChanged(object? sender, int step)
        {
            if(step == 0) {
                this.CurrentTemperaturesGraphData.Clear();
                this.CurrentSetPointsGraphData.Clear();
                this.CurrentPowerGraphData.Clear();
            }
        }

        private void OnChannelsStepsChanged(object? sender, EventArgs e)
        {
            FgChannels Channels = this.MainModel.Channels;

            this.ControlValuesGraphData.Clear();

            int curentCycle = 0;
            foreach (StepChannels step in Channels.Steps)
            {
                this.ControlValuesGraphData.dataCollections[0].Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone1]));
                this.ControlValuesGraphData.dataCollections[1].Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone2]));
                this.ControlValuesGraphData.dataCollections[2].Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone3]));
                this.ControlValuesGraphData.dataCollections[3].Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone4]));
                this.ControlValuesGraphData.dataCollections[4].Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone5]));
                this.ControlValuesGraphData.dataCollections[5].Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone6]));
                this.ControlValuesGraphData.dataCollections[6].Add(new DataPoint(curentCycle, step.TargetTemps[ZoneNames.Zone7]));
                curentCycle += step.Cycles;
            }
            this.ControlValuesGraphData.InvokeOnDataChanged();
        }
    }
}
