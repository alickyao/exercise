using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 酒店查询条件可选对象
    /// </summary>
    public class HotelSearchItemModel:Combobox {
        /// <summary>
        /// 类型
        /// </summary>
        public EnumHotelSearchItemType itemType { get; set; }
    }

    /// <summary>
    /// 艺龙酒店可选查询条件类型
    /// </summary>
    public enum EnumHotelSearchItemType
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
        /// 品牌
        /// </summary>
        B
    }



    /// <summary>
    /// 艺龙酒店接口请求基本参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HotelBaseRequest<T>
    {
        protected EnumLocal _local = EnumLocal.zh_CN;
        /// <summary>
        /// 版本
        /// </summary>
        public double Version
        {
            get;
            set;
        }
        /// <summary>
        /// 语言
        /// </summary>
        public EnumLocal Local
        {
            get { return _local; }
        }
        /// <summary>
        /// 请求参数
        /// </summary>
        public T Request { get; set; }
    }
    /// <summary>
    /// 艺龙酒店接口请求基本返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [XmlRoot("ApiResult")]
    public class HotelBaseResponse<T>
    {
        /// <summary>
        /// 结果代码（0表示成功完成了请求；有些请求逻辑是否成功需要继续判断Result）
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 结果数据对象（不同的请求是不同的对象。错误的请求一般返回Null）
        /// </summary>
        public T Result { get; set; }
        /// <summary>
        /// 结果数据对象（不同的请求是不同的对象。错误的请求一般返回Null）
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 艺龙酒店列表查询请求
    /// </summary>
    public class SearchHotelListRequestModel : RequestBase
    {
        /// <summary>
        /// 默认入住时间为当天
        /// </summary>
        protected DateTime _arrivalDate = DateTime.Now.Date;
        /// <summary>
        /// 默认离店时间为次日
        /// </summary>
        protected DateTime _departureDate = DateTime.Now.AddDays(1).Date;

        /// <summary>
        /// 必填项目
        /// 入住日期（大于等于昨天。注意当凌晨到店的搜索传入昨天的日期）
        /// 默认当天
        /// </summary>
        public DateTime arrivalDate
        {
            get { return _arrivalDate; }
            set { _arrivalDate = value; }
        }
        /// <summary>
        /// 必填项目
        /// 离店日期（至少晚于到店时间1天，不多于20天）
        /// 默认明天
        /// </summary>
        public DateTime departureDate
        {
            get { return _departureDate; }
            set { _departureDate = value; }
        }
        /// <summary>
        /// 城市编码（使用位置查询时候可为空，其他查询需要传值）
        /// 测试数据只能查询北京 0101 的酒店
        /// </summary>
        public string cityId { get; set; }
        /// <summary>
        /// 查询关键词(全文检索，可以是酒店名、位置或品牌等。使用本参数的时候，需要输入CityId或DistictId)
        /// </summary>
        public string queryText { get; set; }
        /// <summary>
        /// 主题，如客栈，家庭旅馆 等，请参考获取主题列表项传入参数对应的值，最多可传入3个用“,”隔开
        /// </summary>
        public string themeIds { get; set; }

        /// <summary>
        /// 查询类型
        /// Intelligent  智能搜索 （默认）
        /// HotelName  酒店名称
        /// LocationName 位置名称
        /// </summary>
        public EnumQueryType queryType { get; set; }

        /// <summary>
        /// 产品类型
        /// All =全部,
        /// LastMinuteSale = 今日特价,       
        /// LimitedTimeSale = 限时抢购,
        /// WithoutGuarantee = 免担保
        /// 本参数是筛选包含指定条件的酒店，结果中的酒店可能包含其他属性的产品请自行过滤。
        /// </summary>
        public EnumProductProperty productProperties { get; set; }
        /// <summary>
        /// 支付类型  All-全部、SelfPay-现付、Prepay-预付
        /// 【默认All】
        /// </summary>
        public EnumPaymentType paymentType { get; set; }
        /// <summary>
        /// 星级（搜索多个星级以逗号分隔）
        /// 输入1,2,3,4,5代表1-5星级酒店
        /// </summary>
        public string starRate { get; set; }
        /// <summary>
        /// 最小价格 不限填0
        /// </summary>
        public int lowRate { get; set; }
        /// <summary>
        /// 最大价格 不限填0
        /// </summary>
        public int highRate { get; set; }
        /// <summary>
        /// 地区编码（行政区有效）
        /// </summary>
        public string districtId { get; set; }
        /// <summary>
        /// 位置查询(点选位置搜索)
        /// 包含经度纬度与半径
        /// 如果不用位置查询，请不要传入该参数
        /// </summary>
        public PositionSetModel Position { get; set; }
        /// <summary>
        /// 排序类型
        /// Default艺龙默认排序
        /// StarRankDesc推荐星级降序
        /// RateAsc价格升序
        /// RateDesc价格降序
        /// DistanceAsc距离升序
        /// </summary>
        public EnumSortType Sort { get; set; }
    }
    /// <summary>
    /// 艺龙酒店查询结果
    /// </summary>
    public class SearchHotelListReponseModel : ReplayBase {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 查询请求参数
        /// </summary>
        public SearchHotelListRequestModel condtion { get; set; }
        /// <summary>
        /// 酒店结果集
        /// </summary>
        public List<ElongHotelListInfoModel> rows { get; set; }
    }
    /// <summary>
    /// 艺龙酒店列表信息
    /// </summary>
    public class ElongHotelListInfoModel
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string hotelName { get; set; }
        /// <summary>
        /// 挂牌星级
        /// 此为酒店对外的挂牌星级，0-无星级；1-一星级；2-二星级；3-三星级；4-四星级；5-五星级。当为0时对外显示可用Category的值，但请进行图标区分。
        /// </summary>
        public string starRate { get; set; }
        /// <summary>
        /// 艺龙推荐星级
        /// -1，0均代表经济型酒店，（此处酒店星级是艺龙推荐星级，而非酒店挂牌星级）
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 是否经经济型酒店
        /// </summary>
        public byte isEconomic { get; set; }
        /// <summary>
        /// 是否是公寓型酒店
        /// </summary>
        public byte isApartment { get; set; }
        /// <summary>
        /// 酒店开业时间
        /// 年-月。“1900-01”表示无值
        /// </summary>
        public string establishmentDate { get; set; }
        /// <summary>
        /// 酒店重新装修时间
        /// 年-月。“1900-01”表示无值
        /// </summary>
        public string renovationDate { get; set; }


        /// <summary>
        /// 百度经纬度，仅用于百度地图展示
        /// </summary>
        public MapPoint baiduPosition { get; set; }
        /// <summary>
        /// 谷歌地图经纬度，用于谷歌地图展示
        /// </summary>
        public MapPoint googlePosition { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 封面图片
        /// </summary>
        public string thumbNailUrl { get; set; }

        /// <summary>
        /// 酒店所在地区信息
        /// 城市，行政区，商圈信息
        /// 包括与其对应的ID
        /// </summary>
        public FullLocationInfoModel locationInfo { get; set; }

        /// <summary>
        /// 用户评分
        /// </summary>
        public Review review { get; set; }
        /// <summary>
        /// 酒店服务指数
        /// </summary>
        public ServiceRank servicerank { get; set; }

        /// <summary>
        /// 最低价格
        /// </summary>
        public decimal lowRate { get; set; }
        /// <summary>
        /// 最低价格的货币
        /// </summary>
        public EnumCurrencyCode currencyCode { get; set; }
        /// <summary>
        /// 预订规则
        /// </summary>
        public BookingRule[] bookingRules { get; set; }
        /// <summary>
        /// 担保规则详情
        /// 出现规则即表示需要担保。当 isTimeGuarantee和 isAmountGuarantee都等于false时候表示无条件强制担保
        /// </summary>
        public GuaranteeRule[] guaranteeRules { get; set; }
        /// <summary>
        /// 酒店是否需要担保
        /// </summary>
        public bool isGuarante { get; set; }
        /// <summary>
        /// 距离-
        /// 距离搜索的时候有值
        /// </summary>
        public decimal distance { get; set; }
    }

    /// <summary>
    /// 获取酒店详情请求参数
    /// </summary>
    public class GetElongHotelInfoRequestModel {
        /// <summary>
        /// 默认输入时间
        /// </summary>
        protected DateTime _arrivalDate = DateTime.Now;
        /// <summary>
        /// 默认离店时间
        /// </summary>
        protected DateTime _departureDate = DateTime.Now.AddDays(1);

        /// <summary>
        /// 必填项目
        /// 入住日期（大于等于昨天。注意当凌晨到店的搜索传入昨天的日期）
        /// 默认当天
        /// </summary>
        public DateTime arrivalDate
        {
            get { return _arrivalDate; }
            set { _arrivalDate = value; }
        }
        /// <summary>
        /// 必填项目
        /// 离店日期（至少晚于到店时间1天，不多于20天）
        /// 默认明天
        /// </summary>
        public DateTime departureDate
        {
            get { return _departureDate; }
            set { _departureDate = value; }
        }
        /// <summary>
        /// 必填项目
        /// 酒店ID
        /// 可输入多个酒店ID，用,隔开。最少1个最多10个
        /// </summary>
        public string hotelId { get; set; }
        /// <summary>
        /// 房型编码  当RatePlanId传值的时候不能为空
        /// 【在用户创建订单界面传入】
        /// </summary>
        public string roomTypeId { get; set; }
        /// <summary>
        /// 产品编码  默认请传0
        /// 【在用户创建订单界面传入】
        /// </summary>
        public int ratePlanId { get; set; }
        /// <summary>
        /// 支付类型  All-全部、SelfPay-现付、Prepay-预付
        /// 【默认ALL， 如果用户在查询酒店列表时有支付类型为其他两种方式，那么这里也要传入用户选择的支付类型，否则返回的结果集中可能会包括用户不希望看到的RatePlan】
        /// </summary>
        public EnumPaymentType paymentType { get; set; }
    }

    /// <summary>
    /// 获取酒店详情返回对象
    /// </summary>
    public class GetElongHotelInfoReponseModel : ReplayBase {
        /// <summary>
        /// 酒店详情
        /// </summary>
        public List<ElongHotelInfoModel> hoteldetail { get; set; }
        /// <summary>
        /// 查询请求
        /// </summary>
        public GetElongHotelInfoRequestModel request { get; set; }
    }
    /// <summary>
    /// 艺龙酒店详情
    /// </summary>
    public class ElongHotelInfoModel : ElongHotelListInfoModel
    {
        /// <summary>
        /// 酒店介绍与服务、设施信息
        /// </summary>
        public ElongHotelFacilitiesDetailInfoModel facilities { get; set; }
        /// <summary>
        /// 酒店图片集
        /// </summary>
        public List<ElongHotelImageModel> images { get; set; }


        /// <summary>
        /// 房型列表
        /// </summary>
        public List<ElongHotelRoomInfoModel> rooms { get; set; }
        /// <summary>
        /// 增值服务
        /// </summary>
        public ValueAdd[] valueAdds { get; set; }
        /// <summary>
        /// 促销规则-
        /// 搜索接口(list和detail)中的价格都经过了这些规则的计算，仅需将规则的描述提示用户即可。
        /// </summary>
        public DrrRule[] drrRules { get; set; }
        /// <summary>
        /// 预付规则
        /// </summary>
        public PrepayRule[] prepayRules { get; set; }
        /// <summary>
        /// 送礼活动
        /// </summary>
        public Gift[] gifts { get; set; }

        /// <summary>
        /// 系统建议的到店时间，当查询条件为获取某一个房型及产品时有值，前端使用该值组成单选菜单共用户进行选择
        /// 属性中的id为：最早到店时间,最晚到店时间，中间使用,进行分割
        /// </summary>
        public List<Combobox> sysProposalArrivalTime { get; set; }

        /// <summary>
        /// 担保类型，当查询条件为获取某一个房型及产品时值有效
        /// </summary>
        public EnumHotelGuaranteeType guaranteeType { get; set; }
    }

    /// <summary>
    /// 担保类型
    /// </summary>
    public enum EnumHotelGuaranteeType {
        /// <summary>
        /// 无需担保即可预订
        /// </summary>
        无需担保,
        /// <summary>
        /// 根据房量进行担保
        /// </summary>
        房量担保,
        /// <summary>
        /// 根据到店时间提交进行担保
        /// </summary>
        到店时间担保,
        /// <summary>
        /// 时间与房量担保（或者关系）
        /// </summary>
        时间或房量担保,
        /// <summary>
        /// 表示无论什么情况都需要担保
        /// </summary>
        无条件担保
    }

    /// <summary>
    /// 艺龙酒店房型信息
    /// </summary>
    public class ElongHotelRoomInfoModel {
        /// <summary>
        /// 房型编号
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 名称（重要）
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 面积（重要）
        /// </summary>
        public string area { get; set; }
        /// <summary>
        /// 楼层（重要）
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 是否有宽带（重要）
        /// </summary>
        public Nullable<byte> broadnetAccess { get; set; }
        /// <summary>
        /// 宽带是否收费（重要）
        /// </summary>
        public Nullable<byte> broadnetFee { get; set; }
        /// <summary>
        /// 床型（重要）
        /// </summary>
        public string bedType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string comments { get; set; }
        /// <summary>
        /// 最大入住人数
        /// </summary>
        public string capacity { get; set; }
        /// <summary>
        /// 房型图片
        /// </summary>
        public string imageUrl { get; set; }
        /// <summary>
        /// 房型图集
        /// </summary>
        public List<ElongHotelImageModel> imagesUrls { get; set; }
        /// <summary>
        /// RatePlan
        /// </summary>
        internal ListRatePlan[] apiRatePlans { get; set; }
        /// <summary>
        /// RatePlan（已经过过滤，只有结算为RMB，酒店前台现付，且宾客为统一价的RatePlan）
        /// 每个房型都可能对应有多个RatePlans销售节点
        /// 每个RatePlan都对应预定规则，担保规则，促销礼品等信息。
        /// 当预订时，需要传入的参数至少有  HoteId,RoomTypeId,RatePlanId，入住与离店时间、房间数量与对应的入住人信息这几个基本参数
        /// </summary>
        public ListRatePlan[] ratePlans { get; set; }

        protected bool _hasrateplan = false;
        /// <summary>
        /// 是否有RatePlan
        /// 因为RatePlan经过了过滤，所以不排除经过过滤后没有任何RatePlay销售数据的房型，
        /// 如果为False 则不必展示该房型。
        /// </summary>
        public bool hasRatePlan
        {
            get { return _hasrateplan; }
            set { _hasrateplan = value; }
        }
        /// <summary>
        /// RatePlan中最低的价格
        /// 展示在Rooms列表
        /// </summary>
        public Decimal lowPrice { get; set; }
    }

    /// <summary>
    /// 艺龙酒店图片
    /// </summary>
    public class ElongHotelImageModel {
        /// <summary>
        /// 图片存放路径
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 类型
        /// 1 - 餐厅 (Restaurant)
        /// 2 - 休闲 (Recreation Facilities)
        /// 3 - 会议室 (Meeting/Conference)
        /// 5 - 外观 (Exterior)
        /// 6 - 大堂/接待台 (Lobby/ Reception)
        /// 8 - 客房 (Guest Room)
        /// 10 - 其他 (Other Facilities)
        /// 11 - 公共区域 (Public Area)
        /// 12 - 周边景点 (Nearby Attractions)
        /// </summary>
        public Nullable<byte> type { get; set; }
        /// <summary>
        /// 大小
        /// 1：jpg图片，固定长边350，固定长边350缩放(用于详情页图片展示)
        /// 2：jpg图片，尺寸70x70(用于详情页图片列表的缩微图)
        /// 3：jpg图片，尺寸120x120(用于列表页)
        /// 5：png图片，尺寸70x70
        /// 6：png图片，尺寸120x120
        /// 7：png图片，固定长边640放缩 
        /// </summary>
        public Nullable<byte> size { get; set; }
        /// <summary>
        /// 是否有水印
        /// 0-no,1-yes
        /// </summary>
        public Nullable<byte> waterMark { get; set; }
        /// <summary>
        /// 对应的房型ID
        /// </summary>
        public string roomId { get; set; }
    }

    /// <summary>
    /// 酒店介绍与服务、设施信息
    /// </summary>
    public class ElongHotelFacilitiesDetailInfoModel {
        /// <summary>
        /// 支持的信用卡
        /// </summary>
        public string creditCards { get; set; }
        /// <summary>
        /// 介绍信息(酒店介绍，一般是酒店最为重要的介绍信息)
        /// </summary>
        public string introEditor { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 服务设施（服务类）
        /// </summary>
        public string generalAmenities { get; set; }
        /// <summary>
        /// 休闲设施（会议/休闲类）
        /// </summary>
        public string recreationAmenities { get; set; }
        /// <summary>
        /// 会议设施（会议/休闲类）
        /// </summary>
        public string conferenceAmenities { get; set; }
        /// <summary>
        /// 餐饮设施（餐饮类）
        /// </summary>
        public string diningAmenities { get; set; }
        /// <summary>
        /// 周边交通（周边类）
        /// </summary>
        public string traffic { get; set; }
        /// <summary>
        /// 周边信息（周边类）
        /// </summary>
        public string surroundings { get; set; }
        /// <summary>
        /// 房间设施
        /// </summary>
        public string roomAmenities { get; set; }
        /// <summary>
        /// 设施列表
        /// </summary>
        public List<string> facilities { get; set; }
        /// <summary>
        /// 设施列表清单
        /// </summary>
        internal string[] fac { get { return _facilities; } }
        protected string[] _facilities = {"免费wifi",
                                        "收费wifi",
                                        "免费宽带",
                                        "收费宽带",
                                        "免费停车场",
                                        "收费停车场",
                                        "免费接机服务",
                                        "收费接机服务",
                                        "室内游泳池",
                                        "室外游泳池",
                                        "健身房",
                                        "商务中心",
                                        "会议室",
                                        "酒店餐厅"
        };
    }

    /// <summary>
    /// 艺龙酒店校验创建订单数据请求
    /// </summary>
    public class ElongHotelValidateCreateOrderRequestModel {
        /// <summary>
        /// 入住时间 如果是凌晨入住，则入住日期是前1晚
        /// </summary>
        public DateTime arrivalDate { get; set; }
        /// <summary>
        /// 建议入住日期和离店日期可以最长选择20天
        /// </summary>
        public DateTime departureDate { get; set; }
        /// <summary>
        /// 最早到店时间
        /// 1、与最晚到店时间两者都是必填字段,可让用户选择两个时间点，也可以只让客人选择最晚到店时间系统根据下面的规则计算出最早到店时间；
        /// 2、最早到店时间范围：入住日6:00(建议14:00,因一般酒店接待开始时间是14点)-23:59。
        /// 最晚到店时间范围：入住日7:00-23:59和次日1:00-6:00，都必须是整点或半点或23:59
        /// 3、最早到店时间须晚于当前时间, 最晚到店时间须晚于最早到店时间，一般相差3个小时；
        /// 4、如果客人到店时间是入住日期的第二天的00:00┅06:00之间，请配置最早到店时间为入住日期的23:59，最晚到店时间为入住日期的第二天的06:00；
        /// 5、更多信息请参考FAQ（open.elong.com）
        /// </summary>
        public DateTime earliestArrivalTime { get; set; }
        /// <summary>
        /// 最晚到店时间
        /// </summary>
        public DateTime latestArrivalTime { get; set; }
        /// <summary>
        /// 酒店ID
        /// </summary>
        public string hotelId { get; set; }
        /// <summary>
        /// 房型编号（来自RatePlan.RoomTypeId）
        /// </summary>
        public string roomTypeId { get; set; }
        /// <summary>
        /// 产品编号（来自RatePlan.RatePlanId）
        /// </summary>
        public int ratePlanId { get; set; }
        /// <summary>
        /// 房间数量
        /// </summary>
        public int numberOfRooms { get; set; }
        /// <summary>
        /// 总价，
        /// RatePlan的TotalPrice * 房间数；单位是元；房间单价可能是小数，总价也可能是带小数
        /// </summary>
        public decimal totalPrice { get; set; }
    }

    /// <summary>
    /// 艺龙预订签的验证请求对象欧
    /// </summary>
    public class ElongHotelValidateCreateOrderReplayModel : ReplayBase
    {
        /// <summary>
        /// 验证结果
        /// OK:  正常可预订，Product：产品问题，Inventory：房量不够，Rate:价格不符
        /// </summary>
        public EnumValidateResult ResultCode { get; set; }
        /// <summary>
        /// 具体结果信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 担保金额 如果是担保订单才有这个值
        /// </summary>
        public decimal GuaranteeRate { get; set; }
        /// <summary>
        /// 货币类型
        /// </summary>
        public EnumCurrencyCode CurrencyCode { get; set; }
        /// <summary>
        /// 最晚取消时间  担保订单可取消的时间，如果返回的时间小于当前时间，则代表此订单不可变更取消
        /// </summary>
        public DateTime CancelTime { get; set; }
    }
    /// <summary>
    /// 艺龙预订请求
    /// </summary>
    public class ElongHotelCreateOrderRequestModel: ElongHotelValidateCreateOrderRequestModel
    {
        /// <summary>
        /// 宾客类型  来自RatePlan
        /// </summary>
        public EnumGuestTypeCode customerType { get; set; }
        /// <summary>
        /// 付款类型  来自RatePlan
        /// </summary>
        public EnumPaymentType paymentType { get; set; }
        /// <summary>
        /// 客人数量
        /// </summary>
        public int numberOfCustomers { get; set; }
        /// <summary>
        /// 货币类型（来自RatePlan）
        /// </summary>
        public EnumCurrencyCode currencyCode { get; set; }
        /// <summary>
        /// 客人的IP地址（格式，如 211.151.230.212；请提供真实的客人IP，否则很有可能被艺龙视为虚假订单；获取最终用户IP的时候注意区分用户是否使用了代理）
        /// </summary>
        public string customerIPAddress { get; set; }
        /// <summary>
        /// 联系人（预订人）
        /// </summary>
        public Contact contact { get; set; }
        /// <summary>
        /// 入住人信息（一般来说预订几间房就要留几个人的信息，且不可重复）
        /// </summary>
        public CreateOrderRoom[] orderRooms { get; set; }
        /// <summary>
        /// 给酒店的备注(选填)
        /// </summary>
        public string noteToHotel { get; set; }
        /// <summary>
        /// 给艺龙的备注(选填)
        /// </summary>
        public string noteToElong { get; set; }
        /// <summary>
        /// 信用卡信息(选填)
        /// 担保订单和预付订单才须传信用卡
        /// </summary>
        public CreditCard creditCard { get; set; }
    }
    /// <summary>
    /// 创建艺龙订单请求
    /// </summary>
    public class UserCreateElongHotelOrderRequestModel {
        /// <summary>
        /// 请求参数
        /// </summary>
        public ElongHotelCreateOrderRequestModel request { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        internal string createdBy { get; set; }
    }
}