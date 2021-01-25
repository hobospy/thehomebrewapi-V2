using System;
using System.Collections.Generic;

namespace thehomebrewapi.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; private set; }
        public bool Revert { get; private set; }

        public PropertyMappingValue(IEnumerable<string> desinationProperties,
            bool revert = false)
        {
            DestinationProperties = desinationProperties ?? throw new ArgumentNullException(nameof(desinationProperties));
            Revert = revert;
        }
    }
}
