using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeatherStation.Model;
using WeatherStation.Web.Models;

namespace WeatherStation.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            WeatherContext context = new WeatherContext();

            ViewData["LatestData"] = context.Weathers.OrderByDescending(x => x.DateTime)
                .First();

            #region Select 6 Hour Data
            List<Weather> hoursData = new List<Weather>();

            var groups = context.Weathers.GroupBy(x => x.DateTime.Date)
                .OrderByDescending(x => x.Key)
                .Take(2)
                .Select(g => g.Select(x => x).ToList()).ToList();

            var today = groups[0].GroupBy(x => x.DateTime.Hour)
                .OrderByDescending(x => x.Key)
                .Select(g => g.Select(x => x).ToList()).ToList();

            int count = today.Count;
            if (count >= 6)
            {
                var temp = today.Take(6);

                foreach (var item in temp)
                {
                    hoursData.Add(item[0]);
                }
            }
            else
            {
                var yesterday = groups[1].GroupBy(x => x.DateTime.Hour)
                    .OrderByDescending(x => x.Key)
                    .Select(g => g.Select(x => x).ToList()).ToList();

                var temp = yesterday.Take(6 - count);

                foreach (var item in today)
                {
                    hoursData.Add(item[0]);
                }

                foreach (var item in temp)
                {
                    hoursData.Add(item[0]);
                }
            }

            hoursData.Reverse();

            ViewData["HoursData"] = hoursData;
            #endregion

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
