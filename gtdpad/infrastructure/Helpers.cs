using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gtdpad.infrastructure
{
    internal static class Helpers
    {
        public static bool IsJsonType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/json", StringComparison.OrdinalIgnoreCase) ||
                   contentMimeType.Equals("text/json", StringComparison.OrdinalIgnoreCase) ||
                  (contentMimeType.StartsWith("application/vnd", StringComparison.OrdinalIgnoreCase) &&
                   contentMimeType.EndsWith("+json", StringComparison.OrdinalIgnoreCase));
        }
    }
}
