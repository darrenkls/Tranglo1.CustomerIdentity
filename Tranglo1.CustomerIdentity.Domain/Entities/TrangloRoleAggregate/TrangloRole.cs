using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class TrangloRole : AggregateRoot<string>
    {
        public TrangloDepartment TrangloDepartment{ get; set; }
        public RoleStatus RoleStatus { get; set; }
        public AuthorityLevel AuthorityLevel { get; set; }
        public string Description { get; set; }
        public bool? IsSuperApprover { get; set; }
        public string CreatorRole { get; set; }
        public TrangloRole() 
        {

        }
        public TrangloRole(string code, string description, TrangloDepartment trangloDepartment, AuthorityLevel authorityLevel, string creatorRole)
        {
            this.Id = code;
            this.Description = description;
            this.TrangloDepartment = trangloDepartment;
            this.RoleStatus = RoleStatus.Active;
            this.AuthorityLevel = authorityLevel;
            this.CreatorRole = creatorRole;
        }
    }
}
