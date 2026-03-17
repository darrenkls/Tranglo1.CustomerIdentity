namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class RolesAvailableToInviteeOutputDTO
    {
        public string UserEnvironment { get; set; }
        public long RoleCode { get; set; }
        public string RoleName { get; set; }
        public long DepartmentCode { get; set; }
    }
}
