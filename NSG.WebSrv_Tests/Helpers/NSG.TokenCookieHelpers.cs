using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace NSG.Integration.Helpers
{
    public static class TokenCookieHelpers
    {
        //
        /// <summary>
        /// 
        /// </summary>
        public static SetCookieHeaderValue _authenticationCookie;

        public static Dictionary<string, string> LoginFormData = new Dictionary<string, string>
            { { "Email", TestData.User_Name }, { "Password", TestData.Password } };
        public static Regex AntiForgeryFormFieldRegex = new Regex(
            @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
        //
        private static string _antiForgeryToken;
        private static SetCookieHeaderValue _antiForgeryCookie;
        private static string AUTHENTICATION_COOKIE = ".AspNetCore.Identity.";
        private static string ANTIFORGERY_COOKIE = ".AspNetCore.AntiForgery.";
        private static string ANTIFORGERY_TOKEN_FORM = "__RequestVerificationToken";
        private static string ANTIFORGERTY_TOKEN_HEADER = "XSRF-TOKEN";
        //
        // AntiForgery
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public static async Task<string> EnsureAntiForgeryTokenAsync(HttpClient httpClient, string controllerPath)
        {
            if (_antiForgeryToken != null) return _antiForgeryToken;
            //
            var _response = await httpClient.GetAsync(controllerPath); //
            _response.EnsureSuccessStatusCode();
            _antiForgeryCookie = TryGetAntiForgeryCookie(_response);
            Assert.IsNotNull(_antiForgeryCookie);
            //
            AddCookieToDefaultRequestHeader(httpClient, _antiForgeryCookie);
            _antiForgeryToken = await GetAntiForgeryTokenAsync(_response);
            Assert.IsNotNull(_antiForgeryToken);
            //
            return _antiForgeryToken;
        }
        //
        /// <summary>
        /// Get the anti-forgery cookie from the response header.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static SetCookieHeaderValue TryGetAntiForgeryCookie(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                return SetCookieHeaderValue.ParseList(values.ToList())
                    .SingleOrDefault(
                        c => c.Name.StartsWith(
                            ANTIFORGERY_COOKIE,
                            StringComparison.InvariantCultureIgnoreCase));
            }
            return null;
        }
        //
        /// <summary>
        /// Add anti-forgery cookie to default request header
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="antiForgeryCookie"></param>
        private static void AddCookieToDefaultRequestHeader( HttpClient httpClient,
            SetCookieHeaderValue antiForgeryCookie)
        {
            httpClient.DefaultRequestHeaders.Add(
                "Cookie",
                new CookieHeaderValue(antiForgeryCookie.Name, antiForgeryCookie.Value)
                    .ToString());
        }
        //
        /// <summary>
        /// Extract the anti-forgery token from the content.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>string of the token.</returns>
        private static async Task<string> GetAntiForgeryTokenAsync(HttpResponseMessage response)
        {
            var responseHtml = await response.Content.ReadAsStringAsync();
            var match = AntiForgeryFormFieldRegex.Match(responseHtml);

            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="controllerPath"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> EnsureAntiForgeryTokenFormAsync(HttpClient httpClient, string controllerPath, Dictionary<string, string> formData = null)
        {
            if (formData == null) formData = new Dictionary<string, string>();
            formData.Add(ANTIFORGERY_TOKEN_FORM, await EnsureAntiForgeryTokenAsync(httpClient, controllerPath));
            //
            return formData;
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="controllerPath"></param>
        /// <returns></returns>
        public static async Task EnsureAntiForgeryTokenHeaderAsync(HttpClient httpClient, string controllerPath)
        {
            httpClient.DefaultRequestHeaders.Add(ANTIFORGERTY_TOKEN_HEADER, await EnsureAntiForgeryTokenAsync(httpClient, controllerPath));
        }
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="loginPath"></param>
        /// <returns></returns>
        public static async Task EnsureAuthenticationCookieAsync(HttpClient httpClient, string loginPath)
        {
            if (_authenticationCookie != null) return;
            var formData = await EnsureAntiForgeryTokenFormAsync(httpClient, loginPath, LoginFormData);
            // "/Account/Login"
            var _response = await httpClient.PostAsync(loginPath, new FormUrlEncodedContent(formData));
            _response.EnsureRedirectStatusCode(); // (implemented below)
            //
            if (_response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                _authenticationCookie = SetCookieHeaderValue.ParseList(values.ToList())
                    .SingleOrDefault(c => c.Name.StartsWith(AUTHENTICATION_COOKIE, StringComparison.InvariantCultureIgnoreCase));
            }
            Assert.IsNotNull(_authenticationCookie);
            httpClient.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_authenticationCookie.Name, _authenticationCookie.Value).ToString());
            //
            // The current pair of antiforgery cookie-token is not valid anymore
            // Since the tokens are generated based on the authenticated user!
            // We need a new token after authentication (The cookie can stay the same)
            _antiForgeryToken = null;
        }
        //
        public static void EnsureRedirectStatusCode(this HttpResponseMessage response)
        {
            if( response.StatusCode != HttpStatusCode.Redirect)
            {
                throw new HttpRequestException(string.Format("Value: {0}", response.StatusCode));
            }
        }
        //
    }
}