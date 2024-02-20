using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;


namespace TroonieMobile
{
    [Flags]
    public enum OS
    {
        None = 0,
        Android = 1 << 0, // = 1,
        Windows = 1 << 1, // = 2,
        iOS = 1 << 2, // = 4,
    }

    public class Config
    {
        private static Config? instance;
        public static Config I
        {
            get
            {
                instance ??= new Config();
                return instance;
            }
        }
        
        public App App { get; private set; }

        public AppShell Shell { get; private set; }
        public OS OS { private set; get; }
        public string InitialPath { private set; get; }

        #region Colors
        public Color Col_MiddleBtnMouseOver { private set; get; }
        public Color Col_FontBtnNormal { private set; get; }

        public Color Col_BorderBtnNormal { private set; get; }

        public LinearGradientBrush Col_TroonieButtonBrushNormal { private set; get; }
    #endregion

    private Config()
        {
            if (Application.Current is App a)
                App = a;
            else
                App = new App();

            if (App.MainPage is AppShell appshell)
                Shell = appshell;
            else
                Shell = new AppShell();

            InitialPath = FileSystem.CacheDirectory;
            DevicePlatform dpf = DeviceInfo.Current.Platform;

            if (dpf == DevicePlatform.Android)
            {
                OS = OS.Android;
                // https://learn.microsoft.com/de-de/dotnet/maui/platform-integration/device/information?view=net-maui-8.0&tabs=android
                InitialPath = "/storage/emulated/0/DCIM/Camera";
            }
            else if (dpf == DevicePlatform.WinUI)
            {
                OS = OS.Windows;

                InitialPath = FileSystem.Current.CacheDirectory;
                int length = InitialPath.IndexOf("AppData");
                InitialPath = InitialPath[..length]; // == TestPath.Substring(0, length);
                InitialPath += "Desktop";
            }

            // dummy values to avoid null in constructor
            Col_MiddleBtnMouseOver = Col_FontBtnNormal = Col_BorderBtnNormal = Colors.Black;
            Col_TroonieButtonBrushNormal = new LinearGradientBrush();
            InitColors();
        }

        private void InitColors()
        {            
            if (App.Resources.TryGetValue("MiddleBtnMouseOver", out object c))
                Col_MiddleBtnMouseOver = (Color)c;

            if (App.Resources.TryGetValue("FontBtnNormal", out c))
                Col_FontBtnNormal = (Color)c;

            if (App.Resources.TryGetValue("BorderBtnNormal", out c))
                Col_BorderBtnNormal = (Color)c;

            if (App.Resources.TryGetValue("TroonieButtonBrushNormal", out c))
                Col_TroonieButtonBrushNormal = (LinearGradientBrush)c;
        }

        public async void ShowSnackbar(string message, string actionButtonText = "Ok", Action? action = null, IView? anchor = null)
        {
            var options = new SnackbarOptions
            {
                BackgroundColor = Col_BorderBtnNormal,
                TextColor = Col_FontBtnNormal,
                ActionButtonTextColor = Col_FontBtnNormal,
                CornerRadius = new CornerRadius(12),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14, FontWeight.Semibold),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(18, FontWeight.Heavy),
                CharacterSpacing = 0d,               
            };

            //Action action = async () => await Toast.Make("Snackbar ActionButton Tapped").Show();
            var snackbar = Snackbar.Make(message, action, actionButtonText, TimeSpan.FromSeconds(2), options, anchor);
            await snackbar.Show(CancellationToken.None);
        }
    }
}
