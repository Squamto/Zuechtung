// -----------------------------------------------------------------------
// <copyright file="TextBoxTabSize.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.View
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;

	/// <summary>
	/// Defines a text box with tab size property.
	/// </summary>
	public class TextBoxTabSize : TextBox
	{
		public TextBoxTabSize()
		{
			// Defaults to 4
			this.TabSize = 4;
		}

		public int TabSize { get; set; }

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Tab)
			{
				string tab = new string(' ', this.TabSize);
				int caretPosition = this.CaretIndex;
				this.Text = this.Text.Insert(caretPosition, tab);
				this.CaretIndex = caretPosition + this.TabSize;
				e.Handled = true;
			}
		}
	}
}
