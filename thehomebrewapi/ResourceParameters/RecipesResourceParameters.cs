namespace thehomebrewapi.ResourceParameters
{
    public class RecipesResourceParameters
    {
        const int INVALID_BEER_TYPE = -1;
        const int MAX_PAGE_SIZE = 50;

        public int BeerType { get; set; } = INVALID_BEER_TYPE;
        public string SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        { 
            get => _pageSize;
            set => _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }

        public string OrderBy { get; set; } = "Name";
    }
}
