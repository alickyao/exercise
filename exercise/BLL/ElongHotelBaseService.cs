using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using cyclonestyle.Models;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 艺龙远程酒店接口服务
    /// </summary>
    public class ElongHotelBaseService
    {
        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            // trust any certificate!!!
            System.Console.WriteLine("Warning, trust any certificate");
            //为了通过证书验证，总是返回true
            return true;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public ElongHotelBaseService()
        {
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        }

        /// <summary>
        /// 返回对象设置
        /// </summary>
        protected static string _format = "json";
        /// <summary>
        /// 远程URL
        /// </summary>
        protected static string URL = System.Configuration.ConfigurationManager.AppSettings["BASE_URL"];
        /// <summary>
        /// 远程URL（HTTPS）
        /// </summary>
        protected static string URL_HTTPS = System.Configuration.ConfigurationManager.AppSettings["BASE_URL_HTTPS"];
        /// <summary>
        /// 版本
        /// </summary>
        protected static string Ver = System.Configuration.ConfigurationManager.AppSettings["BASE_VER"];
        /// <summary>
        /// 用户
        /// </summary>
        protected static string APIUSER = System.Configuration.ConfigurationManager.AppSettings["APIUSER"];
        /// <summary>
        /// APPKEY
        /// </summary>
        protected static string APPKEY = System.Configuration.ConfigurationManager.AppSettings["APPKEY"];
        /// <summary>
        /// 秘钥
        /// </summary>
        protected static string SECRETKEY = System.Configuration.ConfigurationManager.AppSettings["SECRETKEY"];
        /// <summary>
        /// 查询远程接口
        /// </summary>
        /// <typeparam name="T">请求参数类型（类）</typeparam>
        /// <typeparam name="T2">返回对象BaseResponse中Result的类</typeparam>
        /// <param name="value">请求参数值</param>
        /// <param name="Method">远程服务器方法</param>
        /// <returns>酒店接口基本返回对象</returns>
        protected HotelBaseResponse<T2> GetConditionResponse<T, T2>(object value, string Method)
        {
            HotelBaseRequest<T> condition = new HotelBaseRequest<T>();
            condition.Request = (T)value;
            condition.Version = double.Parse(Ver);
            string str = SearilizeObject.Searilize(_format, typeof(HotelBaseResponse<T>), condition, typeof(T));


            DateTime now = DateTime.Now;
            long timestamp = Helps.ConvertDateTimeInt(now);
            string sig = Helps.MD5(timestamp + Helps.MD5(str + APPKEY) + SECRETKEY);


            StringBuilder sb = new StringBuilder();
            sb.Append("format=").Append(_format).Append("&");
            sb.Append("method=").Append(Method).Append("&");
            sb.Append("signature=").Append(sig).Append("&");
            sb.Append("user=").Append(APIUSER).Append("&");
            sb.Append("timestamp=").Append(timestamp).Append("&");


            str = Uri.EscapeDataString(str);
            string url = string.Empty;
            sb.Append("data=").Append(str);
            if (Method == "hotel.order.create" || Method == "common.creditcard.validate" || Method == "hotel.order.checkguest" || Method == "hotel.order.detail" || Method == "hotel.order.cancel")//一些需要用到Https的方法
            {
                url = string.Format(URL_HTTPS, sb.ToString());
            }
            else
            {
                url = string.Format(URL, sb.ToString());
            }

            string res = HttpRequestHelp.Get(url);


            HotelBaseResponse<T2> result = null;
            if (res != null)
            {
                if (typeof(T2) != typeof(object))
                {
                    try
                    {

                        result = SearilizeObject.Deserialize<HotelBaseResponse<T2>>(_format, res);
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                }
            }
            return result;
        }
    }
}