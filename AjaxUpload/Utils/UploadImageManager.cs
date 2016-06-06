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

        public string CreateThumbnail(HttpPostedFileBase file, Size desiredSize, CutOptions cutOption = CutOptions.AllowTransform)
        {
            string fileName = Path.GetFileName(file.FileName);
            string sourceImagePath = GetImageFullPath(fileName);
            string thumbnailPath = GetThumbnailFullPath(fileName);

            using (Bitmap sourceImage = GetImageFromPath(sourceImagePath))
            {
                Size thumbnailSize = GetThumbnailSize(sourceImage, desiredSize, cutOption);

                using (Bitmap thumbnail = GetCompressedImage(sourceImage, thumbnailSize))
                using (Bitmap cropedImage = GetCropedImage(thumbnail, desiredSize, cutOption))
                {
                    cropedImage.Save(thumbnailPath);
                }
            }

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

        private Bitmap GetImageFromPath(string sourceImagePath)
        {
            Bitmap sourceImage = Image.FromFile(sourceImagePath) as Bitmap; // big image 

            if (sourceImage == null)
            {
                throw new ApplicationException("source image isn't valid");
            }

            return sourceImage;
        }

        private Size GetThumbnailSize(Image sourceImage, Size desiredSize, CutOptions cutOption)
        {
            Size size;
            double compressionRate;
            double ratio = (double)sourceImage.Width / sourceImage.Height;

            switch (cutOption)
            {
                case CutOptions.AllowTransform:
                    size = desiredSize;
                    break;
                case CutOptions.CropHeight:
                    compressionRate = sourceImage.Width / desiredSize.Width;
                    int height = Convert.ToInt32(sourceImage.Height / compressionRate);
                    size = new Size(desiredSize.Width, height);
                    break;
                case CutOptions.CropWidth:
                    compressionRate = sourceImage.Height / desiredSize.Height;
                    int width = Convert.ToInt32(sourceImage.Width / compressionRate);
                    size = new Size(width, desiredSize.Height);
                    break;
                default:
                    string msg = string.Format("Cut option {0} is not supported", cutOption);
                    throw new ArgumentException(msg);
            }

            return size;
        }

        private Bitmap GetCompressedImage(Image sourceImage, Size newSize)
        {
            Bitmap thumbnailBitmap = new Bitmap(sourceImage, newSize); // small image

            using(Graphics thumbnailGraphics = Graphics.FromImage(thumbnailBitmap))
            {
                ConfigureGraphics(thumbnailGraphics);

                // How much place drawing will take. Should be the same as the canvas to cover it all.
                Rectangle drawingRectangle = new Rectangle(0, 0, newSize.Width, newSize.Height);
                thumbnailGraphics.DrawImage(sourceImage, drawingRectangle);

                return thumbnailBitmap;
            }  
        }

        private void ConfigureGraphics(Graphics thumbnailGraphics)
        {
            thumbnailGraphics.Clear(Color.Transparent);
            thumbnailGraphics.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraphics.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        private Bitmap GetCropedImage(Bitmap thumbnail, Size desiredSize, CutOptions cutOption)
        {
            if (cutOption == CutOptions.AllowTransform)
            {
                return thumbnail;
            }

            Rectangle selection = new Rectangle(0, 0, desiredSize.Width, desiredSize.Height);

            return thumbnail.Clone(selection, thumbnail.PixelFormat);
        }
        #endregion
    }
}