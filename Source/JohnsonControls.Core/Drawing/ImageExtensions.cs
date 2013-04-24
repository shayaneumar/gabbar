/*----------------------------------------------------------------------------

  (C) Copyright 2012 Johnson Controls, Inc.
      Use or Copying of all or any part of this program, except as
      permitted by License Agreement, is prohibited.

------------------------------------------------------------------------------*/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace JohnsonControls.Drawing
{
    public static class ImageExtensions
    {
        public static Image DownScaleTo(this Image image, int width, int height)
        {
            int srcWidth = image.Width;
            int srcHeight = image.Height;
            if (srcHeight <= height && srcWidth <= width) return image;

            int dstWidth;
            int dstHeight;
            if (srcWidth/(double)width > srcHeight/(double)height)
            {
                dstWidth = width;
                dstHeight = (int)Math.Round(srcHeight * (dstWidth / (double)srcWidth));
            }
            else
            {
                dstHeight = height;
                dstWidth = (int)Math.Round(srcWidth * (dstHeight / (double)srcHeight));
            }
            return image.ResizeTo(dstWidth, dstHeight);
        }

        public static Image ResizeTo(this Image image, int width, int height)
        {
            Image output = new Bitmap(width,height);

            using (var tmpGraphic = Graphics.FromImage(output))
            {
                tmpGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;//Might as well make it look good
                tmpGraphic.DrawImage(image,0,0,width,height);
            }

            return output;
        }
    }
}
