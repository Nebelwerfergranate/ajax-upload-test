using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Net;
using System.Drawing;
using System.IO;
using AjaxUpload.Utils;

namespace AjaxUpload.Controllers
{
    public class HomeController : Controller
    {
        #region Fields
        private string imagesDirRelativePath = "Content\\Images";
        private string thumbnailsDirRelativePath = "Content\\Images\\Thumbnails";
        private string virtualPath;
        private UploadImageManager uploadImageHandler;

        static List<ApplicationImageInfo> db = new List<ApplicationImageInfo>();
        //private
        #endregion

        #region Properties
        private string ImagesDirRelativePath
        {
            get { return imagesDirRelativePath; }
        }

        private string ThumbnailsDirRelativePath
        {
            get { return thumbnailsDirRelativePath; }
        }

        private string VirtualPath
        {
            get
            {
                if (string.IsNullOrEmpty(virtualPath))
                {
                    virtualPath = Server.MapPath("~");
                }

                return virtualPath;
            }
        }

        private UploadImageManager UploadManager
        {
            get
            {
                if (uploadImageHandler == null)
                {
                    uploadImageHandler = new UploadImageManager(VirtualPath, ImagesDirRelativePath, ThumbnailsDirRelativePath);
                }

                return uploadImageHandler;
            }
        }
        #endregion

        #region Primary Methods
        public ActionResult Index()
        {
            return View("AdvancedTest", db);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            foreach (string requestFileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[requestFileName];

                if (file != null)
                {
                    ApplicationImageInfo item = new ApplicationImageInfo();

                    string imageRelativePath = UploadManager.SaveImage(file);
                    item.ImagePath = imageRelativePath;

                    string thumbnailRelativePath = UploadManager.CreateThumbnail(file, new Size(200, 100), CutOptions.CropHeight);
                    item.ThumbnailPath = thumbnailRelativePath;

                    db.Add(item);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion

        #region Secondary Methods
        [HttpGet]
        public ActionResult Test()
        {
            return View();
        }

        // here
        public ActionResult AdvancedTest()
        {
            return View();
        }
        #endregion
    }
}
