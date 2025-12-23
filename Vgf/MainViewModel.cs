// -----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf
{
    using System;
    using Config;
    using Framework;
    using Framework.ViewModel;
    using Model;
    using Model.FG;
    using Model.Log;
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

                this.Table1 = new TableViewModel(this.MainModel, this.ZonenViewModel, this.ReglerZonenViewModel, this.ControlViewModel, this.ConfigViewModel, this.SmartlinkViewModel, this.AdamViewModel);
                this.Table2 = new TableViewModel(this.MainModel, this.ZonenViewModel, this.ReglerZonenViewModel, this.ControlViewModel, this.ConfigViewModel, this.SmartlinkViewModel, this.AdamViewModel);
                this.Table3 = new TableViewModel(this.MainModel, this.ZonenViewModel, this.ReglerZonenViewModel, this.ControlViewModel, this.ConfigViewModel, this.SmartlinkViewModel, this.AdamViewModel);
                this.Table4 = new TableViewModel(this.MainModel, this.ZonenViewModel, this.ReglerZonenViewModel, this.ControlViewModel, this.ConfigViewModel, this.SmartlinkViewModel, this.AdamViewModel);
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
            catch (Exception exception)
            {
                Global.UserMsg(exception);
            }
        }

        public MainModel? MainModel { get; }

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
            this.MainModel.Sampler.ReadNextTemperaturLog();
            this.ControlViewModel.StartTimeAbsolut = this.MainModel.Sampler.LogStart;
            this.ControlViewModel.EndTimeAbsolut = this.MainModel.Sampler.LogEnd;
            this.ControlViewModel.CurrentTime = this.MainModel.Sampler.LogDuration;
            this.ControlViewModel.CurrentTimUntilEnd = "-";
            this.ShowLogData();
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
            this.Table1.SingleValuesGrafikViewModel.ShowLogdata(this.Table1.SingleValuesGrafikViewModel.CurrentZone);
            this.Table2.SingleValuesGrafikViewModel.ShowLogdata(this.Table2.SingleValuesGrafikViewModel.CurrentZone);
            this.Table3.SingleValuesGrafikViewModel.ShowLogdata(this.Table3.SingleValuesGrafikViewModel.CurrentZone);
            this.Table4.SingleValuesGrafikViewModel.ShowLogdata(this.Table4.SingleValuesGrafikViewModel.CurrentZone);
            this.Table1.ControlValuesGrafikViewModel.ShowLogdata(SamplerFile.STARTTABSETVALUES);
            this.Table2.ControlValuesGrafikViewModel.ShowLogdata(SamplerFile.STARTTABSETVALUES);
            this.Table3.ControlValuesGrafikViewModel.ShowLogdata(SamplerFile.STARTTABSETVALUES);
            this.Table4.ControlValuesGrafikViewModel.ShowLogdata(SamplerFile.STARTTABSETVALUES);
            this.Table1.CurrentValuesGrafikViewModel.ShowLogdata(SamplerFile.STARTTABTEMPERATURES);
            this.Table2.CurrentValuesGrafikViewModel.ShowLogdata(SamplerFile.STARTTABTEMPERATURES);
            this.Table3.CurrentValuesGrafikViewModel.ShowLogdata(SamplerFile.STARTTABTEMPERATURES);
            this.Table4.CurrentValuesGrafikViewModel.ShowLogdata(SamplerFile.STARTTABTEMPERATURES);
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
    }
}
