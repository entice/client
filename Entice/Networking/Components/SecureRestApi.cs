using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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

                public static LoginResult Login(string email, string password, out SecureRestApi api)
                {
                        const string ROUTE = "/api/login";

                        var result = LoginResult.Error;
                        api = null;

                        IEnumerable<Cookie> cookies;
                        JObject content;
                        if (Http.Post(ROUTE, new[]
                                {
                                        new KeyValuePair<string, string>("email", email),
                                        new KeyValuePair<string, string>("password", password),
                                        new KeyValuePair<string, string>("client_version", "MS11")
                                }, out cookies, out content))
                        {
                                if (content.GetValue("message").ToString().Equals("Invalid Client Version"))
                                {
                                        result = LoginResult.InvalidClientVersion;
                                }
                                else if (content.GetValue("status").ToString().Equals("ok"))
                                {
                                        api = new SecureRestApi(cookies.First(c => c.Name.Equals("entice_session")));
                                        result = LoginResult.Success;
                                }
                        }

                        return result;
                }

                public bool CreateCharacter(string name, PlayerAppearance apperance)
                {
                        const string ROUTE = "/api/char";

                        return Http.Post(ROUTE, new[]
                                {
                                        new KeyValuePair<string, string>("char_name", name),
                                        new KeyValuePair<string, string>("profession", apperance.Profession.ToString()),
                                        new KeyValuePair<string, string>("campaign", apperance.Campaign.ToString()),
                                        new KeyValuePair<string, string>("sex", apperance.Sex.ToString()),
                                        new KeyValuePair<string, string>("height", apperance.Height.ToString()),
                                        new KeyValuePair<string, string>("skin_color", apperance.SkinColor.ToString()),
                                        new KeyValuePair<string, string>("hair_color", apperance.HairColor.ToString()),
                                        new KeyValuePair<string, string>("hairstyle", apperance.Hairstyle.ToString()),
                                        new KeyValuePair<string, string>("face", apperance.Face.ToString()),
                                }, _cookie);
                }

                public void Logout()
                {
                        const string ROUTE = "/api/logout";

                        Http.Post(ROUTE, new List<KeyValuePair<string, string>>(), _cookie);
                }

                public enum LoginResult
                {
                        Error,
                        InvalidClientVersion,
                        Success
                }

                private bool GetToken(string route, IEnumerable<KeyValuePair<string, string>> parameters, out AccessCredentials accessCredentials)
                {
                        accessCredentials = null;

                        JObject response;
                        if (Http.Get(route, parameters, out response, _cookie))
                        {
                                if (response.GetValue("status").ToString().Equals("ok"))
                                {
                                        accessCredentials = new AccessCredentials
                                                {
                                                        Area = (Area) Enum.Parse(typeof (Area), response.GetValue("map").ToString()),
                                                        ClientId = response.GetValue("client_id").ToString(),
                                                        EntityId = Guid.Parse(response.GetValue("entity_id").ToString()),
                                                        EntityToken = response.GetValue("entity_token").ToString(),
                                                        IsOutpost = response.GetValue("is_outpost").Value<bool>()
                                                };

                                        return true;
                                }
                        }

                        return false;
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

                public bool GetFriends(out IEnumerable<KeyValuePair<KeyValuePair<string, string>, bool>> friends)
                {
                        const string ROUTE = "/api/friend";

                        JObject response;
                        if (Http.Get(ROUTE, null, out response, _cookie))
                                friends = response.GetValue("friends").Select(
                                        c => new KeyValuePair<KeyValuePair<string, string>, bool>(
                                                     new KeyValuePair<string, string>(
                                                     c.Value<string>("base_name"),
                                                     c.Value<string>("current_name")),
                                                     c.Value<string>("status").Equals("online")));

                        else friends = null;

                        return friends != null;
                }

                public bool AddFriend(string name)
                {
                        const string ROUTE = "/api/friend";

                        return Http.Post(ROUTE, new[] {new KeyValuePair<string, string>("char_name", name)}, _cookie);
                }

                public bool RemoveFriend(string name)
                {
                        const string ROUTE = "/api/friend";

                        return Http.Delete(ROUTE, new[] { new KeyValuePair<string, string>("char_name", name) }, _cookie);
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