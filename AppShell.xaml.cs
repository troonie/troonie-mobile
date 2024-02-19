
namespace TroonieMobile
{
    public partial class AppShell : Shell
    {
        private bool initRadioButtonPayloadText, initRadioButtonLeonSteg;
        public RadioButton RadioButtonPayloadText { get; private set; }
        //public RadioButton RadioButtonPayloadFile { get; private set; }
        public RadioButton RadioButtonLeonSteg { get; private set; }
        //public RadioButton RadioButtonLeonStegRGB { get; private set; }

        public AppShell()
        {
            InitializeComponent();

            RadioButtonPayloadText = new RadioButton();
            //RadioButtonPayloadFile = new RadioButton();
            RadioButtonLeonSteg = new RadioButton();
            //RadioButtonLeonStegRGB = new RadioButton();

            // tidy up
            DirectoryInfo di = new(FileSystem.Current.CacheDirectory);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }

            di = new(FileSystem.Current.AppDataDirectory);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }

            //string androidpath = "/storage/emulated/0/Android/data/com.troonie.mobile";
            //if (Config.I.OS == OS.Android && Directory.Exists(androidpath))
            //{ 
            //    di = new DirectoryInfo(androidpath);
            //    foreach (FileInfo file in di.EnumerateFiles())
            //    {
            //        file.Delete();
            //    }
            //    foreach (DirectoryInfo dir in di.EnumerateDirectories())
            //    {
            //        dir.Delete(true);
            //    }
            //}
        }

        private void RadioButtonPayloadText_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!initRadioButtonPayloadText && sender is RadioButton rb)
            {
                RadioButtonPayloadText = rb;
                initRadioButtonPayloadText = true;
            }
            (CurrentPage as StartPage)?.ChangePayloadSpace();
            (CurrentPage as StartPage)?.ChangeVisibilityOfPayloadObjects(e.Value);
        }

        private void RadioButtonPayloadFile_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
        }

        private void RadioButtonLeonSteg_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!initRadioButtonLeonSteg && sender is RadioButton rb)
            {
                RadioButtonLeonSteg = rb;
                initRadioButtonLeonSteg = true;
            }
            (CurrentPage as StartPage)?.ChangePayloadSpace();
        }

        private void RadioButtonLeonStegRGB_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
        }
    }
}
