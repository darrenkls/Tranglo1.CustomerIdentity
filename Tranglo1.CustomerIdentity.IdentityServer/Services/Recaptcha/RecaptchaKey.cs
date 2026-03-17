namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha
{
    public class RecaptchaKey
    {
        public string SecretKey { get; set; }
        public string SiteKey { get; set; }
        public double RecaptchaScore { get; set; }
    }
}
