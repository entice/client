using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Entice.Base;
using Entice.Definitions;
using GuildWarsInterface.Datastructures.Agents;
using GuildWarsInterface.Datastructures.Agents.Components;
using GuildWarsInterface.Declarations;
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

                public static bool Login(string email, string password, out SecureRestApi api, out EnticeWebsocket websocket)
                {
                        const string ROUTE = "/api/login";

                        api = new SecureRestApi(Http.Post(ROUTE, new[]
                                {
                                        new KeyValuePair<string, string>("email", email),
                                        new KeyValuePair<string, string>("password", password)
                                }).FirstOrDefault(c => c.Name.Equals("entice_session")));

                        websocket = new EnticeWebsocket(api._cookie);

                        return api._cookie != null;
                }

                public void Logout()
                {
                        const string ROUTE = "/api/logout";

                        Http.Post(ROUTE, null, _cookie);
                }

                private bool GetToken(string route, IEnumerable<KeyValuePair<string, string>> parameters, out string transferToken, out string clientId)
                {
                        JObject response;
                        if (Http.Get(route, parameters, out response, _cookie))
                        {
                                transferToken = response.GetValue("transfer_token").ToString();
                                clientId = response.GetValue("client_id").ToString();
                        }
                        else
                        {
                                transferToken = null;
                                clientId = null;
                        }

                        return transferToken != clientId;
                }

                public bool RequestTransferToken(Area area, string character, out string transferToken, out string clientId)
                {
                        const string ROUTE = "/api/token/area";

                        return GetToken(ROUTE, new[]
                                {
                                        new KeyValuePair<string, string>("map", area.ToString()),
                                        new KeyValuePair<string, string>("char_name", character)
                                },
                                        out transferToken, out clientId);
                }

                public bool JoinSocial(string room, string character)
                {
                        const string ROUTE = "/api/token/social";

                        string transferToken, clientId;
                        if (!GetToken(ROUTE, new[]
                                {
                                        new KeyValuePair<string, string>("room", room),
                                        new KeyValuePair<string, string>("char_name", character)
                                },
                                      out transferToken, out clientId)) return false;

                        Networking.Social.Join(transferToken, clientId);

                        return true;
                }

                public bool GetCharacters(out IEnumerable<KeyValuePair<PlayerCharacter, IEnumerable<Skill>>> characters)
                {
                        const string ROUTE = "/api/char";

                        JObject response;
                        if (Http.Get(ROUTE, null, out response, _cookie))
                                characters = response.GetValue("characters").Select(
                                        c =>
                                                {
                                                        var character = new PlayerCharacter
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
                                                                };

                                                        var hSkills = c.Value<string>("available_skills");
                                                        byte[] avSkills = Enumerable.Range(0, hSkills.Length)
                                                                                    .Select(x => Convert.ToByte(hSkills.Substring(x, 1), 16))
                                                                                    .Reverse().ToArray();

                                                        var availableSkills = new List<Skill>();

                                                        for (int i = 0; i < avSkills.Length; i++)
                                                        {
                                                                for (int j = 0; j < 4; j++)
                                                                {
                                                                        if (((1 << j) & avSkills[i]) > 0)
                                                                        {
                                                                                availableSkills.Add((Skill) (4 * i + j + 1));
                                                                        }
                                                                }
                                                        }

                                                        return new KeyValuePair<PlayerCharacter, IEnumerable<Skill>>(character, availableSkills);
                                                });

                        else characters = null;

                        return characters != null;
                }
        }
}