using SkiaSharp;

namespace TroonieMobile
{
    /// <summary> Invert image. </summary>
    /// <remarks><para>The filter inverts images.</para> </remarks>    
    public class InvertFilter // : AbstractFilter
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="InvertFilter"/> class.
        /// </summary>
        public InvertFilter()
		{
		}

        #region protected methods

        /// <summary>
        /// Processes the filter on the passed <paramref name="srcBitmap"/>
        /// resulting into <paramref name="dstBitmap"/>.
        /// </summary>
        public unsafe void Process(SKBitmap srcBitmap, SKBitmap dstBitmap)
		{            
            if (srcBitmap.BytesPerPixel != dstBitmap.BytesPerPixel)     
                throw new Exception("BytesPerPixel of source and destination are not identical.");

            int ps = srcBitmap.BytesPerPixel;
            int w = srcBitmap.Width;
            int h = srcBitmap.Height;

            IntPtr pixelsAddrSrc = srcBitmap.GetPixels();
            IntPtr pixelsAddrDst = dstBitmap.GetPixels();

            byte* src = (byte*)pixelsAddrSrc.ToPointer();
            byte* dst = (byte*)pixelsAddrDst.ToPointer();

			// for each line
			for (int y = 0; y < h; y++)
			{
				// for each pixel
				for (int x = 0; x < w; x++, src += ps, dst += ps)
				{
                    // 8 bit grayscale
                    dst[RGBA.B] = (byte)(255 - src[RGBA.B]);

					// rgb(a), 24 and 32 bit
					if (ps != 1) {
                        dst[RGBA.G] = (byte)(255 - src [RGBA.G]);
						dst[RGBA.R] = (byte)(255 - src [RGBA.R]);
                        dst[RGBA.A] = src[RGBA.A];
                    }

                    #region Test
                    //if (y <= 50)
                    //{
                    //    if (x < 50)
                    //    {
                    //        dst[RGBA.R] = 255;
                    //        dst[RGBA.G] = 0;
                    //        dst[RGBA.B] = 0;
                    //    }
                    //    else if (x > w - 50)
                    //    {
                    //        dst[RGBA.R] = 0;
                    //        dst[RGBA.G] = 255;
                    //        dst[RGBA.B] = 0;
                    //    }
                    //}
                    //else if (y > h - 50)
                    //{
                    //    if (x < 50)
                    //    {
                    //        dst[RGBA.R] = 0;
                    //        dst[RGBA.G] = 0;
                    //        dst[RGBA.B] = 255;
                    //    }
                    //    else if (x > w - 50)
                    //    {
                    //        dst[RGBA.R] = 0;
                    //        dst[RGBA.G] = 255;
                    //        dst[RGBA.B] = 255;
                    //    }
                    //}
                    #endregion

                }
            }
		}

		#endregion protected methods             
	}
}