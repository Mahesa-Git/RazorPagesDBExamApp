using RazorPagesDBExam.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesDBExam
{
    public class ReadData
    {
        public static void ReadCSV()
        {
            string[] resultSet = File.ReadAllLines(@"C:\Users\matte\source\repos\RazorPagesDBExamApp\RazorPagesDBExam\File\TemperaturData.csv");
            using (var context = new EFContext())
            {
                foreach (var data in resultSet)
                {
                    WeatherData tmp = new WeatherData();
                    string[] tmpString = data.Split(",");
                    tmp.DateAndTime = DateTime.Parse(tmpString[0]);
                    tmp.Location = tmpString[1];
                    tmp.Temperature = float.Parse(tmpString[2], CultureInfo.InvariantCulture);
                    tmp.Humidity = float.Parse(tmpString[3], CultureInfo.InvariantCulture);

                    context.Add(tmp);
                }
                context.SaveChanges();
            }
        }
    }

}
