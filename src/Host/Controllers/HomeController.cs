using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using Hubs;

namespace Host.Controllers
{
    public class HomeController : HubController<ValuesHub>
    {
        public ActionResult Index()
        {
            return View();
        }

        public void Post(string value)
        {
            Hub.Clients.add(value);
        }

        public void PostApi(string value)
        {
            var baseUri = new Uri(ConfigurationManager.AppSettings["api:baseUri"]);
            var uri = new Uri(baseUri, string.Format("/api/values?value={0}", value));

            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            using(var client = new HttpClient())
            using (var response = client.SendAsync(message).Result)
            {
                Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}