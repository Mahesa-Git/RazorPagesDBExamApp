using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace RazorPagesDBExam.Pages
{
    public class UteModel : PageModel
    {
        [BindProperty(SupportsGet = true)] public string Id { get; set; }
        [BindProperty] public string Input { get; set; }
        public DateTime dt = new DateTime();
        public List<string> ReturnData { get; set; }
        public string returnDataString { get; set; }
        public void OnGet()
        {
            switch (Id)
            {
                case "1":
                    ReturnData = AppMethods.HotToColdestDayAllEntries("Ute");
                    break;
                case "2":
                    ReturnData = AppMethods.HumidityAvgPerDayAllEntries("Ute");
                    break;
                case "3":
                    ReturnData = AppMethods.MoldRisk("Ute");
                    break;
                case "4":
                    returnDataString = AppMethods.MeteorologicalFallAndWinter(true);
                    break;
                case "5":
                    returnDataString = AppMethods.MeteorologicalFallAndWinter(false);
                    break;
                default:
                    break;
            }
        }
        public void OnPost()
        {
            dt = DateTime.Parse(Input);
            returnDataString = AppMethods.AverageTempPerDay(dt, "Ute");
        }
    }
}
