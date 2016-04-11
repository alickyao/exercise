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
    /// 航旅机票查询
    /// </summary>
    public class ApiFlightSearchController : ApiController
    {
        /// <summary>
        /// 航班查询
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchFlightInfoListReplayModel SearchFlightList(SearchFlightInfoListRequestModel condtion) {
            Flight51BookInterFaceService fb = new Flight51BookInterFaceService();
            SearchFlightInfoListReplayModel result = fb.SearchFlight(condtion);
            return result;
        }
    }
}
