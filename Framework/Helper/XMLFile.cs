// -----------------------------------------------------------------------
// <copyright file="XMLFile.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.Helper
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;

	/// <summary>
	/// Defining a class to read, and write xml-files.
	/// </summary>
	public class XMLFile
	{
		private readonly object valuesLock;
		private List<NameValueItem> values;
		private bool isCompleteFileNewCreated;
		private string? filename;

		/// <summary>
		/// Initializes a new instance of the <see cref="XMLFile"/> class.
		/// </summary>
		public XMLFile()
		{
			this.values = [];
			this.valuesLock = new object();
		}

		/// <summary>
		/// Gets a created event for user informations.
		/// </summary>
		public event EventHandler<string>? UserInformationCreated;

        public List<NameValueItem> Values => this.values;

		public string? FileName => this.filename;

		/// <summary>
		/// Refresh all values from file, with before saved filename.
		/// </summary>
		public void Refresh()
		{
			this.ReadAll(this.filename);
		}

		/// <summary>
		/// Read all values from file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public void ReadAllAndSaveFilename(string filename)
		{
			this.filename = filename;
			this.ReadAll(this.filename);
		}

		/// <summary>
		/// Read all values from file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public void ReadAll(string? filename)
		{
			if (filename == null)
			{
				return;
			}

			lock (this.valuesLock)
			{
				if (File.Exists(filename))
				{
					XDocument doc = XMLFunctions.ReadXmlFile(filename);
					this.values = XMLFunctions.XmlToElementList(doc);
				}
				else
				{
					this.values.Add(new NameValueItem
					{
						Name = "CreationInformation",
						Value = "Self created on: " + DateTime.Now,
						IsAttribute = true,
					});
					this.isCompleteFileNewCreated = true;
					string message = "Die Konfigurationsdatei existierte nicht! Es wurde eine neue Konfigurationsdatei angelegt.\r\n";

					string? directory = Path.GetDirectoryName(filename);
					if (Directory.Exists(directory) == false)
					{
						if (directory != null)
						{
							Directory.CreateDirectory(directory);
						}
						message = $"Das IRMA-Verzeichnis und die Konfigurationsdatei existierten nicht! Beides wurde neu angelegt.\r\n";
					}

					this.WriteAll(filename);
					message += "File Name: ";
					message += filename + "\r\n";
					this.UserInformationCreated?.Invoke(this, message);
				}
			}
		}

		/// <summary>
		/// Write all values to the saved file.
		/// </summary>
		public void WriteAll()
		{
			XElement doc = XMLFunctions.ElementListToXml(this.values);
			if (this.filename != null)
			{
				XMLFunctions.WriteXmlFile(this.filename, doc);
			}
		}

		/// <summary>
		/// Write all values to the saved file.
		/// </summary>
		public void WriteAll(string filename)
		{
			XElement doc = XMLFunctions.ElementListToXml(this.values);
			XMLFunctions.WriteXmlFile(filename, doc);
		}

		/// <summary>
		/// Get one value as string, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public string? GetValue(string name, string valueIfNotExist = "New")
		{
			lock (this.valuesLock)
			{
				NameValueItem? configValue = this.values.FirstOrDefault(o => o.Name == name);

				if (configValue != null)
				{
					return configValue.Value;
				}

				configValue = new NameValueItem
				{
					Name = name,
					Value = valueIfNotExist,
				};
				this.values.Add(configValue);
				this.WriteAll();
			}

			if (!this.isCompleteFileNewCreated)
			{
				string text = "Der Wert existierte nicht in der Datei.\r\n";
				text += "Er wurde neu angelegt.\r\n";
				text += "Name der Datei:";
				text += this.filename + "\r\n";
				text += "Paremetername:\r\n" + name;
				this.UserInformationCreated?.Invoke(this, text);
			}

			return string.Empty;
		}

		/// <summary>
		/// Get one value as bool, if not exist create a new.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public bool GetValueAsBool(string name)
		{
			string? valueString = this.GetValue(name, "false");
			if (bool.TryParse(valueString, out bool value))
			{
				return value;
			}

			valueString = false.ToString();
			this.SetValue(name, valueString);
			return false;
		}

		/// <summary>
		/// Get one value as int, if not exist create a new.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public int GetValueAsInt(string name)
		{
            string? valueString = this.GetValue(name, "0");
			if (int.TryParse(valueString, out int value))
			{
				return value;
			}

			valueString = "0";
			this.SetValue(name, valueString);
			return 0;
		}

		/// <summary>
		/// Get one value as double, if not exist create a new.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value.</returns>
		public double GetValueAsDouble(string name)
		{
            string? valueString = this.GetValue(name, "0.0");
			if (double.TryParse(valueString, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
			{
				return value;
			}

			valueString = "0.0";
			this.SetValue(name, valueString);
			return 0.0;
		}

		/// <summary>
		/// Set one value as string, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValue(string name, string value)
		{
			lock (this.valuesLock)
			{
                NameValueItem? configValue = this.values.FirstOrDefault(o => o.Name == name);
				if (configValue != null)
				{
					configValue.Value = value;
				}
				else
				{
					configValue = new NameValueItem
					{
						Name = name,
						Value = value,
					};
					this.values.Add(configValue);
				}
			}

			this.WriteAll();
		}

		/// <summary>
		/// Set one value as bool, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValueAsBool(string name, bool value)
		{
			this.SetValue(name, value.ToString());
		}

		/// <summary>
		/// Set one value as int, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValueAsInt(string name, int value)
		{
			this.SetValue(name, value.ToString());
		}

		/// <summary>
		/// Set one value as double, if not exist, create a new value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public void SetValueAsDouble(string name, double value)
		{
			this.SetValue(name, value.ToString(CultureInfo.InvariantCulture));
		}
	}
}
