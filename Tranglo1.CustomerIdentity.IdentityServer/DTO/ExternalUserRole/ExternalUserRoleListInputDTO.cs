using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole
{
    public class ExternalUserRoleListInputDTO
    {
        public string ExternalUserRoleName { get; set; }
        public int? ExternalUserRoleStatusCode { get; set; }
        public long? SolutionCode { get; set; }
    }
}
