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
    /// 资源文档模块（协议、帮助、更新内容等）
    /// </summary>
    public class ApiDocumentResourceController : ApiController
    {
        /// <summary>
        /// 新增/编辑资源文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase EditDocument(DocumentsResourceModel document) {
            DocumentResourceService drs = new DocumentResourceService();
            drs.Document = document;
            ReplayBase result = drs.Save();
            return result;
        }

        /// <summary>
        /// 查询获取文档信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchDocumentsResourceListReplayModel SearchDocument(SearchDocumentsResourceListRequestModel condtion) {
            SearchDocumentsResourceListReplayModel result = DocumentResourceService.SearchDocument(condtion);
            return result;
        }

        /// <summary>
        /// 文档批量排序及禁用
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase SortDocuments(SortDocumentsRequest condtion) {
            ReplayBase result = DocumentResourceService.SortDocuments(condtion);
            return result;
        }

        /// <summary>
        /// 获取所有现有的分组信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Combobox> GetAllDocumentsGroupInfo() {
            List<Combobox> result = DocumentResourceService.GetAllDocumentsGroupInfo();
            return result;
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="Id">文档ID</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase DelDocument(string Id) {
            ReplayBase result = DocumentResourceService.DelDocument(Id);
            return result;
        }
    }
}
