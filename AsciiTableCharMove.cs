namespace TroonieMobile
{
    public static class AsciiTableCharMove
    {
        ///// <summary>
        ///// The code page of used encoding.
        ///// https://docs.microsoft.com/de-de/dotnet/api/system.text.encoding.codepage
        ///// Windows-1252 --> Western European (Windows)
        ///// </summary>
        //public const int CodePage = 1252;

        /// <summary>
        /// The code page of used encoding.
        /// https://docs.microsoft.com/de-de/dotnet/api/system.text.encoding.codepage
        /// iso-8859-1	--> Western European (ISO)
        /// </summary>
        public const int CodePage = 28591;

        public static byte[] GetBytesFromString(string text)
        {
			return System.Text.Encoding.GetEncoding(CodePage).GetBytes (text);
		}

		public static string GetStringFromBytes(byte[] bytes)
		{
			return System.Text.Encoding.GetEncoding(CodePage).GetString (bytes);
		}
    }
}
