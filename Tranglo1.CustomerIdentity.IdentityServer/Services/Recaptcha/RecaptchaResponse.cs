using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha
{
    internal class RecaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("challenge_ts")]
        public DateTime Challenge_ts { get; set; }
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
        [JsonProperty("error-codes")]
        public List<string> Error_codes { get; set; }

        internal bool IsSuccess()
        {
            //"success" simply means that we sent a well formed request with the right token and secret. Whether this request was a valid reCAPTCHA token for your site 
            //"score" - the score for this request (0.0 - 1.0)
            return this.Success;
        }
    }
}
