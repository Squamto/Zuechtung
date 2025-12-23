// -----------------------------------------------------------------------
// <copyright file="LedViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.ViewModel
{
	using System.Windows.Media;

	/// <summary>
	/// Defines the led view model.
	/// </summary>
	public class LedViewModel : BaseViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LedViewModel"/> class.
		/// </summary>
		public LedViewModel()
		{
			this.ColorOn = Colors.Green.ToString();
			this.ColorOff = Colors.Red.ToString();
			this.ColorThree = Colors.Yellow.ToString();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LedViewModel"/> class.
		/// </summary>
		/// <param name="colorOn">The color of the on state.</param>
		/// <param name="colorOff">The color of the off state.</param>
		public LedViewModel(Color colorOn, Color colorOff, Color colorThree = default)
		{
			this.ColorOn = colorOn.ToString();
			this.ColorOff = colorOff.ToString();
			if (colorThree == default)
			{
				this.ColorThree = Colors.Yellow.ToString();
			}
			else
			{
				this.ColorThree = colorThree.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the color on.
		/// </summary>
		public string ColorOn
		{
			get
			{
				return this.Get<string>();
			}

			set
			{
				this.Set(value);
			}
		}

		/// <summary>
		/// Gets or sets the color off.
		/// </summary>
		public string ColorOff
		{
			get
			{
				return this.Get<string>();
			}

			set
			{
				this.Set(value);
			}
		}

		/// <summary>
		/// Gets or sets the color three.
		/// </summary>
		public string ColorThree
		{
			get
			{
				return this.Get<string>();
			}

			set
			{
				this.Set(value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the value is on or not.
		/// </summary>
		public bool Value
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
		/// Gets or sets a value indicating whether the color three is on or not.
		/// </summary>
		public bool IsColorThree
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
	}
}
