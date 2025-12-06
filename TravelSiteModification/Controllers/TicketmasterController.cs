using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TravelSiteModification.Models;
using TravelSiteModification.Services;

namespace TravelSiteModification.Controllers
{
    public class TicketmasterController : Controller
    {
        private readonly TicketmasterService _tmService;

        public TicketmasterController(TicketmasterService tmService)
        {
            _tmService = tmService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new TicketmasterSearchViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(TicketmasterSearchViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.City))
            {
                ModelState.AddModelError("City", "Please enter a city.");
                return View(model);
            }

            try
            {
                model.Results = await _tmService.SearchEventsAsync(
                    model.City?.Trim(),
                    model.StateCode?.Trim(),
                    model.Keyword?.Trim());

                model.HasSearched = true;

                if (model.Results.Count == 0)
                {
                    model.ErrorMessage = "No Ticketmaster events were found for that search.";
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Error contacting Ticketmaster API: " + ex.Message;
            }

            return View(model);
        }
    }
}
