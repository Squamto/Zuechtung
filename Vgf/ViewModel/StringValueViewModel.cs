// -----------------------------------------------------------------------
// <copyright file="StringValueViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Vgf.ViewModel
{
    using Framework.ViewModel;

    /// <summary>
    /// Defines string value view model.
    /// </summary>
    public class StringValueViewModel : BaseViewModel
    {
        public StringValueViewModel(int sign) 
        { 
            this.Sign = sign;
        }

        public event EventHandler<int> ValueChanged;
        public string Value
        {
            get => this.Get<string>();
            set
            {
                this.Set(value);
                this.ValueChanged?.Invoke(this, this.Sign);
            }
        }

        public int Sign { get; }
    }
}
