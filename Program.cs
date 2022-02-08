using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
namespace WeatherAnalysis
{
    internal delegate Response CallingMethod(List<WeatherCreate> weather);

    public record Response(List<string> stringline)
    {
        public override string ToString()
        {
            string str = "";
            foreach (var item in stringline)
            {
                str += item+"\n"; 
            }
            return str;
        }
    }
    class MethodAnalytics
    {
        public static Response Task0(List<WeatherCreate> weather)
        {
            var resp = new Response(new List<string>());
            var result = from i in weather where i.StartTime.Year is 2018 select i;
            //var result = weather.Count(i => i.StartTime.Year is 2018);
            resp.stringline.Add($"Task0:\n{result.Count()}");
            resp.stringline.Add("---------------------------");
            return resp;

        }
        public static Response Task1(List<WeatherCreate> weather)
        {
            var resp = new Response(new List<string>());
            List<string> statesUsa = new();
            List<string> cityUsa = new();
            for (int j = 0; j < weather.Count; j++)
            {
                statesUsa.Add(weather[j].State);
                cityUsa.Add(weather[j].City);
            }
            IEnumerable<string> distinctStates = statesUsa.Distinct();
            IEnumerable<string> distinctCity = cityUsa.Distinct();
            resp.stringline.Add($"Task1\nВ датасете:{distinctStates.Count()} штатов и {distinctCity.Count()} городов");
            resp.stringline.Add("---------------------------");
            return resp;

        }
        public static Response Task2(List<WeatherCreate> weather)
        {
            var resp = new Response(new List<string>());
            var resulttwo = weather.Where(_ => _.StartTime.Year > 2019).GroupBy(_ => (_.Type, _.City))
                .Where(__ => __.Key.Type == WeatherType.Rain).Select(__ => new { City = __.Key.City, Rains = __.Count() })
                .OrderByDescending(___ => ___.Rains).Take(3);
            resp.stringline.Add("Task2:");
            resp.stringline.Add($"{string.Join(", ", resulttwo.Select(___ => $"{___.Rains} дождей в {___.City}"))}");
            resp.stringline.Add("---------------------------");
            return resp;
        }
        public static Response Task3(List<WeatherCreate> weather)
        {
            var resp = new Response(new List<string>());
            var resultthree = weather.GroupBy(r => r.StartTime.Year).Select(g => new
            {
                Year = g.Key,
                longestSnowfall = g.OrderByDescending(_ => _.EndTime - _.StartTime).First()
            });
            resp.stringline.Add($"Task3:\n{string.Join(", ", resultthree.Select(a => $"{a.Year} - from {a.longestSnowfall}"))}");
            resp.stringline.Add("---------------------------");
            return resp;
        }
        public static Response Task4(List<WeatherCreate> weather)
        {
            var resp = new Response(new List<string>());
            var resultfour = weather.Where(r => r.StartTime.Year == 2019)
                .OrderBy(r => r.StartTime).GroupBy(r => r.State).Select(g => new
                {
                    State = g.Key,
                    Count = g.TakeWhile(r => r.EndTime - r.StartTime <= TimeSpan.FromHours(2)).Count()
                });
            resp.stringline.Add("Task4:");
            foreach (var stateCount in resultfour)
            {
                resp.stringline.Add($"{stateCount.State} - {stateCount.Count} событий превышающие по длительности 2 часа");
            }
            resp.stringline.Add("---------------------------");
            return resp; 
        }
        public static Response Task5(List<WeatherCreate> weather)
        {
            var resp = new Response(new List<string>());
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
            resp.stringline.Add("Task5:");
            foreach (var item in resultfive)
            {
                resp.stringline.Add($"{item.State}-{Math.Round(item.longestEvent.Duration.TotalHours)}");
            }
            resp.stringline.Add("---------------------------");
            return resp;
        }
        public static Response Task6(List<WeatherCreate> weather)
        {
            var resp = new Response(new List<string>());
            var resultsix = weather.GroupBy(i => i.StartTime.Year).Select(i => (i.Key, i.GroupBy(l => l.Type)))
                .Select(i => (i.Key, i.Item2.OrderBy(j => j.Count()))).ToList();
            var mostEvents = resultsix.Select(i => (i.Key, i.Item2.Last())).
                Select(i => (i.Key, i.Item2, i.Item2.Average(j => (j.EndTime - j.StartTime).TotalHours)));
            var rarelyEvents = resultsix.Select(i => (i.Key, i.Item2.First()))
                .Select(i => (i.Key, i.Item2, i.Item2.Average(j => (j.EndTime - j.StartTime).TotalHours)));
            resp.stringline.Add("Task6:");
            foreach(var item in mostEvents)
            {
                resp.stringline.Add($"{item.Key}: {item.Item2.Key.ToString()} в среднем {Math.Round(item.Item3, 3)} часов");
            }
            foreach(var item in rarelyEvents)
            {
                resp.stringline.Add($"{item.Key}: {item.Item2.Key.ToString()} в среднем {Math.Round(item.Item3,3)} чаосв");
            }
            return resp;
        }
    }
    public record WeatherCreate
    (
        string EventId,
        WeatherType Type, 
        WeatherSeverity Severity,
        DateTime StartTime,
        DateTime EndTime,
        string TimeZone,
        string AirportCode,
        string LocationLat,
        string LocationLng, 
        string City,
        string County,
        string State,
        string ZipCode
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
            List<CallingMethod> analyticsWeather = new List<CallingMethod>(7);
            analyticsWeather.Add(MethodAnalytics.Task0);
            analyticsWeather.Add(MethodAnalytics.Task1);
            analyticsWeather.Add(MethodAnalytics.Task2);
            analyticsWeather.Add(MethodAnalytics.Task3);
            analyticsWeather.Add(MethodAnalytics.Task4);
            analyticsWeather.Add(MethodAnalytics.Task5);
            analyticsWeather.Add(MethodAnalytics.Task6);
            // Первый способ - for
            Stopwatch measurementOne = new Stopwatch();
            measurementOne.Start();
            for (int i = 0; i < analyticsWeather.Count(); i++)
            {
                var line = analyticsWeather[i](weather).ToString();
                Console.WriteLine(line);
            }
            measurementOne.Stop();

            // Второй способ - Parallel.ForEach
            Stopwatch measurementTwo = new Stopwatch();
            measurementTwo.Start();
            Parallel.ForEach(analyticsWeather, i =>
            {
                var line = i(weather).ToString();
                //Console.Write(line);
            });
            measurementTwo.Stop();

            // Третий способ - Task WaiAll
            Stopwatch measurementThree = new Stopwatch();
            measurementThree.Start();
            Task[] tasks = new Task[7]
            {
                new Task(() => analyticsWeather[0](weather).ToString()),
                new Task(() => analyticsWeather[1](weather).ToString()),
                new Task(() => analyticsWeather[2](weather).ToString()),
                new Task(() => analyticsWeather[3](weather).ToString()),
                new Task(() => analyticsWeather[4](weather).ToString()),
                new Task(() => analyticsWeather[5](weather).ToString()),
                new Task(() => analyticsWeather[6](weather).ToString())
            };
            foreach (var item in tasks)
                item.Start();
            // ожидаем завершения задач
            Task.WaitAll(tasks);
            measurementThree.Stop();

            Console.WriteLine("\n\n\n\n");
            Console.WriteLine($"Метод for занимает:{measurementOne.ElapsedMilliseconds} миллисекунд\n");
            Console.WriteLine($"Метод Parallel.ForEach занимает:{measurementTwo.ElapsedMilliseconds} миллисекунд\n");
            Console.WriteLine($"Метод Tasks.WaitAll занимает:{measurementThree.ElapsedMilliseconds} миллисекунд\n");

            double effectiveness;
            List<double> list = new List<double>(3) { 0, 0,  0 };
            list[0] = measurementOne.ElapsedMilliseconds;
            list[1] = measurementTwo.ElapsedMilliseconds;
            list[2] = measurementThree.ElapsedMilliseconds;
            double max = list.Max();
            double min = list.Min();
            effectiveness = (max * 100) / min - 100;
            Console.WriteLine($"Самый быстрый эффективнее самого медленного примерно на {(int)effectiveness}%");
        }
    }
}