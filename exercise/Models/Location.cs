using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 坐标对象
    /// </summary>
    public class MapPoint
    {
        /// <summary>
        /// 纬度坐标(小的那个数字)
        /// 39.888411
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// 经度坐标（大的那个数字）
        /// 116.333097
        /// </summary>
        public double lng { get; set; }
    }

    /// <summary>
    /// 位置设置参数
    /// </summary>
    public class PositionSetModel
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public decimal lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public decimal lng { get; set; }
        /// <summary>
        /// 搜索半径，（单位：米）
        /// </summary>
        public int rat { get; set; }
    }
    /// <summary>
    /// 所在地信息（来自百度地图接口）
    /// </summary>
    public class MyGeoInfo
    {
        /// <summary>
        /// 状态
        /// 0	正常
        /// 1	服务器内部错误
        /// 2	请求参数非法
        /// 3	权限校验失败
        /// 4	配额校验失败
        /// 5	ak不存在或者非法
        /// 101	服务禁用
        /// 102	不通过白名单或者安全码不对
        /// 2xx	无权限
        /// 3xx	配额错误
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 坐标点
        /// </summary>
        public MapPoint point { get; set; }
        /// <summary>
        /// 结构化地址信息
        /// </summary>
        public string formatted_address { get; set; }
        /// <summary>
        /// 所在商圈信息，如 "人民大学,中关村,苏州街"
        /// </summary>
        public string business { get; set; }
        /// <summary>
        /// 详细的地址信息
        /// </summary>
        public addressComponent addressComponent { get; set; }
    }
    /// <summary>
    /// 所在区域详情
    /// </summary>
    public class addressComponent
    {
        /// <summary>
        /// elong城市ID
        /// </summary>
        public string elongcityid { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 区县名
        /// </summary>
        public string district { get; set; }
        /// <summary>
        /// 省名
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 街道名
        /// </summary>
        public string street { get; set; }
        /// <summary>
        /// 街道门牌号
        /// </summary>
        public string street_number { get; set; }
    }

    /// <summary>
    /// 城市信息
    /// </summary>
    public class GeoCityInfoModel {
        /// <summary>
        /// 城市名
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 城市编号（ID）
        /// </summary>
        public string CityCode { get; set; }
        /// <summary>
        /// 拼音首字母
        /// </summary>
        public string PY { get; set; }
        /// <summary>
        /// 全拼
        /// </summary>
        public string PinYin { get; set; }

        /// <summary>
        /// 所属省份信息
        /// </summary>
        public Combobox ProvniceInfo { get; set; }
    }

    /// <summary>
    /// 区域检索请求
    /// </summary>
    public class SearchAreaInfoListRequestModel : RequestBase {
        /// <summary>
        /// 城市Code（CityId）
        /// 【必填项目】
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 默认不使用翻页
        /// </summary>
        protected bool _usepagesize = false;
        /// <summary>
        /// 是否翻页（默认不使用翻页，不使用翻页则会返回所有满足条件的记录）
        /// </summary>
        public bool UsePageSize
        {
            get { return _usepagesize; }
            set { _usepagesize = value; }
        }
        /// <summary>
        /// 默认为查询行政区
        /// </summary>
        protected EnumAreaType _t = EnumAreaType.D;

        /// <summary>
        /// 类型（默认为获取行政区）
        /// </summary>
        public EnumAreaType T {
            get { return _t; }
            set { _t = value; }
        }
        /// <summary>
        /// 查询默认字符串
        /// </summary>
        protected string _q = string.Empty;
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string q {
            get { return _q; }
            set { _q = value; }
        }
    }
    /// <summary>
    /// 检索区域信息返回对象
    /// </summary>
    public class SearchAreaInfoListReplayModel {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public List<GeoAreaInfoModel> rows { get; set; }
    }

    /// <summary>
    /// 区域信息
    /// </summary>
    public class GeoAreaInfoModel : Combobox
    {
        /// <summary>
        /// 类别
        /// </summary>
        public EnumAreaType T { get; set; }
    }

    /// <summary>
    /// 城市行政区
    /// </summary>
    public class LocationInfoModel {
        
        /// <summary>
        /// 所在城市
        /// </summary>
        public GeoCityInfoModel cityInfo { get; set; }
        /// <summary>
        /// 行政区名称
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 行政区ID
        /// </summary>
        public string DistrictId { get; set; }
    }

    /// <summary>
    /// 所在区域
    /// </summary>
    public class FullLocationInfoModel {
        /// <summary>
        /// 所在城市
        /// </summary>
        public GeoCityInfoModel cityInfo { get; set; }
        /// <summary>
        /// 行政区名称
        /// </summary>
        public string District { get; set; }
        /// <summary>
        /// 行政区ID
        /// </summary>
        public string DistrictId { get; set; }
        /// <summary>
        /// 所在商圈
        /// </summary>
        public string BusinessZone { get; set; }
        /// <summary>
        /// 所在商圈ID
        /// </summary>
        public string BusinessZoneId { get; set; }
    }

    /// <summary>
    /// 地域分类
    /// </summary>
    public enum EnumAreaType
    {
        /// <summary>
        /// 行政区
        /// </summary>
        D,
        /// <summary>
        /// 商业区
        /// </summary>
        C,
        /// <summary>
        /// 地标
        /// </summary>
        L,
        /// <summary>
        /// 全部
        /// </summary>
        A
    }
}