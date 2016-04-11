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
    /// 艺龙酒店接口相关
    /// </summary>
    public class ApiElongHotelController : ApiController
    {
        /// <summary>
        /// 获取艺龙酒店可选主题列表
        /// 在查询接口一次查询最多传入3个主题，用,隔开
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Combobox> GetHotelThemes() {
            List<Combobox> result = new List<Combobox>() {
                new Combobox() {
                    id="",
                    text = "不限"
                },
                new Combobox() {
                    id = "97",
                    text = "客栈"
                },
                new Combobox() {
                    id = "98",
                    text = "家庭旅馆"
                },
                new Combobox() {
                    id = "99",
                    text = "青年旅舍"
                },
                new Combobox() {
                    id = "100",
                    text = "精品酒店"
                },
                new Combobox() {
                    id = "101",
                    text = "情侣酒店"
                },
                new Combobox() {
                    id = "105",
                    text = "园林庭院"
                },
                new Combobox() {
                    id = "103",
                    text = "海景"
                },
                new Combobox() {
                    id = "106",
                    text = "农家乐"
                },
                new Combobox() {
                    id = "102",
                    text = "温泉酒店"
                },
                new Combobox() {
                    id = "104",
                    text = "高尔夫酒店"
                },
                new Combobox() {
                    id = "107",
                    text = "四合院"
                },
                new Combobox() {
                    id = "259",
                    text = "别墅"
                },
                new Combobox() {
                    id = "260",
                    text = "聚会做饭"
                },
                new Combobox() {
                    id = "261",
                    text = "商旅之家"
                },
                new Combobox() {
                    id = "262",
                    text = "休闲情调"
                },
                new Combobox() {
                    id = "265",
                    text = "看病就医"
                },
                new Combobox() {
                    id = "264",
                    text = "度假休闲"
                },
                new Combobox() {
                    id = "266",
                    text = "培训学习"
                },
                new Combobox() {
                    id = "267",
                    text = "聚会"
                },
                new Combobox() {
                    id = "268",
                    text = "蜜月出行"
                }
            };
            return result;
        }

        /// <summary>
        /// 获取艺龙酒店可选价格返回列表
        /// </summary>
        /// <returns>返回值的价格区间使用,隔开，例如0,200 代表从0到200</returns>
        [HttpGet]
        public List<Combobox> GetHotelPriceList() {
            List<Combobox> result = new List<Combobox>() {
                new Combobox() {
                    id="0,0",
                    text = "不限"
                },
                new Combobox() {
                    id="0,150",
                    text = "￥150以下"
                },
                new Combobox() {
                    id="151,300",
                    text = "￥151-300"
                },
                new Combobox() {
                    id="301,450",
                    text = "￥301-450"
                },
                new Combobox() {
                    id="451,600",
                    text = "￥451-600"
                },
                new Combobox() {
                    id="601,1000",
                    text = "￥601-1000"
                },
                new Combobox() {
                    id="1001,20000",
                    text = "￥1000以上"
                }
            };
            return result;
        }

        /// <summary>
        /// 获取可选热门酒店品牌
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Combobox> GetHotelHotBrandList() {
            List<Combobox> result = ElongHotelService.GetHotelHotBrandList();
            return result;
        }

        /// <summary>
        /// 根据传入的市ID和关键字检索酒店可选项，返回的结果有匹配的品牌、行政区、商圈等信息
        /// </summary>
        /// <param name="Id">市ID，必填</param>
        /// <param name="q">关键字，必填</param>
        /// <returns></returns>
        [HttpGet]
        public List<HotelSearchItemModel> SearchHotelSItemList(string Id, string q = null)
        {
            List<HotelSearchItemModel> result = ElongHotelService.SearchHotelSItemList(Id, q);
            return result;
        }

        /// <summary>
        /// 酒店查询（按城市，或当前GPS坐标点查询酒店列表）
        /// </summary>
        /// <param name="condtion">无论哪种查询方式，入住时间、离店时间必填。CityId和Position对象二选一必填一项。</param>
        /// <returns></returns>
        [HttpPost]
        public SearchHotelListReponseModel SearchHotelList(SearchHotelListRequestModel condtion) {
            ElongHotelService ehs = new ElongHotelService();
            SearchHotelListReponseModel result = ehs.SearchHotelList(condtion);
            return result;
        }
        /// <summary>
        /// 获取酒店详情
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public GetElongHotelInfoReponseModel GetHotelInfo(GetElongHotelInfoRequestModel condtion) {
            ElongHotelService ehs = new ElongHotelService();
            GetElongHotelInfoReponseModel result = ehs.GetHotelInfo(condtion);
            return result;
        }

        /// <summary>
        /// 预订前的验证，可通过此方法检查当前订单是否可以预订，以及参数是否提交正确
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public ElongHotelValidateCreateOrderReplayModel ValidateCreateOrder(ElongHotelValidateCreateOrderRequestModel condtion)
        {
            ElongHotelService ehs = new ElongHotelService();
            ElongHotelValidateCreateOrderReplayModel result = ehs.ValidateCreateOrder(condtion);
            return result;
        }


        /// <summary>
        /// 艺龙酒店预订提交
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase CreateElongHotelOrder(UserCreateElongHotelOrderRequestModel condtion) {
            return new ReplayBase();
        }
    }
}
