using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjaxUpload.Utils
{
    public struct Size
    {
        public Size(int height, int width): this() { }

        public int Height { get; set; }
        public int Width { get; set; }
    }
}