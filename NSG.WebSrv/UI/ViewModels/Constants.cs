//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSG.WebSrv.UI.ViewModels
{
    public static class Constants
    {
        // in AccountController
        public static string ServerNotFoundException = "{0} - 'server/device' not found: {1}";
        public static string UserNotFoundException = "{0} - Unable to load user with ID '{1}'.";
        public static string ConfirmEmailSubjectLine = "Confirm your email";
        public static string ConfirmEmailBody = "Please confirm your account by <a href='{0}'>clicking here</a>.";

        public static string ForgotPasswordSubjectLine = "Reset Password";
        public static string ForgotPasswordBody = "Please reset your password by <a href='{0}'>clicking here</a>.";
        //
        public static string ExternalBadRequestLoginFailure = "{0} - External login failure.";
        public static string ExternalBadRequestAlreadyInUse = "{0} - The external login is already associated with an account.";
        public static string StrengthInBitsException = "{0} - strengthInBits must be evenly divisible by 8.";
        public static string ServerNotValidException = "{0} - Must have a valid 'servers/devices'.";
        public static string UserCreateException = "{0} - User: {1}, creation error: {2}.";
        //
    }
}
//
