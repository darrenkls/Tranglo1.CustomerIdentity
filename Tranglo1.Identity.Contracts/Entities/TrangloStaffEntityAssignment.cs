using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class TrangloStaffEntityAssignment 
    {
        public string LoginId { get; set; }
        public string TrangloEntity { get; set; }
       //public TrangloEntity TrangloEntity { get; set; }
        public CompanyUserBlockStatus BlockStatus { get; set; }
        public CompanyUserAccountStatus AccountStatus { get; set; }
        public int UserId { get; set; }
        public TrangloStaffEntityAssignment(string entity, string loginId)
        {
            this.TrangloEntity = entity;
            this.LoginId = loginId;
        }
        private TrangloStaffEntityAssignment()
        {

        }
    }
}
