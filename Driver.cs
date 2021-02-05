using EastFive.Extensions;
using EastFive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class Driver
    {   
        public static Task<TResult> DownloadSearchResults<TResult>(string searchString,
            Func<Pin[], TResult> onSuccess,
            Func<TResult> onNotFound = default,
            Func<string, TResult> onCouldNotConnect = default,
            Func<string, TResult> onFailure = default)
        {
            var searchUrl = new Uri($"https://www.pinterest.com/search/pins/?q={searchString}");
            return DownloadAsync(searchUrl,
                jsonContent =>
                {
                    try
                    {
                        var searchResponse = JsonConvert.DeserializeObject(jsonContent);
                        var pins = (searchResponse as dynamic).pins as JArray;
                        var allPins = pins
                            .Select(
                                pin =>
                                {
                                    var pinObject = pin as JObject;
                                    try
                                    {
                                        var pinCast = pinObject.ToObject<Pin>();
                                        return pinCast;
                                    }
                                    catch (JsonReaderException)
                                    {
                                        return default;
                                    }
                                })
                            .SelectWhereNotNull()
                            .ToArray();

                        return onSuccess(allPins);
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        return onCouldNotConnect($"Pinterest returned non-json response:{jsonContent}");
                    }
                },
                onNotFound: onNotFound,
                onCouldNotConnect: onCouldNotConnect,
                onFailure: onFailure);
        }

        public static Task<TResult> DownloadBoard<TResult>(string username, string boardname,
            Func<Pin [], TResult> onSuccess,
            Func<TResult> onNotFound = default,
            Func<string, TResult> onCouldNotConnect =default,
            Func<string, TResult> onFailure = default)
        {
            var boardUrl = new Uri($"https://www.pinterest.com/{username}/{boardname}/");
            return DownloadAsync(boardUrl,
                jsonContent =>
                {
                    try
                    {
                        var boardData = JsonConvert.DeserializeObject<BoardInitialData>(jsonContent);
                        var boardName = boardData.routeData.IsDefaultOrNull() ? string.Empty : boardData.routeData.name;
                        var allPins = boardData.resourceResponses
                            .NullToEmpty()
                            .Select(
                                resourceResponse =>
                                {
                                    if (resourceResponse.name != "BoardFeedResource")
                                        return Enumerable.Empty<Pin>();

                                    var pins = resourceResponse.response.data as JArray;
                                    if (pins.IsDefaultOrNull())
                                        return Enumerable.Empty<Pin>();

                                    return pins
                                        .Select(
                                            pin =>
                                            {
                                                var pinObject = pin as JObject;
                                                try
                                                {
                                                    var pinCast = pinObject.ToObject<Pin>();
                                                    return pinCast;
                                                }
                                                catch (JsonReaderException)
                                                {
                                                    return default;
                                                }
                                            })
                                        .SelectWhereNotNull();
                                })
                            .SelectMany()
                            .ToArray();

                        return onSuccess(allPins);
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        return onCouldNotConnect($"Pinterest returned non-json response:{jsonContent}");
                    }
                },
                onNotFound: onNotFound,
                onCouldNotConnect: onCouldNotConnect,
                onFailure: onFailure);
        }

        public static async Task<TResult> DownloadPin<TResult>(string pinId,
            Func<Pin, TResult> onSuccess,
            Func<TResult> onNotFound = default,
            Func<string, TResult> onCouldNotConnect = default,
            Func<string, TResult> onFailure = default)
        {
            using (var httpClient = new HttpClient())
            {
                var boardUrl = new Uri($"https://www.pinterest.com/pin/{pinId}/");

                var request = new HttpRequestMessage(HttpMethod.Get, boardUrl);
                try
                {
                    var response = await httpClient.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            return onNotFound();
                        return onFailure(content);
                    }

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(content);
                    var scripteDataNode = doc.DocumentNode.SelectSingleNode("//script[@id='initial-state']");
                    var jsonContent = scripteDataNode.InnerText;

                    try
                    {
                        var boardData = JsonConvert.DeserializeObject<BoardInitialData>(jsonContent);
                        var boardName = boardData.routeData.name;
                        return boardData.resourceResponses
                            .Where(resourceResponse => resourceResponse.name == "PinResource")
                            .First(
                                (resourceResponse, next) =>
                                {
                                    var pinObject = resourceResponse.response.data as JObject;
                                    try
                                    {
                                        var pinCast = pinObject.ToObject<Pin>();
                                        return onSuccess(pinCast);
                                    }
                                    catch (JsonReaderException ex)
                                    {
                                        return onFailure(ex.Message);
                                    }
                                },
                                () => onFailure("No pins found"));
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        return onCouldNotConnect($"Pinterest returned non-json response:{content}");
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    return onCouldNotConnect($"{ex.GetType().FullName}:{ex.Message}");
                }
                catch (Exception exGeneral)
                {
                    return onCouldNotConnect(exGeneral.Message);
                }
            }
        }

        public static async Task<TResult> ListBoardsAsync<TResult>(string username,
            Func<UserProfileBoardResource[], TResult> onFound,
            Func<TResult> onNotFound = default,
            Func<string, TResult> onCouldNotConnect = default,
            Func<string, TResult> onFailure = default)
        {
            using (var httpClient = new HttpClient())
            {
                var boardUrl = new Uri($"https://www.pinterest.com/{username}/");

                var request = new HttpRequestMessage(
                    HttpMethod.Get, boardUrl);
                try
                {
                    var response = await httpClient.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            return onNotFound();
                        return onFailure(content);
                    }

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(content);
                    var scripteDataNode = doc.DocumentNode.SelectSingleNode("//script[@id='initial-state']");
                    var jsonContent = scripteDataNode.InnerText;

                    try
                    {
                        var boardData = JsonConvert.DeserializeObject<BoardInitialData>(jsonContent);
                        var allBoards = boardData.resourceResponses
                            .Select(
                                resourceResponse =>
                                {
                                    if (resourceResponse.name != "UserProfileBoardResource")
                                        return Enumerable.Empty<UserProfileBoardResource>();

                                    var boards = resourceResponse.response.data as JArray;
                                    if (boards.IsDefaultOrNull())
                                        return Enumerable.Empty<UserProfileBoardResource>();

                                    return boards
                                        .Select(
                                            board =>
                                            {
                                                var boardObject = board as JObject;
                                                try
                                                {
                                                    var boardCast = boardObject.ToObject<UserProfileBoardResource>();
                                                    return boardCast;
                                                }
                                                catch (JsonReaderException)
                                                {
                                                    return default;
                                                }
                                            })
                                        .SelectWhereNotNull();
                                })
                            .SelectMany()
                            .ToArray();

                        return onFound(allBoards);
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        return onCouldNotConnect($"Pinterest returned non-json response:{content}");
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    return onCouldNotConnect($"{ex.GetType().FullName}:{ex.Message}");
                }
                catch (Exception exGeneral)
                {
                    return onCouldNotConnect(exGeneral.Message);
                }
            }
        }

        public static async Task<TResult> DownloadAsync<TResult>(Uri downloadUrl,
            Func<string, TResult> onSuccess,
            Func<TResult> onNotFound = default,
            Func<string, TResult> onCouldNotConnect = default,
            Func<string, TResult> onFailure = default)
        {
            var baseAddress = new Uri("https://www.pinterest.com");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var httpClient = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                //cookieContainer.Add(baseAddress, new Cookie("csrftoken", "44f533516fd7e8148e3454c34bf65e77"));
                //cookieContainer.Add(baseAddress, new Cookie("_auth", "0"));
                //cookieContainer.Add(baseAddress, new Cookie("_b", "AVOvBsd5g1JCd7LdTlEfLBwSN/HWWgwworfGaKFg+3mPsAlhHwKKH6TZOLitVWx/4Gw="));
                //cookieContainer.Add(baseAddress, new Cookie("bei", "false"));
                //cookieContainer.Add(baseAddress, new Cookie("cm_sub", "denied"));
                //cookieContainer.Add(baseAddress, new Cookie("_pinterest_sess", "TWc9PSY0Z2JJZjJHVHhXZUJqYTVPVXdPZXZkM1gydWRMVlZpYVdQT0g1dWE4MERqUmN4N3B4dzB6aW1rOS9WK0tYaEYxUW1MelhuZ04vRkJmcERWLzZMNURXOGk2R0k3MGtYNUZ4czhibmQvVWpnOUc5MjNiZmt0NlRzbjZwU0tEV2IxdmIwTThXV29aRjB4UFhTcUxCWGpsRnRoNzZaOUxDTTlSQWpBeDJDUWRtRGs2MTVWWlJSQ1dmOWJVNlNmQ3oySmxxdHNBbzBDTkxmVVFPV0FrVXRENGMvQ2hNREtoaDlETWJqY29IZWUrNjF2V3A1SWdBNmZRVHlpZ0tpb0NpUnJpVmlrL0RjWnN4OEtyWFF4eWJ6VlFaWlNZalBKYU9Zd3laeVhLNGdXUTVPcmpRN3FBZkg2Vm1BMml4Uk9QL1JUMTZNdUFpRzhPdktPM1BLWVB1RW1nZWpod0tJZVhPYkVFZGI2S0ZEQTFTaEo4Q2pWUXpMK1dhakRUZXNEamt5TUxQRy9ERTkwdnY3SlBtMGJJaUEwb1puckxEdTN2L2hKKzVNc0I1OG9uSWp4K21pMUZNb216MWRLSFFtazBBbmRHMlFJMGQ2Zk1BQUljRzlKdUtiblU1dHdzQWdmc1ExNnp0S2FCWEJYKzNUSUZCblhtNnN5WCtkK1FhU2RWSmR2QmdtK2tncHdPd21MYk5QSWhHTDN5QXgzWHNwU0tsYzBQU2hWWTNFK29LcGlIcTBQWnVEaUE0Qk5vUUNUbzFDNGo0aEY1NlI3MWFDTU15SjNKaFc4UlRBPT0mSWlkQnRqK2JhdnNzcGRSNlZmVGxvQWhPV3prPQ=="));
                //cookieContainer.Add(baseAddress, new Cookie("_routing_id", "00c2cf6b-2fb6-49e4-aaa0-49b1ca6d5e90"));
                //cookieContainer.Add(baseAddress, new Cookie("sessionFunnelEventLogged", "1"));

                var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36");
                request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                try
                {
                    var response = await httpClient.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            return onNotFound();
                        return onFailure(content);
                    }

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(content);
                    var scripteDataNode = doc.DocumentNode.SelectSingleNode("//script[@id='initial-state']");
                    var jsonContent = scripteDataNode.InnerText;

                    return onSuccess(jsonContent);
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    return onCouldNotConnect($"{ex.GetType().FullName}:{ex.Message}");
                }
                catch (Exception exGeneral)
                {
                    return onCouldNotConnect(exGeneral.Message);
                }
            }
        }
    }
}
