using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyUrl.Api.dtos
{
    public class CreateShortUrlRequest
    {
        public string OriginalUrl { get; set; }

        public bool IsPrivate { get; set; }
    }
}
