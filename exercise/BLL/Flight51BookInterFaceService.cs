using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using cyclonestyle.getAvailableFlightWithPriceAndCommisionService;
using cyclonestyle.Models;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 51BOOK机票查询及预订服务
    /// </summary>
    public class Flight51BookInterFaceService
    {
        /// <summary>
        /// 账户编号
        /// </summary>
        protected string agencyCode = System.Configuration.ConfigurationManager.AppSettings["agencyCode"].ToString();
        /// <summary>
        /// 安全码
        /// </summary>
        protected string sign = System.Configuration.ConfigurationManager.AppSettings["sign"].ToString();


        //机票查询参数设置(可设置)


        /// <summary>
        /// 只返回可用舱位
        /// 0 只返回可用舱位;
        /// 1 返回完整舱位列表;
        /// 即只返回舱位数量为1～9或为A的舱位;
        /// </summary>
        public int onlyAvailableSeat = 0;
        /// <summary>
        /// 
        /// </summary>
        public bool onlyAvailableSeatSpecified = true;

        /// <summary>
        /// 是否包括特殊政策  0 仅返回普通政策; 1 允许返回特殊政策;
        /// </summary>
        public int onlyNormalCommision = 0;
        /// <summary>
        /// 
        /// </summary>
        public bool onlyNormalCommisionSpecified = true;


        /// <summary>
        /// 是否只返回在工作时间内政策 0不做限制 1仅返回当前仍在工作时间的政策; 如不做限制则很容易造成无法出票的局面。
        /// </summary>
        public int onlyOnWorkingCommision = 1;
        /// <summary>
        /// 
        /// </summary>
        public bool onlyOnWorkingCommisionSpecified = true;


        /// <summary>
        /// 是否可更换PNR出票 0可以更换PNR后出票 1只用自己的PNR出票 由出票方控制，选择可以更换与我们一点关系也没有。
        /// </summary>
        public int onlySelfPNR = 0;
        /// <summary>
        /// 
        /// </summary>
        public bool onlySelfPNRSpecified = true;

        /// <summary>
        /// 链接
        /// </summary>
        GetAvailableFlightWithPriceAndCommisionService_1_0Client client { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        public Flight51BookInterFaceService() {
            client = new GetAvailableFlightWithPriceAndCommisionService_1_0Client();
        }
        
        /// <summary>
        /// 查询航班信息
        /// </summary>
        /// <returns></returns>
        public SearchFlightInfoListReplayModel SearchFlight(SearchFlightInfoListRequestModel condtion) {
            SearchFlightInfoListReplayModel result = new SearchFlightInfoListReplayModel() {
                request = condtion,
                flightList = new List<FlightInfoModel>()
            };
            try
            {
                #region -- 从远程接口读取航班信息
                string signRq = FormsAuthentication.HashPasswordForStoringInConfigFile(agencyCode + condtion.arrCityCode + onlyAvailableSeat.ToString() + onlyNormalCommision.ToString() + onlyOnWorkingCommision.ToString() + onlySelfPNR.ToString() + condtion.depCityCode + sign, "MD5").ToLower();
                availableFlightWithPriceAndCommisionRequest rq = new availableFlightWithPriceAndCommisionRequest()
                {
                    orgAirportCode = condtion.depCityCode,
                    dstAirportCode = condtion.arrCityCode,
                    date = condtion.depDate.ToShortDateString(),
                    airlineCode = condtion.companyCode,
                    agencyCode = agencyCode,
                    sign = signRq,
                    onlyAvailableSeat = onlyAvailableSeat,
                    onlyAvailableSeatSpecified = onlyAvailableSeatSpecified,
                    onlyNormalCommision = onlyNormalCommision,
                    onlyNormalCommisionSpecified = onlyNormalCommisionSpecified,
                    onlyOnWorkingCommision = onlyOnWorkingCommision,
                    onlyOnWorkingCommisionSpecified = onlyOnWorkingCommisionSpecified,
                    onlySelfPNR = onlySelfPNR,
                    onlySelfPNRSpecified = onlySelfPNRSpecified
                };
                availableFlightWithPriceAndCommisionReply rp = client.getAvailableFlightWithPriceAndCommision(rq);
                #endregion
                if (rp.returnCode == "S")
                {
                    if (rp.flightItems.Length > 0)
                    {
                        wsFlightWithPriceAndCommisionItem cflight = rp.flightItems[0];
                        result.distance = cflight.distance;
                        result.basePrice = cflight.basePrice;
                        if (cflight.flights.Length > 0)
                        {
                            //获取机场信息
                            List<FlightAirPortInfoModel> airport = FlightService.GetFlightAirPortList(new GetFlightAirPortListRequestModel()
                            {
                                showOtherAirport = true
                            });
                            //获取航空公司信息
                            List<FlightAirCompanyInfoModel> aircompany = FlightService.GetFlightAirCompanyList(new GetFlightAirCompanyListRequestModel()
                            {
                                sorttype = EnumSortOrderType.按时间降序
                            });
                            double loweastprice = 9999999;
                            foreach (wsFlightWithPriceAndCommision cf in cflight.flights)
                            {
                                if (cf.seatItems != null)
                                {
                                    if (cf.seatItems.Length > 0)
                                    {
                                        FlightInfoModel info = new FlightInfoModel()
                                        {
                                            airportTax = cf.airportTax,
                                            arrTime = cf.arriTime,
                                            depTime = cf.depTime,
                                            arrTimeShow = cf.arriTime.Substring(0, 2) + ":" + cf.arriTime.Substring(2, 2),
                                            depTimeShow = cf.depTime.Substring(0, 2) + ":" + cf.depTime.Substring(2, 2),
                                            arrJetquay = cf.dstJetquay,
                                            depJetquay = cf.orgJetquay,
                                            flightNo = cf.flightNo,
                                            meal = cf.meal,
                                            fuelTax = cf.fuelTax,
                                            stopnum = cf.stopnum,
                                            planeType = cf.planeType,
                                        };
                                        //出发
                                        FlightAirPortInfoModel depcity = airport.SingleOrDefault(p => p.code == cf.orgCity);
                                        if (depcity != null)
                                        {
                                            info.depCity = depcity;
                                        }
                                        else {
                                            result.ReturnCode = EnumErrorCode.EmptyDate;
                                            result.ReturnMessage = "暂无三字码为[" + cf.orgCity + "]的机场信息，请客服人员在机场资源管理中进行添加操作";
                                            result.flightList = new List<FlightInfoModel>();
                                            SysManagerService.SysSaveSysLog("缺少三字码为[" + cf.orgCity + "]的机场", EnumSysLogType.警告);
                                            return result;
                                        }
                                        //到达
                                        FlightAirPortInfoModel arrcity = airport.SingleOrDefault(p => p.code == cf.dstCity);
                                        if (arrcity != null)
                                        {
                                            info.arrCity = arrcity;
                                        }
                                        else {
                                            result.ReturnCode = EnumErrorCode.EmptyDate;
                                            result.ReturnMessage = "暂无三字码为[" + cf.dstCity + "]的机场信息，请客服人员在机场资源管理中进行添加操作";
                                            SysManagerService.SysSaveSysLog("缺少三字码为[" + cf.dstCity + "]的机场", EnumSysLogType.警告);
                                            result.flightList = new List<FlightInfoModel>();
                                            return result;
                                        }
                                        //航空公司
                                        FlightAirCompanyInfoModel company = aircompany.SingleOrDefault(p => p.code == cf.flightNo.Substring(0, 2));
                                        if (company != null)
                                        {
                                            info.flightCompany = company;
                                        }
                                        else {
                                            result.ReturnCode = EnumErrorCode.EmptyDate;
                                            result.ReturnMessage = "暂无编码为[" + cf.flightNo.Substring(0, 2) + "]的航空公司信息，请客服人员在航空公司资源管理中进行添加操作";
                                            SysManagerService.SysSaveSysLog("暂无编码为[" + cf.flightNo.Substring(0, 2) + "]的航空公司信息", EnumSysLogType.警告);
                                            result.flightList = new List<FlightInfoModel>();
                                            return result;
                                        }
                                        //舱位
                                        foreach (wsSeatWithPriceAndComisionItem s in cf.seatItems)
                                        {
                                            FlightSeatInfoModel seat = new FlightSeatInfoModel()
                                            {
                                                discount = s.discount,
                                                discountText = (s.discount >= 1 && s.seatCode != "Y") ? "" : (s.seatCode == "Y" && s.discount == 1) ? "全价" : (s.discount * 10).ToString() + "折",
                                                isSpecialseatType = s.seatType == 3 ? true : false,
                                                parPrice = s.parPrice,
                                                seatCode = s.seatCode,
                                                seatMsg = s.seatMsg,
                                                seatStatus = s.seatStatus,
                                                settlePrice = s.settlePrice,
                                                airportTax = cf.airportTax,
                                                fuelTax = cf.fuelTax,
                                                totalPrice = s.parPrice + cf.airportTax + cf.fuelTax,
                                                totalPriceText = s.parPrice.ToString("0.##") + " + " + cf.airportTax.ToString("0.##") + " + " + cf.fuelTax.ToString("0.##") + " = ￥" + (s.parPrice + cf.airportTax + cf.fuelTax).ToString("0.##")+"元",
                                                policyInfo = new FlightPolicyInfoModel()
                                                {
                                                    comment = s.policyData.comment,
                                                    commisionMoney = s.policyData.commisionMoney,
                                                    commisionPoint = s.policyData.commisionPoint,
                                                    needSwitchPNR = s.policyData.needSwitchPNR == 1 ? true : false,
                                                    policyId = s.policyData.policyId,
                                                    policyType = s.policyData.policyType,
                                                    vtWorkTime = s.policyData.vtWorkTime,
                                                    workTime = s.policyData.workTime
                                                }
                                            };
                                            if (condtion.seatType == SearchFlightSeatType.公务舱与头等舱)
                                            {
                                                if (seat.discount >= 1 && seat.seatCode != "Y") {
                                                    info.seatList.Add(seat);
                                                }
                                            }
                                            else {
                                                info.seatList.Add(seat);
                                            }
                                        }
                                        if (info.seatList.Count > 0)
                                        {
                                            info.lowPrice = info.seatList.Min(p => p.parPrice);
                                            if (loweastprice > info.lowPrice)
                                            {
                                                loweastprice = info.lowPrice;
                                            }
                                            result.flightList.Add(info);
                                        }
                                    }
                                }
                            }
                            //排序
                            if (condtion.sortType == SearchFlightSortType.按价格正序)
                            {
                                result.flightList.Sort(
                                    delegate (FlightInfoModel a, FlightInfoModel b)
                                    {
                                        return a.lowPrice.CompareTo(b.lowPrice);
                                    }
                                    );
                            }
                            //获取当天最低价
                            List<FlightInfoModel> infolist = result.flightList.Where(p => p.lowPrice == loweastprice).ToList();
                            if (infolist.Count > 0) {
                                foreach (FlightInfoModel i in infolist) {
                                    i.isLowestOfDate = true;
                                }
                            }
                            if (result.flightList.Count == 0)
                            {
                                result.ReturnCode = EnumErrorCode.EmptyDate;
                                result.ReturnMessage = "无航班信息";
                            }
                        }
                        else {
                            result.ReturnCode = EnumErrorCode.EmptyDate;
                            result.ReturnMessage = "无航班信息";
                        }
                    }
                    else {
                        result.ReturnCode = EnumErrorCode.EmptyDate;
                        result.ReturnMessage = "无航班信息";
                    }
                }
                else {
                    SysManagerService.SysSaveSysLog("51BOOK机票接口返回错误：" + rp.returnMessage, EnumSysLogType.警告);
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = rp.returnMessage;
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
            }
            return result;
            
        }
    }
}