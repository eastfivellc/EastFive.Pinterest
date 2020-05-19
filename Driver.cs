using EastFive.Extensions;
using EastFive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class Driver
    {   
        public static async Task<TResult> DownloadBoard<TResult>(string username, string boardname,
            Func<Pin [], TResult> onSuccess,
            Func<TResult> onNotFound = default,
            Func<string, TResult> onCouldNotConnect =default,
            Func<string, TResult> onFailure = default)
        {
            using (var httpClient = new HttpClient())
            {
                var boardUrl = new Uri($"https://www.pinterest.com/{username}/{boardname}/");

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
                        var boardName = boardData.routeData.name;
                        var allPins = boardData.resourceResponses
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
    }
}
