using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyUrl.Api.dtos
{
    public class ShortUrlResponse
    {
        public Guid Id { get; set; }

        public string OriginalUrl { get; set; }

        public string ShortCode { get; set; }

        public string ShortUrl { get; set; }

        public bool IsPrivate { get; set; }

        public int ClickCount { get; set; }

    }
}
