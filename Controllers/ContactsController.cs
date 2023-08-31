using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CustomExternalListWebApi.Controllers
{
    [Route("api/contacts")]
    public class MsAppsController : Controller
    {
        private readonly Dictionary<string, string>[] allContacts;

        public MsAppsController()
        {
            string json = System.IO.File.ReadAllText("data.json");
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            allContacts = JsonSerializer.Deserialize<Dictionary<string, string>[]>(json, options);
        }

        public IActionResult Get()
        {
            return Json(allContacts);
        }

        [Authorize]
        [HttpPost]
        [RequiredScope("access_as_user")]
        public async Task<IActionResult> Post()
        {
            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            string json = await reader.ReadToEndAsync();
            var payload = JsonSerializer.Deserialize<Dictionary<string, JsonValue>>(json);

            var operation = payload["operation"].Deserialize<string>();

            switch (operation)
            {
                case "default":
                    {
                        var input = payload["input"].Deserialize<string>();
                        string idLowerCase = input.ToLowerInvariant();

                        var requestedItems = allContacts.Where(x => x["id"] == idLowerCase);
                        return Json(requestedItems);
                    }
                case "search":
                    {
                        var input = payload["input"].Deserialize<string>();
                        string searchLowerCase = input.ToLowerInvariant();

                        var requestedItems = allContacts
                            .Where(x => x["displayName"].ToLowerInvariant().Contains(searchLowerCase))
                            .Select(x => new 
                            { 
                                ID = x["id"],
                                DisplayName = x["displayName"],
                                City = x["city"]
                            });

                        return Json(requestedItems);
                    }
                case "searchByFields":
                    {
                        var input = payload["input"].Deserialize<Dictionary<string, string>>();
                        var requestedItems = allContacts
                            .Where(x =>
                            {
                                return input.Keys
                                    .All(key => x[key]
                                        .ToLowerInvariant()
                                        .Contains(input[key].ToLowerInvariant()));

                            })
                            .Select(x => new 
                            { 
                                ID = x["id"],
                                DisplayName = x["displayName"],
                                City = x["city"]
                            });
                        return Json(requestedItems);
                    }
                case "get":
                    {
                        var ids = payload["ids"].Deserialize<string[]>();
                        var requestedItems = allContacts.Where(x => ids.Contains(x["id"]));
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
