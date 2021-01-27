namespace thehomebrewapi.ResourceParameters
{
    public class RecipesResourceParameters : BaseResourceParameters
    {
        const short INVALID_BEER_TYPE = -1;

        public short BeerType { get; set; } = INVALID_BEER_TYPE;
    }
}
