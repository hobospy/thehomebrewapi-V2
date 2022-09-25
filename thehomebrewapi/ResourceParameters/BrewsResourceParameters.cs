using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.ResourceParameters
{
    public class BrewsResourceParameters : BaseResourceParameters
    {
        public double MinRating { get; set; } = 0.0;
        public ETypeOfAdditionalInfo IncludeAdditionalInfo { get; set; } = ETypeOfAdditionalInfo.None;
    }
}