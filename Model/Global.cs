// -----------------------------------------------------------------------
// <copyright file="Global.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Model
{
	using System;
	using System.IO;
	using Framework;
	using Framework.Helper;
	using Config;
	using log4net;
	using log4net.Config;
	using Microsoft.Win32;

	/// <summary>
	/// Holding global values and functions for the project.
	/// </summary>
	public static class Global
	{
		private const int LOG_FILTER =
			((int)LogCategories.Always)
            | ((int)LogCategories.Config); 
		private static readonly ILog logger;

		/// <summary>
		/// Initializes static members of the <see cref="Global"/> class.
		/// </summary>
		static Global()
		{
			if (Started == DateTime.MinValue)
			{
				Started = DateTime.Now;
			}

			if (logger == null)
			{
				string config = "log4net.config";
				if (File.Exists(config))
				{
					// set common log path for application
					GlobalContext.Properties["LogPath"] = Conf.AppdataFolder;
					XmlConfigurator.Configure(new FileInfo(config));
				}
				else
				{
					throw new FileNotFoundException(config);
				}

				logger = LogManager.GetLogger(typeof(Global));
			}

			// Message from config
			Conf.I.UserMessageCreated += Global.OnConfigUserMessageCreated;
		}
        
		/// <summary>
        /// Gets the time when the application was started.
        /// </summary>
        public static DateTime Started { get; private set; }

		/// <summary>
		/// Gets the version of the application.
		/// </summary>
		public static Version? ApplicationVersion { get; private set; }

        /// <summary>
        /// Gets the apllication title.
        /// </summary>
        public static string ApplicationTitle
		{
			get
			{
				string title =
					"Vertical Gradient Freeze (VGF) Züchtung     " + "Version: " + ApplicationVersion;
				return title;
			}
		}

		/// <summary>
		/// Gets or sets and sets the action to write one user message to the screen.
		/// </summary>
		public static Action<string>? UserMsgAction { get; set; }

		/// <summary>
		/// Gets or sets and sets the fun to write a user dialog to the screen.
		/// </summary>
		public static Func<string, bool?>? UserDialogFunction { get; set; }

		/// <summary>
		/// Write a user message.
		/// </summary>
		/// <param name="text">The text of the message.</param>
		public static void UserMsg(string text)
		{
			if (UserMsgAction != null)
			{
				UserMsgAction.Invoke(text);
				LogInfo(LogCategories.Always, "UserMsg: ", text);
			}
		}

		/// <summary>
		/// Write a exception user message.
		/// </summary>
		/// <param name="exception">The exception.</param>
		public static void UserMsg(Exception exception)
		{
			UserMsgAction?.Invoke(StringHelper.GetExceptionText(exception));
			LogException(exception);
		}

		/// <summary>
		/// Open a user dialog, and wait for answer.
		/// </summary>
		/// <param name="text">The text of the message.</param>
		/// <returns>The answer.</returns>
		public static bool? UserDialog(string text)
		{
			bool? answerBool = null;
			string answerString = string.Empty;
			if (UserDialogFunction != null)
			{
				answerBool = UserDialogFunction.Invoke(text);
				if (answerBool.HasValue)
				{
					if (answerBool.Value)
					{
						answerString = "Yes";
					}
					else
					{
						answerString = "No";
					}
				}
				else
				{
					answerString = "Cancel";
				}
			}

			LogInfo(LogCategories.Always, "UserDialog", text + " Answer: " + answerString);
			return answerBool;
		}

		/// <summary>
		/// Log the given info text in the logfile.
		/// </summary>
		/// <param name="caption">The given caption.</param>
		/// <param name="text">The given text.</param>
		public static void LogInfo(LogCategories logCategories, string caption, string text)
		{
			if ((((int)logCategories & LOG_FILTER) != 0) || (logCategories == LogCategories.Always))
			{
				logger.Info($"{logCategories}\t{caption}\t{text}");
			}
		}

		/// <summary>
		/// Log the given debug text in the logfile.
		/// </summary>
		/// <param name="caption">The given caption.</param>
		/// <param name="text">The given text.</param>
		public static void LogDebug(string caption, string text)
		{
			logger.Debug(caption + "\t" + text);
		}

		/// <summary>
		/// Log the given exception text in the logfile.
		/// </summary>
		/// <param name="exception">The exception to log.</param>
		public static void LogException(Exception exception)
		{
			logger.Info(StringHelper.GetExceptionText(exception));
		}

		/// <summary>
		/// Opens a multi select file dialog.
		/// </summary>
		public static string[] OpenFileDialogMultiSelect()
		{
            // Configure open file dialog box.
            OpenFileDialog dlg = new()
            {
                DefaultExt = ".csv", // Default file extension
                Filter = "File (.csv)|*.csv", // Filter files by extension
                Multiselect = true
            };

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == true)
			{
				// Open document
				return dlg.FileNames;
			}

			return null;
		}

		/// <summary>
		/// Opens an OpenFile Dialog for files.
		/// </summary>
		/// <param name="isXml">True if the file type is xml, fals if the filetype is csv.</param>
		/// <returns>Filename to Open.</returns>
		public static string OpenFileDialog(bool isXml)
		{
            // Configure open file dialog box.
            OpenFileDialog dlg = new()
            {
                DereferenceLinks = false,
                AddExtension = false,
                CheckFileExists = false,
                CheckPathExists = false,
                Multiselect = false,
                ReadOnlyChecked = false,
                RestoreDirectory = false,
                ShowReadOnly = false,
                ValidateNames = false
            };

            if (isXml)
			{
				dlg.DefaultExt = ".xml"; // Default file extension
				dlg.Filter = "Automation Script File (.xml)|*.xml"; // Filter files by extension
			}
			else
			{
				dlg.DefaultExt = ".txt"; // Default file extension
				dlg.Filter = "txt File (.txt)|*.txt|All files (*.*)|*.*"; // Filter files by extension
			}

			// Show open file dialog box
			bool? result = dlg.ShowDialog();

			string filename = string.Empty;

			// Process open file dialog box results
			if (result == true)
			{
				// Open document
				filename = dlg.FileName;
			}

			return filename;
		}

		/// <summary>
		/// Opens a SaveFile Dialog and returns back a string filename.
		/// </summary>
		/// <param name="isXml">True if the file type is xml, fals if the filetype is csv.</param>
		/// <returns>The name of the file.</returns>
		public static string SaveFileDialog(bool isXml, string defaultFilename = "")
		{
            // Configure save file dialog box.
            SaveFileDialog dlg = new();
			if (isXml)
			{
				if (string.IsNullOrEmpty(defaultFilename))
				{
					dlg.FileName = "*.xml";
				}
				else
				{
					dlg.FileName = defaultFilename;
				}

				dlg.DefaultExt = ".xml"; // Default file extension
				dlg.Filter = "Automation File (.xml)|*.xml"; // Filter files by extension
			}
			else
			{
				if (string.IsNullOrEmpty(defaultFilename))
				{
					dlg.FileName = "*.txt";
				}
				else
				{
					dlg.FileName = defaultFilename;
				}

				dlg.DefaultExt = ".txt"; // Default file extension
				dlg.Filter = "TXT File (.txt)|*.txt"; // Filter files by extension
			}

			// Show save file dialog box.
			bool? result = dlg.ShowDialog();

			string filename = string.Empty;

			// Process save file dialog box results.
			if (result == true)
			{
				// Save document.
				filename = dlg.FileName;
			}

			return filename;
		}

		/// <summary>
		/// Reset the starttime of the programm.
		/// </summary>
		public static void ResetStartTime()
		{
			Started = DateTime.Now;
		}

		/// <summary>
		/// Set the application version.
		/// </summary>
		/// <param name="version">The version to set.</param>
		public static void SetApplicationVersion(Version version)
		{
			ApplicationVersion = version;
		}

		private static void OnConfigUserMessageCreated(object sender, string e)
		{
			Global.UserMsg(e);
		}
	}
}
