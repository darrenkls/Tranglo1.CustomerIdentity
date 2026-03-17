using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha
{
    internal class ReCapchaValidator : ICapchaValidator
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly RecaptchaKey recaptchaKey;

        public ReCapchaValidator(IHttpClientFactory httpClientFactory, RecaptchaKey recaptchaKey)
        {
            this.httpClientFactory = httpClientFactory;
            this.recaptchaKey = recaptchaKey;
        }

        public async Task<Result<bool>> ValidateAsync(string token)
        {
            RecaptchaResponse recaptchaResponse = new RecaptchaResponse();
            using (var client = httpClientFactory.CreateClient())
            {
                var validation = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaKey.SecretKey}&response={token}");
                recaptchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(validation);

                if (recaptchaResponse.IsSuccess() && recaptchaResponse.Score >= recaptchaKey.RecaptchaScore)
                {
                    //var _RecaptchaResponse = await validation.Content.ReadFromJsonAsync<RecaptchaResponse>();
                    return Result.Success(recaptchaResponse.IsSuccess());
                }
                else
                {
                    var error_codes = String.Join(" ", recaptchaResponse.Error_codes.ToArray());
                    return Result.Failure<bool>(error_codes);
                }

            }
        }
    }
}
