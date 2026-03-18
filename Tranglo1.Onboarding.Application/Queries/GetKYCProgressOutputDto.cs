namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetKYCProgressOutputDto
    {
        public long CategoryCode { get; set; }
        public string Category { get; set; }
        public int Status { get; set; }
        public string StatusDesc { get; set; }
    }
}
