using System.Collections.Generic;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.MFA
{
    public class ResetMFAForRequestOutputDTO
    {
        public static readonly string RESET_MFA_SUCCESSFUL_MESSAGE = "2FA reset successful";
        public static readonly string RESET_MFA_EXPIRED_MESSAGE = "Reset link has expired";
        public static readonly string RESET_MFA_USED_MESSAGE = "Reset link already used";

        public ResetMFAForRequestStatus Status { get; private set; }
        public string Message { get; private set; }

        public ResetMFAForRequestOutputDTO(ResetMFAForRequestStatus status)
        {
            Status = status;
            Message = status switch
            {
                ResetMFAForRequestStatus.Successful => RESET_MFA_SUCCESSFUL_MESSAGE,
                ResetMFAForRequestStatus.Expired => RESET_MFA_EXPIRED_MESSAGE,
                ResetMFAForRequestStatus.Used => RESET_MFA_USED_MESSAGE,
            };
        }

        public static ResetMFAForRequestStatus? GetStatusByMessage(string message)
        {
            Dictionary<string, ResetMFAForRequestStatus> dict = new Dictionary<string, ResetMFAForRequestStatus>
            {
                { RESET_MFA_SUCCESSFUL_MESSAGE, ResetMFAForRequestStatus.Successful },
                { RESET_MFA_EXPIRED_MESSAGE, ResetMFAForRequestStatus.Expired },
                { RESET_MFA_USED_MESSAGE, ResetMFAForRequestStatus.Used }
            };

            return dict.TryGetValue(message, out ResetMFAForRequestStatus status)
                ? status
                : (ResetMFAForRequestStatus?)null;
        }
    }

    public enum ResetMFAForRequestStatus
    {
        Successful = 0,
        Expired,
        Used
    }
}
