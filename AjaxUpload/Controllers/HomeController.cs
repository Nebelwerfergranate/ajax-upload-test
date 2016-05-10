using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AjaxUpload.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("AdvancedTest");
        }

        [HttpGet]
        public ActionResult Test() {
            return View();
        }
        
        // here
        public ActionResult AdvancedTest()
        {
            return View();
        } 

        [HttpPost]
        public JsonResult Upload()
        {
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if(upload != null)
                {
                    // getting file name
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    // saving file in a Files folder in project.

                    upload.SaveAs(Server.MapPath("~\\Files\\" + fileName));
                   
                }
            }

            return Json("файл загружен");
        }
    }
}
