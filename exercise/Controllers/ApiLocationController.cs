using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.Models;
using cyclonestyle.BLL;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 地理位置接口
    /// </summary>
    public class ApiLocationController : ApiController
    {
        /// <summary>
        /// 根据坐标点获取当前所在区域信息
        /// </summary>
        /// <param name="mapPoint">当前位置
        /// {"Lat": "39.888411","Lng": "116.333097"}</param>
        /// <returns>当前所在位置信息，对象中可能会包含当前所在地的CityId，该CityId来自艺龙网提供的全国省市信息表，可用于酒店相关的查询或者其他需要的地方</returns>
        [HttpPost]
        public MyGeoInfo GetLocationInfoByPoint([FromBody]MapPoint mapPoint)
        {
            MyGeoInfo result = LocationService.GetLocationInfoByPoint(mapPoint);
            return result;
        }
        /// <summary>
        /// 获取全国省列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Combobox> GetProvniceList() {
            List<Combobox> result = LocationService.GetProvniceList();
            return result;
        }
        /// <summary>
        /// 根据省ID获取城市列表
        /// </summary>
        /// <param name="Id">省ID</param>
        /// <returns></returns>
        [HttpGet]
        public List<GeoCityInfoModel> GetCityListByProvniceId(string Id) {
            List<GeoCityInfoModel> result = LocationService.GetCityListByProvniceId(Id);
            return result;
        }

        /// <summary>
        /// 根据市ID检索区域信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchAreaInfoListReplayModel GetAreaInfoListByCityCode(SearchAreaInfoListRequestModel condtion) {
            SearchAreaInfoListReplayModel result = LocationService.GetAreaInfoListByCityCode(condtion);
            return result;
        }

        /// <summary>
        /// 获取全国热门城市列表，按省排列，每个省取前3个城市
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<GeoCityInfoModel> GetHotCityList() {
            List<GeoCityInfoModel> result = LocationService.GetHotCityList();
            return result;
        }
        /// <summary>
        /// 根据关键字检索城市列表，最多返回20行结果
        /// </summary>
        /// <param name="q">关键字，可以是城市名称以及拼音或拼音首字母，必填，否则返回空集合</param>
        /// <returns></returns>
        [HttpGet]
        public List<GeoCityInfoModel> SearchCityByKeyWords(string q = null) {
            List<GeoCityInfoModel> result = LocationService.SearchCityByKeyWords(q);
            return result;
        }
    }
}
