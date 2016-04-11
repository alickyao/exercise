using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using cyclonestyle.MandaoSmsService;
using cyclonestyle.Models;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 漫道短信接口服务
    /// </summary>
    public class MandaoSmsInterFaceService
    {
        /// <summary>
        /// 正式账号
        /// </summary>
        private static string sn = "SDK-CLY-010-00285";
        /// <summary>
        /// 密码
        /// </summary>
        private static string pwd = "6a#79-7b";
        /// <summary>
        /// 签名（getMD5(sn + pwd)）
        /// </summary>
        private static string sign { get; set; }

        private static WebServiceSoapClient client;

        static MandaoSmsInterFaceService() {
            client = new WebServiceSoapClient();
            //获取签名密码
            sign = getMD5(sn + pwd);
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <returns></returns>
        public static ReplayBase SendSms(SendSmsBaseRequestModel condtion) {
            ReplayBase result = new ReplayBase();
            if (!string.IsNullOrEmpty(condtion.mobilePhone) && !string.IsNullOrEmpty(condtion.content))
            {
                //必须在内容后添加签名，否则不能发送
                if (condtion.content.IndexOf("[优翔网]") == -1 && condtion.content.IndexOf("【优翔网】") == -1) {
                    condtion.content = condtion.content + "[优翔网]";
                }
                string IsTest = System.Configuration.ConfigurationManager.AppSettings["IsTest"].ToString();
                if (IsTest == "true")
                {
                    result = new ReplayBase()
                    {
                        ReturnCode = EnumErrorCode.Success,
                        ReturnMessage = "当前为测试，短信不会真的发送"
                    };
                }
                else {
                    string r = client.mt(sn, sign, condtion.mobilePhone, condtion.content, "", "", "");
                    result = Checkresult(r);
                }
            }
            else {
                result.ReturnCode = EnumErrorCode.EmptyDate;
                result.ReturnMessage = "手机号码和内容必填";
            }
            return result;
        }
        /// <summary>
        /// 获取可发送的短信余额
        /// </summary>
        /// <returns></returns>
        public static string GetSurplusNum() {
            return client.balance(sn, getMD5(sn + pwd));
        }
        /// <summary>
        /// 检查返回值
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static ReplayBase Checkresult(string r) {
            ReplayBase result = new ReplayBase();
            if (r.StartsWith("-"))
            {
                result.ReturnCode = EnumErrorCode.EmptyDate;
                result.ReturnMessage = "发送失败！" + GetErrorText(r) + "。返回值是：" + r;
            }
            else
            {
                result.ReturnCode = EnumErrorCode.Success;
                result.ReturnMessage = r;
            }
            return result;
        }
        /// <summary>
        /// 获取错误详情文本
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string GetErrorText(string code)
        {
            switch (code)
            {
                case "-2":
                    return "序列号未注册或加密不对";

                case "-4":
                    return "余额不足";
                case "-6":
                    return "参数错误，请检测所有参数";
                case "-7":

                    return "权限受限";

                case "-9":

                    return "扩展码权限错误";
                case "-10":

                    return "内容过长，短信不得超过500个字符";
                case "-20":

                    return "相同手机号，相同内容重复提交";
                case "-22":

                    return "Ip鉴权失败";

                default:
                    return "错误，请调试程序";
            }
        }

        /// <summary>
        /// 获取md5码，漫道专用
        /// </summary>
        /// <param name="source">待转换字符串</param>
        /// <returns>md5加密后的字符串</returns>
        private static string getMD5(string source)
        {
            string result = "";
            try
            {
                MD5 getmd5 = new MD5CryptoServiceProvider();
                byte[] targetStr = getmd5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(source));
                result = BitConverter.ToString(targetStr).Replace("-", "");
                return result;
            }
            catch (Exception)
            {
                return "0";
            }
        }
    }
}