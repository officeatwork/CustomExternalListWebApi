using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CustomExternalListWebApi.Services
{
    public class ContactsService
    {

        private readonly Dictionary<string, string>[] allContacts;

        public ContactsService()
        {
            string json = System.IO.File.ReadAllText("data.json");
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            this.allContacts = JsonSerializer.Deserialize<Dictionary<string, string>[]>(json, options);
        }

        public Dictionary<string, string>[] GetAllContacts()
        {
            return this.allContacts;
        }

        public async Task<IEnumerable<dynamic>> HandleCustomExternalListRequest(Stream body)
        {
            using StreamReader reader = new(body, Encoding.UTF8);
            string json = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("empty_request");
            }

            var payload = JsonSerializer.Deserialize<Dictionary<string, JsonValue>>(json);

            var operation = payload["operation"].Deserialize<string>();

            switch (operation)
            {
                case "default":
                    {
                        var input = payload["input"].Deserialize<string>();
                        string idLowerCase = input.ToLowerInvariant();

                        var requestedItems = allContacts.Where(x => x["id"] == idLowerCase);
                        return requestedItems;
                    }
                case "search":
                    {
                        var input = payload["input"].Deserialize<string>();
                        string searchLowerCase = input.ToLowerInvariant();

                        var requestedItems = allContacts
                            .Where(x => x["displayName"].ToLowerInvariant().Contains(searchLowerCase))
                            .Select(x => x
                                .Where(kvp => kvp.Key == "id" || kvp.Key.StartsWith("displayName") || kvp.Key.StartsWith("city"))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                            );

                        return requestedItems;
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
                            .Select(x => x
                                .Where(kvp => kvp.Key == "id" || kvp.Key.StartsWith("displayName") || kvp.Key.StartsWith("city"))
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                            );
                        return requestedItems;
                    }
                case "get":
                    {
                        var ids = payload["ids"].Deserialize<string[]>();
                        var requestedItems = allContacts.Where(x => ids.Contains(x["id"]));
                        return requestedItems;
                    }
                default:
                    {
                        dynamic error = new { error = "unknown_operation" };
                        throw error;
                    }
            }
        }
    }
}