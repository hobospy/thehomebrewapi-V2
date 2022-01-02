using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace thehomebrewapi.Helpers
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var path in swaggerDoc.Paths.ToList())
            {
                var nonRequiredOperations = path.Value.Operations
                    .Where(y => y.Key.ToString().ToLower().Contains("head") | y.Key.ToString().ToLower().Contains("options")).ToList();

                nonRequiredOperations.ForEach(y =>
                {
                    path.Value.Operations.Remove(y);
                });

                path.Value.Operations = path.Value.Operations.OrderBy(o => o.Key.ToString()).ToDictionary(o => o.Key, o => o.Value);
            }

            var paths = swaggerDoc.Paths.OrderBy(p =>
            {
                var sortIndex = p.Key.LastIndexOf('/');
                if (p.Key.Length > sortIndex + 1 && p.Key[sortIndex + 1] == '{' && sortIndex > 0)
                {
                    sortIndex = p.Key.LastIndexOf('/', sortIndex - 1);
                }

                var sortValue = sortIndex + 1 < p.Key.Length ? p.Key.Substring(sortIndex + 1) : p.Key;

                return sortValue;
            }).ToList();
            swaggerDoc.Paths.Clear();
            paths.ForEach(e => swaggerDoc.Paths.Add(e.Key, e.Value));
        }
    }
}