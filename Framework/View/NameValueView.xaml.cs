// -----------------------------------------------------------------------
// <copyright file="NameValueView.xaml.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.View
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Interaction logic for NameValueView.xaml.
	/// </summary>
	public partial class NameValueView : UserControl
	{
		private static readonly DependencyProperty ValueNameProperty = DependencyProperty.Register(
			nameof(ValueName),
			typeof(string),
			typeof(NameValueView),
			new FrameworkPropertyMetadata("-"));

		private static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
			nameof(Unit),
			typeof(string),
			typeof(NameValueView),
			new FrameworkPropertyMetadata("-"));

		private static readonly DependencyProperty ValueNameWidthProperty = DependencyProperty.Register(
			nameof(ValueNameWidth),
			typeof(int),
			typeof(NameValueView),
			new FrameworkPropertyMetadata(200));

		private static readonly DependencyProperty ValueWidthProperty = DependencyProperty.Register(
			nameof(ValueWidth),
			typeof(int),
			typeof(NameValueView),
			new FrameworkPropertyMetadata(100));

		private static readonly DependencyProperty UnitWidthProperty = DependencyProperty.Register(
			nameof(UnitWidth),
			typeof(int),
			typeof(NameValueView),
			new FrameworkPropertyMetadata(0));

		public NameValueView()
		{
			this.InitializeComponent();
		}

		public string ValueName
		{
			get => (string)this.GetValue(ValueNameProperty);
			set => this.SetValue(ValueNameProperty, value);
		}

		public string Unit
		{
			get => (string)this.GetValue(UnitProperty);
			set => this.SetValue(UnitProperty, value);
		}

		public int ValueNameWidth
		{
			get => (int)this.GetValue(ValueNameWidthProperty);
			set => this.SetValue(ValueNameWidthProperty, value);
		}

		public int ValueWidth
		{
			get => (int)this.GetValue(ValueWidthProperty);
			set => this.SetValue(ValueWidthProperty, value);
		}

		public int UnitWidth
		{
			get => (int)this.GetValue(UnitWidthProperty);
			set => this.SetValue(UnitWidthProperty, value);
		}
	}
}
