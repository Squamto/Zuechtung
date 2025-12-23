// -----------------------------------------------------------------------
// <copyright file="ScrollHelper.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.View
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Defines a property for autoscroll to the end in a scroll viewer.
	/// </summary>
	public static class ScrollHelper
	{
		public static readonly DependencyProperty AutoScrollProperty =
			DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(ScrollHelper), new PropertyMetadata(false, AutoScrollPropertyChanged));

		public static bool GetAutoScroll(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoScrollProperty);
		}

		public static void SetAutoScroll(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoScrollProperty, value);
		}

		private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ScrollViewer? scrollViewer = d as ScrollViewer;

			if (scrollViewer != null && (bool)e.NewValue)
			{
				scrollViewer.ScrollToEnd();
			}
		}
	}
}
