//
// ---------------------------------------------------------------------------
// This class maps to the appsettings.json 'ServicesSettings' JSON class.
//
using System;
//
namespace NSG.WebSrv.Infrastructure.Common
{
    //
    /// <summary>
    /// Matches the appsettings.json 'ApplicationSettings' JSON class,
    /// to be injected into services.
    /// </summary>
    public class ApplicationSettings
    {
        //
        /// <summary>
        /// Name of the current application
        /// </summary>
        public string Name { get; set; }
        //
        /// <summary>
        /// Phone number for the current application
        /// </summary>
        public string Phone { get; set; }
        //
    }
    //
}
