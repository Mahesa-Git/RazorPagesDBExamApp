using RazorPagesDBExam.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesDBExam
{
    public class AppMethods
    {
        public static DateTime StartDate = default;
        public static List<string> ReturnData = new List<string>();
        public static string ReturnString = null;
        public static string AverageTempPerDay(DateTime start, string location)
        {
            using (var context = new EFContext())
            {
                string returnValue;

                var resultSet = context.WeatherData
                    .Where(x => x.DateAndTime >= start && x.DateAndTime < start.AddDays(1) && x.Location == $"{location}");

                if (resultSet.Count() == 0)
                    returnValue = "Data saknas för det angivna datumet";
                else
                    returnValue = $"{resultSet.FirstOrDefault().DateAndTime:yyyy/MM/dd} | {Math.Round(resultSet.Average(x => x.Temperature), 1)} °C";
                return returnValue;
            }
        }//Mata in ett datum och se medeltemp.
        public static List<string> HotToColdestDayAllEntries(string location)
        {
            using (var context = new EFContext())
            {
                List<string> avgTemperatures = new List<string>();

                var resultSet = context.WeatherData
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, Avg = x.Average(x => x.Temperature) })
                    .OrderByDescending(x => x.Avg);

                var top = resultSet
                    .Take(10);
                var bottom = resultSet
                    .OrderBy(x => x.Avg)
                    .Take(10)
                    .OrderByDescending(x => x.Avg);

                foreach (var entry in top)
                {
                    avgTemperatures.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round(entry.Avg, 1)} °C");
                }
                avgTemperatures.Add("-----------");
                foreach (var entry in bottom)
                {
                    avgTemperatures.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round(entry.Avg, 1)} °C");
                }
                return avgTemperatures;
            }
        }//top 10 varmaste, och top 10 kallaste dagarna, spannet mellan bortfiltrerat.
        public static List<string> HumidityAvgPerDayAllEntries(string location)
        {
            using (var context = new EFContext())
            {
                List<string> avgHumidity = new List<string>();

                var resultSet = context.WeatherData
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, Avg = x.Average(x => x.Humidity) })
                    .OrderByDescending(x => x.Avg);

                var top = resultSet
                    .Take(10);
                var bottom = resultSet
                    .OrderBy(x => x.Avg)
                    .Take(10)
                    .OrderByDescending(x => x.Avg);

                foreach (var entry in top)
                {
                    avgHumidity.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round(entry.Avg, 1)} % fuktighet");
                }
                avgHumidity.Add("-----------");
                foreach (var entry in bottom)
                {
                    avgHumidity.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round(entry.Avg, 1)} % fuktighet");
                }
                return avgHumidity;
            }
        }//top 10 fuktigaste dagarna och top 10 icke, spannet mellan bortfiltrerat.
        public static string MeteorologicalFallAndWinter(bool fall)
        {
            using (var context = new EFContext())
            {
                int start, temp;
                start = (fall == true) ? 08 : 01;
                temp = (fall == true) ? 10 : 00;
                string output = (fall == true) ? "höst" : "vinter";

                var resultSet = context.WeatherData
                    .Where(x => x.Location == "ute" && (x.DateAndTime.Month >= start && x.DateAndTime.Month <= 12) || (x.DateAndTime.Month >= 01 && x.DateAndTime.Month < 03))
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, Avg = x.Average(x => x.Temperature) })
                    .OrderBy(x => x.DateAndTime);

                int counter = 1;
                int tmpListcounter = default;
                List<WeatherData> sortedEntries = new List<WeatherData>();

                foreach (var entry in resultSet)
                {
                    if (tmpListcounter < 5)
                    {
                        WeatherData tmp = new WeatherData();
                        tmp.DateAndTime = entry.DateAndTime;
                        tmp.Temperature = entry.Avg;
                        sortedEntries.Add(tmp);
                        tmpListcounter++;
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (sortedEntries[i].Temperature <= temp && sortedEntries[counter].Temperature <= temp)
                            {
                                if (DateTime.Compare(sortedEntries[i].DateAndTime.AddDays(1), sortedEntries[counter].DateAndTime) == 0)
                                {
                                    counter++;
                                    if (counter == 5)
                                        return $"{sortedEntries[0].DateAndTime:yyyy/MM/dd} | {Math.Round(sortedEntries[0].Temperature, 1)} °C";
                                    else
                                        continue;
                                }
                                else
                                {
                                    WeatherData tmp = new WeatherData();
                                    tmp.DateAndTime = entry.DateAndTime;
                                    tmp.Temperature = entry.Avg;
                                    sortedEntries.RemoveAt(0);
                                    sortedEntries.Add(tmp);
                                    counter = 1;
                                    break;
                                }
                            }
                            else
                            {
                                WeatherData tmp = new WeatherData();
                                tmp.DateAndTime = entry.DateAndTime;
                                tmp.Temperature = entry.Avg;
                                sortedEntries.RemoveAt(0);
                                sortedEntries.Add(tmp);
                                counter = 1;
                                break;
                            }
                        }
                    }
                }
                return $"Data som visar på start av meteorologisk {output} saknas i databasen";
            }
        }
        public static List<string> MoldRisk(string location)
        {
            using (var context = new EFContext())
            {
                List<string> returnList = new List<string>();

                var resultSet = context.WeatherData
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, AvgTemp = x.Average(x => x.Temperature), AvgHumidity = x.Average(x => x.Humidity) })
                    .OrderByDescending(x => (x.AvgHumidity - 78) * (x.AvgTemp / 15) / 0.22);

                var top = resultSet
                    .Take(10);
                var bottom = resultSet
                    .OrderBy(x => (x.AvgHumidity - 78) * (x.AvgTemp / 15) / 0.22)
                    .Take(10)
                    .OrderByDescending(x => (x.AvgHumidity - 78) * (x.AvgTemp / 15) / 0.22);

                foreach (var entry in top)
                {
                    string result = Math.Round((entry.AvgHumidity - 78) * (entry.AvgTemp / 15) / 0.22, 2) < 0 ? "(Ingen risk)" : "";
                    returnList.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round((entry.AvgHumidity - 78) * (entry.AvgTemp / 15) / 0.22, 2)} % {result}");
                }
                returnList.Add("-----------");
                foreach (var entry in bottom)
                {
                    string result = Math.Round((entry.AvgHumidity - 78) * (entry.AvgTemp / 15) / 0.22, 2) < 0 ? "(Ingen risk)" : "";
                    returnList.Add($"{entry.DateAndTime:yyyy/MM/dd} | {Math.Round((entry.AvgHumidity - 78) * (entry.AvgTemp / 15) / 0.22, 2)} % {result}");
                }
                return returnList;
            }
        }////top 10 dagarna med hög mögelrisk och top 10 icke, spannet mellan bortfiltrerat.
        public static List<string> TempDiffCheckBalconyDoor()
        {
            using (var context = new EFContext())
            {
                var DBQuery = context.WeatherData //Query som grupperar alla mätningar per dag blandat inne/ute
                    .AsEnumerable()
                    .OrderBy(x => x.DateAndTime)
                    .GroupBy(x => x.DateAndTime.Date);

                List<WeatherData> tmp = new List<WeatherData>();
                List<string> returnList = new List<string>();

                foreach (var day in DBQuery) //går igenom varje dag, blandat inne/ute i varje grupp.
                {
                    WeatherData wD = new WeatherData();
                    List<WeatherData> insideListAllDay = new List<WeatherData>(); //Två listor för första sortering inne/ute
                    List<WeatherData> outsideListAllday = new List<WeatherData>();

                    List<WeatherData> insideListSameTime = new List<WeatherData>(); //Ytterligare två listor för en andra sortering: mätningar gjorda samma minut och timme
                    List<WeatherData> outsideListSameTime = new List<WeatherData>();

                    foreach (var entry in day) //Första sortering: inne/ute en dag
                    {
                        if (entry.Location == "Inne")
                            insideListAllDay.Add(entry);
                        else
                            outsideListAllday.Add(entry);
                    }

                    int hour = 0, minute = 0;
                    for (int i = 0; i < insideListAllDay.Count; i++) //Andra sortering mätningar gjorda samma timme och minut ute och inne en dag
                    {
                        hour = insideListAllDay[i].DateAndTime.Hour;
                        minute = insideListAllDay[i].DateAndTime.Minute;

                        for (int j = 0; j < outsideListAllday.Count(); j++)
                        {
                            if (outsideListAllday[j].DateAndTime.Minute == minute && outsideListAllday[j].DateAndTime.Hour == hour)
                            {
                                insideListSameTime.Add(insideListAllDay[i]);
                                outsideListSameTime.Add(outsideListAllday[j]);
                                break;
                            }
                        }
                    }
                    if (insideListSameTime.Count == 0 || outsideListSameTime.Count == 0) //om inga mätningar är gjorda under samma minut på en dag, förusätts balkongdörren inte varit öppen alls
                    {
                        wD.DateAndTime = day.Key.Date;
                        wD.Temperature = 0;
                        tmp.Add(wD);
                        continue;
                    }
                    else // om mätningar faktiskt gjorts samma timme och minut...
                    {
                        float insideTemp = insideListSameTime[0].Temperature;
                        float outsideTemp = outsideListSameTime[0].Temperature;
                        double totalTime = 0;
                        int startIndex = 0, endIndex = 0;

                        for (int i = 1; i < insideListSameTime.Count; i++) //start på index 1
                        {
                            if (insideListSameTime[i].Temperature < insideTemp && outsideListSameTime[i].Temperature > outsideTemp) //om tempen sjunkit inne från förra index inomhus och ökat utomhus
                            {
                                startIndex = i;
                                insideTemp = insideListSameTime[i].Temperature; //sätt bevakningstemperatur inne för att se om den ökar igen..
                                for (int j = i + 1; j < insideListSameTime.Count; j++)
                                {
                                    if (insideListSameTime[j].Temperature > insideTemp) //om tempen på ett senare index skulle uppfylla det som beskrivs ovan
                                    {
                                        endIndex = j;
                                        if ((insideListSameTime[endIndex].DateAndTime - insideListAllDay[startIndex].DateAndTime).TotalMinutes > 90) //Om det uppmätta tidspannet överstiger 90 minuter
                                        {                                                                                                           //bedöms datan vara otillräcklig(ex. för långt mellan mätningar)
                                            i = j; //För att starta där jämförelsen slutade, så vi inte mäter samma period.
                                            break;
                                        }
                                        else
                                        {
                                            totalTime += (insideListSameTime[endIndex].DateAndTime - insideListSameTime[startIndex].DateAndTime).TotalMinutes;
                                            i = j;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                insideTemp = insideListSameTime[i].Temperature;
                                outsideTemp = outsideListSameTime[i].Temperature;
                            }
                        }
                        wD.DateAndTime = day.Key.Date; //objekt av klass som möjliggör senare sortering på när den bedömts vara öppen längst.
                        wD.Temperature = (float)totalTime;
                        tmp.Add(wD);
                        insideListAllDay.Clear(); //rensa listor inför nästa dags mätning
                        outsideListAllday.Clear();
                        insideListSameTime.Clear();
                        outsideListSameTime.Clear();
                    }
                }
                var q1 = tmp
                    .OrderByDescending(x => x.Temperature);
                foreach (var item in q1)
                {
                    returnList.Add($"{item.DateAndTime:yyyy/MM/dd}: {Math.Round(item.Temperature)} minuter");
                }
                return returnList;
            }
        } //vg 1
        public static List<string> TempDiffPerDayMinMax()
        {
            using (var context = new EFContext())
            {
                var DBQuery = context.WeatherData
                    .AsEnumerable()
                    .OrderBy(x => x.DateAndTime)
                    .GroupBy(x => x.DateAndTime.Date);

                List<string> returnList = new List<string>();
                List<TempDiff> tDList = new List<TempDiff>();
                foreach (var day in DBQuery) //går igenom varje dag, blandade inne och ute-mätningar
                {
                    List<WeatherData> insideList = new List<WeatherData>();
                    List<WeatherData> outsideList = new List<WeatherData>();
                    foreach (var entry in day)
                    {
                        if (entry.Location == "Inne") //sortering av ute och inne-mätningar.
                            insideList.Add(entry);
                        else
                            outsideList.Add(entry);
                    }
                    float inMin = insideList.Min(x => x.Temperature); //min och max temperaturer per dag inne respektive ute.
                    float inMax = insideList.Max(x => x.Temperature);
                    float outMin = outsideList.Min(x => x.Temperature);
                    float outMax = outsideList.Max(x => x.Temperature);

                    float diffOne = Math.Abs(outMax - inMin); //kontrollera spannet mellan temperaturerna: vilket är störst.
                    float diffTwo = Math.Abs(inMax - outMin);

                    float minDist = default, tmp = default; 
                   
                    for (int i = 0; i < insideList.Count; i++) //innetemp varje index...
                    {
                        for (int j = 0; j < outsideList.Count; j++)//.. som jämförs mot varje index bland utetemperaturer.
                        {
                            if (i == 0 && j == 0)
                                minDist = Math.Abs(insideList[i].Temperature - outsideList[j].Temperature); //Sätterett värde att senare jämföra mot (första mätning ute och inne den dagen) 
                            else
                            {
                                tmp = Math.Abs(insideList[i].Temperature - outsideList[j].Temperature); //spannet mellan innemätning och utevärdet. 
                                if (minDist > tmp)  //skulle spannet vara mindre än det temporära värdet byts det temporära värdet till detta.
                                    minDist = tmp; //tillslut kommer alla innemätningar ha jämförts mot alla utemätningar och det minsta spannet kvarstår.
                            }                     //har tempen varit samma ute och inne någon gång under dagen blir värdet alltså 0.
                        }                     
                    }

                    TempDiff tD = new TempDiff(); //objekt av klass för sortering efter spannet mellan max skillnad och min skillnad i temp.
                    tD.DateAndTime = day.Key.Date;
                    tD.MinTempDiff = minDist;
                    if (diffOne > diffTwo)
                        tD.MaxTempDiff = diffOne;
                    else
                        tD.MaxTempDiff = diffTwo;

                    tDList.Add(tD);
                    insideList.Clear();
                    outsideList.Clear();
                }
                var q1 = tDList
                    .OrderByDescending(x => Math.Abs(x.MaxTempDiff - x.MinTempDiff));
                foreach (var item in q1)
                {
                    returnList.Add($"{item.DateAndTime:yyyy/MM/dd}: Max {Math.Round(item.MaxTempDiff, 1)} °C |\tMin {Math.Round(item.MinTempDiff, 1)} °C\nResultat: {Math.Round(item.MaxTempDiff - item.MinTempDiff,1)} °C");
                }
                return returnList;
            }
        }//vg 2
        class TempDiff
        {
            public float MaxTempDiff { get; set; }
            public float MinTempDiff { get; set; }
            public DateTime DateAndTime { get; set; }
        }
    }
}
