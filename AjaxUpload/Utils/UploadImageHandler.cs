using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AjaxUpload.Utils
{
    public class UploadImageHandler
    {
        // fields
        string imagesDirectoryPath = string.Empty;
        string thumbnailsDirectoryPath = string.Empty;


        // constructors
        public UploadImageHandler() { }
        public UploadImageHandler(string imagesDirectoryPath, string thumbnailsDirectoryPath)
        {
            ImagesDirectoryPath = imagesDirectoryPath;
            ThumbnailsDirectoryPath = thumbnailsDirectoryPath;
        }


        // properties
        public string ImagesDirectoryPath
        {
            get { return imagesDirectoryPath; }
            set { imagesDirectoryPath = value; }
        }

        public string ThumbnailsDirectoryPath
        {
            get { return thumbnailsDirectoryPath; }
            set { thumbnailsDirectoryPath = value; }
        }


        //Methods
        public string SaveImage(HttpPostedFileBase file)
        {
            string fileName = Path.GetFileName(file.FileName);
            string imageFullPath = GetImageFullPath(fileName);
            file.SaveAs(imageFullPath);

            return imageFullPath;
        }

        public string CreateThumbnail(HttpPostedFileBase file, int maxSize)
        {
            string fileName = Path.GetFileName(file.FileName);
            string imageFullPath = GetImageFullPath(fileName);
            string thumbnailFullPath = GetThumbnailFullPath(fileName);

            Image image = Image.FromFile(imageFullPath);
            Size thumbnailSize = GetThumbNailSize(image, maxSize);

            Rectangle imageRectangle = new Rectangle(0, 0, thumbnailSize.Width, thumbnailSize.Height);
            Bitmap thumbnailBitmap = new Bitmap(thumbnailSize.Width, thumbnailSize.Height);

            Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            thumbnailGraph.DrawImage(image, imageRectangle);

            thumbnailBitmap.Save(thumbnailFullPath);

            image.Dispose();                       
            thumbnailBitmap.Dispose();
            thumbnailGraph.Dispose();

            return thumbnailFullPath;
        }

        private string GetImageFullPath(string fileName)
        {
            if (String.IsNullOrEmpty(ImagesDirectoryPath))
            {
                throw new ApplicationException("ImagesDirectoryPath isn't assigned");
            }
    
            return Path.Combine(ImagesDirectoryPath, fileName);
        }

        private string GetThumbnailFullPath(string fileName)
        {
            if (String.IsNullOrEmpty(ThumbnailsDirectoryPath))
            {
                throw new ApplicationException("ThumblailsDirectoryPath isn't assigned");
            }

            return Path.Combine(ThumbnailsDirectoryPath, fileName);
        }

        private Size GetThumbNailSize(Image image, int maxSize)
        {
            Size thumbnailSize = new Size();

            double ratio = (double)image.Size.Width / image.Size.Height;
            thumbnailSize.Width = maxSize;
            thumbnailSize.Height = maxSize;

            if (ratio > 1.0)
            {
                thumbnailSize.Height = (int)(100 / ratio);
            }
            else if (ratio < 1.0)
            {
                thumbnailSize.Width = (int)(100 * ratio);
            }

            return thumbnailSize;
        }
    }
}