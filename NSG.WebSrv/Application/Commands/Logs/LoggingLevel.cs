// ===========================================================================
using System;
//
namespace NSG.WebSrv.Application.Commands.Logs
{
    //
    /// <summary>
    /// Enum of various logging levels
    /// Note:
    ///  [Range(typeof(byte), "0", "4", ErrorMessage = "'LogLevel' must be between 0 and 4")]
    /// </summary>
    public enum LoggingLevel
    {
        /// <summary>
        /// if one would like to implement some sort of change auditing
        /// </summary>
        Audit = 0,
        /// <summary>
        /// For exceptions and other errors
        /// </summary>
        Error = 1,
        /// <summary>
        /// Warning level of logs
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Info level of logs
        /// </summary>
        Info = 3,
        /// <summary>
        /// Debug level of logs
        /// </summary>
        Debug = 4,
        /// <summary>
        /// Verbose level of logs
        /// </summary>
        Verbose = 5
    };
    //
}
// ===========================================================================
