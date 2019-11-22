using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSG.WebSrv.Infrastructure.Authentication
{
    public class AuthSettings
    {
        //
        /// <summary>
        /// Private jwt secret, read from appsettings.json file
        /// </summary>
        public string JwtSecret { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public int JwtExpireMinutes { get; set; }
        //
        /// <summary>
        /// allow the augular SPA access to web API.
        /// </summary>
        public string CorsAllowOrigins { get; set; }
        //
    }
}
