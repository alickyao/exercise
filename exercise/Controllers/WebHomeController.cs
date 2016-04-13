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
        /// <param name="Path">完整的文件存放路径（url）</param>
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

        /// <summary>
        /// 通用上传文件
        /// </summary>
        /// <param name="dir">存放于upload目录下的文件夹名 可为空 为空则为user</param>
        /// <returns>成功返回上传文件的的存放路径(url)，失败返回空字符串</returns>
        [HttpPost]
        public string UploadFile(string dir = null) {
            if (string.IsNullOrEmpty(dir)) {
                dir = "user";
            }
            string filepath = string.Empty;

            //参数
            string savepath = "/upload/"+ dir +"/" + DateTime.Now.ToString("yyMMdd") + "/";
            string serverpath = Server.MapPath(savepath);
            if (!Directory.Exists(serverpath)) {
                Directory.CreateDirectory(serverpath);
            }
            HttpFileCollectionBase files = Request.Files;
            if (files.Count > 0) {
                HttpPostedFileBase file = files[0];
                string fileName, fileExtension;
                //取得上传得文件名
                fileName = Path.GetFileName(file.FileName);
                //取得文件的扩展名
                fileExtension = Path.GetExtension(fileName);

                string newfilename = Guid.NewGuid().ToString();

                file.SaveAs(serverpath + newfilename + fileExtension);
                filepath = savepath + newfilename + fileExtension;
            }
            return filepath;
        }
    }
}
