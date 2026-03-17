using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{    
    public class TrangloStaffAssignment
    {
        public string LoginId { get; set; }
        public string TrangloEntity { get; set; }
        public string RoleCode { get; set; }
        public TrangloDepartment TrangloDepartment { get; set; }
        public long TrangloDepartmentCode { get; set; }
        
        public TrangloStaffAssignment(long department, string role, string entity, string loginId)
        {
            this.TrangloDepartmentCode = department;
            this.TrangloEntity = entity;
            this.RoleCode = role;
            this.LoginId = loginId;
        }
        public TrangloStaffAssignment()
        {

        }
        /*
        public string LoginId { get; set; }
        public TrangloStaffEntityAssignment TrangloStaffEntityAssignment { get; set; }
        public TrangloRole TrangloRole { get; set; }
        public TrangloDepartment TrangloDepartment { get; set; }
        */
    }    
}
