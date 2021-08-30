using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Linq;

namespace CustomExternalListWebApi.Controllers
{
    [Authorize]
    [Route("api/apps")]
    [RequiredScope("access_as_user")]
    public class MsAppsController : Controller
    {
        private static readonly OfficeApp officeApp = new OfficeApp { Id = "office", Name = "Office", Released = "19901119", Color = "#d83b01" };

        private readonly OfficeApp[] allApps = new[] {
            new OfficeApp { Id = "access", Name = "Access", Released = "19921101", Color = "#a4373a" },
            new OfficeApp { Id = "exchange", Name = "Exchange", Released = "19960411", Color = "#0078d4" },
            new OfficeApp { Id = "excel", Name = "Excel", Released = "19870101", Color = "#217346" },
            officeApp,
            new OfficeApp { Id = "onedrive", Name = "OneDrive", Released = "20070801", Color = "#0078d4" },
            new OfficeApp { Id = "onenote", Name = "OneNote", Released = "20180801", Color = "#7719aa" },
            new OfficeApp { Id = "planner", Name = "Planner", Released = "20160606", Color = "#31752f" },
            new OfficeApp { Id = "powerapps", Name = "PowerApps", Released = "20151130", Color = "#742774" },
            new OfficeApp { Id = "powerpoint", Name = "PowerPoint", Released = "19900522", Color = "#b7472a" },
            new OfficeApp { Id = "publisher", Name = "Publisher", Released = "19910101", Color = "#077568" },
            new OfficeApp { Id = "sharepoint", Name = "SharePoint", Released = "20010328", Color = "#0078d4" },
            new OfficeApp { Id = "skype", Name = "Skype", Released = "20030829", Color = "#0078d4" },
            new OfficeApp { Id = "sway", Name = "Sway", Released = "20140101", Color = "#008272" },
            new OfficeApp { Id = "teams", Name = "Teams", Released = "20170314", Color = "#6264a7" },
            new OfficeApp { Id = "visio", Name = "Visio", Released = "19920101", Color = "#3955a3" },
            new OfficeApp { Id = "word", Name = "Word", Released = "19831025", Color = "#2b579a" },
            new OfficeApp { Id = "yammer", Name = "Yammer", Released = "20080908", Color = "#106ebe" }
        };

        [HttpPost]
        public IActionResult Post([FromBody] Payload payload)
        {
            switch (payload.Operation)
            {
                case "default":
                    {
                        return Json(new[] { officeApp });
                    }
                case "search":
                    {
                        var requestedItems = allApps.Where(x =>
                        {
                            string searchLowerCase = payload.Input.ToLowerInvariant();
                            return x.Name.ToLowerInvariant().Contains(searchLowerCase);
                        }).Select(x => new { x.Id, x.Name, x.Released });
                        return this.Json(requestedItems);
                    }
                case "get":
                    {
                        var requestedItems = allApps.Where(x => payload.Ids.Contains(x.Id));
                        return this.Json(requestedItems);
                    }
                default:
                    {
                        dynamic error = new { error = "unknown_operation" };
                        return BadRequest(error);
                    }
            }
        }
    }
}
