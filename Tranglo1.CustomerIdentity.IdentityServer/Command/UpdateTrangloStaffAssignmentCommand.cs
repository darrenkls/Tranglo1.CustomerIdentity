using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;
using static Tranglo1.CustomerIdentity.IdentityServer.Queries.TrangloStaffUserUpdateInputDTO;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.TrangloUser, UACAction.Edit)]
    [Permission(Permission.ManageAdminUser.Action_Edit_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageAdminUser.Action_View_Code })]
    internal class UpdateTrangloStaffAssignmentCommand : BaseCommand<Result>
    {
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string Emails { get; set; }
        public string Timezone { get; set; }
        public long AccountStatus { get; set; }
        public List<RoleDepartmentEntityInputDTO> roleDepartmentEntityInput { get; set; }
        //public AdminUserModel AdminUser { get; set; }

        public override Task<string> GetAuditLogAsync(Result result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Edited User Details";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
        public class UpdateTrangloStaffAssignmentCommandHandler : IRequestHandler<UpdateTrangloStaffAssignmentCommand, Result>
        {
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ITrangloRoleRepository _trangloRoleRepository;
            private readonly ILogger<UpdateTrangloStaffAssignmentCommandHandler> _logger;
            public IUnitOfWork UnitOfWork { get; }

            public UpdateTrangloStaffAssignmentCommandHandler(
                    IApplicationUserRepository applicationUserRepository,
                    ILogger<UpdateTrangloStaffAssignmentCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    ITrangloRoleRepository trangloRoleRepository
                )
            {
                _applicationUserRepository = applicationUserRepository;
                _logger = logger;
                UnitOfWork = unitOfWork;
                _trangloRoleRepository = trangloRoleRepository;
            }
            public async Task<Result> Handle(UpdateTrangloStaffAssignmentCommand request, CancellationToken cancellationToken)
            {
                
                var trangloStaffUser = await _applicationUserRepository.GetTrangloUserByLoginId(request.LoginId);
                if(trangloStaffUser != null) { 

                    trangloStaffUser.Timezone = request.Timezone;
                    trangloStaffUser.SetAccountStatus(Enumeration.FindById<AccountStatus>(request.AccountStatus));
                    var fullName = FullName.Create(request.Name);
                    var email = Email.Create(request.Emails);
                    trangloStaffUser.SetEmail(email.Value);
                    trangloStaffUser.SetName(fullName.Value);

                  var actionExist = request.roleDepartmentEntityInput.Find(x => x.Action == 0);
                    if(actionExist == null) { 
                    foreach (var i in request.roleDepartmentEntityInput)
                    {
                        TrangloEntity trangloEntity = TrangloEntity.GetByEntityByTrangloId(i.TrangloEntityId);
                        TrangloDepartment trangloDepartment = Enumeration.FindById<TrangloDepartment>(i.TrangloDepartmentCode);
                        var trangloRole = await _trangloRoleRepository.GetTrangloRoleByCodeAsync(i.TrangloRoleCode);

                        if (trangloEntity == null)
                        {
                            return Result.Failure<TrangloStaff>($"Tranglo Entity {i.TrangloEntityId} does not exist");
                        }
                        if (trangloDepartment == null)
                        {
                            return Result.Failure<TrangloStaff>($"Tranglo Department Code {i.TrangloDepartmentCode} does not exist");
                        }
                        if (trangloRole == null)
                        {
                            return Result.Failure<TrangloStaff>($"Role {trangloRole} does not exist");
                        }

                        if (i.Action == 1) //add
                        {
                            trangloStaffUser.AssignToTrangloEntityAssignment(trangloEntity, trangloDepartment, trangloRole);
                            trangloStaffUser.AssignToTrangloStaffEntityAssignment(trangloEntity);
                        }
                    
                        if (i.Action == 2) 
                        {
                        // delete Tranglo Staff Assignment                  
                            trangloStaffUser.RemoveTrangloEntityAssignment(trangloEntity, trangloDepartment, trangloRole);
                        }

                    }
                    }
                }
                var result = await _applicationUserRepository.SaveTrangloUserChanges(trangloStaffUser);
                if(result.IsFailure)
                {
                    return Result.Failure("Error updating record");
                }

                return Result.Success();
            }
        }
    }
}
