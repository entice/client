using System.Collections.Generic;
using System.Linq;
using System.Net;
using Entice.Definitions;
using Entice.Networking.Base;
using Newtonsoft.Json.Linq;

namespace Entice.Networking.Components
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

                public bool GetCharacters(out IEnumerable<string> characters)
                {
                        const string ROUTE = "/api/char";

                        JObject response;
                        characters = Http.Get(ROUTE, null, out response, _cookie) ? response.GetValue("characters").Select(c => c.Value<string>("name")) : null;

                        return characters != null;
                }
        }
}