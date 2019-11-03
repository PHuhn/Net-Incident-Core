// ---------------------------------------------------------------------------
using System;
using System.Text;
//
using NSG.WebSrv.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
//
namespace NSG.WebSrv.Application.Commands.Logs
{
    //
	/// <summary>
	/// Extension method.
	/// </summary>
	public static partial class Extensions
    {
        //
        /// <summary>
        /// Extension method that translates from Log to LogListQuery.
        /// </summary>
        /// <param name="entity">The Log entity class.</param>
        /// <returns>'LogListQuery' or Log list query.</returns>
        public static LogListQuery ToLogListQuery(this LogData entity)
        {
            return new LogListQuery
            {
                Date = entity.Date,
                Application = entity.Application,
                Method = entity.Method,
                Level = entity.Level,
                Message = entity.Message,
                Exception = entity.Exception,
            };
        }
    }
    //
}
// ---------------------------------------------------------------------------
