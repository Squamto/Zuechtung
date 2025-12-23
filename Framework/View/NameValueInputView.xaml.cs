// -----------------------------------------------------------------------
// <copyright file="NameValueInputView.xaml.cs" company="IB Hermann">
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
	public partial class NameValueInputView : UserControl
	{
#pragma warning disable IDE1006 // Naming Styles
		private static readonly DependencyProperty ValueNameInProperty = DependencyProperty.Register(
#pragma warning restore IDE1006 // Naming Styles
			nameof(ValueNameIn),
			typeof(string),
			typeof(NameValueView),
			new FrameworkPropertyMetadata("-"));

		private static readonly DependencyProperty UnitInProperty = DependencyProperty.Register(
			nameof(UnitIn),
			typeof(string),
			typeof(NameValueView),
			new FrameworkPropertyMetadata("-"));

		private static readonly DependencyProperty ValueNameWidthInProperty = DependencyProperty.Register(
			nameof(ValueNameWidthIn),
			typeof(int),
			typeof(NameValueView),
			new FrameworkPropertyMetadata(200));

		private static readonly DependencyProperty ValueWidthInProperty = DependencyProperty.Register(
			nameof(ValueWidthIn),
			typeof(int),
			typeof(NameValueView),
			new FrameworkPropertyMetadata(100));

		private static readonly DependencyProperty UnitWidthInProperty = DependencyProperty.Register(
			nameof(UnitWidthIn),
			typeof(int),
			typeof(NameValueView),
			new FrameworkPropertyMetadata(0));

		public NameValueInputView()
		{
			this.InitializeComponent();
		}

		public string ValueNameIn
		{
			get => (string)this.GetValue(ValueNameInProperty);
			set => this.SetValue(ValueNameInProperty, value);
		}

		public string UnitIn
		{
			get => (string)this.GetValue(UnitInProperty);
			set => this.SetValue(UnitInProperty, value);
		}

		public int ValueNameWidthIn
		{
			get => (int)this.GetValue(ValueNameWidthInProperty);
			set => this.SetValue(ValueNameWidthInProperty, value);
		}

		public int ValueWidthIn
		{
			get => (int)this.GetValue(ValueWidthInProperty);
			set => this.SetValue(ValueWidthInProperty, value);
		}

		public int UnitWidthIn
		{
			get => (int)this.GetValue(UnitWidthInProperty);
			set => this.SetValue(UnitWidthInProperty, value);
		}
	}
}
