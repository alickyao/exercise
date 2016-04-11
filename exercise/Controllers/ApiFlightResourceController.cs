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
    /// 航旅机票资源信息维护
    /// </summary>
    public class ApiFlightResourceController : ApiController
    {
        #region -- 机场
        /// <summary>
        /// 添加/编辑机场信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase EditFlightAirPort(FlightAirPortInfoModel condtion) {
            condtion.modifiedBy = User.Identity.Name;
            ReplayBase result = FlightService.EditFlightAirPort(condtion);
            return result;
        }

        /// <summary>
        /// 删除机场信息（直接从数据中删除）
        /// </summary>
        /// <param name="id">机场ID</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase DelFlightAirPort(string id) {
            FlightService fs = new FlightService();
            ReplayBase result = fs.DelFlightAirPort(id,User.Identity.Name);
            return result;
        }

        /// <summary>
        /// 获取机场信息列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public List<FlightAirPortInfoModel> GetFlightAirPortList(GetFlightAirPortListRequestModel condtion) {
            List<FlightAirPortInfoModel> result = FlightService.GetFlightAirPortList(condtion);
            return result;
        }
        #endregion

        #region -- 航空公司

        /// <summary>
        /// 添加/编辑航空公司
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase EditFlightAirCompany(FlightAirCompanyInfoModel condtion) {
            condtion.modifiedBy = User.Identity.Name;
            ReplayBase result = FlightService.EditFlightAirCompany(condtion);
            return result;
        }

        /// <summary>
        /// 删除航空公司信息（直接从数据中删除）
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase DelFlightAirCompany(string Id) {
            FlightService fs = new FlightService();
            ReplayBase result = fs.DelFlightAirCompany(Id, User.Identity.Name);
            return result;
        }

        /// <summary>
        /// 获取航空公司列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public List<FlightAirCompanyInfoModel> GetFlightAirCompanyList(GetFlightAirCompanyListRequestModel condtion) {
            List<FlightAirCompanyInfoModel> result = FlightService.GetFlightAirCompanyList(condtion);
            return result;
        }

        #endregion
    }
}
