namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class TeamUserListOutputDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string CompanyName { get; set; }
        public string UserEnvironment { get; set; }
        public string AccountStatus { get; set; }
    }
}
