using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Articulate.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Articulate.Routing
{
    public class ArticulateTagsRouteHandler : ArticulateVirtualNodeByIdRouteHandler
    {
        private struct UrlAndPageNames
        {
            public int NodeId { get; set; }
            public string TagsUrlName { get; set; }
            public string TagsPageName { get; set; }
            public string CategoriesUrlName { get; set; }
            public string CategoriesPageName { get; set; }
        }

        private readonly List<UrlAndPageNames> _urlsAndPageNames = new List<UrlAndPageNames>();

        /// <summary>
        /// Constructor used to create a new handler for multi-tenency with domains and ids
        /// </summary>
        /// <param name="contentUrls"></param>
        /// <param name="itemsForRoute"></param>
        /// <param name="logger"></param>
        public ArticulateTagsRouteHandler(ILogger logger, ContentUrls contentUrls, IEnumerable<IPublishedContent> itemsForRoute)
            : base(logger, contentUrls, itemsForRoute)
        {
            foreach (var node in itemsForRoute)
            {
                _urlsAndPageNames.Add(new UrlAndPageNames
                {
                    NodeId = node.Id,
                    TagsUrlName = node.Value<string>("tagsUrlName"),
                    TagsPageName = node.Value<string>("tagsPageName"),
                    CategoriesUrlName = node.Value<string>("categoriesUrlName"),
                    CategoriesPageName = node.Value<string>("categoriesPageName")
                });
            }
        }        

        protected override IPublishedContent FindContent(RequestContext requestContext, UmbracoContext umbracoContext, IPublishedContent baseContent)
        {
            var urlAndPageName = _urlsAndPageNames.Single(x => x.NodeId == baseContent.Id);

            var tag = HttpUtility.UrlDecode(requestContext.RouteData.Values["tag"] == null ? null : requestContext.RouteData.Values["tag"].ToString());
            var actionName = requestContext.RouteData.GetRequiredString("action");
            var urlName = actionName;
            var pageName = actionName.InvariantEquals(urlAndPageName.TagsUrlName) ? urlAndPageName.TagsPageName : urlAndPageName.CategoriesPageName;

            return new ArticulateVirtualPage(
                baseContent,
                tag.IsNullOrWhiteSpace() ? pageName : tag,
                requestContext.RouteData.GetRequiredString("controller"),
                tag.IsNullOrWhiteSpace()
                    ? urlName
                    : urlName.EnsureEndsWith('/') + tag);
        }
    }

}