// -----------------------------------------------------------------------
// <copyright file="NameValueCheckBoxView.xaml.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.View
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Interaction logic for NameValueCheckBoxView.xaml.
	/// </summary>
	public partial class NameValueCheckBoxView : UserControl
	{
		private static readonly DependencyProperty ValueNameCheckProperty = DependencyProperty.Register(
			nameof(ValueNameCheck),
			typeof(string),
			typeof(NameValueCheckBoxView),
			new FrameworkPropertyMetadata("-"));

		private static readonly DependencyProperty ValueNameWidthCheckProperty = DependencyProperty.Register(
			nameof(ValueNameWidthCheck),
			typeof(int),
			typeof(NameValueCheckBoxView),
			new FrameworkPropertyMetadata(200));

		private static readonly DependencyProperty ValueWidthCheckProperty = DependencyProperty.Register(
			nameof(ValueWidthCheck),
			typeof(int),
			typeof(NameValueCheckBoxView),
			new FrameworkPropertyMetadata(100));

		public NameValueCheckBoxView()
		{
			this.InitializeComponent();
		}

		public string ValueNameCheck
		{
			get => (string)this.GetValue(ValueNameCheckProperty);
			set => this.SetValue(ValueNameCheckProperty, value);
		}

		public int ValueNameWidthCheck
		{
			get => (int)this.GetValue(ValueNameWidthCheckProperty);
			set => this.SetValue(ValueNameWidthCheckProperty, value);
		}

		public int ValueWidthCheck
		{
			get => (int)this.GetValue(ValueWidthCheckProperty);
			set => this.SetValue(ValueWidthCheckProperty, value);
		}
	}
}
