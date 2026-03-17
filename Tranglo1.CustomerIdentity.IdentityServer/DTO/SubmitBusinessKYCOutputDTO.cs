using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO
{
    public class SubmitBusinessKYCOutputDTO
    {
        public DateTime? ReviewConcurrentLastModified { get; set; }
        public Guid? ReviewConcurrencyToken { get; set; }
    }
}
