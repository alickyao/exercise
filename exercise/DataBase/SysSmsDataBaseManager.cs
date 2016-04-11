using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Linq.SqlClient;
using cyclonestyle.Models;

namespace cyclonestyle.DataBase
{
    /// <summary>
    /// 消息相关数据库操作专用
    /// 短信，推送，系统消息等专用
    /// </summary>
    public class SysSmsDataBaseManager : BaseSysTemDataBaseManager
    {
        #region -- 短消息
        /// <summary>
        /// 保存已发送的短信
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns>返回新增的发送ID</returns>
        internal static ReplayBase RunSaveSentSms(SendSmsBaseRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                ReplayBase result = new ReplayBase()
                {
                    ReturnMessage = Guid.NewGuid().ToString()
                };
                Us_RunSentSms dbs = new Us_RunSentSms() {
                    Us_RunSentSmsId = result.ReturnMessage,
                    contentMsg = condtion.content,
                    createdOn = DateTime.Now,
                    mobilePhones = condtion.mobilePhone,
                    sendState = false,
                    createdBy = condtion.createdBy
                };
                context.Us_RunSentSms.InsertOnSubmit(dbs);
                context.SubmitChanges();
                return result;
            }
        }
        /// <summary>
        /// 更新已发短信状态与接口返回文本
        /// </summary>
        /// <param name="sentSmsId">已发短信记录的ID</param>
        /// <param name="sendState">发送状态</param>
        /// <param name="returnMessage">返回文本</param>
        internal static void RunUpdateSentSmsStatus(string sentSmsId, bool sendState, string returnMessage)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                Us_RunSentSms dbs = context.Us_RunSentSms.Single(p => p.Us_RunSentSmsId == sentSmsId);
                dbs.sendState = sendState;
                dbs.returnMsg = returnMessage;
                context.SubmitChanges();
            }
        }
        /// <summary>
        /// 获取已发送的短信历史列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchSmsInfoListReplayModel RunSearchHistorySmsList(SearchSmsInfoListRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                SearchSmsInfoListReplayModel result = new SearchSmsInfoListReplayModel();
                result.total = (from c in context.Us_RunSentSms
                                where (1 == 1)
                                && (string.IsNullOrEmpty(condtion.mobilePhones) ? true : SqlMethods.Like(c.mobilePhones, string.Format("%{0}%", condtion.mobilePhones)))
                                && (string.IsNullOrEmpty(condtion.contentMsg) ? true : SqlMethods.Like(c.contentMsg, string.Format("%{0}%", condtion.contentMsg)))
                                select c.Us_RunSentSmsId).Count();
                if (result.total > 0) {
                    result.rows = (from c in context.Us_RunSentSms
                                   where (1 == 1)
                                   && (string.IsNullOrEmpty(condtion.mobilePhones) ? true : SqlMethods.Like(c.mobilePhones, string.Format("%{0}%", condtion.mobilePhones)))
                                   && (string.IsNullOrEmpty(condtion.contentMsg) ? true : SqlMethods.Like(c.contentMsg, string.Format("%{0}%", condtion.contentMsg)))
                                   orderby c.createdOn descending
                                   select new SentSmsInfoModel
                                   {
                                       contentMsg = c.contentMsg,
                                       createdOn = c.createdOn,
                                       id = c.Us_RunSentSmsId,
                                       mobilePhones = c.mobilePhones,
                                       returnMsg = c.returnMsg,
                                       sendState = c.sendState,
                                       createdBy = SysGetSysUserById(c.createdBy)
                                   }).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                }
                else
                {
                    result.rows = new List<SentSmsInfoModel>();
                }
                return result;
            }
        }

        



        #endregion

        #region -- 推送
        /// <summary>
        /// 更新用户的百度推送设置
        /// 系统会自动将数据库中其他相同的推送设置清空，避免当同一个设备登录不同的账号后历史登录的账号收到推送消息
        /// </summary>
        /// <param name="condtion">推送设置</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        internal static ReplayBase MemberUpdateMemberBaiduPushSet(BaiduPushSetModel condtion,string userId)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                //先清空与该配置相同的其他用户的推送设置
                var d = (from c in context.Us_SysUser
                         where (c.baiduPushchannelId == condtion.channelId)
                         && (c.baiduPushUserId == condtion.userId)
                         && (c.baiduPushdeviceType == (byte)condtion.deviceType.GetHashCode())
                         select c).ToList();
                if (d.Count > 0) {
                    foreach (var s in d) {
                        s.baiduPushchannelId = null;
                        s.baiduPushUserId = null;
                        s.baiduPushdeviceType = null;
                    }
                    context.SubmitChanges();
                }
                //然后再更新用户的推送设置
                Us_SysUser dbuser = context.Us_SysUser.Single(p => p.Us_SysUserId == userId);
                dbuser.baiduPushchannelId = condtion.channelId;
                dbuser.baiduPushUserId = condtion.userId;
                dbuser.baiduPushdeviceType = (byte)condtion.deviceType.GetHashCode();
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }

        

        /// <summary>
        /// 获取用户的百度推送设置
        /// </summary>
        /// <param name="dbuser">数据库对象Us_SysUser</param>
        /// <returns></returns>
        internal static BaiduPushSetModel MemberGetMemberBaiduPushSetInfo(Us_SysUser dbuser) {
            if (dbuser.baiduPushdeviceType == null) {
                return null;
            }
            BaiduPushSetModel result = new BaiduPushSetModel() {
               channelId = dbuser.baiduPushchannelId,
               deviceType = (EnumUserDeviceType)dbuser.baiduPushdeviceType,
               userId = dbuser.baiduPushUserId
            };
            return result;
        }
        /// <summary>
        /// 更新用户的个推设置信息
        /// 系统会自动将数据库中其他相同的推送设置清空，避免当同一个设备登录不同的账号后历史登录的账号收到推送消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static ReplayBase MemberUpdateMemberGetuiPushSet(GeTuiSetModel condtion, string userId)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                var d = (from c in context.Us_SysUser
                         where (c.getuiPushClientId == condtion.clientId)
                         && (c.getuiPushDeviceType == (byte)condtion.deviceType.GetHashCode())
                         select c).ToList();
                if (d.Count > 0)
                {
                    foreach (var s in d)
                    {
                        s.getuiPushClientId = null;
                        s.getuiPushDeviceType = null;
                    }
                    context.SubmitChanges();
                }

                Us_SysUser dbuser = context.Us_SysUser.Single(p => p.Us_SysUserId == userId);
                dbuser.getuiPushClientId = condtion.clientId;
                dbuser.getuiPushDeviceType = (byte)condtion.deviceType.GetHashCode();
                context.SubmitChanges();
                return new ReplayBase();
            }
        }

        /// <summary>
        /// 获取用户的个推推送设置
        /// </summary>
        /// <param name="dbuser"></param>
        /// <returns></returns>
        internal static GeTuiSetModel MemberGetMemberGetuiPushSetInfo(Us_SysUser dbuser) {
            if (dbuser.getuiPushDeviceType == null) {
                return null;
            }
            GeTuiSetModel result = new GeTuiSetModel() {
                clientId = dbuser.getuiPushClientId,
                deviceType = (EnumUserDeviceType)dbuser.getuiPushDeviceType,
                userId = dbuser.Us_SysUserId
            };
            return result;
        }

        /// <summary>
        /// 保存待发送的推送消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase RunSaveSentPush(SendGeTuiPushBySetRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                ReplayBase result = new ReplayBase() {
                    ReturnMessage = Guid.NewGuid().ToString()
                };
                Us_RunSentPush dbp = new Us_RunSentPush() {
                    Us_RunSentPushId = result.ReturnMessage,
                    createdBy = condtion.createdBy,
                    createdOn = DateTime.Now,
                    customInfo = Newtonsoft.Json.JsonConvert.SerializeObject(condtion.customInfo),
                    messageType = (byte)condtion.messageType.GetHashCode(),
                    msg = condtion.msg,
                    sentStats = false,
                    title = condtion.title
                };
                foreach (GeTuiSetModel s in condtion.sets) {
                    dbp.Us_RunSentPushDetail.Add(new Us_RunSentPushDetail() {
                        clientId = s.clientId,
                        deviceType = (byte)s.deviceType.GetHashCode(),
                        sentUserId = s.userId,
                        Us_RunSentPushDetailId = Guid.NewGuid().ToString(),
                        Us_RunSentPushId = result.ReturnMessage
                    });
                }
                context.Us_RunSentPush.InsertOnSubmit(dbp);
                context.SubmitChanges();
                return result;
            }
        }

        /// <summary>
        /// 更新推送设置的状态与结果
        /// </summary>
        /// <param name="pushId">推送的ID</param>
        /// <param name="r">推送结果信息</param>
        internal static void RunUpdateSentPushStatus(string pushId, SendGeTuiPushReplay r)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                Us_RunSentPush dbp = context.Us_RunSentPush.Single(p => p.Us_RunSentPushId == pushId);
                dbp.sentStats = r.ReturnCode == EnumErrorCode.Success ? true : false;
                dbp.sentResultAndroid = r.sentResultAndroid;
                dbp.sentResultIos = r.sentResultIos;
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// 查询推送历史
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchSentPushListReplayModel RunSearchHistoryPushList(SearchSentPushListRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                SearchSentPushListReplayModel result = new SearchSentPushListReplayModel();
                result.total = (from c in context.Us_RunSentPush
                                where (1 == 1)
                                && (condtion.beginDate == null ? true : c.createdOn >= condtion.beginDate)
                                && (condtion.endDate == null ? true : c.createdOn <= condtion.endDate)
                                && (string.IsNullOrEmpty(condtion.keyWords) ? true : (SqlMethods.Like(c.title, string.Format("%{0}%", condtion.keyWords)) || SqlMethods.Like(c.msg, string.Format("%{0}%", condtion.keyWords))))
                                select c.Us_RunSentPushId).Count();
                if (result.total > 0)
                {
                    result.rows = (from c in context.Us_RunSentPush
                                   where (1 == 1)
                                   && (condtion.beginDate == null ? true : c.createdOn >= condtion.beginDate)
                                   && (condtion.endDate == null ? true : c.createdOn <= condtion.endDate)
                                   && (string.IsNullOrEmpty(condtion.keyWords) ? true : (SqlMethods.Like(c.title, string.Format("%{0}%", condtion.keyWords)) || SqlMethods.Like(c.msg, string.Format("%{0}%", condtion.keyWords))))
                                   orderby c.createdOn descending
                                   select new SentPushInfoModel
                                   {
                                       id = c.Us_RunSentPushId,
                                       createdBy = BaseSysTemDataBaseManager.SysGetSysUserById(c.createdBy),
                                       createdOn = c.createdOn,
                                       customInfo = c.customInfo,
                                       messageType = (EnumPushMessagesType)c.messageType,
                                       msg = c.msg,
                                       sentResultAndroid = c.sentResultAndroid,
                                       sentResultIos = c.sentResultIos,
                                       sentStats = c.sentStats,
                                       sentUserNum = c.Us_RunSentPushDetail.Count,
                                       title = c.title,
                                       sentUserList = (from x in c.Us_RunSentPushDetail
                                                       select new SentPushInfoDetailInfoModel
                                                       {
                                                           sentDeviceInfo = new GeTuiSetModel()
                                                           {
                                                               clientId = x.clientId,
                                                               deviceType = (EnumUserDeviceType)x.deviceType,
                                                               userId = x.sentUserId
                                                           },
                                                           sentUserInfo = (x.sentUserId == null || x.sentUserId == "") ? null :
                                                           BaseSysTemDataBaseManager.SysGetSysUserById(x.sentUserId)
                                                       }
                                                       ).ToList()
                                   }).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                }
                else {
                    result.rows = new List<SentPushInfoModel>();
                }
                return result;
            }
        }

        #endregion
    }
}