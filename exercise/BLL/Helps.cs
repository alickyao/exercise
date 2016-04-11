using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 一些公共的方法组
    /// </summary>
    public class Helps
    {
        /// <summary>
        /// 获取时间ID（用于创造可读性比较强的编号，格式yyMMddHHmmss_ffff）
        /// </summary>
        /// <returns></returns>
        internal static string GetTimeId(){
            return DateTime.Now.ToString("yyMMddHHmmss_ffff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// 清除HTML标签并截取字符串
        /// </summary>
        /// <param name="Content">要截取的字符串</param>
        /// <param name="length">长度，如果字符串长度大于指定的长度则截取字符串</param>
        /// <param name="s">截取后最后添加到文本后面的字符串</param>
        /// <returns></returns>
        internal static string GetShotContent(string Content, int length, string s = "...")
        {
            if (!string.IsNullOrEmpty(Content))
            {
                Content = NoHTML(Content);
                if (Content.Length > length)
                {
                    Content = Content.Substring(0, length) + s;
                }
            }
            return Content;
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        internal static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }


        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return (long)intResult;
        }

        /// <summary>
        /// md5加密，返回小写字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            string encoded = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "");
            return encoded.ToLower();
        }
    }

    /// <summary>
    /// 发送HTTP请求方法组
    /// </summary>
    public class HttpRequestHelp
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static string Get(string url)
        {
            return Send("GET", url, "");
        }
        /// <summary>
        /// Post
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postData">参数</param>
        /// <returns></returns>
        public static string Post(string url, string postData)
        {
            return Send("POST", url, postData);
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="verb">方式</param>
        /// <param name="url">地址</param>
        /// <param name="postData">参数</param>
        /// <returns></returns>
        public static string Send(string verb, string url, string postData)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            //System.Net.WebProxy proxy = new WebProxy("10.10.101.9", 8080);
            //proxy.Credentials = new NetworkCredential("lx09140019", "118200");
            //req.Proxy = proxy;


            req.Timeout = 60 * 1000;
            //req.Headers.Add("Accept-Encoding", "gzip,deflate");
            req.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.110 Safari/537.36";

            req.Method = verb;

            try
            {
                if (verb == "POST")
                {
                    byte[] data = Encoding.UTF8.GetBytes(postData);

                    req.ContentType = "application/text; charset=utf-8";
                    req.ContentLength = data.Length;

                    Stream requestStream = req.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                }


                using (var res = req.GetResponse())
                {
                    using (var stream = res.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                        string response = sr.ReadToEnd();

                        sr.Close();

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HTTP Exception: " + ex.Message);
            }
            return null;
        }
    }
    /// <summary>
    /// 序列化与翻序列化
    /// </summary>
    public class SearilizeObject
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="format"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Searilize(string format, Type type, object value)
        {
            return Searilize(format, type, value);
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="format"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="extendType"></param>
        /// <returns></returns>
        public static string Searilize(string format, Type type, object value, Type extendType)
        {
            string str = "";
            if (format == "xml")
            {
                StringWriter sw = new StringWriter();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                //ns.Add("", "");

                System.IO.Stream stream = new System.IO.MemoryStream();

                System.Xml.Serialization.XmlSerializer serializer = null;
                if (extendType == null)
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(type);
                }
                else
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(type, new Type[] { extendType });
                }
                serializer.Serialize(stream, value, ns);

                stream.Seek(0, System.IO.SeekOrigin.Begin);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);

                str = reader.ReadToEnd();

            }
            else
            {

                var enumConverter = new Newtonsoft.Json.Converters.StringEnumConverter();
                var settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
                settings.Converters.Add(enumConverter);
                str = JsonConvert.SerializeObject(value, settings);
            }

            return str;
        }

        /// <summary>
        /// 返序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string format, string xml)
        {
            if (format == "xml")
            {


                T t = default(T);
                XmlSerializer xs = new XmlSerializer(typeof(T));
                //MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xml));
                //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                //t = xs.Deserialize(memoryStream);

                //using (StringReader rdr = new StringReader(xml))
                //{
                //    t = (T)xs.Deserialize(rdr);
                //}




                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var rd = new XmlNodeReader(doc.DocumentElement);
                t = (T)xs.Deserialize(rd);

                //XElement root = XElement.Parse(xml); 

                return t;
            }
            if (xml.StartsWith("{") || xml.StartsWith("<"))
            {
                return JsonConvert.DeserializeObject<T>(xml);
            }
            else
            {
                return default(T);
            }
        }

        private static String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
    }
}