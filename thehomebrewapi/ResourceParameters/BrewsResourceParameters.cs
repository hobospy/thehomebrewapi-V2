﻿namespace thehomebrewapi.ResourceParameters
{
    public class BrewsResourceParameters : BaseResourceParameters
    {
        public double MinRating { get; set; } = 0.0;
        public bool IncludeAdditionalInfo { get; set; } = false;
    }
}
