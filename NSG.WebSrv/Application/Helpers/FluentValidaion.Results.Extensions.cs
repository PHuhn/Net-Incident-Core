//
// ---------------------------------------------------------------------------
//
using System;
using System.Text;
using System.Collections.Generic;
//
namespace FluentValidation.Results
{
    public static class Extensions
    {
        //
        public static string FluentValidationErrors(this ValidationResult results)
        {
            if (!results.IsValid)
            {
                StringBuilder _sb = new StringBuilder();
                _sb.AppendFormat("[");
                foreach (var _failure in results.Errors)
                {
                    _sb.AppendFormat("Field: {0}: {1}\n", _failure.PropertyName, _failure.ErrorMessage);
                }
                _sb.AppendFormat("]");
                return _sb.ToString();
            }
            return String.Empty;
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static Dictionary<string, string> FluentValidationToDictionary(this ValidationResult results)
        {
            Dictionary<string, string> _failures = new Dictionary<string, string>();
            if (!results.IsValid)
            {
                foreach (var _failure in results.Errors)
                {
                    _failures.Add(_failure.PropertyName, _failure.ErrorMessage);
                }
            }
            return _failures;
        }
    }
}
