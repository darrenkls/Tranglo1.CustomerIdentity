using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO
{
    public class ChangeCustomerTypeInputDTO
    {
        public long? NewCustomerTypeCode { get; set; }
        public long? CurrentCustomerTypeCode { get; set; }
    }
}