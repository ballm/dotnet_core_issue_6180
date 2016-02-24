using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GetAsyncSlowIssue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var totalDuration = new TimeSpan();
            var numTests = 10;

            // ignore first run due to 'initialisation' making it slower
            var temp = Task.Run(() => Test()).Result;
            
            for (var index = 0; index < numTests; index++)
            {
                var duration = Task.Run(() => Test()).Result;
                
                Console.WriteLine("{0}: {1} ms", index, duration.TotalMilliseconds);

                totalDuration = totalDuration.Add(duration);
            }
            
            Console.WriteLine();
            Console.WriteLine("Total Duration: {0} ms", totalDuration.TotalMilliseconds);
            Console.WriteLine("Average: {0} ms", totalDuration.TotalMilliseconds / numTests);

            Console.ReadLine();
        }

        private static async Task<TimeSpan> Test()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("test", "v1"));
                var uri = new Uri("https://api.github.com/repos/dotnet/corefx/issues");

                var startTime = DateTime.Now;

                await httpClient.GetAsync(uri);

                var endTime = DateTime.Now;

                return endTime.Subtract(startTime);
            }
        }
    }

}
