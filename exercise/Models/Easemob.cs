using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 环信即时通信
/// </summary>
namespace cyclonestyle.Models
{
    /// <summary>
    /// 环信注册请求
    /// </summary>
    public class RegisterUserEasemobRequestModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }

    #region --消息类型
    /// <summary>
    /// 文本消息类型
    /// </summary>
    public class EasemobTextMesModel {
        /// <summary>
        /// 消息类型 默认为 文本  txt
        /// </summary>
        public string type {
            get { return "txt"; }
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string msg { get; set; }
    }
    /// <summary>
    /// 图片消息类型
    /// </summary>
    public class EasemobPicMesModel
    {
        /// <summary>
        /// 消息类型 默认为图片 img
        /// </summary>
        public string type {
            get { return "img"; }
        }
        /// <summary>
        /// 文件上传后返回的URL
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string filename { get; set; }
        /// <summary>
        /// 上传后返回的secret
        /// </summary>
        public string secret { get; set; }
        /// <summary>
        /// 图片大小
        /// </summary>
        public EasemobImgSizeModel size { get; set; }
    }

    /// <summary>
    /// 声音消息
    /// </summary>
    public class EasemobSoundMesModel
    {
        /// <summary>
        /// 消息类型 默认为 audio
        /// </summary>
        public string type {
            get{ return "audio"; }
        }
        /// <summary>
        /// 文件上传后返回的URL
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string filename { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long length { get; set; }
        /// <summary>
        /// 上传后返回的secret
        /// </summary>
        public string secret { get; set; }
    }
    #endregion

    
    /// <summary>
    /// 发送文本信息请求
    /// </summary>
    public class EasemobSendTextMessageRequestModel : EasemobSendMessageRequestModel
    {
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public EasemobTextMesModel msg { get; set; }
    }

    /// <summary>
    /// 发送图片消息请求
    /// </summary>
    public class EasemobSendIMgMessageRequestModel: EasemobSendMessageRequestModel
    {
        /// <summary>
        /// 图片消息内容
        /// </summary>
        public EasemobPicMesModel msg { get; set; }
    }
    /// <summary>
    /// 发送语音消息请求
    /// </summary>
    public class EasemobSendAudioMessageRequestModel : EasemobSendMessageRequestModel {
        /// <summary>
        /// 语音消息内容
        /// </summary>
        public EasemobSoundMesModel msg { get; set; }
    }

    /// <summary>
    /// 发送环信消息基础请求
    /// </summary>
    public class EasemobSendMessageRequestModel
    {
        private string _target_type = "users";//默认为发送给用户
        private string[] _target = { "kefu001" };//默认为联系客服
        /// <summary>
        /// users 给用户发消息, chatgroups 给群发消息, chatrooms 给聊天室发消息
        /// 默认 users
        /// </summary>
        public string target_type {
            get { return _target_type; }
            set { _target_type = value; }
        }
        /// <summary>
        /// 接收的用户，默认为客服
        /// </summary>
        public string[] target {
            get { return _target; }
            set { _target = value; }
        }
        /// <summary>
        /// 发送者
        /// </summary>
        public string from { get; set; }
    }

    /// <summary>
    /// 环信服务端上传文件返回对象
    /// </summary>
    public class EasemobUploadFileRequestModel {
        /// <summary>
        /// 文件唯一id，发送消息时需要用到指定是哪个文件
        /// </summary>
        public string uuid { get; set; }
        /// <summary>
        /// 上传成功后返回，发送消息时需要用到。
        /// </summary>
        public string secret { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string filename { get; set; }
        /// <summary>
        /// 文件的长度
        /// </summary>
        public long length { get; set; }
        /// <summary>
        /// 图片文件的大小
        /// </summary>
        public EasemobImgSizeModel imgsize { get; set; }
    }
    /// <summary>
    /// 图片文件大小
    /// </summary>
    public class EasemobImgSizeModel
    {
        /// <summary>
        /// 宽
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public int height { get; set; }
    }
}