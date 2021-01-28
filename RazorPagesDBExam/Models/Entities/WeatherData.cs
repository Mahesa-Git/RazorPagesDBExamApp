using System;

namespace RazorPagesDBExam.Models.Entities
{
    public partial class WeatherData
    {
        public int ID { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Location { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
    }
}
