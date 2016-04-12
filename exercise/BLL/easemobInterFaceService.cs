using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using cyclonestyle.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 环信移动客服与通信
    /// </summary>
    public class easemobInterFaceService
    {
        const string client_id = "YXA6J53VwPJXEeWakYVhuIQybg";

        const string client_secret = "YXA6gIeqqJWDLOg-vZqYGIkSNvD8o3c";

        /// <summary>
        /// 环信关联App名称
        /// </summary>
        const string appName = "xindongexercis";
        /// <summary>
        /// 环信注册的企业名称
        /// </summary>
        const string orgName = "xindong";
        /// <summary>
        /// 环信移动客服关联环信IM用户名
        /// </summary>
        const string kefuUserName = "kefu001";

        

        /// <summary>
        /// 远程地址
        /// </summary>
        private static string easemobUrl = "https://a1.easemob.com/{0}/{1}/";

        static easemobInterFaceService() {
            easemobUrl = string.Format(easemobUrl, orgName, appName);
        }

        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <returns></returns>
        internal static string queryToken() {
            string cacheKey = client_id + client_secret;
            if (HttpRuntime.Cache.Get(cacheKey) != null && HttpRuntime.Cache.Get(cacheKey).ToString().Length > 0)
            {
                return HttpRuntime.Cache.Get(cacheKey).ToString();
            }
            string postUrl = easemobUrl + "token";
            StringBuilder sb =
                new StringBuilder("{")
                .AppendFormat("\"grant_type\": \"client_credentials\",\"client_id\": \"{0}\",\"client_secret\": \"{1}\"", client_id, client_secret)
                .Append("}");
            ReplayBase result = RequestUrl(postUrl, "POST", sb.ToString(), string.Empty);
            if (result.ReturnCode == EnumErrorCode.Success) {
                string token = string.Empty;
                int expireSeconds = 0;
                try
                {
                    JObject jo = JObject.Parse(result.ReturnMessage);
                    token = jo.GetValue("access_token").ToString();
                    int.TryParse(jo.GetValue("expires_in").ToString(), out expireSeconds);
                    //设置接口令牌缓存
                    if (!string.IsNullOrEmpty(token) && token.Length > 0 && expireSeconds > 0)
                    {
                        System.Web.HttpRuntime.Cache.Insert(cacheKey, token, null, DateTime.Now.AddSeconds(expireSeconds), System.TimeSpan.Zero);
                    }
                }
                catch { return result.ReturnMessage; }
                return token;
            }
            return string.Empty;
        }

        /// <summary>
        /// 环信接口调用
        /// </summary>
        /// <param name="url">接口url地址</param>
        /// <param name="method">接口方法POST，GET，PUT，DELETE</param>
        /// <param name="datas">接口调用请求参数</param>
        /// <param name="token">接口调用令牌，只有获取令牌时为空，其他操作必须有值</param>
        /// <returns></returns>
        private static ReplayBase RequestUrl(string url, string method, string datas, string token)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                // 创建Http请求实例和请求方法
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = method.ToUpperInvariant();

                // 判断令牌是否为空，第一次请求令牌时参数为空，如果不为空，添加令牌头信息
                if (!string.IsNullOrEmpty(token) && token.Length > 1)
                {
                    request.Headers.Add("Authorization", "Bearer " + token);
                }
                if (request.Method.ToString() != "GET" && !string.IsNullOrEmpty(datas) && datas.Length > 0)
                {
                    // 设置请求返回格式为json
                    request.ContentType = "application/json";
                    // 将请求参数写入请求流对象
                    byte[] buffer = Encoding.UTF8.GetBytes(datas);
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
                // 获取返回结果
                using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        result.ReturnCode = EnumErrorCode.Success;
                        result.ReturnMessage = stream.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                // 如果接口调用返回的HTTP码不是200，Http请求则会抛出异常，在这里捕捉并获取对应的错误信息
                using (StreamReader stream = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8))
                {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    string response = stream.ReadToEnd();
                    try
                    {
                        JObject jo = JObject.Parse(response);
                        result.ReturnMessage = jo.GetValue("error").ToString();
                    }
                    catch { result.ReturnMessage = response; }
                }
            }
            catch (Exception ex)
            {
                result.ReturnCode = EnumErrorCode.EmptyDate;
                result.ReturnMessage = "未知异常信息:" + ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 在环信注册新的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static ReplayBase RegisterUser(RegisterUserEasemobRequestModel condtion) {
            string postDate = JsonConvert.SerializeObject(condtion);
            return RequestUrl(easemobUrl + "users", "POST", postDate, queryToken());
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SendMsg(SendMessageRequestModel condtion)
        {
            string postDate = JsonConvert.SerializeObject(condtion);
            ReplayBase result = RequestUrl(easemobUrl + "messages", "POST", postDate, queryToken());
            return result;
        }
    }
}