using Microsoft.AspNetCore.Mvc;

namespace thehomebrewapi.Helpers
{
    public class ExtendedControllerBase : ControllerBase
    {
        internal const string ACCEPT = "Accept";
        internal readonly string ALLOW = "Allow";

        internal readonly string DELETE = "DELETE";
        internal readonly string GET = "GET";
        internal readonly string HEAD = "HEAD";
        internal readonly string OPTIONS = "OPTIONS";
        internal readonly string PATCH = "PATCH";
        internal readonly string POST = "POST";
        internal readonly string PUT = "PUT";

        internal readonly string LINKS = "links";
        internal readonly string NEXT_PAGE = "nextPage";
        internal readonly string PREVIOUS_PAGE = "previousPage";
        internal readonly string SELF = "self";

        internal readonly string HATEOAS = "hateoas";
        internal readonly string PAGINATION_HEADER = "X-Pagination";
    }
}
