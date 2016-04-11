using System;
using System.Collections.Generic;
using cyclonestyle.DataBase;
using cyclonestyle.Models;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 手机短消息服务
    /// </summary>
    public class SmsService
    {
        /// <summary>
        /// 发送短消息[使用电话号码]
        /// </summary>
        /// <param name="condtion">内容文本不得超过200个字符，且发送的手机号码不得超过5000个</param>
        /// <returns></returns>
        internal static ReplayBase SendSmsByPhones(SendSmsBaseRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                if (condtion.content.Length < 200 && condtion.mobilePhone.Split(',').Length < 5000)
                {
                    //记录到已发送短消息接口
                    ReplayBase savedbrp = SysSmsDataBaseManager.RunSaveSentSms(condtion);
                    //通过漫道短信接口发送短息
                    result = MandaoSmsInterFaceService.SendSms(condtion);
                    //更新已发送为成功
                    SysSmsDataBaseManager.RunUpdateSentSmsStatus(savedbrp.ReturnMessage,result.ReturnCode== EnumErrorCode.Success,result.ReturnMessage);
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "内容文本不得超过200个字符，且发送的手机号码不得超过5000个";
                }
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 检索已发送短信历史列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchSmsInfoListReplayModel SearchHistorySmsList(SearchSmsInfoListRequestModel condtion)
        {
            SearchSmsInfoListReplayModel result = new SearchSmsInfoListReplayModel();
            try
            {
                result = SysSmsDataBaseManager.RunSearchHistorySmsList(condtion);
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.rows = new List<SentSmsInfoModel>();
            }
            return result;
        }
    }
}