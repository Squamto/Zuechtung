// -----------------------------------------------------------------------
// <copyright file="BoolToVisibilityConverter.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Framework.ViewModel
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// Helper to invert boolean values in xaml.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class BoolToVisibilityConverter : IValueConverter
	{
		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value == DependencyProperty.UnsetValue || value.GetType() != typeof(bool))
			{
				return Visibility.Collapsed;
			}

			bool inverse = false;

			if (parameter is string)
			{
				bool.TryParse(parameter.ToString(), out inverse);
			}

			bool result = (bool)value;

			if (result)
			{
				if (inverse)
				{
					return Visibility.Collapsed;
				}

				return Visibility.Visible;
			}

			if (inverse)
			{
				return Visibility.Visible;
			}

			return Visibility.Collapsed;
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("Only one-way converter. Cannot convert back.");
		}
	}
}
