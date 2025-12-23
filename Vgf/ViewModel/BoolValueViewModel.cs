// -----------------------------------------------------------------------
// <copyright file="BoolValueViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using Framework.ViewModel;

    /// <summary>
    /// Defines bool value view model.
    /// </summary>
    public class BoolValueViewModel : BaseViewModel
    {
        public BoolValueViewModel(int sign)
        {
            this.Sign = sign;
        }

        public event EventHandler<int> ValueChanged;

        public bool Value
        {
            get => this.Get<bool>();
            set
            {
                this.Set(value);
                this.ValueChanged?.Invoke(this, this.Sign);
            }
        }

        public int Sign { get; }
    }
}
