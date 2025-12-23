// -----------------------------------------------------------------------
// <copyright file="DigitalFilter.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Framework.Helper
{
	/// <summary>
	/// Defines a config value with name and value (as string).
	/// </summary>
	public class NameValueItem
	{
		/// <summary>
		/// Gets or sets the name of the element (dot separated).
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Gets or sets the value of the element.
		/// </summary>
		public string? Value { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the element is converted from an xml-attribute or not.
		/// </summary>
		public bool IsAttribute { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the element is converted from an xml-comment or not.
		/// </summary>
		public bool IsComment { get; set; }
	}
}
