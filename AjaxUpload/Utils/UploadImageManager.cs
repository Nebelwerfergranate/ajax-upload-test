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
    public class UploadImageManager
    {

        #region Fields
        private string virtualPath = string.Empty;
        private string imagesDirRelativePath = string.Empty;
        private string thumbnailsDirRelativePath = string.Empty;
        #endregion

        #region Constructors
        public UploadImageManager() { }

        public UploadImageManager(string virtualPath, string imagesDirRelativePath, string thumbnailsDirRelativePath)
        {
            VirtualPath = virtualPath;
            ImagesDirRelativePath = imagesDirRelativePath;
            ThumbnailsDirRelativePath = thumbnailsDirRelativePath;
        }
        #endregion

        #region Properties
        public string VirtualPath
        {
            get { return virtualPath; }
            set { virtualPath = value; }
        }

        public string ImagesDirRelativePath
        {
            get { return imagesDirRelativePath; }
            set { imagesDirRelativePath = value; }
        }

        public string ThumbnailsDirRelativePath
        {
            get { return thumbnailsDirRelativePath; }
            set { thumbnailsDirRelativePath = value; }
        }
        #endregion

        #region Public Methods
        public string SaveImage(HttpPostedFileBase file)
        {
            string fileName = Path.GetFileName(file.FileName);
            string imageFullPath = GetImageFullPath(fileName);
            file.SaveAs(imageFullPath);

            return Path.Combine(imagesDirRelativePath, fileName);
        }

        public string CreateThumbnail(HttpPostedFileBase file, Size size)
        {
            string fileName = Path.GetFileName(file.FileName);
            string imageFullPath = GetImageFullPath(fileName);
            string thumbnailFullPath = GetThumbnailFullPath(fileName);

            Image image = Image.FromFile(imageFullPath);
            Size thumbnailSize = GetThumbNailSize(image, size);

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

            return Path.Combine(thumbnailsDirRelativePath, fileName);
        }
        #endregion

        #region Private Methods
        private string GetImageFullPath(string fileName)
        {
            if (String.IsNullOrEmpty(ImagesDirRelativePath))
            {
                throw new ApplicationException("ImagesDirectoryPath isn't assigned");
            }

            if (String.IsNullOrEmpty(VirtualPath))
            {
                throw new ApplicationException("VirtualPath isn't assigned");
            }

            return Path.Combine(virtualPath, ImagesDirRelativePath, fileName);
        }

        private string GetThumbnailFullPath(string fileName)
        {
            if (String.IsNullOrEmpty(ThumbnailsDirRelativePath))
            {
                throw new ApplicationException("ThumblailsDirectoryPath isn't assigned");
            }

            if (String.IsNullOrEmpty(VirtualPath))
            {
                throw new ApplicationException("VirtualPath isn't assigned");
            }

            return Path.Combine(VirtualPath, ThumbnailsDirRelativePath, fileName);
        }

        // todo - remake
        private Size GetThumbNailSize(Image image, Size thumbnailSize)
        {
            double ratio = (double)image.Size.Width / image.Size.Height;

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
        #endregion
    }
}