using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.Models;
using cyclonestyle.BLL;
using System.Web;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 环信即时通信
    /// </summary>
    public class ApiEasemobController : ApiController
    {
        /// <summary>
        /// 获取环信令牌
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string QueryToken()
        {
            string result = easemobInterFaceService.queryToken();
            return result;
        }

        /// <summary>
        /// 注册为环信的新用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public ReplayBase RegisterUser(RegisterUserEasemobRequestModel condtion) {
            return easemobInterFaceService.RegisterUser(condtion);
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public ReplayBase SendMsg(EasemobSendTextMessageRequestModel condtion) {
            ReplayBase result = easemobInterFaceService.SendMsg(condtion);
            return result;
        }

        /// <summary>
        /// 发送图片消息
        /// </summary>
        /// <param name="condtion">请求参数中的 condtion.msg.url 为图片文件在服务器上的保存路径，请先调用WebHome/UploadFile将图片上传到服务器</param>
        /// <returns></returns>
        [HttpPost]
        public ReplayBase SendImg(EasemobSendIMgMessageRequestModel condtion) {
            condtion.msg.url = HttpContext.Current.Server.MapPath(condtion.msg.url);
            ReplayBase result = easemobInterFaceService.SendImgMsg(condtion);
            return result;
        }

        /// <summary>
        /// 发送语音消息
        /// </summary>
        /// <param name="condtion">请求参数中的 condtion.msg.url 为语音文件在服务器上的保存路径，请先调用WebHome/UploadFile将语音上传到服务器</param>
        /// <returns></returns>
        [HttpPost]
        public ReplayBase SendAudio(EasemobSendAudioMessageRequestModel condtion) {
            condtion.msg.url = HttpContext.Current.Server.MapPath(condtion.msg.url);
            ReplayBase result = easemobInterFaceService.SendAudioMsg(condtion);
            return result;
        }
    }
}
