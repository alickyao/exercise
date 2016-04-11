using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cyclonestyle.Models;
using cyclonestyle.DataBase;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 艺龙酒店相关
    /// </summary>
    public class ElongHotelService: ElongHotelBaseService
    {
        /// <summary>
        /// 获取艺龙酒店热门品牌列表
        /// </summary>
        /// <returns></returns>
        internal static List<Combobox> GetHotelHotBrandList()
        {
            List<Combobox> result = new List<Combobox>();
            try
            {
                result = BaseSysTemDataBaseManager.RsGetHotelHotBrandList();
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
            }
            return result;
        }
        /// <summary>
        /// 根据市ID获取品牌行政取等信息
        /// </summary>
        /// <param name="id">市ID</param>
        /// <param name="q">关键字</param>
        /// <returns></returns>
        internal static List<HotelSearchItemModel> SearchHotelSItemList(string id, string q)
        {
            List<HotelSearchItemModel> result = new List<HotelSearchItemModel>();
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(q))
                {

                }
                else {
                    result = BaseSysTemDataBaseManager.RsSearchHotelSItemList(id, q);
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), id + q);
            }
            return result;
        }

        /// <summary>
        /// 酒店查询
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        internal SearchHotelListReponseModel SearchHotelList(SearchHotelListRequestModel rq)
        {
            SearchHotelListReponseModel result = new SearchHotelListReponseModel();
            try
            {
                result.condtion = rq;
                string Method = "hotel.list";
                HotelListCondition condition = new HotelListCondition();
                #region -- 设置的一些默认值
                condition.ResultType = "2";
                #endregion
                #region -- 参数赋值
                condition.ArrivalDate = rq.arrivalDate;
                condition.DepartureDate = rq.departureDate;
                condition.CityId = rq.cityId;

                condition.QueryText = rq.queryText;
                condition.QueryType = rq.queryType;

                condition.ProductProperties = rq.productProperties;
                condition.StarRate = rq.starRate;
                condition.LowRate = rq.lowRate;
                condition.HighRate = rq.highRate;
                condition.DistrictId = rq.districtId;
                condition.ThemeIds = rq.themeIds;
                if (rq.Position != null)
                {
                    condition.Position = new Position()
                    {
                        Latitude = rq.Position.lat,
                        Longitude = rq.Position.lng,
                        Radius = rq.Position.rat
                    };
                }

                condition.Sort = rq.Sort;

                condition.PageIndex = rq.Page;
                condition.PageSize = rq.PageSize;
                #endregion

                HotelBaseResponse<HotelList> res = GetConditionResponse<HotelListCondition, HotelList>(condition, Method);
                if (res.Code == "0")
                {
                    result.total = res.Result.Count;
                    result.rows = new List<ElongHotelListInfoModel>();
                    if (res.Result.Hotels != null)
                    {
                        foreach (Hotel apihotel in res.Result.Hotels)
                        {
                            ElongHotel_Hotel dbhotel = BaseSysTemDataBaseManager.RsGetHotelStaticInfoByHotelId(apihotel.HotelId);
                            if (dbhotel != null)
                            {
                                #region -- 静态信息

                                ElongHotelListInfoModel uhotel = new ElongHotelListInfoModel()
                                {
                                    id = apihotel.HotelId
                                };
                                uhotel.address = dbhotel.Address;
                                uhotel.locationInfo = new FullLocationInfoModel()
                                {
                                    BusinessZoneId = dbhotel.BusinessZone,
                                    DistrictId = dbhotel.District,
                                    cityInfo = new GeoCityInfoModel()
                                    {
                                        CityCode = dbhotel.CityId
                                    }
                                };
                                uhotel.locationInfo = BaseSysTemDataBaseManager.RsGetLocaionInfo(uhotel.locationInfo);
                                uhotel.hotelName = dbhotel.Name;
                                uhotel.phone = dbhotel.Phone;

                                uhotel.starRate = dbhotel.StarRate.ToString();
                                uhotel.category = dbhotel.Category.ToString();
                                uhotel.isApartment = dbhotel.IsApartment == null ? (byte)0 : dbhotel.IsApartment.Value;
                                uhotel.isEconomic = dbhotel.IsEconomic == null ? (byte)0 : dbhotel.IsEconomic.Value;

                                uhotel.establishmentDate = dbhotel.EstablishmentDate;
                                uhotel.renovationDate = dbhotel.RenovationDate;

                                ServiceRank sr = new ServiceRank();
                                sr.SummaryScore = dbhotel.SummaryScore == null ? "0" : dbhotel.SummaryScore.Value.ToString("0.##");
                                sr.SummaryRate = dbhotel.SummaryRate;
                                sr.InstantConfirmScore = dbhotel.InstantConfirmScore;
                                sr.InstantConfirmRate = dbhotel.InstantConfirmRate;
                                sr.ComplaintRate = dbhotel.ComplaintRate;
                                sr.ComplaintScore = dbhotel.ComplaintScore;
                                sr.BookingSuccessScore = dbhotel.BookingSuccessScore;
                                sr.BookingSuccessRate = dbhotel.BookingSuccessRate;
                                uhotel.servicerank = sr;

                                Review rv = new Review();
                                rv.Count = dbhotel.ReviewCount == null ? "" : dbhotel.ReviewCount.ToString();
                                rv.Good = dbhotel.ReviewGood == null ? "" : dbhotel.ReviewGood.ToString();
                                rv.Poor = dbhotel.ReviewGood == null ? "" : dbhotel.ReviewPoor.ToString();
                                rv.Score = dbhotel.ReviewScore;
                                uhotel.review = rv;

                                uhotel.thumbNailUrl = BaseSysTemDataBaseManager.RsGetHotelThunmNailUrl(dbhotel.Id);
                                uhotel.baiduPosition = new MapPoint()
                                {
                                    lat = dbhotel.BaiduLat == null ? 0 : dbhotel.BaiduLat.Value,
                                    lng = dbhotel.BaiduLon == null ? 0 : dbhotel.BaiduLon.Value
                                };
                                uhotel.googlePosition = new MapPoint()
                                {
                                    lat = dbhotel.GoogleLat == null ? 0 : dbhotel.GoogleLat.Value,
                                    lng = dbhotel.GoogleLon == null ? 0 : dbhotel.GoogleLon.Value
                                };
                                #endregion
                                #region -- 动态
                                uhotel.bookingRules = apihotel.BookingRules;
                                uhotel.currencyCode = apihotel.CurrencyCode;
                                uhotel.distance = apihotel.Distance;
                                uhotel.guaranteeRules = apihotel.GuaranteeRules;
                                uhotel.lowRate = apihotel.LowRate;
                                uhotel.isGuarante = uhotel.guaranteeRules.Count() > 0 ? true : false;
                                #endregion
                                result.rows.Add(uhotel);
                            }
                        }
                    }
                    else {
                        result.ReturnCode = EnumErrorCode.EmptyDate;
                        result.ReturnMessage = "没有匹配的酒店信息";
                    }
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = res.Code;
                    SysManagerService.SysSaveSysLog("艺龙酒店列表查询接口返回错误:" + res.Code + "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(rq), EnumSysLogType.警告);
                }
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), rq);
            }
            return result;
        }

        

        /// <summary>
        /// 获取艺龙酒店详情
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        internal GetElongHotelInfoReponseModel GetHotelInfo(GetElongHotelInfoRequestModel rq)
        {
            GetElongHotelInfoReponseModel result = new GetElongHotelInfoReponseModel();
            try
            {
                string Method = "hotel.detail";
                result.request = rq;
                HotelDetailCondition condition = new HotelDetailCondition
                {
                    ArrivalDate = rq.arrivalDate,
                    DepartureDate = rq.departureDate,
                    HotelIds = rq.hotelId,
                    RoomTypeId = rq.roomTypeId,
                    RatePlanId = rq.ratePlanId,
                    PaymentType = rq.paymentType,
                    Options = "2"
                };
                HotelBaseResponse<HotelList> res = GetConditionResponse<HotelDetailCondition, HotelList>(condition, Method);
                if (res.Code == "0")
                {
                    result.hoteldetail = new List<ElongHotelInfoModel>();
                    if (res.Result.Count > 0)
                    {
                        foreach (Hotel apihotel in res.Result.Hotels)
                        {
                            ElongHotel_Hotel dbhotel = BaseSysTemDataBaseManager.RsGetHotelStaticInfoByHotelId(apihotel.HotelId);
                            if (dbhotel != null)
                            {
                                ElongHotelInfoModel hotelinfo = new ElongHotelInfoModel()
                                {
                                    id = apihotel.HotelId
                                };

                                #region -- 静态信息赋值
                                hotelinfo.address = dbhotel.Address;
                                hotelinfo.locationInfo = new FullLocationInfoModel()
                                {
                                    BusinessZoneId = dbhotel.BusinessZone,
                                    DistrictId = dbhotel.District,
                                    cityInfo = new GeoCityInfoModel()
                                    {
                                        CityCode = dbhotel.CityId
                                    }
                                };
                                hotelinfo.locationInfo = BaseSysTemDataBaseManager.RsGetLocaionInfo(hotelinfo.locationInfo);
                                hotelinfo.hotelName = dbhotel.Name;
                                hotelinfo.phone = dbhotel.Phone;

                                hotelinfo.starRate = dbhotel.StarRate.ToString();
                                hotelinfo.category = dbhotel.Category.ToString();
                                hotelinfo.isApartment = dbhotel.IsApartment == null ? (byte)0 : dbhotel.IsApartment.Value;
                                hotelinfo.isEconomic = dbhotel.IsEconomic == null ? (byte)0 : dbhotel.IsEconomic.Value;

                                hotelinfo.establishmentDate = dbhotel.EstablishmentDate;
                                hotelinfo.renovationDate = dbhotel.RenovationDate;

                                hotelinfo.thumbNailUrl = BaseSysTemDataBaseManager.RsGetHotelThunmNailUrl(hotelinfo.id);

                                #region -- 服务指数
                                ServiceRank sr = new ServiceRank();
                                sr.SummaryScore = dbhotel.SummaryScore == null ? "0" : dbhotel.SummaryScore.Value.ToString("0.##");
                                sr.SummaryRate = dbhotel.SummaryRate;
                                sr.InstantConfirmScore = dbhotel.InstantConfirmScore;
                                sr.InstantConfirmRate = dbhotel.InstantConfirmRate;
                                sr.ComplaintRate = dbhotel.ComplaintRate;
                                sr.ComplaintScore = dbhotel.ComplaintScore;
                                sr.BookingSuccessScore = dbhotel.BookingSuccessScore;
                                sr.BookingSuccessRate = dbhotel.BookingSuccessRate;
                                hotelinfo.servicerank = sr;
                                #endregion

                                #region -- 评价
                                Review rv = new Review();
                                rv.Count = dbhotel.ReviewCount == null ? "" : dbhotel.ReviewCount.ToString();
                                rv.Good = dbhotel.ReviewGood == null ? "" : dbhotel.ReviewGood.ToString();
                                rv.Poor = dbhotel.ReviewGood == null ? "" : dbhotel.ReviewPoor.ToString();
                                rv.Score = dbhotel.ReviewScore;
                                hotelinfo.review = rv;
                                #endregion

                                #region -- 地理位置坐标
                                hotelinfo.baiduPosition = new MapPoint()
                                {
                                    lat = dbhotel.BaiduLat == null ? 0 : dbhotel.BaiduLat.Value,
                                    lng = dbhotel.BaiduLon == null ? 0 : dbhotel.BaiduLon.Value
                                };
                                hotelinfo.googlePosition = new MapPoint()
                                {
                                    lat = dbhotel.GoogleLat == null ? 0 : dbhotel.GoogleLat.Value,
                                    lng = dbhotel.GoogleLon == null ? 0 : dbhotel.GoogleLon.Value
                                };
                                #endregion

                                #region -- 服务与设施信息
                                ElongHotelFacilitiesDetailInfoModel fd = new ElongHotelFacilitiesDetailInfoModel()
                                {
                                    conferenceAmenities = dbhotel.ConferenceAmenities,
                                    creditCards = dbhotel.CreditCards,
                                    description = dbhotel.Description,
                                    diningAmenities = dbhotel.DiningAmenities,
                                    generalAmenities = dbhotel.GeneralAmenities,
                                    introEditor = dbhotel.IntroEditor,
                                    recreationAmenities = dbhotel.RecreationAmenities,
                                    roomAmenities = dbhotel.RoomAmenities,
                                    surroundings = dbhotel.Surroundings,
                                    traffic = dbhotel.Traffic,
                                };
                                if (!string.IsNullOrEmpty(dbhotel.Facilities))
                                {
                                    List<string> FacilitiesList = new List<string>();
                                    string[] farr = dbhotel.Facilities.Split(',');
                                    foreach (string f in farr)
                                    {
                                        try
                                        {
                                            //防止数组溢出
                                            FacilitiesList.Add(fd.fac[int.Parse(f) - 1]);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    fd.facilities = FacilitiesList;
                                    hotelinfo.facilities = fd;
                                }
                                #endregion

                                #region -- 图片
                                List<ElongHotelImageModel> ilist = BaseSysTemDataBaseManager.RsGetElongHotelImageListByHotelId(hotelinfo.id);
                                foreach (ElongHotelImageModel img in ilist)
                                {
                                    //如果url不是以http开始，则需要增加前缀http://www.elongstatic.com/pp/hotels/hotel
                                    if (img.url.IndexOf("http://") == -1)
                                    {
                                        img.url = "http://www.elongstatic.com/pp/hotels/hotel" + img.url;
                                    }
                                }
                                hotelinfo.images = ilist;

                                #endregion

                                #endregion

                                #region -- 动态部分

                                #region -- 房型

                                List<ElongHotelRoomInfoModel> roomlist = new List<ElongHotelRoomInfoModel>();
                                if (apihotel.Rooms != null)
                                {
                                    foreach (Room apiroom in apihotel.Rooms)
                                    {
                                        ElongHotelRoomInfoModel room = new ElongHotelRoomInfoModel()
                                        {
                                            id = apiroom.RoomId,
                                            name = apiroom.Name,
                                            floor = apiroom.Floor,
                                            imageUrl = apiroom.ImageUrl,
                                            apiRatePlans = apiroom.RatePlans
                                        };
                                        //RatePlans 过滤 保留货币为RMB 宾客为内宾
                                        room.ratePlans = room.apiRatePlans.Where(p => p.CurrencyCode == EnumCurrencyCode.RMB && (p.CustomerType == EnumGuestTypeCode.All || p.CustomerType == EnumGuestTypeCode.Chinese)).ToArray();
                                        if (room.ratePlans.Count() > 0)
                                        {
                                            room.hasRatePlan = true;
                                            room.lowPrice = room.ratePlans.OrderBy(p => p.AverageRate).Skip(0).Take(1).ToList().First().AverageRate;
                                        }
                                        roomlist.Add(room);
                                    }
                                    #region -- 房型静态信息读取
                                    List<ElongHotel_HotelRooms> dbrooms = BaseSysTemDataBaseManager.RsGetElongHotelRoomsByHotelId(hotelinfo.id);
                                    foreach (ElongHotelRoomInfoModel room in roomlist)
                                    {
                                        ElongHotel_HotelRooms dbroom = dbrooms.SingleOrDefault(p => p.Id == room.id);
                                        if (dbroom != null)
                                        {
                                            room.area = dbroom.Area;
                                            room.bedType = dbroom.BedType;
                                            room.broadnetAccess = dbroom.BroadnetAccess;
                                            room.broadnetFee = dbroom.BroadnetFee;
                                            room.capacity = dbroom.Capacity;
                                            room.comments = dbroom.Comments;
                                            room.description = dbroom.Description;
                                            room.imagesUrls = ilist.Where(p => string.IsNullOrEmpty(p.roomId) ? false : p.roomId.Split(',').Contains(room.id)).ToList();
                                            if (string.IsNullOrEmpty(room.imageUrl))
                                            {
                                                if (room.imagesUrls.Count > 0)
                                                {
                                                    room.imageUrl = room.imagesUrls[0].url;//如果接口中提供的IMAGEURL为空，而本地有该房型图片，则将第一张放置为房型封面图片
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                hotelinfo.rooms = roomlist;

                                #endregion

                                #region -- 预订规则

                                hotelinfo.bookingRules = apihotel.BookingRules;
                                hotelinfo.guaranteeRules = apihotel.GuaranteeRules;
                                hotelinfo.drrRules = apihotel.DrrRules;
                                hotelinfo.gifts = apihotel.Gifts;
                                hotelinfo.prepayRules = apihotel.PrepayRules;
                                hotelinfo.valueAdds = apihotel.ValueAdds;

                                #endregion

                                #endregion

                                #region -- 建议到店时间计算 与担保类型 当查询某房型与产品时，根据担保条件进行计算并推荐到店时间分割点

                                if (rq.ratePlanId != 0 && !string.IsNullOrEmpty(rq.roomTypeId)) {
                                    if (hotelinfo.rooms.Count() > 0) {
                                        if (hotelinfo.rooms[0].ratePlans.Count() > 0) {
                                            ListRatePlan rate = hotelinfo.rooms[0].ratePlans[0];
                                            //获取担保规则
                                            GuaranteeRule rule = null;//担保规则
                                            hotelinfo.guaranteeType = EnumHotelGuaranteeType.无需担保;
                                            //获取担保细则
                                            if (!string.IsNullOrEmpty(rate.GuaranteeRuleIds))
                                            {
                                                int id = int.Parse(rate.GuaranteeRuleIds);
                                                rule = hotelinfo.guaranteeRules.SingleOrDefault(p => p.GuranteeRuleId == id);
                                                if (rule != null)
                                                {
                                                    if (rule.IsAmountGuarantee == false && rule.IsTimeGuarantee == false)
                                                    {
                                                        //当房量担保和到店时间担保都为false时，预订该酒店为无条件担保
                                                        hotelinfo.guaranteeType = EnumHotelGuaranteeType.无条件担保;
                                                    }
                                                    else if (rule.IsTimeGuarantee == true && rule.IsAmountGuarantee == false)
                                                    {
                                                        hotelinfo.guaranteeType = EnumHotelGuaranteeType.到店时间担保;
                                                    }
                                                    else if (rule.IsAmountGuarantee == true && rule.IsTimeGuarantee == false)
                                                    {
                                                        hotelinfo.guaranteeType = EnumHotelGuaranteeType.房量担保;
                                                    }
                                                    else {
                                                        hotelinfo.guaranteeType = EnumHotelGuaranteeType.时间或房量担保;
                                                    }
                                                }
                                            }
                                            //根据担保的类型计算建议的到店时间可选项

                                            //计算出可以选择的最早到店时间，默认情况下是预订入住时间的当天的下午16:00
                                            DateTime now = DateTime.Now;//当前时间
                                            //now = DateTime.Parse("2016-2-29 06:35");//可测试
                                            DateTime earliestTime = DateTime.Parse(rq.arrivalDate.Date.ToShortDateString() + " 16:00");
                                            hotelinfo.sysProposalArrivalTime = new List<Combobox>();//初始化建议可选的到店时间
                                            List<string> splittime = new List<string>();//初始化时间分割数组
                                            if (now.Date == rq.arrivalDate.Date)
                                            {
                                                //是预订今天的酒店，如果已经过16:00则最早到店时间为下一个整点
                                                if (now > earliestTime) {
                                                    earliestTime = DateTime.Parse(now.AddHours(1).ToString("yyyy-MM-dd HH:00"));
                                                }
                                            }
                                            if (now.AddDays(-1).Date == rq.arrivalDate.Date) {
                                                //当凌晨查询入住时间是昨天的酒店时，最早到店时间就是当天时间 往后一个小时的整点
                                                earliestTime = DateTime.Parse(now.AddHours(1).ToString("yyyy-MM-dd HH:00"));
                                            }
                                            if (hotelinfo.guaranteeType == EnumHotelGuaranteeType.到店时间担保 || hotelinfo.guaranteeType == EnumHotelGuaranteeType.时间或房量担保)
                                            {
                                                //需要根据担保规则中的担保时间点进行分割 [分割点 23:59 第二天06:00]
                                                //获取分割点
                                                if (earliestTime < DateTime.Parse(rq.arrivalDate.Date.ToShortDateString() + " " + rule.StartTime))
                                                {
                                                    splittime.Add(rule.StartTime);
                                                }
                                                //判断结束时间是否大于开始时间，如果是则还需要增加一个时间节点，无需担保开始时间
                                                if (DateTime.Parse(now.ToShortDateString() + " " + rule.StartTime) < DateTime.Parse(now.ToShortDateString() + " " + rule.EndTime)) {
                                                    splittime.Add(rule.EndTime);
                                                }
                                            }
                                            else {
                                                //无需根据担保条件进行时间分割 [18：00 20:00 23:59 第二天06:00]
                                                if (earliestTime < DateTime.Parse(rq.arrivalDate.Date.ToShortDateString() + " 18:00")) {
                                                    splittime.Add("18:00");
                                                }
                                                if (earliestTime < DateTime.Parse(rq.arrivalDate.Date.ToShortDateString() + " 20:00"))
                                                {
                                                    splittime.Add("20:00");
                                                }
                                            }
                                            if (earliestTime < DateTime.Parse(rq.arrivalDate.Date.ToShortDateString() + " 23:59"))
                                            {
                                                splittime.Add("23:59");
                                            }
                                            if (earliestTime < DateTime.Parse(rq.arrivalDate.AddDays(1).Date.ToShortDateString() + " 06:00"))
                                            {
                                                splittime.Add("06:00");
                                            }
                                            //建议的入住时间
                                            foreach (string time in splittime) {
                                                DateTime startime = DateTime.Now;//开始
                                                DateTime endtime = DateTime.Now;//结束
                                                if (time == "06:00")
                                                {
                                                    if (earliestTime < DateTime.Parse(rq.arrivalDate.ToShortDateString() + " 23:59")) {
                                                        //如果最早到店时间小于入住当天的23:59分
                                                        //则开始时间是入住当天的23:59分
                                                        startime = DateTime.Parse(rq.arrivalDate.ToShortDateString() + " 23:59");
                                                    }
                                                    else {
                                                        //否则最早入住时间就是上面计算的最早到店时间
                                                        startime = earliestTime;
                                                    }
                                                    //最晚到店时间是入住时间的第二天凌晨06:00
                                                    endtime = DateTime.Parse(rq.arrivalDate.AddDays(1).ToShortDateString() + " 06:00");
                                                }
                                                else {
                                                    //其他时间段
                                                    if (earliestTime < DateTime.Parse(rq.arrivalDate.ToShortDateString() + " " + time).AddHours(-2))
                                                    {
                                                        //如果最早到店时间小于节点时间减去2个小时,则开始时间为节点时间减去2小时
                                                        //因为除了23:59其他都必须是整点，所以这里还要判断如果节点时间是23:59分则开始时间需要取整数
                                                        if (time != "23:59")
                                                        {
                                                            startime = DateTime.Parse(rq.arrivalDate.ToShortDateString() + " " + time).AddHours(-2);
                                                        }
                                                        else {
                                                            startime = DateTime.Parse(rq.arrivalDate.ToShortDateString() + " 21:00");
                                                        }
                                                    }
                                                    else {
                                                        //否则最早入住时间就是上面计算的最早到店时间
                                                        startime = earliestTime;
                                                    }
                                                    endtime = DateTime.Parse(rq.arrivalDate.ToShortDateString() + " " + time);
                                                }
                                                hotelinfo.sysProposalArrivalTime.Add(new Combobox()
                                                {
                                                    id = startime.ToString("yyyy-MM-dd HH:mm") + "," + endtime.ToString("yyyy-MM-dd HH:mm"),
                                                    text = time == "06:00" ? (now.Date == rq.arrivalDate.AddDays(1).Date?"最晚当日凌晨06:00到达":"最晚次日凌晨06:00到达") : "最晚当日" + time + "到达"
                                                });
                                            }
                                        }
                                    }
                                }

                                #endregion

                                result.hoteldetail.Add(hotelinfo);
                            }
                        }
                    }
                    else {
                        result.ReturnCode = EnumErrorCode.EmptyDate;
                        result.ReturnMessage = "没有匹配的酒店信息";
                    }
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = res.Code;
                    SysManagerService.SysSaveSysLog("艺龙酒店详情查询接口返回错误:" + res.Code + "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(rq), EnumSysLogType.警告);
                }
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), rq);
            }
            return result;
        }

        /// <summary>
        /// 验证创建订单
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        internal ElongHotelValidateCreateOrderReplayModel ValidateCreateOrder(ElongHotelValidateCreateOrderRequestModel rq)
        {
            ElongHotelValidateCreateOrderReplayModel result = new ElongHotelValidateCreateOrderReplayModel();
            try
            {
                string Method = "hotel.data.validate";
                ValidateCondition condition = new ValidateCondition()
                {
                    ArrivalDate = rq.arrivalDate,
                    DepartureDate = rq.departureDate,
                    EarliestArrivalTime = rq.earliestArrivalTime,
                    LatestArrivalTime = rq.latestArrivalTime,
                    HotelId = rq.hotelId,
                    RoomTypeId = rq.roomTypeId,
                    RatePlanId = rq.ratePlanId,
                    NumberOfRooms = rq.numberOfRooms,
                    TotalPrice = rq.totalPrice
                };
                HotelBaseResponse<ValidateResult> res = GetConditionResponse<ValidateCondition, ValidateResult>(condition, Method);
                if (res.Code == "0")
                {
                    result.ResultCode = res.Result.ResultCode;
                    if (result.ResultCode == EnumValidateResult.OK)
                    {
                        result.CancelTime = res.Result.CancelTime;
                        result.CurrencyCode = res.Result.CurrencyCode;
                        result.ErrorMessage = res.Result.ErrorMessage;
                        result.GuaranteeRate = res.Result.GuaranteeRate;
                    }
                    else {
                        result.ReturnCode = EnumErrorCode.EmptyDate;
                        string errormes = string.Empty;
                        switch (result.ResultCode) {
                            case EnumValidateResult.Inventory:
                                errormes = "房量不够";
                                break;
                            case EnumValidateResult.Product:
                                errormes = "产品问题";
                                break;
                            case EnumValidateResult.Rate:
                                errormes = "价格不符";
                                break;
                        }
                        result.ReturnMessage = "无法预订，原因" + errormes;
                    }
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = res.Code;
                    SysManagerService.SysSaveSysLog("艺龙酒店提交订单验证接口返回错误:" + res.Code + "请求参数:" + Newtonsoft.Json.JsonConvert.SerializeObject(rq), EnumSysLogType.警告);
                }
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), rq);
            }
            return result;
        }
    }
}