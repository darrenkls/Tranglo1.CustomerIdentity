namespace Tranglo1.CustomerIdentity.IdentityServer.Models
{
    public class InviteePasswordVerificationViewModel : InviteePasswordVerificationInputModel
    {
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string ResetPasswordToken { get; set; }
        public string EmailConfirmationToken { get; set; }
    }
}
