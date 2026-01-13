// -----------------------------------------------------------------------
// <copyright file="ControlViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;
    using Config;
    using Framework.Helper;
    using Framework.ViewModel;
    using Model;
    using Model.FG;

    /// <summary>
    /// Defines the control view model.
    /// </summary>
    public class ControlViewModel : BaseViewModel
    {
        private const string DATETIMEFORMAT = "dd':'MM':'yyyy'     'HH':'mm':'ss";
        private readonly FifoUniqueEntries fifoUniqueEntries;
        private double maxTemperatureGradientKelvinPerHour;
        private double minTemperatureCheckGradient;
        private MainModel mainModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlViewModel"/> class.
        /// </summary>
        public ControlViewModel(MainModel mainModel)
        {
            this.mainModel = mainModel;
            this.Channels = mainModel.Channels;
            this.mainModel.IsConnectededChanged += this.OnMainModelIsConnectededChanged;
            this.Steps = new DispatchedObservableCollection<int>(this.Dispatcher);
            this.Channels.CurrentStepChanged += this.OnChannelsCurrentStepChanged;
            this.Channels.CurrentCycleChanged += this.OnChannelsCurrentCycleChanged;
            this.Channels.ControlStateChanged += this.OnChannelsControlStateChanged;
            this.ControlValueSteps = [];
            this.ControlValueSteps.CollectionChanged += this.ControlValueStepsCollectionChanged;
            this.StopCommand = new RelayCommand(this.Channels.Stop, (o) => true, (e) => Global.UserMsg(e));
            this.StartCommand = new RelayCommand(this.Start, (o) => (this.ControlValueSteps.Count > 0) && (!this.IsBusy) && this.mainModel.SmartlinkModel.IsConnectd, Global.UserMsg);
            this.PauseCommand = new RelayCommand(this.Channels.Pause, (o) => (this.Channels.ControlState == ControlStates.Run), Global.UserMsg);
            this.FurtherCommand = new RelayCommand(this.Further, (o) => (this.Channels.ControlState == ControlStates.Pause) && this.mainModel.SmartlinkModel.IsConnectd, Global.UserMsg);
            this.StopAndHoldCommand = new RelayCommand(this.Channels.StopAndHold, (o) => (this.Channels.ControlState == ControlStates.Run), Global.UserMsg);
            this.CheckGradientCommand = new RelayCommand(() => this.CheckGradientOfTable(), (o) => this.ControlValueSteps.Count > 0, Global.UserMsg);
            this.LimitGradientCommand = new RelayCommand(this.LimitGradientOfTable, (o) => ((this.ControlValueSteps.Count > 0) && !this.IsBusy), Global.UserMsg);
            this.WriteControlValueStepsXmlCommand = new RelayCommand(() => this.WriteControlValueSteps(true), (o) => this.ControlValueSteps.Count > 0, Global.UserMsg);
            this.ReadControlValueStepsXmlCommand = new RelayCommand(() => this.ReadControlValueSteps(true), (o) => !this.IsBusy, Global.UserMsg);
            this.ReadControlValueStepsTxtCommand = new RelayCommand(() => this.ReadControlValueSteps(false), (o) => !this.IsBusy, Global.UserMsg);
            this.WriteControlValueStepsTxtCommand = new RelayCommand(() => this.WriteControlValueSteps(false), (o) => this.ControlValueSteps.Count > 0, Global.UserMsg);
            this.ReadLastControlValuesFileName1Command = new RelayCommand(() => this.ReadControlValueSteps(this.LastControlValuesFileName1), (o) => !this.IsBusy, Global.UserMsg);
            this.ReadLastControlValuesFileName2Command = new RelayCommand(() => this.ReadControlValueSteps(this.LastControlValuesFileName2), (o) => !this.IsBusy, Global.UserMsg);
            this.ReadLastControlValuesFileName3Command = new RelayCommand(() => this.ReadControlValueSteps(this.LastControlValuesFileName3), (o) => !this.IsBusy, Global.UserMsg);
            this.ReadLastControlValuesFileName4Command = new RelayCommand(() => this.ReadControlValueSteps(this.LastControlValuesFileName4), (o) => !this.IsBusy, Global.UserMsg);

            this.TableKeyUpCommand = new RelayCommand((o) => this.TableKeyUp(o), (o) => true, (e) => Global.UserMsg(e));
            this.CurrentState = ControlStates.Stop.ToString();
            this.CurrentStep = "0";
            this.CurrentCycle = "0";
            this.CurrentTime = "00:00:00:00";
            this.CurrentTimUntilEnd = "00:00:00:00";
            this.LastControlValuesFileName1 = Conf.I.GetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LastControlValuesFileName1));
            this.LastControlValuesFileName2 = Conf.I.GetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LastControlValuesFileName2));
            this.LastControlValuesFileName3 = Conf.I.GetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LastControlValuesFileName3));
            this.LastControlValuesFileName4 = Conf.I.GetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LastControlValuesFileName4));
            this.maxTemperatureGradientKelvinPerHour = Conf.I.GetValueAsDouble(ConfigNames.ValDeviceBase(AreaBaseConfig.MaxTemperatureGradientKelvinPerHour));
            this.minTemperatureCheckGradient = Conf.I.GetValueAsDouble(ConfigNames.ValDeviceBase(AreaBaseConfig.MinTemperatureCheckGradient));
            this.fifoUniqueEntries = new FifoUniqueEntries(4);
            this.fifoUniqueEntries.Enqueue(this.LastControlValuesFileName4);
            this.fifoUniqueEntries.Enqueue(this.LastControlValuesFileName3);
            this.fifoUniqueEntries.Enqueue(this.LastControlValuesFileName2);
            this.fifoUniqueEntries.Enqueue(this.LastControlValuesFileName1);
            this.EnableExeutionLog(Global.LogInfo);
        }

        public FgChannels Channels { get; }

        public ObservableCollection<ControlValueStepViewModel> ControlValueSteps { get; }

        public bool IsBusy { get => this.Get<bool>(); set => this.Set(value);}

        public string CurrentState { get => this.Get<string>(); set => this.Set(value); }

        public string CurrentStep { get => this.Get<string>(); set => this.Set(value); }

        public string CurrentCycle { get => this.Get<string>(); set => this.Set(value); }

        public string CurrentTime { get => this.Get<string>(); set => this.Set(value); }

        public string CurrentTimUntilEnd { get => this.Get<string>(); set => this.Set(value);}

        public DispatchedObservableCollection<int> Steps { get => this.Get<DispatchedObservableCollection<int>>(); set => this.Set(value); }

        public string StartTimeAbsolut
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string EndTimeAbsolut
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string LastControlValuesFileName1
        {
            get => this.Get<string>();
            set
            {
                this.Set(value);
                Conf.I.SetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LastControlValuesFileName1), value);
            }
        }

        public string LastControlValuesFileName2
        {
            get => this.Get<string>();
            set
            {
                this.Set(value);
                Conf.I.SetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LastControlValuesFileName2), value);
            }
        }

        public string LastControlValuesFileName3
        {
            get => this.Get<string>();
            set
            {
                this.Set(value);
                Conf.I.SetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LastControlValuesFileName3), value);
            }
        }

        public string LastControlValuesFileName4
        {
            get => this.Get<string>();
            set
            {
                this.Set(value);
                Conf.I.SetValue(ConfigNames.ValDeviceBase(AreaBaseConfig.LastControlValuesFileName4), value);
            }
        }

        public bool IsInputZyklenChecked
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.IsInputStundenChecked = false;
                this.IsInputMinutenChecked = false;
            }
}

        public bool IsInputStundenChecked
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.IsInputZyklenChecked = false;
                this.IsInputMinutenChecked = false;
            }
        }

        public bool IsInputMinutenChecked
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.IsInputZyklenChecked = false;
                this.IsInputStundenChecked = false;
            }
        }
        
        public RelayCommand StopCommand { get; }

        public RelayCommand StartCommand { get; }

        public RelayCommand PauseCommand { get; }

        public RelayCommand FurtherCommand { get; }

        public RelayCommand StopAndHoldCommand { get; }
        
        public RelayCommand CheckGradientCommand { get; }

        public RelayCommand LimitGradientCommand { get; }

        public RelayCommand WriteControlValueStepsXmlCommand { get; }

        public RelayCommand ReadControlValueStepsXmlCommand { get; }

        public RelayCommand ReadControlValueStepsTxtCommand { get; }

        public RelayCommand WriteControlValueStepsTxtCommand { get; }

        public RelayCommand TableKeyUpCommand { get; }

        public RelayCommand ReadLastControlValuesFileName1Command { get; }
        public RelayCommand ReadLastControlValuesFileName2Command { get; }
        public RelayCommand ReadLastControlValuesFileName3Command { get; }
        public RelayCommand ReadLastControlValuesFileName4Command { get; }

        public void Init()
        {
            this.ReadControlValueSteps(this.LastControlValuesFileName1);
        }

        private void Start()
        {
            this.RefreshStepsInModel();

            // Write Steps to Log
            {
                ControlValueSteps steps = new();
                foreach(ControlValueStepViewModel step in this.ControlValueSteps) {
                    steps.Steps.Add(step.AsStep());
                }
                Global.LogDebug("Current Config:", "\r\n" + steps.ConvertToString());
            }

            // Set Windows Sleep Options
            try {
                if(Conf.I.GetValueAsBool(ConfigNames.ValDeviceBase(AreaBaseConfig.PreventDisplayOffDuringRun))) {
                    if(!WindowsPowerManager.IsAvailable()) {
                        throw new Exception("Windows Standby konnte nicht deaktiviert werden!");
                    } else {
                        WindowsPowerManager.PreventSleepAndDisplayOff();
                    }
                } else if(Conf.I.GetValueAsBool(ConfigNames.ValDeviceBase(AreaBaseConfig.PreventWindowsSleepDuringRun))) {
                    if(!WindowsPowerManager.IsAvailable()) {
                        throw new Exception("Windows Standby konnte nicht deaktiviert werden!");
                    } else {
                        WindowsPowerManager.PreventSleep();
                    }
                }
            } catch(Exception ex) {
                Global.UserMsg(ex);
            }

            if (this.CheckGradientOfTable())
            {
                Global.UserMsg("Der Führungsgrößengeneraor kann nicht gestartet werden, weil im geplatnten Temperaturregime zu hohe Temperaturgradienten enthalten sind.");
                return;
            }
            this.StartTimeAbsolut = DateTime.Now.ToString(ControlViewModel.DATETIMEFORMAT);
            this.Channels.Start();
        }

        private void Further()
        {
            this.Channels.Further();
            this.EndTimeAbsolut = (DateTime.Now + this.Channels.GetTimeToEnd()).ToString(ControlViewModel.DATETIMEFORMAT);
        }


        private void TableKeyUp(object param)
        {
            KeyEventArgs key = param as KeyEventArgs;
            if (key != null)
            {
                if ((key.Key == Key.Enter) || (key.Key == Key.Tab))
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(50);
                        this.RefreshStepsInModel();
                    });
                }
            }
        }

        private void RefreshStepsInModel()
        {
            try
            {
                this.Steps.Clear();
                int i = 1;
                TimeSpan timeKumulativ = TimeSpan.FromSeconds(0);
                foreach (ControlValueStepViewModel step in this.ControlValueSteps)
                {
                    step.Step = i;
                    TimeSpan st = TimeSpan.FromSeconds(step.Cycles);
                    timeKumulativ += st;
                    step.StepTime = st.ToString();
                    step.StepTimeKumulativ = timeKumulativ.ToString();
                    this.Steps.Add(i);
                    i++;
                }

                this.Channels.Steps.Clear();
                foreach (ControlValueStepViewModel step in this.ControlValueSteps)
                {
                    this.Channels.Steps.Add(new StepChannels(
                        step.Cycles,
                        step.Zone1,
                        step.Zone2,
                        step.Zone3,
                        step.Zone4,
                        step.Zone5,
                        step.Zone6,
                        step.Zone7));
                }
                this.Channels.StepsRefreshed();
            }
            catch (Exception ex) 
            { 
                Global.UserMsg(ex);
            }
        }

        private void LimitGradientOfTable()
        {
            ControlValueStepViewModel lastStep = null;
            foreach (ControlValueStepViewModel step in this.ControlValueSteps)
            {
                if (lastStep != null)
                {
                    this.LinitGradient(lastStep, step);
                }
                lastStep = step;
            }
            this.RefreshStepsInModel();
        }

        private void LinitGradient(ControlValueStepViewModel lastStep, ControlValueStepViewModel step)
        {
            lastStep.Cycles = this.LimitZonenGradient(lastStep.Zone1, step.Zone1, lastStep.Cycles);
            lastStep.Cycles = this.LimitZonenGradient(lastStep.Zone2, step.Zone2, lastStep.Cycles);
            lastStep.Cycles = this.LimitZonenGradient(lastStep.Zone3, step.Zone3, lastStep.Cycles);
            lastStep.Cycles = this.LimitZonenGradient(lastStep.Zone4, step.Zone4, lastStep.Cycles);
            lastStep.Cycles = this.LimitZonenGradient(lastStep.Zone5, step.Zone5, lastStep.Cycles);
            lastStep.Cycles = this.LimitZonenGradient(lastStep.Zone6, step.Zone6, lastStep.Cycles);
            lastStep.Cycles = this.LimitZonenGradient(lastStep.Zone7, step.Zone7, lastStep.Cycles);
        }

        private int LimitZonenGradient(double lastTemp, double temp, int cycles)
        {
            double limitedCycles = cycles;
            double grad = Math.Abs((lastTemp - temp) / ((cycles) / 3600.0));
            if ((grad > this.maxTemperatureGradientKelvinPerHour) && (temp >= this.minTemperatureCheckGradient))
            {
                limitedCycles = (((lastTemp-temp) * 3600) + this.maxTemperatureGradientKelvinPerHour) / this.maxTemperatureGradientKelvinPerHour;
                limitedCycles = Math.Abs(Math.Round(Math.Ceiling(limitedCycles + 1) / 10.0) * 10.0);
            }
            return (int)limitedCycles;
        }

        private bool CheckGradientOfTable()
        {
            bool isError = false;
            int i = 1;
            ControlValueStepViewModel lastStep = null;
            foreach (ControlValueStepViewModel step in this.ControlValueSteps)
            {
                if (lastStep != null)
                {
                    if (this.CheckGradient(lastStep, step, i))
                    {
                        isError = true; 
                    }
                }
                lastStep = step;
                i++;
            }
            return isError;
        }

        private bool CheckGradient(ControlValueStepViewModel lastStep, ControlValueStepViewModel step, int stepNr)
        {
            bool isError = false;
            this.CheckZonenGradient(1, lastStep.Zone1, step.Zone1, lastStep.Cycles, stepNr, ref isError);
            this.CheckZonenGradient(2, lastStep.Zone2, step.Zone2, lastStep.Cycles, stepNr, ref isError);
            this.CheckZonenGradient(3, lastStep.Zone3, step.Zone3, lastStep.Cycles, stepNr, ref isError);
            this.CheckZonenGradient(4, lastStep.Zone4, step.Zone4, lastStep.Cycles, stepNr, ref isError);
            this.CheckZonenGradient(5, lastStep.Zone5, step.Zone5, lastStep.Cycles, stepNr, ref isError);
            this.CheckZonenGradient(6, lastStep.Zone6, step.Zone6, lastStep.Cycles, stepNr, ref isError);
            this.CheckZonenGradient(7, lastStep.Zone7, step.Zone7, lastStep.Cycles, stepNr, ref isError);
            return isError;
        }

        private void CheckZonenGradient(int zNr, double lastTemp, double temp, int cycles, int stepNr, ref bool isError)
        {
            double grad = (Math.Floor(Math.Abs((lastTemp - temp) / ((cycles) / 3600.0)) * 100)) / 100;
            if ((grad > this.maxTemperatureGradientKelvinPerHour) && (temp >= this.minTemperatureCheckGradient))
            {
                Global.UserMsg($"Der Gradient in Step {stepNr} Zone{zNr} größer als {this.maxTemperatureGradientKelvinPerHour}K/h\nund die Endtemperatur ist größer als {this.minTemperatureCheckGradient}°C\n\n"
                    + $"Starttemperatur: {lastTemp}°C\tEndtemperatur: {temp}°C\nZyklen: {cycles}\tSchrittzeit: {cycles}s\nGradient: {grad:F2}K/h");
                isError = true;
            }
        }

        private void WriteControlValueSteps(bool isXml)
        {
            string filename = Global.SaveFileDialog(isXml);
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            this.SaveLastControlValueFileName(filename);
            ControlValueSteps steps = new();
            foreach (ControlValueStepViewModel step in this.ControlValueSteps)
            {
                steps.Steps.Add(step.AsStep());
            }
            if (isXml)
            {
                steps.ConvertToXml(filename);
            }
            else
            {
                steps.ConvertToTxt(filename);
            }
        }

        private void ReadControlValueSteps(string filename)
        {
            this.SaveLastControlValueFileName(filename);
            ControlValueSteps steps = new(filename);
            this.ControlValueSteps.Clear();
            foreach (ControlValueStep controlValueStep in steps.Steps)
            {
                this.ControlValueSteps.Add(new ControlValueStepViewModel(controlValueStep));
            }
            this.RefreshStepsInModel();
        }

        private void ReadControlValueSteps(bool isXml)
        {
            string filename = Global.OpenFileDialog(isXml);
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            this.ReadControlValueSteps(filename);
        }

        private static string ConvertTimeToString(TimeSpan timeSpan)
        {
            string days = timeSpan.Days.ToString().PadLeft(2, '0');
            string hours = timeSpan.Hours.ToString().PadLeft(2, '0');
            string minutes = timeSpan.Minutes.ToString().PadLeft(2, '0');
            string seconds = timeSpan.Seconds.ToString().PadLeft(2, '0');
            return $"{days}:{hours}:{minutes}:{seconds}";
        }

        private void SaveLastControlValueFileName(string filename)
        {
            //string save1 = string.Empty;
            //if (filename != this.LastControlValuesFileName1)
            //{
            //    save1 = this.LastControlValuesFileName1;
            //    this.LastControlValuesFileName1 = filename;
            //}
            //string save2 = string.Empty;
            //if (save1 != this.LastControlValuesFileName2)
            //{
            //    save2 = this.LastControlValuesFileName2;
            //    this.LastControlValuesFileName2 = save1;
            //}
            //if (save2 != this.LastControlValuesFileName3)
            //{
            //    save1 = this.LastControlValuesFileName3;
            //    this.LastControlValuesFileName3 = save2;
            //}
            //if (save1 != this.LastControlValuesFileName4)
            //{
            //    this.LastControlValuesFileName4 = save1;
            //}
            this.fifoUniqueEntries.Enqueue(filename);
            this.LastControlValuesFileName1 = this.fifoUniqueEntries.GetElementAt(3);
            this.LastControlValuesFileName2 = this.fifoUniqueEntries.GetElementAt(2);
            this.LastControlValuesFileName3 = this.fifoUniqueEntries.GetElementAt(1);
            this.LastControlValuesFileName4 = this.fifoUniqueEntries.GetElementAt(0);
        }

        private void OnChannelsControlStateChanged(object sender, ControlStates e)
        {
            this.CurrentState = e.ToString();
            this.IsBusy = (this.Channels.ControlState == ControlStates.Run)
                || (this.Channels.ControlState == ControlStates.RequestStop)
                || (this.Channels.ControlState == ControlStates.Pause);
            RequeryCommands();
        }

        private void OnChannelsCurrentStepChanged(object sender, int e)
        {
            this.CurrentStep = (e + 1).ToString();
        }

        private void OnChannelsCurrentCycleChanged(object sender, int e)
        {
            this.CurrentCycle = e.ToString();
            this.CurrentTime = ConvertTimeToString(this.Channels.GetRunTime());
            this.CurrentTimUntilEnd = ConvertTimeToString(this.Channels.GetTimeToEnd());
            this.EndTimeAbsolut = (DateTime.Now + this.Channels.GetTimeToEnd()).ToString(ControlViewModel.DATETIMEFORMAT);
        }

        private void ControlValueStepsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RefreshStepsInModel();
            RequeryCommands();
        }

        private void OnMainModelIsConnectededChanged(object? sender, bool e)
        {
            RequeryCommands();
        }
    }
}
