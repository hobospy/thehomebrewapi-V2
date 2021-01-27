using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using thehomebrewapi.Models;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            // Create links for root
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetRoot", new { }),
                "self",
                "GET"));

            links.Add(
                new LinkDto(Url.Link("GetRecipes", new { }),
                "recipes",
                "GET"));

            links.Add(
                new LinkDto(Url.Link("CreateRecipe", new { }),
                "create_recipe",
                "POST"));

            links.Add(
                new LinkDto(Url.Link("GetBrews", new { }),
                "brews",
                "GET"));

            links.Add(
                new LinkDto(Url.Link("CreateBrew", new { }),
                "create_brew",
                "POST"));

            links.Add(
                new LinkDto(Url.Link("GetWaterProfiles", new { }),
                "water_profiles",
                "GET"));

            links.Add(
                new LinkDto(Url.Link("CreateWaterProfile", new { }),
                "create_water_profile",
                "POST"));

            return Ok(links);
        }
    }
}
