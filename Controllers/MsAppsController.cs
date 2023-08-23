﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Linq;
using System.Text.Json;

namespace CustomExternalListWebApi.Controllers
{
    [Route("api/apps")]
    public class MsAppsController : Controller
    {
        private readonly OfficeApp[] allApps;

        public MsAppsController()
        {
            string json = System.IO.File.ReadAllText("data.json");
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            allApps = JsonSerializer.Deserialize<OfficeApp[]>(json, options);
        }

        public IActionResult Get()
        {
            return Json(allApps);
        }

        [Authorize]
        [HttpPost]
        [RequiredScope("access_as_user")]
        public IActionResult Post([FromBody] Payload payload)
        {
            switch (payload.Operation)
            {
                case "default":
                    {
                        var requestedItems = allApps.Where(x =>
                        {
                            string idLowerCase = payload.Input.ToLowerInvariant();
                            return x.Id == idLowerCase;
                        }).Select(x => new { x.Id, x.Name, x.Released });
                        return Json(requestedItems);
                    }
                case "search":
                    {
                        var requestedItems = allApps.Where(x =>
                        {
                            string searchLowerCase = payload.Input.ToLowerInvariant();
                            return x.Name.ToLowerInvariant().Contains(searchLowerCase);
                        }).Select(x => new { x.Id, x.Name, x.Released });
                        return Json(requestedItems);
                    }
                case "get":
                    {
                        var requestedItems = allApps.Where(x => payload.Ids.Contains(x.Id));
                        return Json(requestedItems);
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
