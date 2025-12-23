// -----------------------------------------------------------------------
// <copyright file="Conf.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Config
{
	using System;
    using System.Collections.Generic;
    using System.IO;
	using Framework.Helper;

	/// <summary>
	/// Defining a clas to read, hold and write all config-values.
	/// </summary>
	public class Conf
	{
		private static Conf? conf;
		private readonly XMLFile file;

		/// <summary>
		/// Initializes a new instance of the <see cref="Conf"/> class.
		/// Deny a public constructor for the <see cref="Conf"/> class.
		/// </summary>
		private Conf()
		{
			this.file = new XMLFile();
			this.file.UserInformationCreated += this.OnFileUserInformationCreated;
		}

		/// <summary>
		/// Created event of a user message.
		/// </summary>
		public event EventHandler<string>? UserMessageCreated;

		/// <summary>
		/// Gets the instance of the <see cref="Conf"></see>.
		/// </summary>
		public static Conf I
		{
			get
			{
				if (conf == null)
				{
					conf = new Conf();
				}

				return conf;
			}
		}

		/// <summary>
		/// Gets the full path to the folder which contains data files and configuration of the application.
		/// </summary>
		public static string AppdataFolder => AppDomain.CurrentDomain.BaseDirectory;

		/// <summary>
		/// Gets the full path to the configuration data of the application.
		/// </summary>
		public static string ConfigDataFullPath => Path.Combine(AppdataFolder, "Config", "configdata.xml");

        public List<NameValueItem> Values => this.file.Values;

		public string? CurrentFileName => this.file.FileName;

		/// <summary>
		/// Init the config data with reading all parameters from starrd filename.
		/// </summary>
		public bool Init()
		{
            bool rv = false;
			if (!File.Exists(Conf.ConfigDataFullPath))
			{
				Directory.CreateDirectory(Path.Combine(AppdataFolder, "Config"));
				File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "defaultconfig.xml"), Conf.ConfigDataFullPath);
                rv = true;
			}

			this.file.ReadAllAndSaveFilename(Conf.ConfigDataFullPath);
            return rv;
		}

		/// <summary>
		/// Read all config values from file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public void ReadAllAndSaveFileName(string filename)
		{
			this.file.ReadAllAndSaveFilename(filename);
		}

		/// <summary>
		/// Read all config values from file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public void ReadAll(string filename)
		{
			this.file.ReadAll(filename);
		}

		/// <summary>
		/// Write all config values to file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public void WriteAll(string filename)
		{
			this.file.WriteAll(filename);
		}

		/// <summary>
		/// Write all config values to file.
		/// </summary>
		public void WriteAll()
		{
			this.WriteAll(Conf.ConfigDataFullPath);
		}

		/// <summary>
		/// Get one generic value, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public T GetValue<T>(string name)
		{
            return typeof(T).Name switch
            {
                nameof(String) => (T)(object)this.GetValue(name),
                nameof(Boolean) => (T)(object)this.GetValueAsBool(name),
                nameof(Int32) => (T)(object)this.GetValueAsInt(name),
                nameof(Double) => (T)(object)this.GetValueAsDouble(name),
                _ => (T)(object)this.GetValue(name),
            };
        }

		/// <summary>
		/// Get one value as string, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public string GetValue(string name)
		{
			return this.file.GetValue(name);
		}

		/// <summary>
		/// Get one value as bool, if not exist create a new.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public bool GetValueAsBool(string name)
		{
			return this.file.GetValueAsBool(name);
		}

		/// <summary>
		/// Get one value as int, if not exist create a new.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public int GetValueAsInt(string name)
		{
			return this.file.GetValueAsInt(name);
		}

		/// <summary>
		/// Get one value as double, if not exist create a new.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public double GetValueAsDouble(string name)
		{
			return this.file.GetValueAsDouble(name);
		}

		/// <summary>
		/// Set one generic value, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValue<T>(string name, T value)
		{
			this.file.SetValue(name, value.ToString());
		}

		/// <summary>
		/// Set one value as string, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValue(string name, string value)
		{
			this.file.SetValue(name, value);
		}

		/// <summary>
		/// Set one value as bool, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValueAsBool(string name, bool value)
		{
			this.file.SetValueAsBool(name, value);
		}

		/// <summary>
		/// Set one value as int, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValueAsInt(string name, int value)
		{
			this.file.SetValueAsInt(name, value);
		}

		/// <summary>
		/// Set one value as double, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValueAsDouble(string name, double value)
		{
			this.file.SetValueAsDouble(name, value);
		}

		/// <summary>
		/// User informations created event the file.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The unser informations.</param>
		private void OnFileUserInformationCreated(object sender, string e)
		{
			this.UserMessageCreated?.Invoke(this, e);
		}
	}
}
