namespace thehomebrewapi.ResourceParameters
{
    public class TastingNotesResourceParameters : BaseResourceParameters
    {
        public const int INVALID_BREW_ID = -1;

        public int BrewId { get; set; } = INVALID_BREW_ID;

        public TastingNotesResourceParameters()
        {
            OrderBy = "Date";
        }
    }
}
