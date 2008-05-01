using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace CSS_Merge
{
	#region enum Formatting
	public enum Formatting
	{
		Readable,
		Optimized
	}
	#endregion

	#region enum Sorting
	public enum Sorting
	{
		ByTypeThenName,
		ByName
	}
	#endregion

	#region class CssIO
	public class CssIO
	{
		/// <summary>
		/// Interprets a CSS file and returns a representitive class.
		/// </summary>
		/// <param name="filename">The full filename of the file to be read</param>
		/// <returns>A class representing the CSS file</returns>
		public static CssFile ReadFile(string filename)
		{
			FileStream fs = new FileStream(filename, FileMode.Open);

			try
			{
				StreamReader sr = new StreamReader(fs);
				string file = sr.ReadToEnd();

				VerifyStringTerminations(file);

				file = StripComments(file);

				// Strip line extenders (\[newline])
				file = Regex.Replace(file, @"\\\n", "");

				// Create and return CssFile objects with
				// classes in no particular order
				CssFile cssFile = new CssFile(filename);

				// Identify selectors and their blocks of properties
				// and parse into classes. Multiple blocks for the same
				// selector may exist, so merge properties if this happens

				Dictionary<string, CssClass> uniqueClasses = new Dictionary<string, CssClass>();
				Match m = Regex.Match(file, @"[^{]+{[^}]*}");
				while (m.Success)
				{
					CssClass[] cssClasses = CssClass.Parse(m.Groups[0].Value);

					foreach (CssClass cssClass in cssClasses)
					{
						if (!uniqueClasses.ContainsKey(cssClass.Selector))
							uniqueClasses.Add(cssClass.Selector, cssClass);		// New class
						else
						{
							// Need to merge properties. Properties of the same
							// identifier will overwrite those defined earlier

							CssClass existingClass = uniqueClasses[cssClass.Selector];
							foreach (KeyValuePair<string, string> kvpProperty in cssClass.Properties)
								existingClass.Properties[kvpProperty.Key] = kvpProperty.Value;
						}
					}

					m = m.NextMatch();
				}

				cssFile.CssClasses.AddRange(uniqueClasses.Values);

				return cssFile;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				fs.Close();
			}
		}

		/// <summary>
		/// Creates a CSS file from a CSSFile class.
		/// </summary>
		/// <param name="cssFile">The CSSFile class to serialize</param>
		/// <param name="formatting">A parameter setting the formatting - space-optimized or human-readable</param>
		public static void WriteFile(CssFile cssFile, Formatting formatting)
		{
			StringBuilder sb = new StringBuilder();

			bool first = true;
			foreach (CssClass cssClass in cssFile.CssClasses)
			{
				if (!first && formatting != Formatting.Optimized)
					sb.Append("\r\n\r\n");

				first = false;

				sb.Append(cssClass.Serialize(formatting == Formatting.Optimized));
			}

			FileStream fs = new FileStream(cssFile.Filename, FileMode.Create);

			StreamWriter sw = new StreamWriter(fs);
			sw.AutoFlush = true;

			sw.Write(sb.ToString());

			fs.Close();
		}

		private static void VerifyStringTerminations(string s)
		{
			// Simplify scan process by removing 
			// carrier return character
			s = s.Replace("\r\n", "\n");

			// Step through each character making note
			// of entry into quoted area or backslashes.
			// The algorithm must be wary of broken-out
			// " and ' characters and also line feeds
			// using the \ character at the end of a line

			char startChar = (char)0;
			bool lastCharBackslash = false;
			int line = 0;
			foreach (char c in s)
			{
				switch (c)
				{
					case '"':
						if ((int)startChar == 0)
							startChar = '"';			// Entry into " string
						else if (startChar == '"')
						{
							if (!lastCharBackslash)
								startChar = (char)0;	// Exit out of " string if not \" 
						}
						break;

					case '\'':
						if ((int)startChar == 0)
							startChar = '\'';			// Entry into ' string
						else if (startChar == '\'')
						{
							if (!lastCharBackslash)
								startChar = (char)0;	// Exit out of ' string if not \'
						}
						break;

					case '\n':
						if (startChar != (char)0 && !lastCharBackslash)
							throw new Exception("Unterminated string on line " + line + ".");
						line++;
						break;
				}

				lastCharBackslash = false;
				if (c == '\\')
					lastCharBackslash = true;
			}

			if (startChar != (char)0)
				throw new Exception("Unterminated string at end of file.");
		}
		private static string StripComments(string s)
		{
			// Strip in-line '//' comments - find // either
			// at the beginning of a line or after some whitespace
			s = Regex.Replace(s, @"(^|[\s]+)//[^\n]*", "");	

			// Move through removing anything between
			// a /* and a */

			int startIndex = s.IndexOf("/*");
			while (startIndex >= 0)
			{
				int endIndex = s.IndexOf("*/", startIndex);
				if (endIndex > startIndex)
					s = s.Substring(0, startIndex) + s.Substring(endIndex + 2, s.Length - endIndex - 2);
				else
					throw new Exception("Unterminated /* */ comment bracket");

				startIndex = s.IndexOf("/*");
			}

			// Do the same with <!-- and --> comments

			startIndex = s.IndexOf("<!--");
			while (startIndex >= 0)
			{
				int endIndex = s.IndexOf("-->", startIndex);
				if (endIndex > startIndex)
					s = s.Substring(0, startIndex) + s.Substring(endIndex + 3, s.Length - endIndex - 3);
				else
					throw new Exception("Unterminated <!-- --> comment bracket");

				startIndex = s.IndexOf("<!--");
			}

			return s;
		}
	}
	#endregion

	#region class CssFile
	/// <summary>
	/// Represents a CSS file - stores the filename and a collection of CSS classe objects
	/// </summary>
	public class CssFile
	{
		private string filename;
		private List<CssClass> cssClasses;

		/// <summary>
		/// Create a new empty CSS file
		/// </summary>
		/// <param name="filename">The destination filename of the CSS file</param>
		/// <remarks>No file is created at this stage - use CssIO to save</remarks>
		public CssFile(string filename)
		{
			this.filename = filename;
			cssClasses = new List<CssClass>();
		}


		/// <summary>
		/// Gets or sets the full filename of the CSS file
		/// </summary>
		public string Filename
		{
			get { return filename; }
			set { filename = value; }
		}

		/// <summary>
		/// Gets an indexed list of CSSClass objects
		/// </summary>
		public List<CssClass> CssClasses
		{
			get { return cssClasses; }
		}


		/// <summary>
		/// Sort CSS classes into an order
		/// </summary>
		/// <param name="sorting">The order of sorting to be used</param>
		public void Sort(Sorting sorting)
		{
			string[] selectors = new string[cssClasses.Count];
			for (int i = 0; i < cssClasses.Count; i++)
			{
				if (sorting == Sorting.ByName)
				{
					string modifiedSelector = cssClasses[i].Selector;
					while (modifiedSelector.Length > 0 && !Char.IsLetterOrDigit(modifiedSelector[0]))
						modifiedSelector = modifiedSelector.Substring(1);

					if (modifiedSelector.Length == 0)
						modifiedSelector = cssClasses[i].Selector;	// No letters/numbers - reset

					selectors[i] = modifiedSelector;
				}
				else
					selectors[i] = cssClasses[i].Selector;
			}

			CssClass[] sortedClasses = cssClasses.ToArray();
			Array.Sort(selectors, sortedClasses);

			cssClasses = new List<CssClass>(sortedClasses);
		}
		
		/// <summary>
		/// Helper override for form
		/// </summary>
		/// <returns>[Name of file] - ([Full directory])</returns>
		public override string ToString()
		{
			FileInfo fi = new FileInfo(filename);

			return fi.Name + " ( " + fi.Directory + " )";
		}
	}
	#endregion

	#region class CssClass
	/// <summary>
	/// Represents a CSS class
	/// </summary>
	public class CssClass
	{
		private string selector;
		private SortedDictionary<string, string> properties;

		/// <summary>
		/// Creates a new CSS class object with an
		/// empty set of parameters
		/// </summary>
		/// <param name="selector">CSS class selector</param>
		public CssClass(string selector)
		{
			this.selector = selector;
			this.properties = new SortedDictionary<string, string>();
		}


		/// <summary>
		/// Gets or sets the selector name of the CSS class
		/// </summary>
		public string Selector
		{
			get { return selector; }
			set { selector = value; }
		}
	
		/// <summary>
		/// Gets a sorted dictionary collection of the properties
		/// and their values within the CSS class
		/// </summary>
		public SortedDictionary<string, string> Properties
		{
			get { return properties; }
		}
		

		/// <summary>
		/// Parses a set of classes from a string. The string
		/// must have a selector set and a curly bracketed area.
		/// </summary>
		/// <param name="s">The string to parse</param>
		/// <returns>A collection of CSS classes</returns>
		public static CssClass[] Parse(string s)
		{
			Match m = Regex.Match(s, @"([^{]+){([^}]*)}");
			
			if (!m.Success)
				throw new Exception("Malformed class");

			Dictionary<string, CssClass> cssClassesDict = new Dictionary<string, CssClass>();

			List<string> selectors = new List<string>(m.Groups[1].Value.Split(','));
			for (int i = selectors.Count - 1; i >= 0; i--)
			{
				// Trim outside of selectors and 
				// remove any inner excess space
				selectors[i] = Regex.Replace(selectors[i].Trim(), @"\s+", " ").ToLower();
				if (selectors[i].Length == 0)
					selectors.RemoveAt(i);
			}

			string propertiesBunch = m.Groups[2].Value;
			Dictionary<string, string> properties = new Dictionary<string, string>();

			Match mProperty = Regex.Match(propertiesBunch, @"([^:]+):([^;]+);?");

			while (mProperty.Success)
			{
				string propertyName = mProperty.Groups[1].Value;
				string propertyValue = mProperty.Groups[2].Value;

				propertyName = Regex.Replace(propertyName.Trim(), @"\s+", " ");
				propertyValue = Regex.Replace(propertyValue.Trim(), @"\s+", " ");

				if (propertyName.Length > 0 && propertyValue.Length > 0)
				{
					// Will overwrite if a previously defined 
					// property of that name was defined in the file
					properties[propertyName] = propertyValue;
				}

				mProperty = mProperty.NextMatch();
			}

			/*
			List<string> propertiesNameAndValue = new List<string>(m.Groups[2].Value.Split(';'));

			foreach (string propertyNameAndValue in propertiesNameAndValue)
			{
				if (propertyNameAndValue.Trim().Length == 0)
					continue;
				string[] propertyTokens = propertyNameAndValue.Split(':');

				if (propertyTokens.Length != 2)
				{
					// No ':' separator (or too many) between
					// property name and value
					throw new Exception("Malformed property '" + propertyNameAndValue + "'"
						+ "\r\n\r\nin class selector '" + m.Groups[1].Value.Trim() + "'");
				}

				string propertyName = propertyTokens[0];
				string propertyValue = propertyTokens[1];

				propertyName = Regex.Replace(propertyName.Trim(), @"\s+", " ");
				propertyValue = Regex.Replace(propertyValue.Trim(), @"\s+", " ");

				if (propertyName.Length > 0 && propertyValue.Length > 0)
				{
					// Will overwrite if a previously defined 
					// property of that name was defined in the file
					properties[propertyName] = propertyValue;
				}
			}
			*/

			// Only create classes if properties
			// exist for them
			if (properties.Count > 0)
			{
				// For each selector that owns the block,
				// create a CssClass with its properties
				foreach (string selector in selectors)
				{
					CssClass cssClass = new CssClass(selector);
					foreach (KeyValuePair<string, string> property in properties)
						cssClass.Properties[property.Key] = property.Value;

					// Will overwrite if a preveriously defined
					// class of that name was defined in the file
					cssClassesDict[selector] = cssClass;
				}
			}

			CssClass[] classes = new CssClass[cssClassesDict.Values.Count];
			cssClassesDict.Values.CopyTo(classes, 0);

			return classes;
		}
		
		/// <summary>
		/// Returns a readable representation of the CSS class.
		/// </summary>
		/// <param name="optimized">True for space-optimized, False for human-readable</param>
		/// <returns></returns>
		public string Serialize(bool optimized)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(selector);

			if (!optimized)
				sb.Append(" ");

			sb.Append("{");

			if (!optimized)
				sb.Append("\r\n");

			int i = 0;
			foreach (KeyValuePair<string, string> kvpProperty in properties)
			{
				i++;

				if (!optimized)
					sb.Append("\t");

				sb.Append(kvpProperty.Key);
				sb.Append(":");

				if (!optimized)
					sb.Append(" ");

				if (!optimized)
					sb.Append(kvpProperty.Value);
				else
					sb.Append(kvpProperty.Value.Replace(", ", ","));	// Squeeze in

				if (i < properties.Count)
					sb.Append(";");

				if (!optimized)
					sb.Append("\r\n");
			}

			sb.Append("}");

			return sb.ToString();
		}
	}
	#endregion

	#region class CssConflict
	/// <summary>
	/// Helper class to resolve conflicts between
	/// CSS classes in multiple files
	/// </summary>
	public class CssConflict
	{
		private KeyValuePair<CssFile, CssClass>[] cssClasses;
		private int selectedIndex = 0;

		/// <summary>
		/// Create a CssConflict class with a set of CSS files
		/// and the CSS class that is conflicting. The 0th
		/// element is selected by default
		/// </summary>
		/// <param name="cssClasses"></param>
		public CssConflict(KeyValuePair<CssFile, CssClass>[] cssClasses)
		{
			this.cssClasses = cssClasses;
		}


		/// <summary>
		/// Gets a list of CSS files and classes that are
		/// conflicting
		/// </summary>
		public KeyValuePair<CssFile, CssClass>[] CssClasses
		{
			get { return cssClasses; }
		}
		
		/// <summary>
		/// Gets or sets the selected file and class pair
		/// that the user wants to appear in the merged
		/// CSS file
		/// </summary>
		public int SelectedIndex
		{
			get { return selectedIndex; }
			set { selectedIndex = value; }
		}


		/// <summary>
		/// Determines conflicts between a collection of
		/// CSS files and returns a set of CssConflict objects
		/// representing the choices (including those with
		/// no conflicts)
		/// </summary>
		/// <param name="cssFiles">A collection of CSS files</param>
		/// <returns>A collection of CssConflict objects</returns>
		public static CssConflict[] GetConflictSet(params CssFile[] cssFiles)
		{
			// Create a unique set of selectors and map
			// to a list of files/classes that contain
			// those selectors

			Dictionary<string, List<KeyValuePair<CssFile, CssClass>>> dict 
				= new Dictionary<string, List<KeyValuePair<CssFile, CssClass>>>();
			
			foreach (CssFile cssFile in cssFiles)
			{
				foreach (CssClass cssClass in cssFile.CssClasses)
				{
					if (!dict.ContainsKey(cssClass.Selector.ToLower()))
						dict.Add(cssClass.Selector.ToLower(), new List<KeyValuePair<CssFile, CssClass>>());

					dict[cssClass.Selector.ToLower()].Add(new KeyValuePair<CssFile, CssClass>(cssFile, cssClass));
				}
			}

			string[] selectors = new string[dict.Count];
			dict.Keys.CopyTo(selectors, 0);
			Array.Sort(selectors);

			CssConflict[] conficts = new CssConflict[selectors.Length];
			for (int i = 0; i < selectors.Length; i++)
				conficts[i] = new CssConflict(dict[selectors[i]].ToArray());

			return conficts;
		}
		
		/// <summary>
		/// Merges together CSS conflicts after appropriate
		/// classes have been chosen by the user.
		/// </summary>
		/// <param name="conflictSet">A collection of CssConflict objects</param>
		/// <returns>The merged collection of CSS classes</returns>
		public static CssClass[] GetMergedClassSet(params CssConflict[] conflictSet)
		{
			CssClass[] mergedClasses = new CssClass[conflictSet.Length];
			for (int i=0; i<conflictSet.Length; i++)
				mergedClasses[i] = conflictSet[i].CssClasses[conflictSet[i].SelectedIndex].Value;

			return mergedClasses;
		}

		/// <summary>
		/// Helper override for form
		/// </summary>
		/// <returns>Class selector of the conflict</returns>
		public override string ToString()
		{
			return cssClasses[0].Value.Selector;
		}
	}
	#endregion
}
