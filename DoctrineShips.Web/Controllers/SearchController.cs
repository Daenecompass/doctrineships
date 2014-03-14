﻿namespace DoctrineShips.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI;
    using DevTrends.MvcDonutCaching;
    using DoctrineShips.Service;
    using DoctrineShips.Web.ViewModels;
    using Tools;

    public class SearchController : Controller
    {
        private readonly IDoctrineShipsServices doctrineShipsServices;

        public SearchController(IDoctrineShipsServices doctrineShipsServices)
        {
            this.doctrineShipsServices = doctrineShipsServices;
        }

        [DonutOutputCache(Duration = 300, VaryByCustom = "Account", Location = OutputCacheLocation.Server)]
        public ActionResult CustomerContracts(string customerId)
        {
            // Cleanse the passed customer id string to prevent XSS.
            int cleanCustomerId = Conversion.StringToInt32(Server.HtmlEncode(customerId));

            // Instantiate a new view model to populate the view.
            SearchCustomerContractsViewModel viewModel = new SearchCustomerContractsViewModel();
            
            // Fetch the customer details.
            viewModel.Customer = this.doctrineShipsServices.GetCustomer(cleanCustomerId);

            // Get a list of outstanding item exchange contracts for the passed customer. 
            var unsortedContracts = this.doctrineShipsServices.GetCustomerContracts(cleanCustomerId);

            // Group the list by station.
            viewModel.Contracts = unsortedContracts
                                  .OrderBy(o => o.DateExpired)
                                  .GroupBy(u => u.StartStationId)
                                  .Select(grp => grp.ToList())
                                  .ToList();

            return View(viewModel);
        }

        [DonutOutputCache(Duration = 300, VaryByCustom = "Account", Location = OutputCacheLocation.Server)]
        public ActionResult CustomerContractsStation(string stationId, string customerId)
        {
            // Cleanse the passed station and customer id strings to prevent XSS.
            int cleanCustomerId = Conversion.StringToInt32(Server.HtmlEncode(customerId));
            int cleanStationId = Conversion.StringToInt32(Server.HtmlEncode(stationId));

            // Instantiate a new view model to populate the view.
            SearchCustomerContractsStationViewModel viewModel = new SearchCustomerContractsStationViewModel();

            // Fetch the customer details.
            viewModel.Customer = this.doctrineShipsServices.GetCustomer(cleanCustomerId);

            // Get a list of outstanding item exchange contracts for the passed customer id. 
            var unsortedContracts = this.doctrineShipsServices.GetCustomerContracts(cleanCustomerId);

            // Filter the list by the station id.
            viewModel.Contracts = unsortedContracts
                                    .Where(x => x.StartStationId == cleanStationId)
                                    .OrderBy(o => o.DateExpired)
                                    .ToList();

            return View(viewModel);
        }

        [DonutOutputCache(Duration = 300, VaryByCustom = "Account", Location = OutputCacheLocation.Server)]
        public ActionResult SalesAgentContracts(string salesAgentId)
        {
            // Cleanse the passed salesAgent id string to prevent XSS.
            int cleanSalesAgentId = Conversion.StringToInt32(Server.HtmlEncode(salesAgentId));

            // Instantiate a new view model to populate the view.
            SearchSalesAgentContractsViewModel viewModel = new SearchSalesAgentContractsViewModel();

            // Fetch the salesAgent details.
            viewModel.SalesAgent = this.doctrineShipsServices.GetSalesAgent(cleanSalesAgentId);

            // Get a list of outstanding item exchange contracts for the passed salesAgent. 
            var unsortedContracts = this.doctrineShipsServices.GetSalesAgentContracts(cleanSalesAgentId);

            // Group the list by station.
            viewModel.Contracts = unsortedContracts
                                  .OrderBy(o => o.DateExpired)
                                  .GroupBy(u => u.StartStationId)
                                  .Select(grp => grp.ToList())
                                  .ToList();

            return View(viewModel);
        }
    }
}