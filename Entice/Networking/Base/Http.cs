using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Entice.Networking.Base
{
        internal static class Http
        {
                public static IEnumerable<Cookie> Post(string route, IEnumerable<KeyValuePair<string, string>> parameters, Cookie cookie = null)
                {
                        return PostAsync(route, parameters, cookie).Result;
                }

                private static async Task<IEnumerable<Cookie>> PostAsync(string route, IEnumerable<KeyValuePair<string, string>> parameters, Cookie cookie = null)
                {
                        using (var handler = new HttpClientHandler())
                        {
                                if (cookie != null) handler.CookieContainer.Add(cookie);

                                using (var client = new HttpClient(handler))
                                {
                                        await client.PostAsync(FormUri(route, parameters), new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()));

                                        return handler.CookieContainer.GetCookies(new Uri(FormUri(route))).Cast<Cookie>();
                                }
                        }
                }

                public static bool Get(string route, IEnumerable<KeyValuePair<string, string>> parameters, out JObject result, Cookie cookie = null)
                {
                        KeyValuePair<bool, string> get = GetAsync(route, parameters, cookie).Result;

                        result = get.Key ? JObject.Parse(get.Value) : null;
                        return get.Key;
                }

                private static async Task<KeyValuePair<bool, string>> GetAsync(string route, IEnumerable<KeyValuePair<string, string>> parameters, Cookie cookie = null)
                {
                        using (var handler = new HttpClientHandler())
                        {
                                if (cookie != null) handler.CookieContainer.Add(cookie);

                                using (var client = new HttpClient(handler))
                                {
                                        HttpResponseMessage response = await client.GetAsync(FormUri(route, parameters));

                                        return new KeyValuePair<bool, string>(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
                                }
                        }
                }

                private static string FormUri(string route, IEnumerable<KeyValuePair<string, string>> parameters = null)
                {
                        const string BASE_URI = "http://entice-web.herokuapp.com";

                        string[] ps = parameters == null ? new string[0] : parameters.Select(p => string.Format("{0}={1}", p.Key, p.Value)).ToArray();

                        return BASE_URI + route + (!ps.Any() ? "" : "?" + string.Join("&", ps));
                }
        }
}