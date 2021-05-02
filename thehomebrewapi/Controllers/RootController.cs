using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using thehomebrewapi.Helpers;
using thehomebrewapi.Models;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ExtendedControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            // Create links for root
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetRoot", new { }),
                "self",
                this.GET));

            links.Add(
                new LinkDto(Url.Link("GetRecipes", new { }),
                "recipes",
                this.GET));

            links.Add(
                new LinkDto(Url.Link("CreateRecipe", new { }),
                "create_recipe",
                this.POST));

            links.Add(
                new LinkDto(Url.Link("GetBrews", new { }),
                "brews",
                this.GET));

            links.Add(
                new LinkDto(Url.Link("CreateBrew", new { }),
                "create_brew",
                this.POST));

            links.Add(
                new LinkDto(Url.Link("GetWaterProfiles", new { }),
                "water_profiles",
                this.GET));

            links.Add(
                new LinkDto(Url.Link("CreateWaterProfile", new { }),
                "create_water_profile",
                this.POST));

            links.Add(
                new LinkDto(Url.Link("GetEnums", new { }),
                "enums",
                this.GET));

            return Ok(links);
        }
    }
}
