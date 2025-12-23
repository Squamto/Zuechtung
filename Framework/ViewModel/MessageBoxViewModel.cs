// -----------------------------------------------------------------------
// <copyright file="MessageBoxViewModel.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Framework.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Media;

	/// <summary>
	/// View model for the message box.
	/// </summary>
	public class MessageBoxViewModel : BaseViewModel
	{
		private readonly Queue<string> messages;
		private readonly object msgLocker;
		private string? logFileFullPath;
		private DateTime lastStandardAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxViewModel" /> class.
		/// </summary>
		public MessageBoxViewModel()
		{
			this.messages = new Queue<string>();
			this.Medium("Ok", "Cancel", string.Empty, string.Empty);
			this.msgLocker = new object();
			this.Button1Command = new RelayCommand(
				() => { this.Button1Action?.Invoke(); },
				p => { return true; });
			this.Button2Command = new RelayCommand(
				() => { this.Button2Action?.Invoke(); },
				p => { return true; });
			this.Button3Command = new RelayCommand(
				() => { this.Button3Action?.Invoke(); },
				p => { return true; });
			this.Button4Command = new RelayCommand(
				() => { this.Button4Action?.Invoke(); },
				p => { return true; });
			this.MessageTextColor = "Black";
			this.EnableExeutionLog(null);
		}

		public event EventHandler<bool>? VisiblityChanged;

		/// <summary>
		/// Gets or sets the message text.
		/// </summary>
		public string MessageText
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets the font size of the message text.
		/// </summary>
		public double FontSize
		{
			get => this.Get<double>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the message box is visible or not.
		/// </summary>
		public bool IsMessageBoxVisible
		{
			get => this.Get<bool>();
			set
			{
				this.Set(value);
				this.VisiblityChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the width of the box.
		/// </summary>
		public int BoxWidth
		{
			get => this.Get<int>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets the height of the box.
		/// </summary>
		public int BoxHeight
		{
			get => this.Get<int>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets and sets the opacity of the message box.
		/// </summary>
		public double Opacity
		{
			get => this.Get<double>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the button 1 is visible or not.
		/// </summary>
		public bool IsButton1Visible
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the button 2 is visible or not.
		/// </summary>
		public bool IsButton2Visible
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the button 3 is visible or not.
		/// </summary>
		public bool IsButton3Visible
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		public string MessageTextColor
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the button 4 is visible or not.
		/// </summary>
		public bool IsButton4Visible
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets text on button 1.
		/// </summary>
		public string TextButton1
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets text on button 2.
		/// </summary>
		public string TextButton2
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets text on button 3.
		/// </summary>
		public string TextButton3
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets or sets text on button 4.
		/// </summary>
		public string TextButton4
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		/// <summary>
		/// Gets the dialog result.
		/// </summary>
		public bool? DialogResult { get; private set; }

		/// <summary>
		/// Gets or sets a function that is called if the button 1 was pressed.
		/// </summary>
		public Action? Button1Action { get; set; }

		/// <summary>
		/// Gets or sets a function that is called if the button 2 was pressed.
		/// </summary>
		public Action? Button2Action { get; set; }

		/// <summary>
		/// Gets or sets a function that is called if the button 3 was pressed.
		/// </summary>
		public Action? Button3Action { get; set; }

		/// <summary>
		/// Gets or sets a function that is called if the button 4 was pressed.
		/// </summary>
		public Action? Button4Action { get; set; }

		/// <summary>
		/// Gets button 1 command.
		/// </summary>
		public RelayCommand Button1Command { get; }

		/// <summary>
		/// Gets button 2 command.
		/// </summary>
		public RelayCommand Button2Command { get; }

		/// <summary>
		/// Gets button 3 command.
		/// </summary>
		public RelayCommand Button3Command { get; }

		/// <summary>
		/// Gets button 4 command.
		/// </summary>
		public RelayCommand Button4Command { get; }

		/// <summary>
		/// Shows a standard dialog and waits for the result.
		/// </summary>
		/// <param name="message">The message to show.</param>
		/// <returns>True for yes, false for no and null for cancel.</returns>
		public bool? ShowDialog(string message, string text1 = "Yes", string text2 = "No", string text3 = "Cancel")
		{
			this.Medium(text1, text2, text3, null);
			this.SetStandardDialogAction();
			this.MessageText = message;
			this.IsMessageBoxVisible = true;
			while (this.IsMessageBoxVisible)
			{
				Thread.Sleep(100);
			}

			return this.DialogResult;
		}

		/// <summary>
		/// Shows a standrd message (not waiting until ok).
		/// </summary>
		/// <param name="message">The message to show.</param>
		public void ShowMessage(string message, string color = "Black")
		{
			Task.Factory.StartNew(() =>
			{
				lock (this.msgLocker)
				{
					this.MessageTextColor = color;
					if (((DateTime.Now - this.lastStandardAction) < TimeSpan.FromSeconds(5)))
					{
						if (message == this.MessageText)
						{
							return;
						}
					}

					string msg = message;
					if (this.IsMessageBoxVisible)
					{
						if (msg != this.MessageText)
						{
							if (this.messages.FirstOrDefault(o => o.Contains("Mehr als 10 Nachrichten.")) != null)
							{
								return;
							}

							if (this.messages.Count > 10)
							{
								msg = "Mehr als 10 Nachrichten.\r\n";
								msg += "Bitte lesen sie in der Log Datei!\r\n";
								msg += "Aktuelle Zeit: " + DateTime.Now + "\r\n";
								if (!string.IsNullOrEmpty(this.logFileFullPath))
								{
									msg += "Log-Datei:\r\n";
									msg += this.logFileFullPath + "\r\n";
								}

								this.messages.Enqueue(msg);
								return;
							}

							this.messages.Enqueue(msg);
						}

						return;
					}

					this.Medium("Ok", null, null, null);
					this.MessageText = msg;
					this.IsMessageBoxVisible = true;
				}
			});
		}

		/// <summary>
		/// Sets the propertys for an big message box.
		/// </summary>
		/// <param name="text1">The text for the ok buttom.</param>
		/// <param name="text2">The text for the cancel buttom.</param>
		/// <param name="text3">The text for the text1 buttom.</param>
		/// <param name="text4">The text for the text2 buttom.</param>
		public void Big(string text1, string text2, string text3, string text4)
		{
			this.SetStandardMessageAction();
			this.FontSize = 24;
			this.IsMessageBoxVisible = false;
			this.BoxWidth = 800;
			this.BoxHeight = 500;
			this.Opacity = 100;
			this.IsButton1Visible = !string.IsNullOrEmpty(text1);
			this.IsButton2Visible = !string.IsNullOrEmpty(text2);
			this.IsButton3Visible = !string.IsNullOrEmpty(text3);
			this.IsButton4Visible = !string.IsNullOrEmpty(text4);
			this.TextButton1 = text1;
			this.TextButton2 = text2;
			this.TextButton3 = text3;
			this.TextButton4 = text4;
		}

		/// <summary>
		/// Sets the propertys for an medium message box.
		/// </summary>
		/// <param name="text1">The text for the ok buttom.</param>
		/// <param name="text2">The text for the cancel buttom.</param>
		/// <param name="text3">The text for the text1 buttom.</param>
		/// <param name="text4">The text for the text2 buttom.</param>
		public void Medium(string text1, string text2, string text3, string text4)
		{
			this.SetStandardMessageAction();
			this.FontSize = 16;
			this.IsMessageBoxVisible = false;
			this.BoxWidth = 600;
			this.BoxHeight = 350;
			this.Opacity = 100;
			this.IsButton1Visible = !string.IsNullOrEmpty(text1);
			this.IsButton2Visible = !string.IsNullOrEmpty(text2);
			this.IsButton3Visible = !string.IsNullOrEmpty(text3);
			this.IsButton4Visible = !string.IsNullOrEmpty(text4);
			this.TextButton1 = text1;
			this.TextButton2 = text2;
			this.TextButton3 = text3;
			this.TextButton4 = text4;
		}

		/// <summary>
		/// Sets the propertys for an small message box.
		/// </summary>
		/// <param name="text1">The text for the ok buttom.</param>
		/// <param name="text2">The text for the cancel buttom.</param>
		/// <param name="text3">The text for the text1 buttom.</param>
		/// <param name="text4">The text for the text2 buttom.</param>
		public void Small(string text1, string text2, string text3, string text4)
		{
			this.SetStandardMessageAction();
			this.FontSize = 8;
			this.IsMessageBoxVisible = false;
			this.BoxWidth = 400;
			this.BoxHeight = 250;
			this.Opacity = 100;
			this.IsButton1Visible = !string.IsNullOrEmpty(text1);
			this.IsButton2Visible = !string.IsNullOrEmpty(text2);
			this.IsButton3Visible = !string.IsNullOrEmpty(text3);
			this.IsButton4Visible = !string.IsNullOrEmpty(text4);
			this.TextButton1 = text1;
			this.TextButton2 = text2;
			this.TextButton3 = text3;
			this.TextButton4 = text4;
		}

		/// <summary>
		/// Sets the standard actions for the message function.
		/// </summary>
		public void SetStandardMessageAction()
		{
			this.Button1Action = this.StandardAction;
			this.Button2Action = this.StandardAction;
			this.Button3Action = this.StandardAction;
			this.Button4Action = this.StandardAction;
		}

		/// <summary>
		/// Sets the standard actions for the dialog function.
		/// </summary>
		public void SetStandardDialogAction()
		{
			this.Button1Action = this.YesDialogAction;
			this.Button2Action = this.NoDialogAction;
			this.Button3Action = this.CancelDialogAction;
			this.Button4Action = this.StandardAction;
		}

		/// <summary>
		/// Set the path of the log file (only used for user message).
		/// </summary>
		/// <param name="path">The path.</param>
		public void SetLogFileFullPath(string path)
		{
			this.logFileFullPath = path;
		}

		/// <summary>
		/// The standard action for tzhe message fuction.
		/// </summary>
		private void StandardAction()
		{
			this.IsMessageBoxVisible = false;
			this.lastStandardAction = DateTime.Now;
			if (this.messages.Count > 0)
			{
				this.ShowMessage(this.messages.Dequeue());
			}
		}

		/// <summary>
		/// The standard action for the dialog function if the user press the yes button.
		/// </summary>
		private void YesDialogAction()
		{
			this.DialogResult = true;
			this.IsMessageBoxVisible = false;
		}

		/// <summary>
		/// The standard action for the dialog function if the user press the no button.
		/// </summary>
		private void NoDialogAction()
		{
			this.DialogResult = false;
			this.IsMessageBoxVisible = false;
		}

		/// <summary>
		/// The standard action for the dialog function if the user press the cancel button.
		/// </summary>
		private void CancelDialogAction()
		{
			this.DialogResult = null;
			this.IsMessageBoxVisible = false;
		}
	}
}
