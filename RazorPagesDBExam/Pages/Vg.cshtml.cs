using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesDBExam.Pages
{
    public class VgModel : PageModel
    {
        [BindProperty(SupportsGet = true)] public string Id { get; set; }
        public List<string> ReturnData { get; set; }
        public string Visibility { get; set; }
        public void OnGet()
        {
            switch (Id)
            {
                case "1":
                    ReturnData = AppMethods.TempDiffCheckBalconyDoor();
                    Visibility = "invisible";
                    break;
                case "2":
                    ReturnData = AppMethods.TempDiffPerDayMinMax();
                    Visibility = "visible";
                    break;
                default:
                    Visibility = "invisible";
                    break;
            }
        }
    }
}
