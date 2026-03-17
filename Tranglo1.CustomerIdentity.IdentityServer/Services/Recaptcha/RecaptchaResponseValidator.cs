using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha
{
    public static class RecaptchaResponseValidator
    {
        public static Result<bool> Validate(string recaptchaSecretKey, string encodedResponse, double recaptchaScore, IHttpClientFactory _httpClientFactory)
        {
            var secret = recaptchaSecretKey;
            RecaptchaResponse recaptchaResponse = new RecaptchaResponse();

            var client = _httpClientFactory.CreateClient();

            var googleReply = client.GetStringAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={encodedResponse}");

            recaptchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(googleReply.Result);

            string error_codes = "";
            if (!recaptchaResponse.IsSuccess() && recaptchaResponse.Score <= recaptchaScore)
            {
                error_codes = String.Join(" ", recaptchaResponse.Error_codes.ToArray());
                return Result.Failure<bool>(error_codes);
            }

            return Result.Success<bool>(recaptchaResponse.IsSuccess() && recaptchaResponse.Score >= recaptchaScore);
        }
    }
}
