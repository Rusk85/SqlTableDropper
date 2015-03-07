using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
	public static class StringUtils
	{
	    /// <summary>
	    /// Creates url friendly slug of a string
	    /// </summary>
	    /// <param name="text"></param>
	    /// <returns></returns>
	    public static string MakeSlug(this string text)
	    {
	        if (String.IsNullOrEmpty(text)) return String.Empty;

	        // to lowercase, trim extra spaces
	        text = text.ToLower().Trim();

	        var len = text.Length;
	        var sb = new StringBuilder(len);
	        bool prevdash = false;
	        char c;

	        for (int i = 0; i < text.Length; i++)
	        {
	            c = text[i];
	            if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-')
	            {
	                if (!prevdash)
	                {
	                    sb.Append('-');
	                    prevdash = true;
	                }
	            }
	            else if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
	            {
	                sb.Append(c);
	                prevdash = false;
	            }
	            if (i == 80) break;
	        }

	        text = sb.ToString();
	        // remove trailing dash, if there is one
	        if (text.EndsWith("-"))
	            text = text.Substring(0, text.Length - 1);
	        return text;
	    }

	    /// <summary>
		/// Parses string to culture. Returns null if unsuccessful.
		/// </summary>
		/// <param name="lang"></param>
		/// <returns></returns>
		public static CultureInfo ToCulture(this string lang)
		{
			CultureInfo c = null;
			try
			{
				c = new CultureInfo(lang); 
			}
			catch(ArgumentNullException)
			{
				
			}			
			catch(ArgumentException)
			{
				
			}
			
			return c;
		}
		


		/// <summary>
		/// Cuts the string to the specified length
		/// </summary>
		/// <param name="value">string</param>
		/// <param name="length">maximum length</param>
		/// <returns></returns>
		public static string Cut(this string value,int length)
		{
			if (String.IsNullOrEmpty(value)) return "";	
			var l = value.Length > length ? length : value.Length;
			return value.Substring(0, l);
		}

        public static string RemoveLastChar(this string value)
        {
            value.MustNotBeNull();
            if (value.Length == 0) return value;
            return value.Remove(value.Length - 1, 1);
        }


        /// <summary>
        /// Returns true if the string is empty 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="checkBlancs">trim blancs</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string data,bool checkBlancs=false)
        {
            if (data == null) return true;
            if (checkBlancs)
            {
                data = data.Trim();
            }
            return String.IsNullOrEmpty(data);
        }

		/// <summary>
		/// Converts strings form unicode to specified encoding
		/// </summary>
		/// <param name="s">String</param>
		/// <param name="encoding">Encoding</param>
		/// <returns></returns>
		public static string ConvertToEncoding(this string s, Encoding encoding)
		{
			var bytes = Encoding.Unicode.GetBytes(s);
			bytes = Encoding.Convert(Encoding.Unicode, encoding, bytes);
			return Encoding.ASCII.GetString(bytes);
		}
		
		/// <summary>
		/// Capitalizes the first letter from string.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Capitalize(this string s)
		{
			if (String.IsNullOrEmpty(s)) return String.Empty;
			return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1, s.Length - 1).ToLowerInvariant();
		}

		

		/// <summary>
		/// Reads the Stream as an UTF8 String
		/// </summary>
		/// <param name="data">Stream</param>
		/// <returns></returns>
		public static string ReadAsString(this Stream data)
		{
			using (var r = new StreamReader(data, Encoding.UTF8))
				return r.ReadToEnd();
		}

		/// <summary>
		/// Returns true if teh string is a valid email format.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static bool IsEmail(this string text)
		{
			return Regex.IsMatch(text, @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" 
    + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" 
    + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$");
		}

        /// <summary>
        /// Returns the first line from a multilined string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetFirstLine(this string source)
        {
            if (string.IsNullOrEmpty(source)) return "";
            return source.Split('\n').FirstOrDefault();
        }
		
	/// <summary>
	/// Generates a random string of the specified length starting with prefix
	/// </summary>
	/// <param name="prefix"></param>
	/// <param name="length"></param>
	/// <returns></returns>
        public static string GenerateRandomString(this string prefix, int length)
	{
	    if (prefix == null) prefix = "";
	    if (length <= prefix.Length) return prefix;
        return prefix+CreateRandomString(length-prefix.Length);
        }

		/// <summary>
		/// Generates a random string (only letters) of the specified length
		/// </summary>
		/// <param name="length">Maximum string length</param>
		/// <returns></returns>
		public static string CreateRandomString(int length)
		{
		    var buff = new byte[length];
            var _random = new Random();
            
            _random.NextBytes(buff);

		    return Convert.ToBase64String(buff).Substring(0,length);

            //StringBuilder builder = new StringBuilder();
            //for (int i = 0; i < length; i++)
            //{

            //    //26 letters in the alfabet, ascii + 65 for the capital letters
            //    builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65))));

            //}
            //return builder.ToString();

		}
        
        public static string ToFormat(this string pattern, params object[] args)
        {
            pattern.MustNotBeEmpty();
            return String.Format(pattern, args);
        }


        public static T ToEnum<T>(this string value)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("Type '{0}' is not an enum".ToFormat(typeof(T)));
            return (T)Enum.Parse(typeof(T), value, true);
        }
	}
}

namespace System.Text
{
    public static class TextUtils
    {
        /// <summary>
        /// Removes last character from builder
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public static StringBuilder RemoveLast(this StringBuilder sb)
        {
            sb.MustNotBeNull();
            return sb.Remove(sb.Length - 1, 1);
        }

        /// <summary>
        /// Remove last char if matches the provided value
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value">value to check</param>
        /// <returns></returns>
        public static StringBuilder RemoveLastIfEquals(this StringBuilder sb,char value)
        {
            var last = sb[sb.Length - 1];
            if (value==last)
            {
                sb.RemoveLast();
            }
            return sb;
        }

        /// <summary>
        /// Remove last char if matches the provided value
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value">value to check</param>
        /// <returns></returns>
        public static StringBuilder RemoveLastIfEquals(this StringBuilder sb, string value)
        {
            if (value.IsNullOrEmpty()) return sb;
            if (sb.ToString().EndsWith(value))
            {
                sb.Remove(sb.Length - value.Length, value.Length);
            }
            //this is probably more efficient but too much code
            //bool eq = true;
            //int pos = sb.Length - value.Length;
            //if (pos < 0) return sb;
            //foreach(var ch in value)
            //{
            //    if (sb[pos]!=ch)
            //    {
            //        eq = false;
            //        break;
            //    }
            //    pos++;
            //}
            //if (!eq) return sb;

            //sb.Remove(sb.Length - value.Length, value.Length);            
            return sb;
        }           
            
    }

}