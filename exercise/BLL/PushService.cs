using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cyclonestyle.Models;
using cyclonestyle.DataBase;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 推送服务
    /// </summary>
    public class PushService
    {
        #region -- 百度推送
        /// <summary>
        /// 更新用户的百度推送设置
        /// 系统会自动将数据库中其他相同的推送设置清空，避免当同一个设备登录不同的账号后历史登录的账号收到推送消息
        /// </summary>
        /// <param name="condtion">百度推送设置</param>
        /// <param name="userId">用户的ID</param>
        /// <returns></returns>
        internal static ReplayBase UpdateMemberBaiduPushSet(BaiduPushSetModel condtion, string userId)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = SysSmsDataBaseManager.MemberUpdateMemberBaiduPushSet(condtion,userId);
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }
        /// <summary>
        /// 发送百度推送消息【可批量】
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SendBaiduPushByPushSet(SendBaiduPushbySetsRequstModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                if (condtion.sets.Count > 0)
                {
                    int s = 0;//计数器，成功
                    int e = 0;//计数器，失败
                    foreach (BaiduPushSetModel set in condtion.sets) {
                        SendBaiduPushBaseRequestModel q = new SendBaiduPushBaseRequestModel()
                        {
                            set = set,
                            createdBy = condtion.createdBy,
                            customInfo = condtion.customInfo,
                            messagesStyle = condtion.messagesStyle,
                            messageType = condtion.messageType,
                            msg = condtion.msg,
                            title = condtion.title
                        };
                        ReplayBase r = BaiDuPushInterFaceService.SendBaiduPushMsg(q);
                        if (r.ReturnCode == EnumErrorCode.Success)
                        {
                            s++;
                        }
                        else {
                            e++;
                        }
                    }
                    result.ReturnCode = EnumErrorCode.Success;
                    result.ReturnMessage = "已发送" + s.ToString() + "条成功," + e.ToString() + "条失败";
                }
                else {
                    result.ReturnCode = EnumErrorCode.ServiceError;
                    result.ReturnMessage = "请至少传入一条接收者的推送设置";
                }
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        


        #endregion

        /// <summary>
        /// 更新用户的个推设置
        /// 系统会自动将数据库中其他相同的推送设置清空，避免当同一个设备登录不同的账号后历史登录的账号收到推送消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static ReplayBase UpdateMemberGetuiPushSet(GeTuiSetModel condtion, string userId)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = SysSmsDataBaseManager.MemberUpdateMemberGetuiPushSet(condtion, userId);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 发送个推信息【可批量，支持IOS和安卓 批量与单个用户，通知与传透】
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SendGTuiPushByPusSets(SendGeTuiPushBySetRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                condtion.sets = condtion.pushSets;
                if (condtion.sets.Count > 0)
                {
                    ReplayBase record = SysSmsDataBaseManager.RunSaveSentPush(condtion);
                    SendGeTuiPushReplay r = GTuiPushInterFaceService.PushMessage(condtion);
                    SysSmsDataBaseManager.RunUpdateSentPushStatus(record.ReturnMessage, r);
                    result.ReturnMessage = r.sentResultAndroid + "&" + r.sentResultIos;
                }
                else {
                    result.ReturnCode = EnumErrorCode.ServiceError;
                    result.ReturnMessage = "请至少传入一条接收者的推送设置";
                }
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 发送个推信息【按用户列表进行发送】【可批量，支持IOS和安卓 批量与单个用户，通知与传透】
        /// 程序可自动判定这些用户是否有推送设置，如果有才会提交推送平台进行发送
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SendGTuiPushByUsersList(SendGeTuiPushByUsersIdRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            SendGeTuiPushBySetRequestModel q = new SendGeTuiPushBySetRequestModel()
            {
                createdBy = condtion.createdBy,
                customInfo = condtion.customInfo,
                messageType = condtion.messageType,
                msg = condtion.msg,
                title = condtion.title
            };
            if (condtion.userIds.Count > 0)
            {
                //获取用户列表
                List<MembersBaseInfoModel> userlist = MembersService.GetMembersList(new GetMembersListRequstModel() {
                    userIds = condtion.userIds
                });
                foreach (MembersBaseInfoModel user in userlist) {
                    if (user.getuiPushSet != null)
                    {
                        q.pushSets.Add(new GeTuiSetModel() {
                            clientId = user.getuiPushSet.clientId,
                            deviceType = (EnumUserDeviceType)user.getuiPushSet.deviceType,
                            userId = user.UserId
                        });
                    }
                }
                if (q.pushSets.Count > 0)
                {
                    result = SendGTuiPushByPusSets(q);
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "设置的接收者中均无推送接收配置，无法进行推送";
                }
            }
            else {
                result.ReturnCode = EnumErrorCode.EmptyDate;
                result.ReturnMessage = "请至少传入一条接收者的推送设置";
            }
            return result;
        }


        /// <summary>
        /// 获取历史推送
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchSentPushListReplayModel SearchPushHistoryList(SearchSentPushListRequestModel condtion)
        {
            SearchSentPushListReplayModel result = new SearchSentPushListReplayModel();
            try
            {
                result = SysSmsDataBaseManager.RunSearchHistoryPushList(condtion);
            }
            catch (Exception e) {
                result.rows = new List<SentPushInfoModel>();
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }
    }
}