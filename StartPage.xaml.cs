using CommunityToolkit.Maui.Storage;
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
            await Config.ShowDisplayAlert(this, "Open image", "Please open an image first.", DisplayAlertButtonText.Ok);
            return;
        }

        if (EntryPassword.Text == null || EntryPassword.Text.Length < 4)
        {
            await Config.ShowDisplayAlert(this, "Password too short", "Please enter a longer Password.", DisplayAlertButtonText.Ok);
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
                    await Config.ShowDisplayAlert(this, "Select file", "Please select a file to encrypt first.", DisplayAlertButtonText.Ok);
                    return;
                }

                if (IsImageSizeTooSmall)
                {
                    await Config.ShowDisplayAlert(this, "Image too small", "Please open an image with bigger width and height.", DisplayAlertButtonText.Ok);
                    return;
                }
                
                bytes = file;
            }

            #region Downscaling image
            bool answer = await Config.ShowDisplayAlert(this, "Downscaling image", 
                "Should Troonie downscale the image to reduce needed storage space for saving?", DisplayAlertButtonText.YesNo);
            //Debug.WriteLine("Answer: " + answer);
            if (answer)
            {
                int tW = bitmap.Width;
                int tH = bitmap.Height;
                CheckDownscaling(bytes.Length, ref tW, ref tH, !c.Shell.RadioButtonLeonSteg.IsChecked);
                bitmap = bitmap.Resize(new SKSizeI(tW, tH), SKFilterQuality.High);
            }
            #endregion

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
                await Config.ShowDisplayAlert(this, "Success", $"Steg-Image is saved: {fileSaveResult.FilePath}", DisplayAlertButtonText.Ok);
            }
            else if (c.OS == OS.Android) // && File.Exists(fileSaveResult.FilePath)) 
            {
                await Config.ShowDisplayAlert(this, "Success (Android)", $"Steg-Image is saved: {fileSaveResult.FilePath}", DisplayAlertButtonText.Ok);
            }
            else
            {
                await Config.ShowDisplayAlert(this, "Error", $"Error: Steg-Image is not saved, {fileSaveResult.Exception.Message}", DisplayAlertButtonText.Ok);
            }

        }
        else /* reading */ 
        {
            ls.Read(bitmap, EntryPassword.Text, out byte[] bytes);

            if (ls.CancelToken)
            {
                await Config.ShowDisplayAlert(this, "Error", "Reading process is aborted. No result.", DisplayAlertButtonText.Ok);
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
                        await Config.ShowDisplayAlert(this, "Success", $"File is saved: {fileSaveResult.FilePath}", DisplayAlertButtonText.Ok);
                    }
                    else
                    {
                        await Config.ShowDisplayAlert(this, "Error", $"File is not saved, {fileSaveResult.Exception.Message}", DisplayAlertButtonText.Ok);
                    }
                }
                
            }
            

                        
        }

        
    }

    private async void OnEditorTextChanged(object sender, EventArgs e)
    {
        if (bitmap == null)
        {
            PayoadEditor.Text = string.Empty;

            if (timer.ElapsedMilliseconds > 3000)
            {
                timer.Restart();
                await Config.ShowDisplayAlert(this, "Open image", "Please open an image first.", DisplayAlertButtonText.Ok);
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
                await Config.ShowDisplayAlert(this, "File too big", "File is too big (>20mb). Please choose smaller file.", DisplayAlertButtonText.Ok);
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

            if (c.Shell.RadioButtonPayloadText.IsChecked) 
            {
                l = PayoadEditor.Text.Length;             

                if (l > dim)
                {
                    PayoadEditor.Text = PayoadEditor.Text[..dim];
                }
                else
                {
                    Space.Text = GetText(l, dim);
                }
            }
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

    private static void CheckDownscaling(int payloadByteCount, ref int width, ref int height, bool isLeonStegRGB)
    {
        const int subtrahend = 200;
        const int minbiggerSideLength = 1400;
        /* 3 == BitStegRGB,  1 == BitSteg */
        int multiplicator = isLeonStegRGB ? 3 : 1;
        int dim = multiplicator * (width * height) / 8 - LeonSteg.LengthEndText;
        int biggerSideLength = Math.Max(width, height);
        int tW = width;
        int tH = height;

        while (dim > payloadByteCount)
        {
            tW = width;
            tH = height;
            biggerSideLength -= subtrahend;
            CalcBiggerSideLength(biggerSideLength, ref tW, ref tH);
            dim = multiplicator * (tW * tH) / 8 - LeonSteg.LengthEndText;

            if (Math.Max(tW, tH) < minbiggerSideLength)
            { 
                break;
            }
        }

        width = tW;
        height = tH;
    }

    private static void CalcBiggerSideLength(
    int biggerSideLength,
    ref int origWidth,
    ref int origHeight)
    {
        float ratio = (float)origWidth / origHeight;
        if (origWidth > origHeight)
        {
            origWidth = biggerSideLength;
            origHeight = (int)Math.Round(origWidth / ratio);
        }
        else
        {
            origHeight = biggerSideLength;
            origWidth = (int)Math.Round(origHeight * ratio);
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