// -----------------------------------------------------------------------
// <copyright file="ControlValueStepViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using Framework.ViewModel;
    using Model.FG;

    /// <summary>
    /// Defines the control view model.
    /// </summary>
    public class ControlValueStepViewModel : BaseViewModel
    {
        public ControlValueStepViewModel()
        {

        }

        public ControlValueStepViewModel(ControlValueStep step)
        {
            this.Cycles = step.Cycles;
            this.Zone1 = step.Zone1;
            this.Zone2 = step.Zone2;
            this.Zone3 = step.Zone3;
            this.Zone4 = step.Zone4;
            this.Zone5 = step.Zone5;
            this.Zone6 = step.Zone6;
            this.Zone7 = step.Zone7;
        }

        public ControlValueStep AsStep()
        {
            ControlValueStep step = new ControlValueStep();
            step.Cycles = this.Cycles;
            step.Zone1 = this.Zone1;
            step.Zone2 = this.Zone2;
            step.Zone3 = this.Zone3;
            step.Zone4 = this.Zone4;
            step.Zone5 = this.Zone5;
            step.Zone6 = this.Zone6;
            step.Zone7 = this.Zone7;
            return step;
        }

        public int Step
        {
            get => this.Get<int>();
            set => this.Set(value);
        }

        public int Cycles
        {
            get => this.Get<int>();
            set
            {
                this.Set(value);
                this.OnNotifyPropertyChanged(nameof(this.Hours));
                this.OnNotifyPropertyChanged(nameof(this.Minutes));
            }
        }

        public double Hours
        {
            set
            { 
                this.Cycles = (int)(value * 3600.0);
            }
            get
            {
                return this.Cycles / 3600.0;
            }
        }

        public double Minutes
        {
            set
            {
                this.Cycles = (int)(value * 60.0);
            }
            get
            {
                return this.Cycles / 60.0;
            }
        }

        public string StepTime
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public string StepTimeKumulativ
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public double Zone1
        {
            get => this.Get<double>();
            set => this.Set(value);
        }

        public double Zone2
        {
            get => this.Get<double>();
            set => this.Set(value);
        }

        public double Zone3
        {
            get => this.Get<double>();
            set => this.Set(value);
        }

        public double Zone4
        {
            get => this.Get<double>();
            set => this.Set(value);
        }

        public double Zone5
        {
            get => this.Get<double>();
            set => this.Set(value);
        }

        public double Zone6
        {
            get => this.Get<double>();
            set => this.Set(value);
        }

        public double Zone7
        {
            get => this.Get<double>();
            set => this.Set(value);
        }
    }
}
