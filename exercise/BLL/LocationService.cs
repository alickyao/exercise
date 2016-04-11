using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cyclonestyle.Models;
using cyclonestyle.DataBase;
using Newtonsoft.Json;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 位置服务
    /// </summary>
    public class LocationService
    {
        /// <summary>
        /// 根据坐标点获取当前所在区域信息
        /// </summary>
        /// <param name="mapPoint">点位信息</param>
        /// <returns></returns>
        internal static MyGeoInfo GetLocationInfoByPoint(MapPoint mapPoint)
        {
            MyGeoInfo result = new MyGeoInfo();
            try
            {
                string ak = System.Configuration.ConfigurationManager.AppSettings["Baidu_ak"].ToString();  //百度开发秘钥
                string url = "http://api.map.baidu.com/geocoder/v2/?ak=" + ak + "&location=" + mapPoint.lat.ToString() + "," + mapPoint.lng.ToString() + "&output=json&pois=0";
                string json_Result = HttpRequestHelp.Get(url);
                dynamic postion = JsonConvert.DeserializeObject(json_Result);
                addressComponent address = new addressComponent();
                if (postion.status == "0")
                {
                    string citynane = postion.result.addressComponent.city;
                    #region -- 匹配艺龙城市ID（用于酒店搜索）;
                    for (int i = citynane.Length; i > 0; i--)
                    {
                        citynane = citynane.Substring(0, i);
                        string cityid = BaseSysTemDataBaseManager.RsGetCityIdByCityName(citynane);
                        if (!string.IsNullOrEmpty(cityid))
                        {
                            address.elongcityid = cityid;
                            break;
                        }
                    }
                    #endregion
                    address.city = postion.result.addressComponent.city;
                    address.district = postion.result.addressComponent.district;
                    address.province = postion.result.addressComponent.province;
                    address.street = postion.result.addressComponent.street;
                    address.street_number = postion.result.addressComponent.street_number;

                    result.business = postion.result.business;
                    result.formatted_address = postion.result.formatted_address;
                    MapPoint loc = new MapPoint();
                    loc.lat = postion.result.location.lat;
                    loc.lng = postion.result.location.lng;
                    result.point = loc;
                    result.status = postion.status;
                    #region -- 如果未能成功匹配城市ID，则判断是否在自治州
                    if (string.IsNullOrEmpty(address.elongcityid))
                    {
                        if (address.city.Contains("自治州"))
                        {
                            //自治州
                            string district = address.district;
                            for (int i = district.Length; i > 0; i--)
                            {
                                district = district.Substring(0, i);
                                string cityid = BaseSysTemDataBaseManager.RsGetCityIdByCityName(district);
                                if (!string.IsNullOrEmpty(cityid))
                                {
                                    address.elongcityid = cityid;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                    result.addressComponent = address;
                }
                else {
                    result.status = postion.status;
                    SysManagerService.SysSaveErrorLogMsg("从百度地图接口根据点位信息获取位置详情时发生错误，错误代码{0}，错误代码对照表：1服务器内部错误.2请求参数非法.3权限校验失败.4配额校验失败.5ak不存在或者非法.101服务禁用.102不通过白名单或者安全码不对.2xx无权限.3xx配额错误", mapPoint);
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), mapPoint);
            }
            return result;
        }


        /// <summary>
        /// 获取省份信息列表
        /// </summary>
        /// <returns></returns>
        internal static List<Combobox> GetProvniceList()
        {
            List<Combobox> result = new List<Combobox>();
            try
            {
                result = BaseSysTemDataBaseManager.RsGetProvniceList();
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 根据省ID获取城市列表
        /// </summary>
        /// <param name="id">省ID</param>
        /// <returns></returns>
        internal static List<GeoCityInfoModel> GetCityListByProvniceId(string id)
        {
            List<GeoCityInfoModel> result = new List<GeoCityInfoModel>();
            try
            {
                result = BaseSysTemDataBaseManager.RsGetCityListByProvniceId(id);
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 根据市ID检索区域信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchAreaInfoListReplayModel GetAreaInfoListByCityCode(SearchAreaInfoListRequestModel condtion)
        {
            SearchAreaInfoListReplayModel result = new SearchAreaInfoListReplayModel();
            try
            {
                if (!string.IsNullOrEmpty(condtion.CityCode)) {
                    result = BaseSysTemDataBaseManager.RsGetAreaInfoListByCityCode(condtion);
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 获取全国热门城市列表，按省排列，每个省去前3个城市
        /// </summary>
        /// <returns></returns>
        internal static List<GeoCityInfoModel> GetHotCityList()
        {
            List<GeoCityInfoModel> result = new List<GeoCityInfoModel>();
            try
            {
                result = BaseSysTemDataBaseManager.RsGetHotCityList();
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
            }
            return result;
        }
        /// <summary>
        /// 检索城市by关键字
        /// </summary>
        /// <param name="q">关键字，汉子或拼音</param>
        /// <returns></returns>
        internal static List<GeoCityInfoModel> SearchCityByKeyWords(string q)
        {
            List<GeoCityInfoModel> result = new List<GeoCityInfoModel>();
            try
            {
                if (!string.IsNullOrEmpty(q))
                {
                    result = BaseSysTemDataBaseManager.RsSearchCityByKeyWords(q);
                }
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), q);
            }
            return result;
        }
    }
}