using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Entice.Base;
using Entice.Definitions;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Datastructures.Agents.Components;
using Newtonsoft.Json.Linq;

namespace Entice.Components
{
        internal class SecureRestApi
        {
                private readonly Cookie _cookie;

                private SecureRestApi(Cookie cookie)
                {
                        _cookie = cookie;
                }

                public static bool Login(string email, string password, out SecureRestApi api)
                {
                        const string ROUTE = "/api/login";

                        var response = Http.Post(ROUTE, new[]
                                {
                                        new KeyValuePair<string, string>("email", email),
                                        new KeyValuePair<string, string>("password", password)
                                });

                        api = new SecureRestApi(response.Value.FirstOrDefault(c => c.Name.Equals("entice_session")));

                        return api._cookie != null;
                }

                public bool CreateCharacter(string name, PlayerAppearance apperance)
                {
                        const string ROUTE = "/api/char";

                        return Http.Post(ROUTE, new[]
                                {
                                        new KeyValuePair<string, string>("name", name),
                                        new KeyValuePair<string, string>("profession", apperance.Profession.ToString()),
                                        new KeyValuePair<string, string>("campaign", apperance.Campaign.ToString()),
                                        new KeyValuePair<string, string>("sex", apperance.Sex.ToString()),
                                        new KeyValuePair<string, string>("height", apperance.Height.ToString()),
                                        new KeyValuePair<string, string>("skin_color", apperance.SkinColor.ToString()),
                                        new KeyValuePair<string, string>("hair_color", apperance.HairColor.ToString()),
                                        new KeyValuePair<string, string>("hairstyle", apperance.Hairstyle.ToString()),
                                        new KeyValuePair<string, string>("face", apperance.Face.ToString()),
                                }, _cookie).Key;
                }

                public void Logout()
                {
                        const string ROUTE = "/api/logout";

                        Http.Post(ROUTE, new List<KeyValuePair<string, string>>(), _cookie);
                }

                private bool GetToken(string route, IEnumerable<KeyValuePair<string, string>> parameters, out AccessCredentials accessCredentials)
                {
                        JObject response;
                        if (Http.Get(route, parameters, out response, _cookie))
                        {
                                accessCredentials = new AccessCredentials
                                        {
                                                Area = (Area) Enum.Parse(typeof (Area), response.GetValue("map").ToString()),
                                                ClientId = response.GetValue("client_id").ToString(),
                                                EntityId = Guid.Parse(response.GetValue("entity_id").ToString()),
                                                EntityToken = response.GetValue("entity_token").ToString(),
                                                IsOutpost = response.GetValue("is_outpost").Value<bool>()
                                        };
                        }
                        else
                        {
                                accessCredentials = null;
                        }

                        return accessCredentials != null;
                }

                public bool RequestAccessToken(Area area, string character, out AccessCredentials accessCredentials)
                {
                        const string ROUTE = "/api/token/entity";

                        return GetToken(ROUTE, new[]
                                {
                                        new KeyValuePair<string, string>("map", area.ToString()),
                                        new KeyValuePair<string, string>("char_name", character)
                                },
                                        out accessCredentials);
                }

                public bool GetCharacters(out IEnumerable<PlayerCharacter> characters)
                {
                        const string ROUTE = "/api/char";

                        JObject response;
                        if (Http.Get(ROUTE, null, out response, _cookie))
                                characters = response.GetValue("characters").Select(
                                        c => new PlayerCharacter
                                                {
                                                        Name = c.Value<string>("name"),
                                                        Appearance = new PlayerAppearance(c.Value<uint>("sex"),
                                                                                          c.Value<uint>("height"),
                                                                                          c.Value<uint>("skin_color"),
                                                                                          c.Value<uint>("hair_color"),
                                                                                          c.Value<uint>("face"),
                                                                                          c.Value<uint>("profession"),
                                                                                          c.Value<uint>("hairstyle"),
                                                                                          c.Value<uint>("campaign"))
                                                });

                        else characters = null;

                        return characters != null;
                }

                public class AccessCredentials
                {
                        public string ClientId { get; set; }
                        public string EntityToken { get; set; }
                        public Guid EntityId { get; set; }
                        public Area Area { get; set; }
                        public bool IsOutpost { get; set; }
                }
        }
}