// -----------------------------------------------------------------------
// <copyright file="NameValueCheckBoxViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.ViewModel
{
	using System;

	/// <summary>
	/// Defines the view model for name value items with bool input (checkbox).
	/// </summary>
	public class NameValueCheckBoxViewModel : BaseViewModel
	{
		private readonly object sign;

		/// <summary>
		/// Initializes a new instance of the <see cref="NameValueInputViewModel"/> class.
		/// </summary>
		public NameValueCheckBoxViewModel(object sign = null)
		{
			this.sign = sign;
			this.IsEnabled = true;
		}

		public event EventHandler<object>? IsValueDataChanged;

		/// <summary>
		/// Gets or sets a value indicating whether the input view is enabled or not.
		/// </summary>
		public bool IsEnabled
		{
			get
			{
				return this.Get<bool>();
			}

			set
			{
				this.Set(value);
			}
		}

		/// <summary>
		/// Gets or sets the name data.
		/// </summary>
		public string NameData
		{
			get
			{
				return this.Get<string>();
			}

			set
			{
				this.Set(value);
				this.IsValueDataChanged?.Invoke(this, this.sign);
			}
		}

		/// <summary>
		/// Gets or sets the value data.
		/// </summary>
		public bool ValueData
		{
			get
			{
				return this.Get<bool>();
			}

			set
			{
				this.Set(value);
				this.IsValueDataChanged?.Invoke(this, this.sign);
			}
		}
	}
}
