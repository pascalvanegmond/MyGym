using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyGym.API.models;

namespace MyGym.API
{
    public interface IGymService
    {
        Task<Visits> GetVisitsDetailsAsync();
        Task<List<string>> Last2UniqueClubVisitsAsync();
        Task<string> StartReservationAsync(DateTime dateTime);
        Task<string> CancelReservationAsync();
    }
}