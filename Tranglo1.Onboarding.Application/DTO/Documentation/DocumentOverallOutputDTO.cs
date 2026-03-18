using System;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.Documentation
{
    public class DocumentOverallOutputDTO
    {
        public int BusinessProfileCode { get; set; }
        public long DocumentCategoryCode { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
