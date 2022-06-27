using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NasBI.Common
{
    public class Config
    {
        public static Config LoadConfig()
        {
            return new Config();
        }
        public string UnraidServerHealthCheckUrl { get; set; } = "http://unraid.stdcore.io:60270";
        public string TruenasServerHealthCheckUrl { get; set; } = "http://nas.stdcore.io:60271";
    }
}
