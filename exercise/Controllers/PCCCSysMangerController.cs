using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cyclonestyle.BLL;
using cyclonestyle.Models;
using Aspose.Cells;
using System.IO;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 后台系统用户、角色、菜单权限、日志管理
    /// </summary>
    public class PCCCSysMangerController : Controller
    {
        //
        // GET: /PCCCSysManger/

        /// <summary>
        /// 角色设置
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult RoleSet()
        {
            RequestBase condtion = new RequestBase();
            ViewBag.condtion = condtion;
            return View();
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns>角色列表JSON字串</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Users")]
        public string GetRoles() {
            List<SysUserRoleListModel> result = SysManagerService.GetSysRoleAllRowsList();
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 系统菜单设置
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult MenuTreeSet() {
            return View();
        }

        /// <summary>
        /// 系统用户设置
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult SysUserSet() {
            RequestSysUserListModel condtion = new RequestSysUserListModel();
            ViewBag.condtion = condtion;
            ViewBag.DefaultPwd = System.Configuration.ConfigurationManager.AppSettings["DefaultSysUserPassWord"].ToString();
            return View();
        }

        /// <summary>
        /// 系统用户日志
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult SysUserLog(RequestSearchSysUserLog condtion) {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.condtion = condtion;
            return View();
        }

        /// <summary>
        /// 系统运行日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult SysLog(GetSysLogRequestModel condtion) {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.condtion = condtion;
            return View();
        }

        /// <summary>
        /// 导出系统运行日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public string ExcelSysLog(GetSysLogRequestModel condtion) {

            condtion.PageSize = 1000;

            GetSysErrorLogReplayModel result = SysManagerService.SearchSysLog(condtion);

            string dirPath = "/upload/Excel/" + DateTime.Now.ToString("yyMMdd") + "/";
            string FileName = Helps.GetTimeId() + ".xls";
            string SavePath = Server.MapPath(dirPath);

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            SavePath = SavePath + FileName;
            dirPath = dirPath + FileName;

            Workbook wb = new Workbook();
            Worksheet ws = wb.Worksheets[0];
            Cells cell = ws.Cells;

            //合并第一行单元格
            Range range = cell.CreateRange(0, 0, 1, 5);
            range.Merge();
            cell["A1"].PutValue("系统运行日志导出");

            //设置行高
            cell.SetRowHeight(0, 20);

            //设置字体样式
            Style style1 = wb.Styles[wb.Styles.Add()];
            style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
            style1.Font.Name = "宋体";
            style1.Font.IsBold = true;//设置粗体
            style1.Font.Size = 12;//设置字体大小

            Style style2 = wb.Styles[wb.Styles.Add()];
            style2.HorizontalAlignment = TextAlignmentType.Left;
            style2.Font.Size = 10;

            //给单元格关联样式
            cell["A1"].SetStyle(style1);

            //设置列标题
            cell[1, 0].PutValue("序号");
            cell[1, 1].PutValue("类型");
            cell[1, 2].PutValue("时间");
            cell[1, 3].PutValue("内容");
            cell[1, 4].PutValue("请求参数");

            int i = 2;
            foreach (SysErrorLogModel row in result.rows) {
                cell[i, 0].PutValue(i - 1);
                cell[i, 1].PutValue(row.LogTypeText);
                cell[i, 2].PutValue(row.CreatedOn.ToString("yyyy-MM-dd"));
                cell[i, 3].PutValue(row.Errormsg);
                cell[i, 4].PutValue(row.Condtion);
                cell.SetColumnWidth(0,10);
                cell.SetColumnWidth(1, 10);
                cell.SetColumnWidth(2, 10);
                cell.SetColumnWidth(3, 40);
                cell.SetColumnWidth(4, 40);
                i++;
            }
            wb.Save(SavePath);
            return dirPath;
        }
    }
}
