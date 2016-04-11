using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.BLL;
using cyclonestyle.Models;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 推送相关
    /// </summary>
    public class ApiPushController : ApiController
    {
        #region -- 百度推送
        ///// <summary>
        ///// 更新当前登录用户的百度推送设置
        ///// </summary>
        ///// <param name="condtion"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public ReplayBase UpdateMemberBaiduPushSet(BaiduPushSetModel condtion) {
        //    ReplayBase result = PushService.UpdateMemberBaiduPushSet(condtion,User.Identity.Name);
        //    return result;
        //}

        ///// <summary>
        ///// 发送一个百度推送消息【使用用户的百度推送设置】
        ///// </summary>
        ///// <param name="condtion"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Authorize]
        //public ReplayBase SendBaiduPushMsgByPushSet(SendBaiduPushbySetsRequstModel condtion) {
        //    condtion.createdBy = User.Identity.Name;
        //    ReplayBase result = PushService.SendBaiduPushByPushSet(condtion);
        //    return result;
        //}
        #endregion

        
        /// <summary>
        /// 更新用户的个推设置信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase UpdateMemberGetuiPushSet(GeTuiSetModel condtion) {
            ReplayBase result = PushService.UpdateMemberGetuiPushSet(condtion, User.Identity.Name);
            return result;
        }

        /// <summary>
        /// 发送个推信息【按推送设置进行发送】【可批量，支持IOS和安卓 批量与单个用户，通知与传透】
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase SendGTuiPushByPushSets(SendGeTuiPushBySetRequestModel condtion) {
            condtion.createdBy = User.Identity.Name;
            ReplayBase result = PushService.SendGTuiPushByPusSets(condtion);
            return result;
        }

        /// <summary>
        /// 发送个推信息【按用户列表进行发送】【可批量，支持IOS和安卓 批量与单个用户，通知与传透】
        /// 程序可自动判定这些用户是否有推送设置，如果有才会提交推送平台进行发送
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public ReplayBase SendGTuiPushByUsersList(SendGeTuiPushByUsersIdRequestModel condtion) {
            condtion.createdBy = User.Identity.Name;
            ReplayBase result = PushService.SendGTuiPushByUsersList(condtion);
            return result;
        }

        /// <summary>
        /// 【后台专用】查询已发送的推送历史
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public SearchSentPushListReplayModel SearchPushHistoryList(SearchSentPushListRequestModel condtion) {
            SearchSentPushListReplayModel result = PushService.SearchPushHistoryList(condtion);
            return result;
        }
    }
}
