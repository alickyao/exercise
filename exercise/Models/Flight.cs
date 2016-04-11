using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    #region -- 机场基础信息
    /// <summary>
    /// 机场检索请求
    /// </summary>
    public class GetFlightAirPortListRequestModel {
        /// <summary>
        /// 排序方式
        /// </summary>
        public EnumSortOrderType sorttype { get; set; }

        /// <summary>
        /// 列表中是否包含第二机场的信息，默认false
        /// 例如北京目前有2个机场，如果该参数为false那么该机场不会出现在结果中，用于构建选择机场下拉菜单
        /// </summary>
        public bool showOtherAirport { get; set; }
    }

    /// <summary>
    /// 机场信息
    /// </summary>
    public class FlightAirPortInfoModel
    {
        /// <summary>
        /// 创建/编辑者ID
        /// </summary>
        internal string modifiedBy { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 机场三字码（唯一不可重复）
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 城市的拼音（全拼）
        /// </summary>
        public string pinyin { get; set; }
        /// <summary>
        /// 城市的拼音（首字母）
        /// </summary>
        public string PY { get; set; }
        /// <summary>
        /// 是否热门城市
        /// </summary>
        public bool hot { get; set; }
        /// <summary>
        /// 机场全名
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// 是否该城市的第二个或第三个机场
        /// </summary>
        public bool isOtherPortofTheCity { get; set; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime modifiedOn { get; set; }
    }

    /// <summary>
    /// 获取机场列表请求
    /// </summary>
    public class GetFlightAirCompanyListRequestModel {
        /// <summary>
        /// 排序方式
        /// </summary>
        public EnumSortOrderType sorttype { get; set; }
    }

    /// <summary>
    /// 航空公司信息
    /// </summary>
    public class FlightAirCompanyInfoModel {
        /// <summary>
        /// 创建/编辑者ID
        /// </summary>
        internal string modifiedBy { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 编码（唯一，不可重复）
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 简称（例如 东方航空）
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// 航空公司全名
        /// </summary>
        public string company { get; set; }
        /// <summary>
        /// 航空公司LOGO图片地址
        /// </summary>
        public string imgFilePath { get; set; }
        /// <summary>
        /// 创建/编辑时间
        /// </summary>
        public DateTime modifiedOn { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public Nullable<int> sort { get; set; }
    }
    #endregion

    #region -- 航班机票查询

    /// <summary>
    /// 舱位过滤
    /// </summary>
    public enum SearchFlightSeatType {
        /// <summary>
        /// 显示全部仓位
        /// </summary>
        全部,
        /// <summary>
        /// 仅显示公务舱和头等舱
        /// </summary>
        公务舱与头等舱
    }
    /// <summary>
    /// 航班检索排序方式
    /// </summary>
    public enum SearchFlightSortType {
        /// <summary>
        /// 按出港时间正序排序
        /// </summary>
        按出港时间正序,
        /// <summary>
        /// 按价格正序排序
        /// </summary>
        按价格正序
    }

    /// <summary>
    /// 航班查询请求对象
    /// </summary>
    public class SearchFlightInfoListRequestModel {
        /// <summary>
        /// 出发时间默认为当天
        /// </summary>
        protected DateTime _depDate = DateTime.Now;
        /// <summary>
        /// 出发时间【必填】
        /// </summary>
        public DateTime depDate {
            get { return _depDate; }
            set { _depDate = value; }
        }
        /// <summary>
        /// 出发地（机场三字码）【必填】
        /// </summary>
        public string depCityCode { get; set; }
        /// <summary>
        /// 目的地（机场三字码）【必填】
        /// </summary>
        public string arrCityCode { get; set; }


        /// <summary>
        /// 航空公司编码【选填】
        /// </summary>
        public string companyCode { get; set; }
           
        /// <summary>
        /// 舱位过滤类型
        /// </summary>
        public SearchFlightSeatType seatType { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public SearchFlightSortType sortType { get; set; }
    }

    /// <summary>
    /// 航班检索返回对象
    /// </summary>
    public class SearchFlightInfoListReplayModel : ReplayBase {
        /// <summary>
        /// 请求参数
        /// </summary>
        public SearchFlightInfoListRequestModel request { get; set; }
        /// <summary>
        /// 航线里程数量
        /// </summary>
        public double distance { get; set; }
        /// <summary>
        /// 经济舱Y舱的价格（无折扣的经济舱标准价）
        /// </summary>
        public double basePrice { get; set; }
        /// <summary>
        /// 航班信息列表
        /// </summary>
        public List<FlightInfoModel> flightList { get; set; }
    }
    /// <summary>
    /// 航班信息
    /// </summary>
    public class FlightInfoModel {
        /// <summary>
        /// 航班号
        /// </summary>
        public string flightNo { get; set; }
        /// <summary>
        /// 对应的航空公司
        /// </summary>
        public FlightAirCompanyInfoModel flightCompany { get; set; }
        /// <summary>
        /// 出发地
        /// </summary>
        public FlightAirPortInfoModel depCity { get; set; }
        /// <summary>
        /// 目的地
        /// </summary>
        public FlightAirPortInfoModel arrCity { get; set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public string depTime { get; set; }
        /// <summary>
        /// 起飞时间（格式化后的显示值）
        /// </summary>
        public string depTimeShow { get; set; }

        /// <summary>
        /// 降落时间
        /// </summary>
        public string arrTime { get; set; }

        /// <summary>
        /// 降落时间（格式化后的显示值）
        /// </summary>
        public string arrTimeShow { get; set; }

        /// <summary>
        /// 餐食标准
        /// </summary>
        public string meal { get; set; }
        /// <summary>
        /// 机型
        /// </summary>
        public string planeType { get; set; }
        /// <summary>
        /// 经停次数
        /// </summary>
        public int stopnum { get; set; }
        /// <summary>
        /// 出发航站楼
        /// </summary>
        public string depJetquay { get; set; }

        /// <summary>
        /// 到达航站楼
        /// </summary>
        public string arrJetquay { get; set; }
        /// <summary>
        /// 机场建设费
        /// </summary>
        public double airportTax { get; set; }
        /// <summary>
        /// 燃油附加费
        /// </summary>
        public double fuelTax { get; set; }
        /// <summary>
        /// 最低价格
        /// </summary>
        public double lowPrice { get; set; }
        /// <summary>
        /// 是否是当天最低价的航班
        /// </summary>
        public bool isLowestOfDate { get; set; }
        /// <summary>
        /// 初始化舱位信息
        /// </summary>
        protected List<FlightSeatInfoModel> _seatList = new List<FlightSeatInfoModel>();
        /// <summary>
        /// 舱位信息
        /// </summary>
        public List<FlightSeatInfoModel> seatList {
            get { return _seatList; }
            set { _seatList = value; }
        }
    }
    /// <summary>
    /// 舱位信息
    /// </summary>
    public class FlightSeatInfoModel {
        /// <summary>
        /// 舱位码
        /// </summary>
        public string seatCode { get; set; }
        /// <summary>
        /// 舱位名称
        /// </summary>
        public string seatMsg { get; set; }
        /// <summary>
        /// 舱位状态 A代表作为充足，如果是数字则表示还剩余多少座位
        /// </summary>
        public string seatStatus { get; set; }
        /// <summary>
        /// 折扣值
        /// </summary>
        public double discount { get; set; }
        /// <summary>
        /// 显示折扣
        /// </summary>
        public string discountText { get; set; }
        /// <summary>
        /// 票面价
        /// </summary>
        public double parPrice { get; set; }

        /// <summary>
        /// 机场建设费
        /// </summary>
        public double airportTax { get; set; }
        /// <summary>
        /// 燃油附加费
        /// </summary>
        public double fuelTax { get; set; }
        /// <summary>
        /// 票面价+机场建设费+燃油附加费的合计（订购该舱位客人需要支付的金额）
        /// </summary>
        public double totalPrice { get; set; }

        /// <summary>
        /// 合计金额的计算公式
        /// </summary>
        public string totalPriceText { get; set; }

        /// <summary>
        /// 是否特价舱位（当为True时表示该舱位为特价舱位）
        /// </summary>
        public bool isSpecialseatType { get; set; }

        /// <summary>
        /// 结算价（这是我们公司从接口购买该机票的采购价，请不要在前端展示该信息）
        /// </summary>
        public double settlePrice { get; set; }
        /// <summary>
        /// 初始化政策信息
        /// </summary>
        protected FlightPolicyInfoModel _policyInfo = new FlightPolicyInfoModel();

        /// <summary>
        /// 政策信息（用于出票与采购，请不要在前端展示）
        /// </summary>
        public FlightPolicyInfoModel policyInfo {
            get { return _policyInfo; }
            set { _policyInfo = value; }
        }
    }
    /// <summary>
    /// 舱位政策信息
    /// </summary>
    public class FlightPolicyInfoModel {
        /// <summary>
        /// 政策ID
        /// </summary>
        public int policyId { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        public string commisionPoint { get; set; }
        /// <summary>
        /// 返现
        /// </summary>
        public double commisionMoney { get; set; }
        /// <summary>
        /// 是否需要更换PNR
        /// </summary>
        public bool needSwitchPNR { get; set; }
        /// <summary>
        /// 政策类型  B2B 航空公司网站政策  BSP 中性票政策  不限
        /// </summary>
        public string policyType { get; set; }
        /// <summary>
        /// 供应商工作时间
        /// </summary>
        public string workTime { get; set; }
        /// <summary>
        /// 废票时间
        /// </summary>
        public string vtWorkTime { get; set; }
        /// <summary>
        /// 政策备注
        /// </summary>
        public string comment { get; set; }
    }
    #endregion
}