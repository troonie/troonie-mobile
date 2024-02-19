using Android.Provider;

namespace TroonieMobile
{
    internal class Metatags
    {

    public static void GetMetaTags()
        
        {
            //Java.IO.File? storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
            //string camerapath = string.Empty;
            //if (storagePath != null)
            //    camerapath = System.IO.Path.Combine(storagePath.ToString(), "Camera");

            //var uriExternal = MediaStore.Images.Media.ExternalContentUri; //Since Android 12
            //var uriInternal = MediaStore.Images.Media.InternalContentUri;

            //string[] projection = { 
            //    (MediaStore.Images.Media.InterfaceConsts.Id), 
            //    (MediaStore.Images.Media.InterfaceConsts.DateAdded), 
            //    (MediaStore.Images.Media.InterfaceConsts.RelativePath), 
            //    MediaStore.Images.Media.InterfaceConsts.DisplayName,
            //    MediaStore.Images.Media.InterfaceConsts.DateTaken};
            //var cursor = Android.App.Application.Context.ContentResolver.Query(uriExternal, projection, null, null, MediaStore.Images.Media.InterfaceConsts.DateAdded);


            //if (cursor != null)
            //{
            //    int columnIndexID = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Id);
            //    int RelativePathID = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.RelativePath);
            //    int DisplayNameID = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.DisplayName);
            //    int DateTakenID = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.DateTaken);
            //    int AuthorID = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Author);

            //    while (cursor.MoveToNext())
            //    {
            //        //This is the image path on the Device
            //        string RelativePath = cursor.GetString(RelativePathID);

            //        //This is the path you'll use to load the Image
            //        string ExternalSrcPath = uriExternal + "/" + cursor.GetLong(columnIndexID);

            //        string DisplayName = cursor.GetString(DisplayNameID);
            //        string DateTaken = cursor.GetString(DateTakenID);
            //        string Author = cursor.GetString(AuthorID);
            //    }
            //}
        }
    }
}
