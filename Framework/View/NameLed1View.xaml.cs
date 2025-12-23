// -----------------------------------------------------------------------
// <copyright file="NameLed1View.xaml.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.View
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Interaction logic for NameLed1View.xaml.
	/// </summary>
	public partial class NameLed1View : UserControl
	{
		private static readonly DependencyProperty LedValueNameProperty = DependencyProperty.Register(
			nameof(LedValueName),
			typeof(string),
			typeof(NameValueView),
			new FrameworkPropertyMetadata("-"));

		private static readonly DependencyProperty LedValueNameWidthProperty = DependencyProperty.Register(
			nameof(LedValueNameWidth),
			typeof(int),
			typeof(NameValueView),
			new FrameworkPropertyMetadata(200));

		private static readonly DependencyProperty LedValueWidthProperty = DependencyProperty.Register(
			nameof(LedValueWidth),
			typeof(int),
			typeof(NameValueView),
			new FrameworkPropertyMetadata(100));

		public NameLed1View()
		{
			this.InitializeComponent();
		}

		public string LedValueName
		{
			get => (string)this.GetValue(LedValueNameProperty);
			set => this.SetValue(LedValueNameProperty, value);
		}

		public int LedValueNameWidth
		{
			get => (int)this.GetValue(LedValueNameWidthProperty);
			set => this.SetValue(LedValueNameWidthProperty, value);
		}

		public int LedValueWidth
		{
			get => (int)this.GetValue(LedValueWidthProperty);
			set => this.SetValue(LedValueWidthProperty, value);
		}
	}
}
