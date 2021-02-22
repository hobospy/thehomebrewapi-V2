using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using thehomebrewapi.Helpers;
using thehomebrewapi.Models;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/enums")]
    public class EnumsController : ExtendedControllerBase
    {
        private readonly List<string> _availableEnums = new List<string>()
                                            {
                                                nameof(ETypeOfBeer),
                                                nameof(ETypeOfIngredient),
                                                nameof(EUnitOfMeasure),
                                                nameof(ETypeOfDuration),
                                                nameof(EBrewedState)
                                            };

        [HttpGet(Name = "GetEnums")]
        [HttpHead]
        public ActionResult<IEnumerable<string>> GetEnums()
        {
            return Ok(_availableEnums);
        }

        [HttpGet("{enumType}")]
        public ActionResult<IEnumerable<EnumDto>> GetEnumDetails(string enumType)
        {
            if (!_availableEnums.Contains(enumType, StringComparer.InvariantCultureIgnoreCase))
            {
                return BadRequest();
            }

            var returnValue = new List<EnumDto>();
            if (enumType.Equals(nameof(ETypeOfBeer), StringComparison.InvariantCultureIgnoreCase))
            {
                returnValue = EnumExtensions.GetEnumValues<ETypeOfBeer>().ToList();
            }
            if (enumType.Equals(nameof(ETypeOfIngredient), StringComparison.InvariantCultureIgnoreCase))
            {
                returnValue = EnumExtensions.GetEnumValues<ETypeOfIngredient>().ToList();
            }
            if (enumType.Equals(nameof(EUnitOfMeasure), StringComparison.InvariantCultureIgnoreCase))
            {
                returnValue = EnumExtensions.GetEnumValues<EUnitOfMeasure>().ToList();
            }
            if (enumType.Equals(nameof(ETypeOfDuration), StringComparison.InvariantCultureIgnoreCase))
            {
                returnValue = EnumExtensions.GetEnumValues<ETypeOfDuration>().ToList();
            }
            if (enumType.Equals(nameof(EBrewedState), StringComparison.InvariantCultureIgnoreCase))
            {
                returnValue = EnumExtensions.GetEnumValues<EBrewedState>().ToList();
            }

            return Ok(returnValue);
        }

        [HttpOptions]
        public ActionResult GetEnumsOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.GET}, {this.HEAD}, {this.OPTIONS}");
            return Ok();
        }

        [HttpOptions("{enumType}")]
        public ActionResult GetEnumOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.GET}, {this.OPTIONS}");
            return Ok();
        }
    }
}
