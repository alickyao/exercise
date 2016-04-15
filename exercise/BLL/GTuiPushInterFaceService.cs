using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.igetui.api.openservice;
using com.igetui.api.openservice.igetui;
using com.igetui.api.openservice.igetui.template;
using com.igetui.api.openservice.payload;
using cyclonestyle.Models;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 个推推送接口服务
    /// </summary>
    public class GTuiPushInterFaceService
    {
        //http的域名
        private static String HOST = "http://sdk.open.api.igexin.com/apiex.htm";
        private static String APPID = "M1swc9kds09LncjWcf8jO2";                     //您应用的AppId
        private static String APPKEY = "y7xrMzVw6Y7Ed2ba0d9h7A";                    //您应用的AppKey
        private static String MASTERSECRET = "kkje3xSRqQ6KNxTSeuMNz8";              //您应用的MasterSecret 


        private static String CLIENTID = "b73ad667a6d2a3a72b17664c9ca91269";        //您获取的clientID

        //配置

        /// <summary>
        /// 用户当前不在线时，是否离线存储,可选
        /// </summary>
        private static bool IsOffline = true;
        /// <summary>
        /// 离线有效时间，单位为毫秒，可选
        /// </summary>
        private static long OfflineExpireTime = 1000 * 3600 * 12;
        /// <summary>
        /// 判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为非WIFI环境
        /// </summary>
        private static int PushNetWorkType = 0;

        /// <summary>
        /// 发送推送[目前仅支持传透和通知，支持Android和IOS]
        /// </summary>
        /// <param name="condtion"></param>
        internal static SendGeTuiPushReplay PushMessage(SendGeTuiPushBySetRequestModel condtion)
        {
            SendGeTuiPushReplay result = new SendGeTuiPushReplay();
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            ListMessage listMessage = new ListMessage() {
                IsOffline = IsOffline,
                OfflineExpireTime = OfflineExpireTime,
                PushNetWorkType = PushNetWorkType
            };
            SingleMessage singleMessage = new SingleMessage() {
                IsOffline = IsOffline,
                OfflineExpireTime = OfflineExpireTime,
                PushNetWorkType = PushNetWorkType
            };
            TransmissionTemplate tt = TransmissionTemplateDemo();//传透
            NotificationTemplate nt = NotificationTemplateDemo();//通知

            if (condtion.sets.Count > 0)
            {
                //分离请求中的设备类型
                List<GeTuiSetModel> ios = condtion.sets.Where(p => p.deviceType != EnumUserDeviceType.Android).ToList();
                List<GeTuiSetModel> android = condtion.sets.Where(p => p.deviceType == EnumUserDeviceType.Android).ToList();

                //IOS和安卓分开处理
                if (ios.Count > 0) {
                    //IOS设备发送
                    //IOS需使用传透方式来发送
                    APNTemplate template = new APNTemplate();
                    APNPayload apnpayload = new APNPayload();
                    DictionaryAlertMsg alertMsg = new DictionaryAlertMsg();
                    alertMsg.Body = condtion.msg;
                    alertMsg.ActionLocKey = "";
                    alertMsg.LocKey = "";
                    alertMsg.addLocArg("");
                    alertMsg.LaunchImage = "";
                    //IOS8.2支持字段
                    alertMsg.Title = condtion.title;
                    alertMsg.TitleLocKey = "";
                    alertMsg.addTitleLocArg("");

                    apnpayload.AlertMsg = alertMsg;
                    apnpayload.Badge = 1;
                    apnpayload.ContentAvailable = 1;
                    //apnpayload.Category = "";
                    apnpayload.Sound = "sms-received1.caf";
                    apnpayload.addCustomMsg("customInfo", condtion.customInfo);
                    template.setAPNInfo(apnpayload);
                    template.AppId = APPID;
                    template.AppKey = APPKEY;

                    if (ios.Count > 1)
                    {
                        //批量
                        listMessage.Data = template;
                        List<string> devicetokenlist = new List<string>();
                        foreach (GeTuiSetModel set in ios)
                        {
                            devicetokenlist.Add(set.clientId);
                        }
                        string contentId = push.getAPNContentId(APPID, listMessage);
                        result.sentResultIos = push.pushAPNMessageToList(APPID, contentId, devicetokenlist);
                    }
                    else if (ios.Count == 1) {
                        //单个
                        singleMessage.Data = template;
                        result.sentResultIos = push.pushAPNMessageToSingle(APPID, ios[0].clientId, singleMessage);
                    }
                }
                if (android.Count > 0) {
                    switch (condtion.messageType)
                    {
                        case EnumPushMessagesType.透传消息:
                            tt.TransmissionContent = condtion.msg;
                            listMessage.Data = tt;
                            singleMessage.Data = tt;
                            break;
                        case EnumPushMessagesType.通知:
                            nt.Title = condtion.title;
                            nt.Text = condtion.msg;
                            nt.TransmissionContent = Newtonsoft.Json.JsonConvert.SerializeObject(condtion.customInfo);
                            listMessage.Data = nt;
                            singleMessage.Data = nt;
                            break;
                    }

                    //安卓设备发送
                    if (android.Count > 1)
                    {
                        //多个用户
                        List<Target> ts = new List<Target>();
                        foreach (GeTuiSetModel set in android)
                        {
                            ts.Add(new Target()
                            {
                                appId = APPID,
                                clientId = set.clientId
                            });
                        }
                        string contentId = push.getContentId(listMessage, "ToList_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                        result.sentResultAndroid = push.pushMessageToList(contentId, ts);
                    }
                    else if (android.Count == 1)
                    {
                        //单个用户
                        Target t = new Target();
                        t.appId = APPID;
                        t.clientId = android[0].clientId;
                        result.sentResultAndroid = push.pushMessageToSingle(singleMessage, t);
                    }
                }
                result.ReturnCode = EnumErrorCode.Success;
            }
            else {
                result.ReturnCode = EnumErrorCode.EmptyDate;
                result.ReturnMessage = "没有设置任何的接收者";
                SysManagerService.SysSaveSysLog("推送消息：[" + condtion.msg + "]没有设置任何的接收者", EnumSysLogType.警告);
            }
            return result;
        }

        /// <summary>
        /// 给多个用户发送推送(DEMO)
        /// </summary>
        private static void PushMessageToList()
        {
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);

            ListMessage message = new ListMessage();
            /*消息模版：
                 1.TransmissionTemplate:透传功能模板
                 2.LinkTemplate:通知打开链接功能模板
                 3.NotificationTemplate：通知透传功能模板
                 4.NotyPopLoadTemplate：通知弹框下载功能模板
             */

            TransmissionTemplate template = TransmissionTemplateDemo();
            //NotificationTemplate template =  NotificationTemplateDemo();
            //LinkTemplate template = LinkTemplateDemo();
            //NotyPopLoadTemplate template = NotyPopLoadTemplateDemo();
            template.TransmissionContent = "测试";

            message.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            message.Data = template;
            //message.PushNetWorkType = 0;             //判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为非WIFI环境

            //设置接收者
            List<com.igetui.api.openservice.igetui.Target> targetList = new List<com.igetui.api.openservice.igetui.Target>();
            com.igetui.api.openservice.igetui.Target target1 = new com.igetui.api.openservice.igetui.Target();
            target1.appId = APPID;
            //target1.clientId = CLIENTID;
            target1.clientId = CLIENTID;

            // 如需要，可以设置多个接收者
            com.igetui.api.openservice.igetui.Target target2 = new com.igetui.api.openservice.igetui.Target();
            target2.appId = APPID;
            //target2.clientId = "f563cffaba68587e7ff2f23110f6defd";
            target2.clientId = CLIENTID;

            targetList.Add(target1);
            targetList.Add(target2);

            String contentId = push.getContentId(message, "ToList_任务组名");
            String pushResult = push.pushMessageToList(contentId, targetList);
            System.Console.WriteLine("-----------------------------------------------");
            System.Console.WriteLine("服务端返回结果:" + pushResult);
        }

        /// <summary>
        /// 给单个用户发送推送(DEMO)
        /// </summary>
        private static void PushMessageToSingle() {
            IGtPush push = new IGtPush(HOST, APPKEY, MASTERSECRET);
            TransmissionTemplate template = TransmissionTemplateDemo();
            template.TransmissionContent = "测试";

            // 单推消息模型
            SingleMessage message = new SingleMessage();
            message.IsOffline = true;                         // 用户当前不在线时，是否离线存储,可选
            message.OfflineExpireTime = 1000 * 3600 * 12;            // 离线有效时间，单位为毫秒，可选
            message.Data = template;
            message.PushNetWorkType = 0;        //判断是否客户端是否wifi环境下推送，1为在WIFI环境下，0为非WIFI环境
            com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
            target.appId = APPID;
            target.clientId = CLIENTID;
            String pushResult = push.pushMessageToSingle(message, target);
        }

        /*
         * 
         * 所有推送接口均支持四个消息模板，依次为透传模板，通知透传模板，通知链接模板，通知弹框下载模板
         * 注：IOS离线推送需通过APN进行转发，需填写pushInfo字段，目前仅不支持通知弹框下载功能
         *
         */

        /// <summary>
        /// 透传模板
        /// </summary>
        /// <returns></returns>
        private static TransmissionTemplate TransmissionTemplateDemo()
        {
            TransmissionTemplate template = new TransmissionTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.TransmissionType = "1";    //应用启动类型，1：强制应用启动 2：等待应用启动
            template.TransmissionContent = "";  //透传内容

            //iOS简单推送
            //APNPayload apnpayload = new APNPayload();
            //SimpleAlertMsg alertMsg = new SimpleAlertMsg("alertMsg");
            //apnpayload.AlertMsg = alertMsg;
            //apnpayload.Badge = 11;
            //apnpayload.ContentAvailable = 1;
            //apnpayload.Category = "";
            //apnpayload.Sound = "";
            //apnpayload.addCustomMsg("", "");
            //template.setAPNInfo(apnpayload);

            //APN高级推送
            APNPayload apnpayload = new APNPayload();
            DictionaryAlertMsg alertMsg = new DictionaryAlertMsg();
            alertMsg.Body = "Body";
            alertMsg.ActionLocKey = "ActionLocKey";
            alertMsg.LocKey = "LocKey";
            alertMsg.addLocArg("LocArg");
            alertMsg.LaunchImage = "LaunchImage";
            //IOS8.2支持字段
            alertMsg.Title = "Title";
            alertMsg.TitleLocKey = "TitleLocKey";
            alertMsg.addTitleLocArg("TitleLocArg");

            apnpayload.AlertMsg = alertMsg;
            apnpayload.Badge = 10;
            apnpayload.ContentAvailable = 1;
            //apnpayload.Category = "";
            apnpayload.Sound = "test1.wav";
            apnpayload.addCustomMsg("payload", "payload");
            template.setAPNInfo(apnpayload);


            //设置客户端展示时间
            //String begin = "2015-03-06 14:28:10";
            //String end = "2015-03-06 14:38:20";
            //template.setDuration(begin, end);

            return template;
        }
        /// <summary>
        /// 通知透传模板
        /// </summary>
        /// <returns></returns>
        private static NotificationTemplate NotificationTemplateDemo()
        {
            NotificationTemplate template = new NotificationTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.Title = "请填写通知标题";         //通知栏标题
            template.Text = "请填写通知内容";          //通知栏内容
            template.Logo = "";               //通知栏显示本地图片
            template.LogoURL = "";                    //通知栏显示网络图标

            template.TransmissionType = "1";          //应用启动类型，1：强制应用启动  2：等待应用启动
            template.TransmissionContent = "请填写透传内容";   //透传内容

            //设置客户端展示时间
            //String begin = "2015-03-06 14:36:10";
            //String end = "2015-03-06 14:46:20";
            //template.setDuration(begin, end);

            template.IsRing = true;                //接收到消息是否响铃，true：响铃 false：不响铃
            template.IsVibrate = true;               //接收到消息是否震动，true：震动 false：不震动
            template.IsClearable = true;             //接收到消息是否可清除，true：可清除 false：不可清除
            return template;
        }

        /// <summary>
        /// 通知链接模板
        /// </summary>
        /// <returns></returns>
        private static LinkTemplate LinkTemplateDemo()
        {
            LinkTemplate template = new LinkTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.Title = "请填写通知标题";         //通知栏标题
            template.Text = "请填写通知内容";          //通知栏内容
            template.Logo = "";               //通知栏显示本地图片
            template.LogoURL = "";  //通知栏显示网络图标，如无法读取，则显示本地默认图标，可为空
            template.Url = "http://www.baidu.com";      //打开的链接地址

            template.IsRing = true;                 //接收到消息是否响铃，true：响铃 false：不响铃
            template.IsVibrate = true;               //接收到消息是否震动，true：震动 false：不震动
            template.IsClearable = true;             //接收到消息是否可清除，true：可清除 false：不可清除

            return template;
        }

        /// <summary>
        /// 通知弹框下载模板
        /// </summary>
        /// <returns></returns>
        private static NotyPopLoadTemplate NotyPopLoadTemplateDemo()
        {
            NotyPopLoadTemplate template = new NotyPopLoadTemplate();
            template.AppId = APPID;
            template.AppKey = APPKEY;
            template.NotyTitle = "请填写通知标题";     //通知栏标题
            template.NotyContent = "请填写通知内容";   //通知栏内容
            template.NotyIcon = "icon.png";           //通知栏显示本地图片
            template.LogoURL = "http://www-igexin.qiniudn.com/wp-content/uploads/2013/08/logo_getui1.png";                    //通知栏显示网络图标

            template.PopTitle = "弹框标题";             //弹框显示标题
            template.PopContent = "弹框内容";           //弹框显示内容
            template.PopImage = "";                     //弹框显示图片
            template.PopButton1 = "下载";               //弹框左边按钮显示文本
            template.PopButton2 = "取消";               //弹框右边按钮显示文本

            template.LoadTitle = "下载标题";            //通知栏显示下载标题
            template.LoadIcon = "file://push.png";      //通知栏显示下载图标,可为空
            template.LoadUrl = "http://www.appchina.com/market/d/425201/cop.baidu_0/com.gexin.im.apk";//下载地址，不可为空

            template.IsActived = true;                  //应用安装完成后，是否自动启动
            template.IsAutoInstall = true;              //下载应用完成后，是否弹出安装界面，true：弹出安装界面，false：手动点击弹出安装界面

            template.IsBelled = true;                 //接收到消息是否响铃，true：响铃 false：不响铃
            template.IsVibrationed = true;               //接收到消息是否震动，true：震动 false：不震动
            template.IsCleared = true;             //接收到消息是否可清除，true：可清除 false：不可清除
            return template;
        }
    }
}