using System;
using System.Collections.Generic;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class TrangloStaff : ApplicationUser
    {
        //public override string LoginId => this?.FullName?.Value;

        private List<TrangloStaffAssignment> trangloStaffAssignments = new List<TrangloStaffAssignment>();
        public IReadOnlyList<TrangloStaffAssignment> TrangloStaffAssignments => this.trangloStaffAssignments.AsReadOnly();
        private List<TrangloStaffEntityAssignment> trangloStaffEntityAssignments = new List<TrangloStaffEntityAssignment>();
        public IReadOnlyList<TrangloStaffEntityAssignment> TrangloStaffEntityAssignments => this.trangloStaffEntityAssignments.AsReadOnly();

        protected TrangloStaff() : base()
        {

        }
        
            //=> AccountStatus = AccountStatus.Active;

        public TrangloStaff(string loginId, FullName fullName, Email email) 
            : base(fullName, email)
		{
			if (string.IsNullOrEmpty(loginId))
			{
                throw new ArgumentException(nameof(loginId));
			}

            base.LoginId = loginId;
            this.SetAccountStatus(AccountStatus.Active);
            //AccountStatus = AccountStatus.Active;           
        }

        public TrangloStaffAssignment AssignToTrangloEntityAssignment(TrangloEntity trangloEntity, TrangloDepartment trangloDepartment, TrangloRole trangloRole)
        {
            TrangloStaffAssignment trangloStaffAssignment = this.trangloStaffAssignments.Find(x => x.LoginId == this.LoginId
                                                                                                && x.TrangloEntity == trangloEntity.TrangloEntityCode
                                                                                                && x.TrangloDepartmentCode == trangloDepartment.Id
                                                                                                && x.RoleCode == trangloRole.Id);
            
            if (trangloStaffAssignment != null)
            {
                return trangloStaffAssignment;
            }

            trangloStaffAssignment = new TrangloStaffAssignment(trangloDepartment.Id, trangloRole.Id, trangloEntity.TrangloEntityCode, LoginId)
            {
                LoginId = this.LoginId,
                TrangloEntity = trangloEntity.TrangloEntityCode,
                TrangloDepartmentCode = trangloDepartment.Id,
                RoleCode = trangloRole.Id
            };

            trangloStaffAssignments.Add(trangloStaffAssignment);
            
         


            return trangloStaffAssignment;
        }

        public TrangloStaffEntityAssignment AssignToTrangloStaffEntityAssignment(TrangloEntity trangloEntity)
        {
            TrangloStaffEntityAssignment trangloStaffEntityAssignment = this.trangloStaffEntityAssignments.Find(x => x.LoginId == this.LoginId
                                                                                                             && x.TrangloEntity == trangloEntity.TrangloEntityCode);

            if (trangloStaffEntityAssignment != null)
            {
                return trangloStaffEntityAssignment;
            }
       
            trangloStaffEntityAssignment = new TrangloStaffEntityAssignment(trangloEntity.TrangloEntityCode, LoginId)
            {
                LoginId = this.LoginId,
                TrangloEntity = trangloEntity.TrangloEntityCode,
                AccountStatus = Enumeration.FindById<CompanyUserAccountStatus>(1),
                BlockStatus = Enumeration.FindById<CompanyUserBlockStatus>(2)
            };

            trangloStaffEntityAssignments.Add(trangloStaffEntityAssignment);

            return trangloStaffEntityAssignment;
        }

        public void RemoveTrangloEntityAssignment(TrangloEntity trangloEntity, TrangloDepartment trangloDepartment, TrangloRole trangloRole)
        {
            TrangloStaffAssignment _trangloStaffAssignment = this.trangloStaffAssignments.Find(x => x.LoginId == this.LoginId
                                                                                    && x.TrangloEntity == trangloEntity.TrangloEntityCode
                                                                                    && x.TrangloDepartmentCode == trangloDepartment.Id
                                                                                    && x.RoleCode == trangloRole.Id);

            if (_trangloStaffAssignment != null)
            {
                trangloStaffAssignments.Remove(_trangloStaffAssignment);
            }

            var result = trangloStaffAssignments.Find(x => x.TrangloEntity == trangloEntity.TrangloEntityCode && x.LoginId == LoginId);
            TrangloStaffEntityAssignment _trangloStaffEntityAssignment = this.trangloStaffEntityAssignments.Find(x => x.LoginId == this.LoginId
                                                                                                                && x.TrangloEntity == trangloEntity.TrangloEntityCode);
            
            if(result == null)
            {
                trangloStaffEntityAssignments.Remove(_trangloStaffEntityAssignment);
            }
            
        }

        //public void RemoveTrangloStaffEntityAssignment(TrangloEntity trangloEntity, int count)
        //{
        //    var asd = _
        //}AccountStatus.Active
    }
}