using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MyGym.API
{
    public class StartApi
    {
        private readonly IGymService _gymService;

        public StartApi(IGymService gymService)
        {
            _gymService = gymService;
        }

        [FunctionName("MyGymVisits")]
        public async Task<IActionResult> Visits(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("Visits triggered");

            var visits = await _gymService.GetVisitsDetailsAsync();
            //var visits = await _gymService.Last2UniqueClubVisits();
            
            return visits != null
                ? (ActionResult)new OkObjectResult(visits)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
        
        [FunctionName("MyGymBook")]
        public async Task<IActionResult> Book(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("Book triggered");

            var message = await _gymService.StartReservationAsync(DateTime.Now);

            return new OkObjectResult(message);
        }
        
        [FunctionName("MyGymCancelBook")]
        public async Task<IActionResult> CancelBook(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("CancelBook triggered");

            var message = await _gymService.CancelReservationAsync();

            return new OkObjectResult(message);
        }
    }
}