using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ApiConsume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wangkanai.Detection.Services;
using ApiConsume.Helper.Session;
using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApiConsume.Resources.Enums;
using ApiConsume.Resources.Consts;

namespace APIConsume.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDetectionService _detectionService;
        private readonly string _sessionKey = "ApiSessionKey";
        private readonly string _searchSessionKey = "SearchSessionKey";
        private readonly AuthenticationHeaderValue _authheader;
        private SessionInfo sessionInfo = null;
        private SearchModel searchSessionInfo = null;


        public HomeController(IDetectionService detectionService)
        {
            _detectionService = detectionService;
            _authheader = new AuthenticationHeaderValue("Basic", GlobalConsts._apiToken);
        }

        public async Task<IActionResult> Index()
        {
            IndexModel model = new IndexModel();

            sessionInfo = await SessionAction(sessionInfo);

            #region Get All Bus Locations

            var reqBody = new
            {
                DeviceSession = new
                {
                    SessionId = sessionInfo.SessionId,
                    DeviceId = sessionInfo.DeviceId
                },
                Date = DateTime.Now.ToString("s"),
                Language = Thread.CurrentThread.CurrentCulture.Name
            };

            var jsonContentData = JsonConvert.SerializeObject(reqBody)
                .Replace("DeviceSession", "device-session")
                .Replace("SessionId", "session-id")
                .Replace("DeviceId", "device-id");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = _authheader;
                StringContent content = new StringContent(jsonContentData, Encoding.Default, "application/json");

                using (var data = await httpClient.PostAsync(GlobalConsts._getBusLocationsUrl, content))
                {
                    var result = await data.Content.ReadAsStringAsync();

                    var resultdata = (JObject)JsonConvert.DeserializeObject(result);

                    if (resultdata["data"].HasValues && resultdata["status"].Value<string>() == ResponseStatus.Success.ToString())
                    {
                        bool isFirst = true;
                        foreach (var item in resultdata["data"])
                        {
                            var value = item["id"].Value<string>();
                            var text = item["name"].Value<string>();
                            var selected = isFirst ? true : false;
                            isFirst = false;

                            model.Origins.Add(new SelectListItem { Value = value, Text = text, Selected = selected });
                            model.Destinations.Add(new SelectListItem { Value = value, Text = text, Selected = selected });
                        }
                    }
                }
            }

            #endregion

            return View(model);
        }

        [Route("/Home/ValidateIndex")]
        [HttpPost]
        public async Task<JsonResult> ValidateIndex(SearchModel model)
        {
            if (model.DestinationId == model.OriginId)
            {
                return Json(new { result = "NOTOK", message = "Varış ve kalkış şehirleri aynı olamaz." });
            }

            sessionInfo = await SessionAction(sessionInfo);

            HttpContext.Session.Set<SearchModel>(_searchSessionKey, model);

            return Json(new { result = "OK"});
        }

        [Route("/Home/JourneyIndex")]
        public async Task<IActionResult> JourneyIndex()
        {
            List<JourneyModel> model = new List<JourneyModel>();

            sessionInfo = await SessionAction(sessionInfo);

            searchSessionInfo = HttpContext.Session.Get<SearchModel>(_searchSessionKey);

            var reqBody = new
            {
                DeviceSession = new
                {
                    SessionId = sessionInfo.SessionId,
                    DeviceId = sessionInfo.DeviceId
                },
                Date = DateTime.Now.ToString("s"),
                Language = Thread.CurrentThread.CurrentCulture.Name,
                Data = new
                {
                    OriginId= searchSessionInfo.OriginId,
                    DestinationId = searchSessionInfo.DestinationId,
                    DepartureDate = searchSessionInfo.DepartureDate.ToString("s")
                }
            };

            var jsonContentData = JsonConvert.SerializeObject(reqBody)
                .Replace("DeviceSession", "device-session")
                .Replace("SessionId", "session-id")
                .Replace("DeviceId", "device-id")
                .Replace("OriginId", "origin-id")
                .Replace("DestinationId", "destination-id")
                .Replace("DepartureDate", "departure-date")
                ;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = _authheader;
                StringContent content = new StringContent(jsonContentData, Encoding.Default, "application/json");

                using (var data = await httpClient.PostAsync(GlobalConsts._getBusJourneysUrl, content))
                {
                    var result = await data.Content.ReadAsStringAsync();

                    var resultdata = (JObject)JsonConvert.DeserializeObject(result);

                    if (resultdata["data"].HasValues && resultdata["status"].Value<string>() == "Success")
                    {
                        foreach (var item in resultdata["data"].Values("journey"))
                        {
                            var origin = item["origin"].Value<string>();
                            var destination = item["destination"].Value<string>();
                            var departure = item["departure"].Value<DateTime>();
                            var arrival = item["arrival"].Value<DateTime>();
                            var currency = item["currency"].Value<string>();
                            var price = item["internet-price"].Value<string>();

                            model.Add(new JourneyModel { 
                                
                                Origin = origin,
                                Destiniation = destination,
                                Departure = departure.ToString("HH:mm"),
                                Arrival = arrival.ToString("HH:mm"),
                                Currency = currency,
                                Price = price
                            });
                        }
                    }
                }
            }

            ViewBag.OriginName = searchSessionInfo.OriginName;
            ViewBag.DestinationName = searchSessionInfo.DestinationName;
            ViewBag.DepartureDateLongDate = searchSessionInfo.DepartureDate.ToString("D");

            return View(model);
        }

        #region Private Methods

        private async Task<SessionInfo> SessionAction(SessionInfo sessionInfo)
        {
            if (HttpContext.Session.Get<SessionInfo>(_sessionKey) == default)
            {
                #region Platform detection

                var browserName = _detectionService.Browser.Name.ToString();
                var version = _detectionService.Browser.Version.ToString();

                var sessionReqBody = new
                {
                    Type = 1,
                    Connection = new
                    {
                        IpAddress = HttpContext.Connection.LocalIpAddress.ToString(),
                        Port = HttpContext.Connection.LocalPort.ToString()
                    },
                    Browser = new
                    {
                        Name = browserName,
                        Version = version
                    }
                };

                var jsonSessionContentData = JsonConvert.SerializeObject(sessionReqBody).Replace("IpAddress", "ip-address");

                #endregion

                #region Set-Get Session And Device Id

                sessionInfo = await SessionHelper.SetUserSessionInfo(jsonSessionContentData);

                HttpContext.Session.Set<SessionInfo>(_sessionKey, sessionInfo);

                #endregion
            }
            else
            {
                sessionInfo = HttpContext.Session.Get<SessionInfo>(_sessionKey);
            }

            return sessionInfo;
        }

        #endregion
    }
}