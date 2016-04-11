using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using cyclonestyle.Models;
using Newtonsoft.Json;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 百度推送接口服务
    /// </summary>
    public class BaiDuPushInterFaceService
    {
        /// <summary>
        /// 发送一个百度推送消息
        /// </summary>
        /// <param name="condtion">发送请求参数</param>
        /// <returns>返回发送成功或者失败的标示与远程服务器的返回消息</returns>
        internal static ReplayBase SendBaiduPushMsg(SendBaiduPushBaseRequestModel condtion)
        {
            ReplayBase result = new ReplayBase();

            string sk = string.Empty;
            string ak = string.Empty;
            if (condtion.set.deviceType == EnumUserDeviceType.Android)
            {
                sk = "MxjYGdNnzFO1ORUKi3KxTfqWDEUKrI7F";
                ak = "w7TgjjNMbl8hzRd8r9IDdg7o";
            }
            else {
                sk = "krWK29gFKtG1wt5t8QcBEgyBHkIoOruP";
                ak = "EnKzXtyghmd7NhZQY1ukFSCv";
            }
            BaiDuPushInterFaceService Bpush = new BaiDuPushInterFaceService("POST",sk);
            String apiKey = ak;
            String messages = "";
            String method = "push_msg";
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            uint device_type = 3;
            uint unixTime = (uint)ts.TotalSeconds;

            uint message_type;
            string messageksy = "xxxxxx";

            if (condtion.messageType  == EnumPushMessagesType.透传消息)
            {
                message_type = 0;
                messages = condtion.msg;
            }
            else {
                message_type = 1;

                if (condtion.set.deviceType == EnumUserDeviceType.IOSDeveloper || condtion.set.deviceType == EnumUserDeviceType.IOSProduction)
                {
                    device_type = 4;
                    IOSNotification notification = new IOSNotification();
                    notification.title = condtion.title;
                    notification.description = condtion.msg;
                    if (condtion.customInfo != null)
                    {
                        notification.OpenType = condtion.customInfo.openType.GetHashCode();
                        notification.Id = condtion.customInfo.id;
                    }
                    notification.GetAps();
                    messages = notification.getJsonString();
                }
                else
                {
                    BaiduPushNotification notification = new BaiduPushNotification();
                    notification.title = condtion.title;
                    notification.description = condtion.msg;
                    notification.notification_basic_style = condtion.messagesStyle;
                    if (condtion.customInfo != null)
                    {
                        notification.custom_content = JsonConvert.SerializeObject(condtion.customInfo);
                    }
                    messages = notification.getJsonString();
                }
                PushOptions pOpts;
                if (!string.IsNullOrEmpty(condtion.set.channelId))
                {
                    pOpts = new PushOptions(method, apiKey, condtion.set.userId, condtion.set.channelId, device_type, messages, messageksy, unixTime);
                }
                else {
                    pOpts = new PushOptions(method, apiKey, device_type, messages, messageksy, unixTime);
                }
                pOpts.message_type = message_type;
                if (condtion.set.deviceType == EnumUserDeviceType.IOSProduction)
                {
                    pOpts.deploy_status = 2;
                }
                else if (condtion.set.deviceType == EnumUserDeviceType.IOSDeveloper)
                {
                    pOpts.deploy_status = 1;
                }
                string response = Bpush.PushMessage(pOpts);
                if (response.IndexOf("error_code") > 0)
                {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                }
                else {
                    result.ReturnCode = EnumErrorCode.Success;
                }
                result.ReturnMessage = response;
            }

            return result;
        }


        public PushOptions opts { get; set; }

        public string httpMehtod { get; set; }
        public string url { get; set; }
        public string secret_key { get; set; }

        public BaiDuPushInterFaceService(string httpMehtod, string secret_key)
        {
            this.httpMehtod = httpMehtod;
            this.url = "http://channel.api.duapp.com/rest/2.0/channel/channel";
            this.secret_key = secret_key;
        }

        /// <summary>
        /// 提交百度推送
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        public string PushMessage(PushOptions opts)
        {

            this.opts = opts;

            Dictionary<string, string> dic = new Dictionary<string, string>();

            //将键值对按照key的升级排列
            var props = typeof(PushOptions).GetProperties().OrderBy(p => p.Name);
            foreach (var p in props)
            {
                if (p.GetValue(this.opts, null) != null)
                {
                    dic.Add(p.Name, p.GetValue(this.opts, null).ToString());
                }
            }
            //生成sign时，不能包含sign标签，所有移除
            dic.Remove("sign");

            var preData = new StringBuilder();
            foreach (var l in dic)
            {
                preData.Append(l.Key);
                preData.Append("=");
                preData.Append(l.Value);

            }

            //按要求拼接字符串，并urlencode编码
            var str = HttpUtility.UrlEncode(this.httpMehtod.ToUpper() + this.url + preData.ToString() + this.secret_key, System.Text.Encoding.UTF8);

            var strSignUpper = new StringBuilder();
            int perIndex = 0;
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i].ToString();
                if (str[i] == '%')
                {
                    perIndex = i;
                }
                if (i - perIndex == 1 || i - perIndex == 2)
                {
                    c = c.ToUpper();
                }
                strSignUpper.Append(c);
            }

            strSignUpper = strSignUpper.Replace("(", "%28").Replace(")", "%29");


            var sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strSignUpper.ToString(), "MD5").ToLower();

            //加入生成好的sign键值对
            dic.Add("sign", sign);
            var strb = new StringBuilder();
            //int tagIndex = 0;
            foreach (var l in dic)
            {

                strb.Append(l.Key);
                strb.Append("=");
                strb.Append(l.Value);
                strb.Append("&");
            }

            var postStr = strb.ToString().EndsWith("&") ? strb.ToString().Remove(strb.ToString().LastIndexOf('&')) : strb.ToString();


            byte[] data = Encoding.UTF8.GetBytes(postStr);//编码，尤其是汉字，事先要看下抓取网页的编码方式  
            WebClient webClient = new WebClient();
            try
            {
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
                byte[] responseData = webClient.UploadData(this.url, "POST", data);//得到返回字符流  
                string srcString = Encoding.UTF8.GetString(responseData);//解码  
                return "Post:" + postStr + "\r\n\r\n" + "Response:" + srcString;
            }
            catch (WebException ex)
            {
                Stream stream = ex.Response.GetResponseStream();
                string m = ex.Response.Headers.ToString();
                byte[] buf = new byte[256];
                stream.Read(buf, 0, 256);
                stream.Close();
                int count = 0;
                foreach (var b in buf)
                {
                    if (b > 0)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                return "Post:" + postStr + ex.Message + "\r\n\r\n" + Encoding.UTF8.GetString(buf, 0, count);
            }
        }

    }
    /// <summary>
    /// 推送详细设置（百度推送接口）
    /// </summary>
    public class PushOptions
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="method"></param>
        /// <param name="apikey"></param>
        /// <param name="user_id"></param>
        /// <param name="push_type"></param>
        /// <param name="channel_id"></param>
        /// <param name="tag"></param>
        /// <param name="device_type"></param>
        /// <param name="message_type"></param>
        /// <param name="messages"></param>
        /// <param name="msg_keys"></param>
        /// <param name="message_expires"></param>
        /// <param name="timestamp"></param>
        /// <param name="expires"></param>
        /// <param name="v"></param>
        public PushOptions(string method,string apikey, string user_id,uint push_type,string channel_id,string tag,uint device_type,uint message_type,string messages,string msg_keys,uint message_expires,uint timestamp,uint expires,uint v)
        {
            this.method = method;
            this.apikey = apikey;
            this.user_id = user_id;
            this.push_type = push_type;
            this.channel_id = channel_id;
            this.tag = tag;
            this.device_type = device_type;
            this.message_type = message_type;
            this.messages = messages;
            this.msg_keys = msg_keys;
            this.message_expires = message_expires;
            this.timestamp = timestamp;
            this.expires = expires;
            this.v = v;
        }

        /// <summary>
        /// 单播[初始化]
        /// </summary>
        /// <param name="method"></param>
        /// <param name="apikey"></param>
        /// <param name="user_id"></param>
        /// <param name="channel_id"></param>
        /// <param name="device_type"></param>
        /// <param name="messages"></param>
        /// <param name="msg_keys"></param>
        /// <param name="timestamp"></param>
        public PushOptions(string method,string apikey,string user_id,string channel_id,uint device_type,string messages,string msg_keys,uint timestamp)
        {
            this.method = method;
            this.apikey = apikey;
            this.user_id = user_id;
            this.channel_id = channel_id;
            this.push_type = 1;
            this.messages = messages;
            this.msg_keys = msg_keys;
            this.timestamp = timestamp;
            this.device_type = device_type;
        }
        /// <summary>
        /// 组播[初始化]
        /// </summary>
        /// <param name="method"></param>
        /// <param name="apikey"></param>
        /// <param name="tag"></param>
        /// <param name="device_type"></param>
        /// <param name="messages"></param>
        /// <param name="msg_keys"></param>
        /// <param name="timestamp"></param>
        public PushOptions(string method,string apikey,string tag,uint device_type,string messages,string msg_keys,uint timestamp)
        {
            this.method = method;
            this.apikey = apikey;
            this.tag = tag;
            this.push_type = 2;
            this.messages = messages;
            this.msg_keys = msg_keys;
            this.timestamp = timestamp;
            this.device_type = device_type;
        }

        /// <summary>
        /// 广播[初始化]
        /// </summary>
        /// <param name="method"></param>
        /// <param name="apikey"></param>
        /// <param name="device_type"></param>
        /// <param name="messages"></param>
        /// <param name="msg_keys"></param>
        /// <param name="timestamp"></param>
        public PushOptions(string method,string apikey,uint device_type,string messages,string msg_keys,uint timestamp)
        {
            this.method = method;
            this.apikey = apikey;
            this.push_type = 3;
            this.messages = messages;
            this.msg_keys = msg_keys;
            this.timestamp = timestamp;
            this.device_type = device_type;
        }
        /// <summary>
        /// string 	是 	方法名，必须存在：push_msg。
        /// </summary>
        public string method { get; set; } 	//string 	是 	方法名，必须存在：push_msg。
        /// <summary>
        /// string 	是 	访问令牌，明文AK，可从此值获得App的信息，配合sign中的sk做合法性身份认证。
        /// </summary>
        public string apikey { get; set; }	//string 	是 	访问令牌，明文AK，可从此值获得App的信息，配合sign中的sk做合法性身份认证。
        /// <summary>
        /// string 	否 	用户标识，在Android里，channel_id + userid指定某一个特定client。不超过256字节，如果存在此字段，则只推送给此用户。
        /// </summary>
        public string user_id { get; set; }	//string 	否 	用户标识，在Android里，channel_id + userid指定某一个特定client。不超过256字节，如果存在此字段，则只推送给此用户。
        /// <summary>
        /// uint 是 推送类型，取值范围为：1～3
        /// 1：单个人，必须指定user_id 和 channel_id （指定用户的指定设备）或者user_id（指定用户的所有设备）
        /// 2：一群人，必须指定 tag
        /// 3：所有人，无需指定tag、user_id、channel_id*/
        /// </summary>
        public uint push_type { get; set; } /*	uint 	是 	推送类型，取值范围为：1～3

                                                                1：单个人，必须指定user_id 和 channel_id （指定用户的指定设备）或者user_id（指定用户的所有设备）

                                                                2：一群人，必须指定 tag

                                                                3：所有人，无需指定tag、user_id、channel_id*/
        /// <summary>
        /// string 	否 	通道标识
        /// </summary>
        public string channel_id { get; set; }	//string 	否 	通道标识
        /// <summary>
        /// string 	否 	标签名称，不超过128字节
        /// </summary>
        public string tag { get; set; }	//string 	否 	标签名称，不超过128字节
        /// <summary>
        /// uint    否 设备类型，取值范围为：1～5
        /// 百度Channel支持多种设备，各种设备的类型编号如下：（支持某种组合，如：1,2,4:）
        /// 1：浏览器设备；
        /// 2：PC设备；
        /// 3：Andriod设备；
        /// 4：iOS设备；
        /// 5：Windows Phone设备；
        /// 如果存在此字段，则向指定的设备类型推送消息。 默认不区分设备类型
        /// </summary>
        public uint? device_type { get; set; }	/*uint 	否 	设备类型，取值范围为：1～5

                                        百度Channel支持多种设备，各种设备的类型编号如下：（支持某种组合，如：1,2,4:）

                                        1：浏览器设备；

                                        2：PC设备；

                                        3：Andriod设备；

                                        4：iOS设备；

                                        5：Windows Phone设备；

                                        如果存在此字段，则向指定的设备类型推送消息。 默认不区分设备类型。*/

        /// <summary>
        /// uint 	否 	消息类型
        /// 0：消息（透传）
        /// 1：通知
        /// 默认值为0。
        /// </summary>
        public uint? message_type { get; set; } /*uint 	否 	消息类型

                                                    0：消息（透传）

                                                    1：通知

                                                    默认值为0。*/
        /// <summary>
        /// string 	是 	指定消息内容，单个消息为单独字符串，多个消息用json数组表示
        /// 如果有二进制的消息内容，请先做BASE64的编码。
        /// 一次推送最多10个消息。
        /// 注：当message_type=1且为Android端接收消息时，需按照以下格式：详见属性定义文件
        /// title : 通知标题，可以为空；如果为空则设为appid对应的Android应用名称。 description：通知文本内容，不能为空，否则Android端上不展示。
        /// </summary>
        public string messages { get; set; }    /*string 	是 	指定消息内容，单个消息为单独字符串，多个消息用json数组表示。

                                                    如果有二进制的消息内容，请先做BASE64的编码。

                                                    一次推送最多10个消息。

                                                    注：当message_type=1且为Android端接收消息时，需按照以下格式：

                                                    "{ 
                                                       \"title\" : \"hello\" ,
                                                       \"description\": \"hello\"
                                                     }"

                                                    说明：

                                                        title : 通知标题，可以为空；如果为空则设为appid对应的Android应用名称。
                                                        description：通知文本内容，不能为空，否则Android端上不展示。 */

        /// <summary>
        /// string 	是 	指定消息标识，必须和messages一一对应。相同消息标识的消息会自动覆盖。单个消息为单独字符串，多个msg_key也使用json数组表示。特别提醒：该功能只支持android、browser、pc三种设备类型。。
        /// </summary>
        public string msg_keys { get; set; }    //string 	是 	指定消息标识，必须和messages一一对应。相同消息标识的消息会自动覆盖。单个消息为单独字符串，多个msg_key也使用json数组表示。特别提醒：该功能只支持android、browser、pc三种设备类型。。

        /// <summary>
        /// uint 	否 	指定消息的过期时间，默认为86400秒。必须和messages一一对应。
        /// </summary>
        public uint? message_expires { get; set; }  //uint 	否 	指定消息的过期时间，默认为86400秒。必须和messages一一对应。

        /// <summary>
        /// uint 	是 	用户发起请求时的unix时间戳。本次请求签名的有效时间为该时间戳+10分钟。
        /// </summary>
        public uint timestamp { get; set; } //uint 	是 	用户发起请求时的unix时间戳。本次请求签名的有效时间为该时间戳+10分钟。

        /// <summary>
        /// string 	是 	调用参数签名值，与apikey成对出现。
        /// </summary>
        public string sign { get; set; }    //string 	是 	调用参数签名值，与apikey成对出现。

        /// <summary>
        /// uint 	否 	用户指定本次请求签名的失效时间。格式为unix时间戳形式。
        /// </summary>
        public uint? expires { get; set; }//	uint 	否 	用户指定本次请求签名的失效时间。格式为unix时间戳形式。

        /// <summary>
        /// uint 	否 	API版本号，默认使用最高版本。
        /// </summary>
        public uint? v { get; set; }	//uint 	否 	API版本号，默认使用最高版本。
        /// <summary>
        /// 部署状态。指定应用当前的部署状态，可取值：1：开发状态 2：生产状态
        /// </summary>
        public uint? deploy_status { get; set; } //部署状态。指定应用当前的部署状态，可取值：1：开发状态 2：生产状态
    }

    /// <summary>
    /// IOS设备通知
    /// </summary>
    public class IOSNotification
    {
        /// <summary>
        /// 通知标题，可以为空；如果为空则设为appid对应的应用名;
        /// </summary>
        public string title { get; set; } //通知标题，可以为空；如果为空则设为appid对应的应用名;
        /// <summary>
        /// 通知文本内容，不能为空;
        /// </summary>
        public string description { get; set; } //通知文本内容，不能为空;
        /// <summary>
        /// 打开模式
        /// </summary>
        public int OpenType { get; set; }//打开模式
        /// <summary>
        /// 对应ID
        /// </summary>
        public string Id { get; set; }//对应ID
        /// <summary>
        /// public string aps { get; set; }
        /// </summary>
        public aspc aps { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        public IOSNotification()
        {

        }
        /// <summary>
        /// 默认值
        /// </summary>
        public void GetAps()
        {
            this.aps = new aspc
            {
                alert = title,
                sound = "sms-received1.caf"
            };
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public string getJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    /// <summary>
    /// IOS提示与声音
    /// </summary>
    public class aspc
    {
        /// <summary>
        /// 提示标题
        /// </summary>
        public string alert { get; set; }
        /// <summary>
        /// 声音
        /// </summary>
        public string sound { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BaiduPushNotification
    {
        /// <summary>
        /// 通知标题，可以为空；如果为空则设为appid对应的应用名;
        /// </summary>
        public string title { get; set; } //通知标题，可以为空；如果为空则设为appid对应的应用名;
        /// <summary>
        /// 通知文本内容，不能为空;
        /// </summary>
        public string description { get; set; } //通知文本内容，不能为空;
        /// <summary>
        /// android客户端自定义通知样式，如果没有设置默认为0;
        /// </summary>
        public int notification_builder_id { get; set; } //android客户端自定义通知样式，如果没有设置默认为0;
        /// <summary>
        /// 只有notification_builder_id为0时才有效，才需要设置，如果notification_builder_id为0则可以设置通知的基本样式包括(响铃：0x04;振动：0x02;可清除：0x01;),这是一个flag整形，每一位代表一种样式;
        /// </summary>
        public int notification_basic_style { get; set; } //只有notification_builder_id为0时才有效，才需要设置，如果notification_builder_id为0则可以设置通知的基本样式包括(响铃：0x04;振动：0x02;可清除：0x01;),这是一个flag整形，每一位代表一种样式;
        /// <summary>
        /// 点击通知后的行为(打开Url：1; 自定义行为：2：其它值则默认打开应用;);
        /// </summary>
        public int open_type { get; set; }//点击通知后的行为(打开Url：1; 自定义行为：2：其它值则默认打开应用;);
        /// <summary>
        /// 只有open_type为1时才有效，才需要设置，如果open_type为1则可以设置需要打开的Url地址;
        /// </summary>
        public string url { get; set; } //只有open_type为1时才有效，才需要设置，如果open_type为1则可以设置需要打开的Url地址;
        /// <summary>
        /// 只有open_type为1时才有效，才需要设置,(需要请求用户授权：1；默认直接打开：0), 如果open_type为1则可以设置打开的Url地址时是否请求用户授权;
        /// </summary>
        public int user_confirm { get; set; } //只有open_type为1时才有效，才需要设置,(需要请求用户授权：1；默认直接打开：0), 如果open_type为1则可以设置打开的Url地址时是否请求用户授权;
        /// <summary>
        /// 只有open_type为2时才有效，才需要设置, 如果open_type为2则可以设置自定义打开行为(具体参考管理控制台文档);
        /// </summary>
        public string pkg_content { get; set; }//只有open_type为2时才有效，才需要设置, 如果open_type为2则可以设置自定义打开行为(具体参考管理控制台文档);
        /// <summary>
        /// 自定义内容，键值对，Json对象形式(可选)；在android客户端，这些键值对将以Intent中的extra进行传递。
        /// </summary>
        public string custom_content { get; set; }// 自定义内容，键值对，Json对象形式(可选)；在android客户端，这些键值对将以Intent中的extra进行传递。
        /// <summary>
        /// 初始化
        /// </summary>
        public BaiduPushNotification()
        {
            notification_builder_id = 0;
            notification_basic_style = 5;

            url = "";
            user_confirm = 0;
            pkg_content = "";
            custom_content = "";
            open_type = 0;

        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public string getJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}