using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cw1
{
    public class Program
    {
        private const string MatchEmailPattern =
            @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
            + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
        
        public static async Task Main(string[] args)
        {
            if (!args.Any())
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!Uri.TryCreate(args[0], UriKind.Absolute, out var uriResult))
            {
                throw new ArgumentException(nameof(args));
            }
            
            var emails = await GetEmails(args[0]);

            if (emails == null || !emails.Any())
            {
                Console.WriteLine("Nie znaleziono adresów email");
            }
            else
            {
                foreach (var email in emails.Distinct())
                {
                    Console.WriteLine(email);
                }
            }
        }

        private static async Task<IEnumerable<string>> GetEmails(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);

                var stringContent = await response.Content.ReadAsStringAsync();

                var emailRegex = new Regex(MatchEmailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matches = emailRegex.Matches(stringContent);

                return matches.Select(m => m.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine("Błąd w czasie pobierania strony");
                return null;
            }
        }
    }
}