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
        // Fields
        private string imagesDirectoryPath;
        private string thumbnailsDirectoryPath;
        private UploadImageHandler uploadImageHandler;

        // Properties
        private string ImagesDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(imagesDirectoryPath))
                {
                    imagesDirectoryPath = Server.MapPath("~\\Content\\Images");
                }

                return imagesDirectoryPath;
            }
        }

        private string ThumbnailsDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(thumbnailsDirectoryPath))
                {
                    thumbnailsDirectoryPath = Server.MapPath("~\\Content\\Images\\Thumbnails");
                }

                return thumbnailsDirectoryPath;
            }
        }

        private UploadImageHandler UploadImageHandler
        {
            get
            {
                if(uploadImageHandler == null)
                {
                    uploadImageHandler = new UploadImageHandler(ImagesDirectoryPath, ThumbnailsDirectoryPath);
                }

                return uploadImageHandler;
            }
        }

        #region
        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("AdvancedTest");
        }

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

        [HttpPost]
        public ActionResult Upload()
        {
            foreach (string requestFileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[requestFileName];

                if (file != null)
                {
                    string imageFullPath = UploadImageHandler.SaveImage(file);
                    // save imagePath into DB
                    string thumbnailFullPath = UploadImageHandler.CreateThumbnail(file, 100);
                    // save thumbnailPath into DB 
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
