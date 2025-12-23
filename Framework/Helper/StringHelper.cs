// -----------------------------------------------------------------------
// <copyright file="StringHelper.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Framework.Helper
{
	using System;

	/// <summary>
	/// Helper methods for strings.
	/// </summary>
	public class StringHelper
	{
		/// <summary>
		/// Get the complete exception text, include inner exception and call stack.
		/// </summary>
		public static string GetExceptionText(Exception exception)
		{
			string text = "Exception !!!!! ";
			text += GetInnerExceptionText(exception);
			return text;
		}

		/// <summary>
		/// Get all exception texts, includ all inner exceptions.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>The messages of all exceptions, include all inner exceptions.</returns>
		private static string GetInnerExceptionText(Exception exception, string addText = "")
		{
			if (exception.InnerException == null)
			{
				return exception.Message + "\r\n" + addText + "\r\n" + exception.StackTrace + "\r\n";
			}

			return GetInnerExceptionText(exception.InnerException, addText + "\r\n" + exception.Message);
		}
	}
}
