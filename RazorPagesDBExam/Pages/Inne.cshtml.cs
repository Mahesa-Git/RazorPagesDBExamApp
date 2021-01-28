using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace RazorPagesDBExam.Pages
{
    public class InneModel : PageModel
    {

        [BindProperty(SupportsGet = true)] public string Id { get; set; }
        [BindProperty] public string Input { get; set; }
        public DateTime Dt = new DateTime();
        public List<string> ReturnData { get; set; }
        public string ReturnDataString { get; set; }
        public string Visibility { get; set; }
        public void OnGet()
        {
            switch (Id)
            {
                case "1":
                    ReturnData = AppMethods.HotToColdestDayAllEntries("Inne");
                    break;
                case "2":
                    ReturnData = AppMethods.HumidityAvgPerDayAllEntries("Inne");
                    break;
                case "3":
                    ReturnData = AppMethods.MoldRisk("Inne");
                    break;
                default:
                    break;
            }
        }
        public void OnPost()
        {
            Dt = DateTime.Parse(Input);
            ReturnDataString = AppMethods.AverageTempPerDay(Dt, "Inne");
        }
    }
}
