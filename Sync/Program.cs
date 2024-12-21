using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Sync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Sync().GetAwaiter().GetResult();
        }

        private static async Task Sync()
        {
            var apiKey = "v_eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiN2VhYTBhNTQtYTBiZC00OTNlLWFjNDMtZjNjZGEwZmVlNWQ5IiwiZXhwIjoyMTQ3NDgzNjQ3LCJpc3MiOiJodHRwczovL2FwcC52aXJ0dW91c3NvZnR3YXJlLmNvbSIsImF1ZCI6Imh0dHBzOi8vYXBpLnZpcnR1b3Vzc29mdHdhcmUuY29tIn0.oN0bfmYMS7lPxGtVH3ouEVhD0Kuzoqa2nAnuvPTyPpk";
            var configuration = new Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var state = "AZ";

            if (take > 0 && maxContacts > 0)
            {
                //if value of maxContacts is less than value of take, then we only retrieve maxContacts number of contacts
                take = Math.Min(take, maxContacts);

                using (var writer = new StreamWriter($"Contacts_{DateTime.Now:MM_dd_yyyy}.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    bool hasMore;

                    do
                    {
                        var contacts = await virtuousService.GetContactsByStateAsync(skip, take, state);
                        skip += take;
                        csv.WriteRecords(contacts);
                        hasMore = skip < maxContacts;
                    }
                    while (hasMore);
                }
            }
            else
            {
                Console.WriteLine("maxContacts should be greater than 0");
            }
        }
    }
}
