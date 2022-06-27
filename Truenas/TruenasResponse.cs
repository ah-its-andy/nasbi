using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NasBI.Truenas
{
    public class TruenasResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public byte[] Data { get; set; }
    }
}
