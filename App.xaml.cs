namespace TroonieMobile
{
    public partial class App : Application
    {                    
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);
            // Manipulate Window object
            // Get display size
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

            window.Height = 1000;
            window.Width = 1000;
            // Center the window
            window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 4.0;
            window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2.0;

            return window;
        }
    }
}
