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
    /// 环信即时通信
    /// </summary>
    public class ApiEasemobController : ApiController
    {
        /// <summary>
        /// 注册为环信的新用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public ReplayBase RegisterUser(RegisterUserEasemobRequestModel condtion) {
            return easemobInterFaceService.RegisterUser(condtion);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public ReplayBase SendMsg(SendMessageRequestModel condtion) {
            ReplayBase result = easemobInterFaceService.SendMsg(condtion);
            return result;
        }
    }
}
