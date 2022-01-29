using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace WeatherAnalysis
{
    //public record Table
    //{
    //public Table(string[] line)
    //{
    //    Event_id = line[0];
    //    Type = WeatherType.Parse(line[1]);
    //    Severity = line[2];
    //    StartTime = DateTime.Parse(line[3]);
    //    EndTime = DateTime.Parse(line[4]);
    //    TimeZone = line[5];
    //    AirportCode = line[6];
    //    LocationLat = line[7];
    //    LocationLng = line[8];
    //    City = line[9];
    //    County = line[10];
    //    State = line[11];
    //    ZipCode = line[12];
    //}
    ///EventId,Type,
    ///Severity,StartTime(UTC),EndTime(UTC),TimeZone,
    ///AirportCode,LocationLat,LocationLng,City,County,State,ZipCode
    //    public string Event_id;// { get; init; }
    //    public WeatherType Type; //{ get; init; }// Enum
    //    public WeatherSeverity Severity; //{ get; init; }// Enum
    //    public DateTime StartTime; //{ get; init; }
    //    public DateTime EndTime;// { get; init; }
    //    public string TimeZone; //{ get; init; }
    //    public string AirportCode;// { get; init; }
    //    public string LocationLat;//{ get; init; }
    //    public string LocationLng; //{ get; init; }
    //    public string City;//{ get; init; }
    //    public string County; //{ get; init; }
    //    public string State;//{ get; init; }
    //    public string ZipCode; //{ get; init; }
    //}
    public record WeatherCreate
    (
        string EventId, //{ get; init; }
        WeatherType Type, //{ get; init; }
        WeatherSeverity Severity,// { get; init; }
        DateTime StartTime, //{ get; init; }
        DateTime EndTime,// { get; init; }
        string TimeZone,// { get; init; }
        string AirportCode,// { get; init; }
        string LocationLat, //{ get; init; }
        string LocationLng, //{ get; init; }
        string City, //{ get; init; }
        string County, //{ get; init; }
        string State, //{ get; init; }
        string ZipCode //{ get; init; }
    );
    public enum WeatherType
    {
        UNK = 0,
        Snow = 1,
        Fog = 2,
        Cold = 3,
        Storm = 4,
        Rain = 5,
        Precipitation = 6,
        Hail = 7
    }
    public enum WeatherSeverity
    {
        UNK = 0,
        Light = 1,
        Severe = 2,
        Moderate = 3,
        Heavy = 4,
        Other = 5
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<WeatherCreate> weather = new();

            string path = "C:\\Users\\yogan\\Desktop\\WeatherEvents_Jan2016-Dec2020Блокнотик.txt";

            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                // Считал первую строчку чтобы схалявить.
                sr.ReadLine();
                string line;
                int countLine = 0;
                while ((line = sr.ReadLine()) != null)
                {

                    string[] str = line.Split(",");
                    var crutch = new WeatherCreate(
                        str[0],
                        (WeatherType)Enum.Parse(typeof(WeatherType), str[1]),
                        (WeatherSeverity)Enum.Parse(typeof(WeatherSeverity), str[2]),
                        DateTime.Parse(str[3]),
                        DateTime.Parse(str[4]),
                        str[5],
                        str[6],
                        str[7],
                        str[8],
                        str[9],
                        str[10],
                        str[11],
                        str[12]);
                    weather.Add(crutch);
                    countLine += 1;
                }
            }


            //List<string> typeweather = new List<string>();
            //for (int i = 0; i < weather.Count(); i++)
            //{
            //    typeweather.Add(weather[i].Type);
            //}
            //IEnumerable<string> distinctType = typeweather.Distinct();

            //Console.WriteLine("Уникальные явления погоды:");
            //foreach (var type in distinctType)
            //{
            //    Console.WriteLine(type);
            //}
            Console.WriteLine("Task 0:");
            var result = from i in weather where i.StartTime.Year is 2018 select i;
            //var result = weather.Count(i => i.StartTime.Year is 2018);
            Console.WriteLine(result.Count());
            Console.WriteLine("---------------------------\n");

            Console.WriteLine("Task 1:");
            List<string> statesUsa = new();
            List<string> cityUsa = new();
            for (int j = 0; j < weather.Count; j++)
            {
                statesUsa.Add(weather[j].State);
                cityUsa.Add(weather[j].City);
            }
            IEnumerable<string> distinctStates = statesUsa.Distinct();
            IEnumerable<string> distinctCity = cityUsa.Distinct();
            Console.WriteLine($"В датасете:{distinctStates.Count()} штатов и {distinctCity.Count()} городов");
            Console.WriteLine("---------------------------\n");

            Console.WriteLine("Task 2:");
            var resulttwo = weather.Where(_ => _.StartTime.Year > 2019).GroupBy(_ => (_.Type, _.City))
                .Where(__ => __.Key.Type == WeatherType.Rain).Select(__ => new { City = __.Key.City, Rains = __.Count() })
                .OrderByDescending(___ => ___.Rains).Take(3);
            Console.WriteLine($"{string.Join(", ", resulttwo.Select(___ => $"{___.Rains} дождей в {___.City}"))}");
            Console.WriteLine("----------------------------");

            Console.WriteLine("Task 3:");
            var resultthree = weather.GroupBy(r => r.StartTime.Year).Select(g => new
            {
                Year = g.Key,
                longestSnowfall = g.OrderByDescending(_ => _.EndTime - _.StartTime).First()
            });
            Console.WriteLine($"{string.Join(", ", resultthree.Select(a => $"{a.Year} - from {a.longestSnowfall}"))}");
            Console.WriteLine("----------------------------");

            Console.WriteLine("Task 4:");
            var resultfour = weather.Where(r => r.StartTime.Year == 2019)
                .OrderBy(r => r.StartTime).GroupBy(r => r.State).Select(g => new
                {
                    State = g.Key,
                    Count = g.TakeWhile(r => r.EndTime - r.StartTime <= TimeSpan.FromHours(2)).Count()
                });
            foreach (var stateCount in resultfour)
            {
                Console.WriteLine($"{stateCount.State} - {stateCount.Count} событий превышающие по длительности 2 часа");
            }
            Console.WriteLine("---------------------------");

            Console.WriteLine("Task 5:");
            var resultfive = weather.Where(r => r.StartTime.Year is 2017 or 2018 or 2019! & r.Severity == WeatherSeverity.Severe)
                .Select(r => new
                {
                    r.State,
                    Start = r.StartTime,
                    End = r.EndTime,
                    Duration = r.EndTime - r.StartTime,
                    r.City
                }).GroupBy(r => r.State).Select(g => new
                {
                    State = g.Key,
                    longestEvent = g.OrderByDescending(r => r.Duration).First()
                }).OrderByDescending(r => r.longestEvent.Duration);
            foreach (var item in resultfive)
            {
                Console.WriteLine($"{item.State}-{Math.Round(item.longestEvent.Duration.TotalHours)}");
            }
            Console.WriteLine("-----------------------------");


        }
    }
}
