using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.ExternalUserRoleAggregate
{
    public class ExternalUserRole : AggregateRoot<long>
    {
        public string RoleCode { get; private set; }
        public string ExternalUserRoleName { get; private set; }
        public ExternalUserRoleStatus ExternalUserRoleStatus { get; private set; }
        public Solution? Solution { get; private set; }
        private ExternalUserRole()
        {

        }


        public ExternalUserRole(string roleName, string roleCode, Solution solution)
        {
            ExternalUserRoleName = roleName;
            RoleCode = roleCode;
            SetRoleStatusCode(ExternalUserRoleStatus.Active);
            Solution = solution;
        }

        public void SetRoleStatusCode(ExternalUserRoleStatus roleStatus)
        {
            if (ExternalUserRoleStatus != roleStatus)
            {
                ExternalUserRoleStatus = roleStatus;
            }
        }
    }
}