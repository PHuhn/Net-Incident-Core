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
        //
        public static string LogToString(this LogData entity)
        {
            //
            StringBuilder _return = new StringBuilder("record:[");
            _return.AppendFormat("Id: {0}, ", entity.Id.ToString());
            _return.AppendFormat("Date: {0}, ", entity.Date.ToString());
            _return.AppendFormat("Application: {0}, ", entity.Application);
            _return.AppendFormat("Method: {0}, ", entity.Method);
            _return.AppendFormat("LogLevel: {0}, ", entity.LogLevel.ToString());
            _return.AppendFormat("Level: {0}, ", entity.Level);
            _return.AppendFormat("UserAccount: {0}, ", entity.UserAccount);
            _return.AppendFormat("Message: {0}, ", entity.Message);
            _return.AppendFormat("Exception: {0}]", entity.Exception);
            return _return.ToString();
            //
        }
        //
    }
    //
}
// ---------------------------------------------------------------------------
