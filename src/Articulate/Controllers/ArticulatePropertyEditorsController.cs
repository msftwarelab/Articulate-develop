using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbraco.Core.IO;
using Umbraco.Web.WebApi;

namespace Articulate.Controllers
{
    public class ArticulatePropertyEditorsController : UmbracoAuthorizedApiController
    {
        public IEnumerable<string> GetThemes()
        {
            var dir = IOHelper.MapPath(PathHelper.VirtualThemePath);
            return Directory.GetDirectories(dir).Select(x => new DirectoryInfo(x).Name);
        }
    }
}