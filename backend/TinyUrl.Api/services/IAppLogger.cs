using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyUrl.Api.services
{
    public interface IAppLogger
    {
        void Log(string message);
    }
}
