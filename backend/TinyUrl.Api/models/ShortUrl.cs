using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyUrl.Api.models
{
    public class ShortUrl
    {
        public Guid Id { get; set; }

        public string OriginalUrl { get; set; } = string.Empty;

        public string ShortCode { get; set; } = string.Empty;

        public bool IsPrivate { get; set; }

        public int ClickCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
