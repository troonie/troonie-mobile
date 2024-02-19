namespace TroonieMobile
{
    /// <summary>Represents the channel indices of ARGB structure.</summary>
    public struct RGBA
    {
        private static readonly byte b = DeviceInfo.Current.Platform == DevicePlatform.Android ? (byte)2 : (byte)0;
        private static readonly byte r = DeviceInfo.Current.Platform == DevicePlatform.Android ? (byte)0 : (byte)2;

        /// <summary>Color channel <c>blue</c></summary>
        public static byte B { get { return b; } }
        /// <summary>Color channel <c>green</c></summary>
        public const byte G = 1;
        

        /// <summary>Color channel <c>red</c></summary>
        public static byte R { get { return r; } }
        /// <summary>Transparence channel <c>alpha</c></summary>
        public const byte A = 3;

        public static int CheckIndexDependencyOfPlatform(int index)
        {
            return index switch
            {
                0 => DeviceInfo.Current.Platform == DevicePlatform.Android ? 2 : 0,
                2 => DeviceInfo.Current.Platform == DevicePlatform.Android ? 0 : 2,
                _ => index
            };
        }
    }
}
