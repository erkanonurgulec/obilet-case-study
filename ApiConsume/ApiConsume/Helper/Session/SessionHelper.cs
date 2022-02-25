using ApiConsume.Resources.Consts;
using ApiConsume.Resources.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsume.Helper.Session
{
    public static class SessionHelper
    {
        /// <summary>
        /// Set user session info
        /// </summary>
        /// <param name="contentData"></param>
        /// <returns></returns>
        public static async Task<SessionInfo> SetUserSessionInfo(string contentData)
        {
            SessionInfo sessionInfo = null;

            using (var httpClient = new HttpClient())
            {
                var authheader = new AuthenticationHeaderValue("Basic", GlobalConsts._apiToken);
                httpClient.DefaultRequestHeaders.Authorization = authheader;
                StringContent content = new StringContent(contentData, Encoding.Default, "application/json");

                using (var data = await httpClient.PostAsync(GlobalConsts._getSessionUrl, content))
                {
                    var result = await data.Content.ReadAsStringAsync();

                    var resultdata = (JObject)JsonConvert.DeserializeObject(result);

                    if (resultdata["data"].HasValues && resultdata["status"].Value<string>() == ResponseStatus.Success.ToString()) 
                    {
                        sessionInfo = new SessionInfo
                        {
                            SessionId = resultdata["data"]["session-id"].Value<string>(),
                            DeviceId = resultdata["data"]["device-id"].Value<string>()
                        };
                    }
                }
            }

            return sessionInfo;
        }
    }
}
