﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cyclonestyle.Models;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 后台-资源文档管理
    /// </summary>
    public class PCCCDocumentResourceController : Controller
    {
        //
        // GET: /PCCCDocumentResource/

        /// <summary>
        /// 资源文档管理
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult DocumentsGrid()
        {
            SearchDocumentsResourceListRequestModel condtion = new SearchDocumentsResourceListRequestModel();
            ViewBag.condtion = condtion;
            return View();
        }

        /// <summary>
        /// 文本编辑器文件上传
        /// </summary>
        [Authorize(Roles = "Admin,Users")]
        public void EditorImgFileUpload() {
            Hashtable hash = new Hashtable();
            Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
            bool error = false;
            //获取上传的文件
            HttpFileCollectionBase Files = Request.Files;

            string UploadPath = "/upload/Editor/" + DateTime.Now.ToString("yyMMdd") + "/";

            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

            //最大文件大小
            string maxsize = System.Configuration.ConfigurationManager.AppSettings["HtmlEditFileMaxSize"].ToString();
            maxsize = (decimal.Parse(maxsize) * 1000000).ToString("0");
            int maxSize = int.Parse(maxsize);
            //文件类型设定
            string dirName = Request.QueryString["dir"];
            if (String.IsNullOrEmpty(dirName))
            {
                dirName = "image";
            }

            
            if (Files == null) {
                hash["error"] = 1;
                hash["message"] = "请选择上传的文件";
                error = true;
            }
            if (!error) {
                if (Files.Count > 0)
                {
                    HttpPostedFileBase postedFile = Files[0];

                    //取得上传得文件名
                    string fileName = System.IO.Path.GetFileName(postedFile.FileName);
                    //取得文件的扩展名
                    string fileExtension = System.IO.Path.GetExtension(fileName);
                    //重新定义的文件名
                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    

                    if (postedFile.InputStream == null)
                    {
                        hash["error"] = 1;
                        hash["message"] = "请选择上传的文件";
                        error = true;
                    }
                    if (postedFile.InputStream.Length > maxSize) {
                        hash["error"] = 1;
                        hash["message"] = "文件超出了限制的" + (maxSize / 1000000).ToString() + "M，无法上传";
                        error = true;
                    }

                    if (String.IsNullOrEmpty(fileExtension) || Array.IndexOf(((String)extTable[dirName]).Split(','), fileExtension.Substring(1).ToLower()) == -1)
                    {
                        hash["error"] = 1;
                        hash["message"] = "上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。";
                        error = true;
                    }
                    if (!error)
                    {
                        string SavePath = Server.MapPath(UploadPath);
                        if (!Directory.Exists(SavePath))
                        {
                            Directory.CreateDirectory(SavePath);
                        }
                        //完整的保存路径
                        string NewFileUrl = SavePath + newFileName + fileExtension;
                        postedFile.SaveAs(NewFileUrl);

                        hash["error"] = 0;
                        hash["url"] = UploadPath + newFileName + fileExtension;
                        string result = JsonConvert.SerializeObject(hash);
                        Response.Write(result);
                        Response.End();
                    }
                }
                else {
                    hash["error"] = 1;
                    hash["message"] = "请选择上传的文件";
                }
                string errorresponse = JsonConvert.SerializeObject(hash);
                
                Response.Write(errorresponse);
                Response.End();
            }
        }
    }
}
