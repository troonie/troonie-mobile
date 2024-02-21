using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls.Compatibility;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Diagnostics;


namespace TroonieMobile;

public partial class StartPage : ContentPage
{
    private bool IsImageSizeTooSmall;
    private string bitmapFilename;
#pragma warning disable IDE0052
    private string bitmapFullpath;
#pragma warning restore IDE0052 
                               
    //private string payloadFilename;
    private SKBitmap? bitmap;
    private byte[]? file;    
    private readonly Config c = Config.I;
    private readonly Stopwatch timer;

    public StartPage()
	{
		InitializeComponent();

        timer = new Stopwatch();
        timer.Start();
        RadioButtonRead.IsChecked = true;

        // TODO: Comment in, if image as parameter can be passed.
        // ChangePayloadSpace();

        PayoadEditor.Text = string.Empty;
        bitmapFilename = string.Empty;
        bitmapFullpath = string.Empty;
        //payloadFilename = string.Empty;
    }

    ~StartPage()
    {
        c.ShowSnackbar($"Troonie is closed.");
        timer.Stop();
    }


    private void RadioButtonRead_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (FrameFileEntryAndFileButton != null)
            FrameFileEntryAndFileButton.IsEnabled = !e.Value;
    }

    private async void OnClickedBtnSteganography(object sender, EventArgs e)
    {
        if (bitmap == null)
        {
            c.ShowSnackbar("Please open an image first.");
            return;
        }

        if (EntryPassword.Text == null || EntryPassword.Text.Length < 4)
        {
            c.ShowSnackbar("Password too short.");
            return;
        }

        LeonSteg ls = c.Shell.RadioButtonLeonSteg.IsChecked ? new LeonSteg() : new LeonStegRGB();

        if (RadioButtonWrite.IsChecked)
        {
            byte[] bytes;

            if (c.Shell.RadioButtonPayloadText.IsChecked)
            {
                bytes = AsciiTableCharMove.GetBytesFromString(PayoadEditor.Text);
            }
            else
            {
                if (file == null)
                {
                    c.ShowSnackbar("Please select file to encrypt first.");
                    return;
                }

                if (IsImageSizeTooSmall)
                {
                    c.ShowSnackbar("Image too small. Please open image with taller width and height.");
                    return;
                }
                
                bytes = file;
            }

            /* int error = */ 
            ls.Write(bitmap, EntryPassword.Text, bytes);

            CancellationToken ct = default;
            bool success;
            FileSaverResult fileSaveResult;            
            string resultFilename = string.Concat(bitmapFilename.AsSpan(0, bitmapFilename.LastIndexOf('.')), "_t.png");
            if (c.OS == OS.Android)
            {
                SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = data.AsStream(true);
                fileSaveResult = await FileSaver.Default.SaveAsync(c.InitialPath, resultFilename, stream, ct);
            }
            else
            {
                using var memStream = new MemoryStream(bitmap.ByteCount);
                success = bitmap.Encode(memStream, SKEncodedImageFormat.Png, 100);
                fileSaveResult = await FileSaver.Default.SaveAsync(c.InitialPath, resultFilename, memStream, ct);
            }

            if (fileSaveResult.IsSuccessful)
            {
                c.ShowSnackbar($"Steg-Image is saved: {fileSaveResult.FilePath}");
            }
            else
            {
                c.ShowSnackbar($"Error: Steg-Image is not saved, {fileSaveResult.Exception.Message}");
            }

        }
        else /* reading */ 
        {
            ls.Read(bitmap, EntryPassword.Text, out byte[] bytes);

            if (ls.CancelToken)
            {
                c.ShowSnackbar($"Error: Reading process is aborted. No result.");
            }
            else
            {
                if (c.Shell.RadioButtonPayloadText.IsChecked)
                {
                    PayoadEditor.Text = AsciiTableCharMove.GetStringFromBytes(bytes);
                }
                else
                {
                    CancellationToken ct = default;
                    using var stream = new MemoryStream(bytes);
                    FileSaverResult fileSaveResult = await FileSaver.Default.SaveAsync("result.pdf", stream, ct);
                    if (fileSaveResult.IsSuccessful)
                    {
                        c.ShowSnackbar($"File is saved: {fileSaveResult.FilePath}");
                    }
                    else
                    {
                        c.ShowSnackbar($"Error: File is not saved, {fileSaveResult.Exception.Message}");
                    }
                }
                
            }
            

                        
        }

        
    }

    private void OnEditorTextChanged(object sender, EventArgs e)
    {
        if (bitmap == null)
        {
            PayoadEditor.Text = string.Empty;

            if (timer.ElapsedMilliseconds > 3000)
            {
                timer.Restart();

                c.ShowSnackbar("Please open an image first.");
            }
        }

        ChangePayloadSpace();
    }

    //private void OnEditorCompleted(object sender, EventArgs e)
    //{
    //}

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();

        if (bitmap != null)
        {
            SKRect r = new(0, 0, info.Width, info.Height);
            canvas.DrawBitmap(bitmap, r);
        }

        //float x = (info.Width - bitmap.Width) / 2;
        //float y = (info.Height - bitmap.Height) / 2;

        //canvas.DrawBitmap(bitmap, x, y);
    }

    private async void BtnOpenImage_Clicked(object sender, EventArgs e)
    {
        FileResult photo = await MediaPicker.Default.PickPhotoAsync();

        if (photo != null)
        {
            bitmapFilename = photo.FileName;
            bitmapFullpath = photo.FullPath;
            using Stream sourceStream = await photo.OpenReadAsync();
            bitmap = SKBitmap.Decode(sourceStream);
            if (c.Shell.Downscaling == Downscaling.px1800 &&
                (bitmap.Height > 1800 || bitmap.Width > 1800) )
            {
                bool answer = await DisplayAlert("Question?", "Downscaling the image", "Yes", "No");
                Debug.WriteLine("Answer: " + answer);
                if (answer)
                    bitmap = bitmap.Resize(new SKSizeI(1800, 1800), SKFilterQuality.High);
            }
            skiaView.InvalidateSurface();

            ChangePayloadSpace();
            Title = bitmapFilename;            
        }
    }

    private void BtnOptions_Clicked(object sender, EventArgs e)
    {
        c.Shell.FlyoutIsPresented = !c.Shell.FlyoutIsPresented;
    }

    private async void BtnFile_Clicked(object sender, EventArgs e)
    {
        PickOptions options = new() { PickerTitle = "Select file to encrypt." };
        var result = await FilePicker.Default.PickAsync(options);
        Stream s;
        if (result != null && (s = await result.OpenReadAsync()) != null)
        {
            // check whether file is too big
            if (s.Length > (long)20 * 1024 /*kb*/ * 1024 /*mb*/)
            {
                s.Close();
                s.Dispose();
                c.ShowSnackbar("File is too big (>20mb). Please choose smaller file.");
                return;
            }

            using (MemoryStream ms = new())
            {
                s.CopyTo(ms);
                file = ms.ToArray();
            }

            //payloadFilename = result.FileName;
            EntryFile.Text = result.FullPath;
            ChangePayloadSpace();
        }
    }

    public void ChangeVisibilityOfPayloadObjects(bool isTextVisible)
    {
        PayoadEditor.IsVisible = isTextVisible;
        FrameFileEntryAndFileButton.IsVisible = !isTextVisible;
    }

    public void ChangePayloadSpace()
    {
        Space.TextColor = Config.I.Col_FontBtnNormal;
        IsImageSizeTooSmall = false;

        if (bitmap != null)
        { 
            long l;
            string text;
            /* 3 == BitStegRGB,  1 == BitSteg */
            int multiplicator = c.Shell.RadioButtonLeonSteg.IsChecked ? 1 : 3;
            int dim = multiplicator * (bitmap.Width * bitmap.Height) / 8 - LeonSteg.LengthEndText;

            // if (!Config.I.IsFileAsPayloadEnabled)
            if (c.Shell.RadioButtonPayloadText.IsChecked) 
            {
                l = PayoadEditor.Text.Length; // textviewContent.Buffer.Text.Length;               

                if (l > dim)
                {
                    PayoadEditor.Text = PayoadEditor.Text[..dim];
                }
                else
                {
                    Space.Text = GetText(l, dim);
                }
            }
            // TODO File size
            //else if (rdBtnEncrypt.Active && File.Exists(hypertextlabelFileChooser.Text))
            else
            {
                if (file == null)
                { 
                    return;
                }
                l = file.LongLength;
                text = GetText(l, dim);

                if (l > dim)
                {
                    IsImageSizeTooSmall = true;
                    Space.TextColor = Colors.Red; // Config.I.Col_MiddleBtnMouseOver;
                    Space.Text = "Caution! "/* + Language.I.L[29]*/ + "  " + text + " ";
                }
                else
                {                    
                    Space.Text = text;
                }
            }
        }
    }

    private static string GetText(long l, int dim)
    {
        string text; 
        if (l > 1024 * 1024)
        {
            long l_mega = l / (1024 * 1024);
            int dim_mega = dim / (1024 * 1024);
            text = l_mega + " / " + dim_mega + " MB" /*+ Language.I.L[249]*/ + "  (" + l + " / " + dim + " Bytes" /*+ Language.I.L[247]*/ + ")";
        }
        else if (l > 1024)
        {
            long l_kilo = l / 1024;
            int dim_kilo = dim / 1024;
            text = l_kilo + " / " + dim_kilo + " KB" /*+ Language.I.L[248]*/ + "  (" + l + " / " + dim + " Bytes" /*+ Language.I.L[247]*/ + ")";
        }
        else
        {
            text = l + " / " + dim + " Bytes" /*+ Language.I.L[247]*/;
        }

        return text;
    }
    
}