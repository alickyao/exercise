using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 基础页
    /// </summary>
    public class WebHomeController : Controller
    {
        //
        // GET: /WebHome/

        /// <summary>
        /// 项目首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 身份验证错误
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthorizeError() {
            return View();
        }

        /// <summary>
        /// 通用下载文件
        /// </summary>
        /// <param name="Path">文件URL路径</param>
        public void DownloadFile(string Path) {
            string serverpath = Server.MapPath(Path);
            Response.ClearHeaders();
            Response.Clear();
            Response.Expires = 0;
            Response.Buffer = true;
            Response.AddHeader("Accept-Language", "zh-tw");
            string name = System.IO.Path.GetFileName(serverpath);
            System.IO.FileStream files = new FileStream(serverpath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] byteFile = null;
            if (files.Length == 0)
            {
                byteFile = new byte[1];
            }
            else
            {
                byteFile = new byte[files.Length];
            }
            files.Read(byteFile, 0, (int)byteFile.Length);
            files.Close();

            Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8));
            Response.ContentType = "application/octet-stream;charset=gbk";
            Response.BinaryWrite(byteFile);
            Response.End();
        }
    }
}
