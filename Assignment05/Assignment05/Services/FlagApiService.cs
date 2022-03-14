using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;


using Newtonsoft.Json;
using Xamarin.Essentials;
using Assignment05.Models;


namespace Assignment05.Services
{
    class FlagApiService
    {

        public async static Task<List<Country>> GetCountries()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string key = "TheCamel"; // replace with your own, but first check it works 
                string url = $"http://api.geonames.org/countryInfoJSON?username={key}";

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    CountryInfo info = JsonConvert.DeserializeObject<CountryInfo>(json);
                    return info.Geonames;
                }
            }

            return new List<Country>();
        }


    }
}
