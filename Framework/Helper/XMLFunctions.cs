// -----------------------------------------------------------------------
// <copyright file="XMLFunctions.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Framework.Helper
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using System.Xml.Linq;

	/// <summary>
	/// Defines some xml functions for common use.
	/// </summary>
	public class XMLFunctions
	{
        private static readonly string[] stringArray = ["."];

        /// <summary>
        /// Convert a list of elements with dot separated names to an xml file.
        /// </summary>
        /// <param name="elements">List of name value elements, with dot separated names and options.</param>
        /// <returns>A root xml element with a full xml structure as child elements.</returns>
        public static XElement ElementListToXml(List<NameValueItem> elements)
		{
			// Get the name of the root element, all elements must have the same root element.
			string rootName = string.Empty;
			if (elements != null && elements.Count > 0)
			{
				if (elements[0].Name.Contains('.'))
				{
					rootName = elements[0].Name.Split(["."], StringSplitOptions.None)[0];
				}
				else
				{
					rootName = elements[0].Name;
				}
			}

			// Create the root element.
			XElement rootElement = new(rootName);

			// Loop thru all elements, to add the other elemnts to the root
			foreach (NameValueItem element in elements)
			{
				AddElementStructure(rootElement, element, stringArray);
			}

			return rootElement;
		}

		/// <summary>
		/// Converting an xml-document to an element list with point separated names.
		/// </summary>
		/// <param name="xDoc">The xml document to convert.</param>
		/// <returns>The list of elements with point separated names.</returns>
		public static List<NameValueItem> XmlToElementList(XDocument xDoc)
		{
            List<NameValueItem> liste = [];
			string currentPrefix = string.Empty;
			foreach (XElement element in xDoc.Elements())
			{
				GetElements(element, currentPrefix, ref liste);
			}

			return liste;
		}

		/// <summary>
		/// Reading an xml-file in an xml document.
		/// </summary>
		public static XDocument ReadXmlFile(string fileName)
		{
			FileStream file;
			XDocument document;
			using (file = File.Open(fileName, FileMode.Open))
			{
				document = XDocument.Load(file);
			}

			return document;
		}

		/// <summary>
		/// Writing an xml-document to an xml-file.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		/// <param name="root">The root element of the xml-document.</param>
		public static void WriteXmlFile(string fileName, XElement root)
		{
			FileStream file = File.Create(fileName);
            XmlWriterSettings settings = new()
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.None
            };
            XmlWriter xmlWriter = XmlWriter.Create(file, settings);
			xmlWriter.WriteStartDocument();
			root.WriteTo(xmlWriter);
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			file.Close();
		}

		/// <summary>
		/// Writing a file with the list of elemnts with point separated names.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		/// <param name="elements">THe list of elemnts with point separated names.</param>
		internal static void WriteElementListFile(string fileName, List<NameValueItem> elements)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			foreach (NameValueItem element in elements)
			{
				File.AppendAllText(fileName, element.Name + "\t" + element.Value + "\t" + element.IsAttribute + "\t" + element.IsComment + "\n");
			}
		}

		/// <summary>
		/// Get all elements with all sub-elements (recursive) form the given xml-element and add them to a list of elements with
		/// point separated names.
		/// </summary>
		/// <param name="element">The given element to analyse (recursive).</param>
		/// <param name="currentName">The point separated name of the current level..</param>
		/// <param name="elements">List of elements with point separated names.</param>
		private static void GetElements(XElement element, string currentName, ref List<NameValueItem> elements)
		{
			// Add the nema of the element to the current name
			currentName = AddXmlElementName(currentName, element);

			// Add all attributes to the list of elements with point separated names.
			AddAttributes(element, currentName, ref elements);

			// Add elements and comments
			foreach (XNode node in element.Nodes())
			{
				// Add elements, with sub elements (recursive)
				if (node.NodeType == XmlNodeType.Element)
				{
					XElement subElement = (XElement)node;
					if (!subElement.HasElements)
					{
						if (!subElement.HasAttributes)
						{
							elements.Add(new NameValueItem
							{
								Name = currentName + "." + subElement.Name.LocalName,
								Value = subElement.Value,
							});
						}
					}

					GetElements(subElement, currentName, ref elements);
				}

				// Add comments
				if (node.NodeType == XmlNodeType.Comment)
				{
					XComment comment = (XComment)node;
					elements.Add(new NameValueItem { Name = currentName, Value = comment.Value, IsComment = true });
				}
			}

			// Go one level down
			SubtractNameFromEnd(currentName, element.Name.LocalName);
		}

		/// <summary>
		/// Subtracting the given name from the end of the name, if the name have an dot inseide, subtract it also.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="nameToSubtract">The name to subtract.</param>
		/// <returns>The new name.</returns>
		private static string SubtractNameFromEnd(string name, string nameToSubtract)
		{
			int subLength;
			if (name.Contains('.'))
			{
				subLength = name.Length - ("." + nameToSubtract).Length;
			}
			else
			{
				subLength = name.Length - nameToSubtract.Length;
			}

            return name[..subLength];
		}

		/// <summary>
		/// Adding all attributes who are inside the element to the given list of elements with point separated names.
		/// </summary>
		/// <param name="element">The xml-element.</param>
		/// <param name="baseName">The base name of the element name.</param>
		/// <param name="elements">The list of elements with point separated names to wich the attributes should be added.</param>
		private static void AddAttributes(XElement element, string baseName, ref List<NameValueItem> elements)
		{
			foreach (XAttribute attribute in element.Attributes())
			{
				elements.Add(new NameValueItem
				{
					Name = baseName + "." + attribute.Name.LocalName,
					Value = attribute.Value,
					IsAttribute = true,
				});
			}
		}

		/// <summary>
		/// Add the name of the xml element to the given name.
		/// </summary>
		/// <param name="name">The name to wich the name should be added.</param>
		/// <param name="element">The element, from what the name should be added.</param>
		/// <returns>The new name.</returns>
		private static string AddXmlElementName(string name, XElement element)
		{
			if (string.IsNullOrEmpty(name))
			{
				name += element.Name.LocalName;
			}
			else
			{
				name += "." + element.Name.LocalName;
			}

			return name;
		}

		/// <summary>
		/// Adding the given point sparated element structrure of the name of the element to the given root xml-element.
		/// </summary>
		/// <param name="rootElement">The root element.</param>
		/// <param name="element">The element structure to add.</param>
		private static void AddElementStructure(XElement rootElement, NameValueItem element, string[] separator)
		{
			// Get the hirarchy of the element, while spitting the name in parts between the points
			string[] hirachyElements = element.Name.Split(separator, StringSplitOptions.None);

			// Check if hirachy elements existing
			if (hirachyElements == null || hirachyElements.Length == 0)
			{
				return;
			}

			// The first hirarchy element must have the name of the root element!!!
			if (hirachyElements[0] != rootElement.Name.LocalName)
			{
				throw new Exception("The list should only have one root element.");
			}

			// Begin with the first root element
			XElement currentXElement = rootElement;

			// Go thru all hirarchy elements
			foreach (string hirarchyElement in hirachyElements)
			{
				// Skip the root element
				if (hirarchyElement != rootElement.Name.LocalName)
				{
					// Set the curent xml-element either to an existing x-element in the inner level, or create a new one
					// If the element is an attribute or comment create it in the last level
					currentXElement = GetOrCreateInnerXmlElement(hirarchyElement, currentXElement, element, hirachyElements.Last());
				}
			}

			// Add a comment, if this comment is in the outer level
			if (element.IsComment)
			{
				currentXElement.Add(new XComment(element.Value));
			}
			else
			{
				// Set the value if the element is in the outer level and not an attribute.
				if (!element.IsAttribute)
				{
					currentXElement.Value = element.Value;
				}
			}
		}

		/// <summary>
		/// If an xml-element with the name exits, gets it, or create a new one if not.
		/// Creating a attribute or comment if this level is the last (outer).
		/// </summary>
		/// <param name="name">The name of the next element.</param>
		/// <param name="xElement">The current xml-element.</param>
		/// <param name="nameValueElement">The name value element.</param>
		/// <param name="lastName">The last name.</param>
		/// <returns>The next xml element.</returns>
		private static XElement GetOrCreateInnerXmlElement(string name, XElement xElement, NameValueItem nameValueElement, string lastName)
		{
			// Get the inner elements of the current xml element, matching with the current hirarchy element
			XElement innerElement = GetExistingElement(name, xElement);

			// If inner elements existing, return that, as the new current elemnt
			if (innerElement != null)
			{
				return innerElement;
			}

			// Check if this is the last hirarchy element and the element is an attribute
			if (name == lastName && nameValueElement.IsAttribute)
			{
                // Add an Attribute
                XAttribute attribute = new(name,
                                           nameValueElement.Value);
				xElement.Add(attribute);
			}
			else
			{
				// Check if its the last hirarchy element and the element is an comment
				if (name == lastName && nameValueElement.IsComment)
				{
                    // Add an comment
                    XComment comment = new(nameValueElement.Value);
					xElement.Parent.Add(comment);
				}
				else
				{
                    // If its not the last element with comment or attribute, add here a new xml-element
                    XElement newXElement = new(name);
					xElement.Add(newXElement);
					xElement = newXElement;
				}
			}

			return xElement;
		}

		/// <summary>
		/// Gets an existing inner xml element in the given xml element.
		/// </summary>
		/// <param name="name">The name of the element to find.</param>
		/// <param name="xElement">The xml element in that searching.</param>
		/// <returns>The existing inner xml element or null.</returns>
		private static XElement? GetExistingElement(string name, XElement xElement)
		{
			// Get the inner elements of the current xml element, matching with the current hirarchy element
			IEnumerable<XElement> innerElement = xElement.Elements(name);

			// If inner elements existing, use the first element as current element
			if (innerElement != null && innerElement.Any())
			{
				// Only one elemnt should matching the name of the current hirarchy element
				return innerElement?.FirstOrDefault();
			}

			return null;
		}
	}
}
