using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyGym.API.models;
using Newtonsoft.Json;

namespace MyGym.API
{
    public class GymService: IGymService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _loginCookie;
        
        public GymService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _loginCookie = _configuration["LoginCookie"];
        }
        
        public async Task<Visits> GetVisitsDetailsAsync()
        {
            var visits = await GetVisits();

            var groupedClubVisits = visits.GroupBy(x => x.Club);
            var groupedClubVisitsDetails = new List<VisitClubDetails>();
            
            groupedClubVisitsDetails.AddRange(groupedClubVisits.Select(groupedClubVisit => new VisitClubDetails
                {Club = groupedClubVisit.FirstOrDefault().Club, NumberOfVisits = groupedClubVisit.Count()})
                .OrderByDescending(x => x.NumberOfVisits));
            
            return new Visits
            {
                TotalVisits = visits.Count(),
                VisitClubDetails = groupedClubVisitsDetails,
                Visit = visits
            };
        }
        
        public async Task<List<string>> Last2UniqueClubVisitsAsync()
        {
            var visits = await GetVisits();

            return visits.GroupBy(x => x.Club).Take(2).Select(x => x.FirstOrDefault().Club).ToList();
        }

        public async Task<string> StartReservationAsync(DateTime preferedBookDate)
        {
            var dateTime = preferedBookDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var defaultClubId = await GetIdsFromLast2UniqueClubVisitsAsync();
            // Todo: Choice between clubs or default club from the settings
            var selectedClub = defaultClubId.LastOrDefault();
            var getAvailabilityBody = new GetAvailabilityBody
            {
                ClubId = selectedClub.ClubId,
                DateTime = dateTime
            };
            var availableOptions = await GetAvailableTimeSlotsAsync(getAvailabilityBody);

            if (!availableOptions.Any())
            {
                return "No available options for the selected time.";
            }
            
            var bestMatch = availableOptions.FirstOrDefault(x => x.OpenForReservation == true &&
                                                                 DateTime.Parse((string) x.StartDateTime) > preferedBookDate);

            var reservationBody = new BookReservationBody
            {
                DoorPolicy = bestMatch,
                Duration = "90",
                ClubOfChoice = new BookReservationClubDetails
                {
                    Name = selectedClub.ClubName,
                    Id = selectedClub.ClubId,
                    Country = "Nederland",
                    Bookable = true,
                    Status = null,
                    DebtCheck = true,
                    LabelName = "1452"
                }
            };

            return await BookReservationAsync(reservationBody);
        }

        public async Task<string> CancelReservationAsync()
        {
            var getOpenReservation = await GetOpenReservationAsync();
   
            if (!getOpenReservation.Data.Any() || getOpenReservation.Data.FirstOrDefault().DoorPolicyPeopleId == null)
            {
                return "No Reservation found";
            }
            return await CancelReservationAsync(new CancelReservationBody{DoorPolicyPeopleId = getOpenReservation.Data.FirstOrDefault().DoorPolicyPeopleId});
        }

        private async Task<List<ClubNameAndId>> GetIdsFromLast2UniqueClubVisitsAsync()
        {
            var last2Clubs = (await Last2UniqueClubVisitsAsync()).Select(x  => x.Replace("Basic-Fit ", ""));
            var allClubs = await GetClubsAsync();

            return (allClubs.Where(x => last2Clubs.Contains(x.ClubName))).ToList();
        }

        private async Task<List<Visit>> GetVisits()
        {
            const string url = "https://my.basic-fit.com/member/visits";

            var responseString = await HttpGetAsync(url, _loginCookie);
            return JsonConvert.DeserializeObject<List<Visit>>(responseString);
        }
        
        private async Task<List<DoorPolicy>> GetAvailableTimeSlotsAsync(GetAvailabilityBody getAvailabilityBody)
        {
            const string url = "https://my.basic-fit.com/door-policy/get-availability";

            var body = new StringContent(JsonConvert.SerializeObject(getAvailabilityBody), Encoding.UTF8, "application/json" );

            var responseString = await HttpPostAsync(url, _loginCookie, body);
            return JsonConvert.DeserializeObject<List<DoorPolicy>>(responseString);
        }
        
        private async Task<List<ClubNameAndId>> GetClubsAsync()
        {
            const string url = "https://my.basic-fit.com/door-policy/get-clubs";
            
            var responseString = await HttpGetAsync(url, _loginCookie);
            return JsonConvert.DeserializeObject<List<ClubNameAndId>>(responseString);
        }
        
        private async Task<OpenReservation> GetOpenReservationAsync()
        {
            const string url = "https://my.basic-fit.com/door-policy/get-open-reservation";
            
            var responseString = await HttpGetAsync(url, _loginCookie);
            return JsonConvert.DeserializeObject<OpenReservation>(responseString);
        }
        
        private async Task<string> BookReservationAsync(BookReservationBody bookReservationBody)
        {
            const string url = "https://my.basic-fit.com/door-policy/book-door-policy";

            var body = new StringContent(JsonConvert.SerializeObject(bookReservationBody), Encoding.UTF8, "application/json" );

            return await HttpPostAsync(url, _loginCookie, body);
        }

        private async Task<string> CancelReservationAsync(CancelReservationBody cancelReservationBody)
        {
            const string url = "https://my.basic-fit.com/door-policy/cancel-door-policy";

            var body = new StringContent(JsonConvert.SerializeObject(cancelReservationBody), Encoding.UTF8, "application/json" );

            return await HttpPostAsync(url, _loginCookie, body);
        }
        
        private async Task<string> HttpGetAsync(string url, string loginCookie, bool tokenRefreshed = false)
        {
            SetHeadersAndCookies(loginCookie);
            
            var response = await _httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (!tokenRefreshed)
                {
                    //Todo: renew token
                    //Set tokenRefreshed to true
                }
                Console.WriteLine("Token expired");
            }

            if (response.IsSuccessStatusCode) return responseString;
            throw new Exception(responseString, new Exception("Backend Issue")); //TODO: Better error handling
        }
        
        private async Task<string> HttpPostAsync(string url, string loginCookie, HttpContent body, bool tokenRefreshed = false)
        {
            SetHeadersAndCookies(loginCookie);
            
            var response = await _httpClient.PostAsync(url,body);
            var responseString = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (!tokenRefreshed)
                {
                    //Todo: renew token
                    //Set tokenRefreshed to true
                }
                Console.WriteLine("Token expired");
            }

            if (response.IsSuccessStatusCode) return responseString;
            throw new Exception(responseString, new Exception("Backend Issue")); //TODO: Better error handling
        }

        private void SetHeadersAndCookies(string loginCookie)
        {
            var cookies = new List<string>()
            {
                "cookieconsent_level=20",
                "cookieconsent_variant=wnl__a1003",
                "cookieconsent_seen=1",
                "LanguageCookie=nl-nl",
                $"connect.sid={loginCookie}"
            };
            
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://my.basic-fit.com");            
            _httpClient.DefaultRequestHeaders.Add("mbf-rct-app-api-2-caller", "true");
            _httpClient.DefaultRequestHeaders.Add("Cookie", string.Join(";", cookies));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.21.0");
        }
        
        //TODO: Implement login system
        private void Login()
        {
            var loginDetails = new Login()
            {
                Email = _configuration["Email"],
                Password = _configuration["Password"],
                GoogleCaptcha = _configuration["GoogleCaptcha"]
            };
        }
    }
}