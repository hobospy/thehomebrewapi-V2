using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using thehomebrewapi.Models;
using thehomebrewapi.Services;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/waterProfiles")]
    public class WaterProfilesController : ControllerBase
    {
        private readonly IHomeBrewRepository _homeBrewRepository;
        private readonly IMapper _mapper;

        public WaterProfilesController(IHomeBrewRepository homeBrewRepository, IMapper mapper)
        {
            _homeBrewRepository = homeBrewRepository ??
                throw new ArgumentNullException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetWaterProfiles()
        {
            var waterProfiles = _homeBrewRepository.GetWaterProfiles();

            return Ok(_mapper.Map<IEnumerable<WaterProfileDto>>(waterProfiles));
        }

        [HttpGet("{id}", Name = "GetWaterProfile")]
        public IActionResult GetWaterProfile(int id, bool includeAdditions = false)
        {
            var waterProfile = _homeBrewRepository.GetWaterProfile(id, includeAdditions);

            if (waterProfile == null)
            {
                return NotFound();
            }

            if (includeAdditions)
            {
                return Ok(_mapper.Map<WaterProfileDto>(waterProfile));
            }

            return Ok(_mapper.Map<WaterProfileWithoutAdditionsDto>(waterProfile));
        }

        [HttpPost]
        public IActionResult CreateWaterProfile([FromBody] WaterProfileForCreationDto waterProfile)
        {
            foreach(var addition in waterProfile.Additions)
            {
                if (addition.Amount == 0)
                {
                    ModelState.AddModelError(
                            "Description",
                            "The amount for all water profile additions must be a value greater than 0.");

                    break;
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var finalWaterProfile = _mapper.Map<Entities.WaterProfile>(waterProfile);

            _homeBrewRepository.AddWaterProfile(finalWaterProfile);
            _homeBrewRepository.Save();

            var createdWaterProfileToReturn = _mapper.Map<Models.WaterProfileDto>(finalWaterProfile);

            return CreatedAtRoute(
                "GetWaterProfile",
                new { id = finalWaterProfile.Id, includeAdditions = true },
                createdWaterProfileToReturn);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateWaterProfile(int id, [FromBody] WaterProfileForUpdateDto waterProfile)
        {
            foreach (var addition in waterProfile.Additions)
            {
                if (addition.Amount == 0)
                {
                    ModelState.AddModelError(
                            "Addition amount error",
                            $"The amount for water profile addition {addition.Name} must be a value greater than 0.");
                }

                if (!Enum.IsDefined(typeof(EUnitOfMeasure), addition.Unit))
                {
                    ModelState.AddModelError(
                            "Addition unit error",
                            $"The unit for water profile addition {addition.Name} must be a valid unit of measure.");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var waterProfileEntity = _homeBrewRepository.GetWaterProfile(id, true);
            if (waterProfileEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(waterProfile, waterProfileEntity);

            _homeBrewRepository.UpdateWaterProfile(waterProfileEntity);

            _homeBrewRepository.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartialUpdateWaterProfile(int id,
            [FromBody] JsonPatchDocument<WaterProfileForUpdateDto> patchDoc)
        {
            var waterProfileEntity = _homeBrewRepository.GetWaterProfile(id, true);
            if (waterProfileEntity == null)
            {
                return NotFound();
            }

            var waterProfileToPatch = _mapper.Map<WaterProfileForUpdateDto>(waterProfileEntity);

            patchDoc.ApplyTo(waterProfileToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var addition in waterProfileToPatch.Additions)
            {
                if (addition.Amount == 0)
                {
                    ModelState.AddModelError(
                            "Addition amount error",
                            $"The amount for water profile addition {addition.Name} must be a value greater than 0.");
                }

                if (!Enum.IsDefined(typeof(EUnitOfMeasure), addition.Unit))
                {
                    ModelState.AddModelError(
                            "Addition unit error",
                            $"The unit for water profile addition {addition.Name} must be a valid unit of measure.");
                }
            }

            if (!TryValidateModel(waterProfileToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(waterProfileToPatch, waterProfileEntity);

            _homeBrewRepository.UpdateWaterProfile(waterProfileEntity);

            _homeBrewRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteWaterProfile(int id)
        {
            var waterProfileEntity = _homeBrewRepository.GetWaterProfile(id, false);
            if (waterProfileEntity == null)
            {
                return NotFound();
            }

            _homeBrewRepository.DeleteWaterProfile(waterProfileEntity);

            _homeBrewRepository.Save();

            return NoContent();
        }
    }
}
