using System;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.Watchlist
{
    public class SingleScreeningListResultOutputDTO
    {
        public string ClientReference { set; get; }
        public Guid Reference { set; get; }
        public Summary Summary { set; get; }
    }

    public class Summary
    {
        public int SanctionList { set; get; }
        public int PEP { set; get; }
        public int SOE { set; get; }
        public int AdverseMedia { set; get; }
        public int Enforcement { get; set; }
        public int AssociatedEntity { get; set; }
    }
}
