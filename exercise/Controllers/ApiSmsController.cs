using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.Models;
using cyclonestyle.BLL;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 短消息服务
    /// </summary>
    public class ApiSmsController : ApiController
    {
        /// <summary>
        /// 获取短信平台可发送短信的余额
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        [HttpGet]
        public string GetSurplusNum() {
            return MandaoSmsInterFaceService.GetSurplusNum();
        }

        /// <summary>
        /// 发送短消息[使用电话号码]
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        [HttpPost]
        public ReplayBase SendSmsByPhones(SendSmsBaseRequestModel condtion) {
            condtion.createdBy = User.Identity.Name;
            ReplayBase result = SmsService.SendSmsByPhones(condtion);
            return result;
        }

        /// <summary>
        /// 获取短信历史发送历史清单
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        [HttpPost]
        public SearchSmsInfoListReplayModel SearchHistorySmsList(SearchSmsInfoListRequestModel condtion) {
            SearchSmsInfoListReplayModel result = SmsService.SearchHistorySmsList(condtion);
            return result;
        }
    }
}
