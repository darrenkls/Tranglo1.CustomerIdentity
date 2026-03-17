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
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;
using static Tranglo1.CustomerIdentity.IdentityServer.Command.UpdateTrangloStaffAssignmentCommand;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.TrangloUser, UACAction.Create)]
    [Permission(Permission.ManageAdminUser.Action_Add_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageAdminUser.Action_View_Code })]
    internal class SaveTrangloStaffCommand : BaseCommand<Result<AddTrangloStaffOutputDTO>>
    {
        public string LoginId { get; set; }
        public AddTrangloStaffInputDTO AddTrangloStaff{ get; set; }
        public override Task<string> GetAuditLogAsync(Result<AddTrangloStaffOutputDTO> result)
        {
            if (result.IsSuccess) 
            {
                return Task.FromResult("Added New User");
            }

            return base.GetAuditLogAsync(result);
        }

        public class SaveTrangloStaffCommandHandler : IRequestHandler<SaveTrangloStaffCommand, Result<AddTrangloStaffOutputDTO>>
        {
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ITrangloRoleRepository _trangloRoleRepository;
            private readonly ILogger<SaveTrangloStaffCommandHandler> _logger;
            public IUnitOfWork UnitOfWork { get; }

            public SaveTrangloStaffCommandHandler(
                    IApplicationUserRepository applicationUserRepository,
                    ILogger<SaveTrangloStaffCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    ITrangloRoleRepository trangloRoleRepository
                )
            {
                _applicationUserRepository = applicationUserRepository;
                _logger = logger;
                UnitOfWork = unitOfWork;
                _trangloRoleRepository = trangloRoleRepository;
            }
            public async Task<Result<AddTrangloStaffOutputDTO>> Handle(SaveTrangloStaffCommand request, CancellationToken cancellationToken)
            {
                var trangloUser = await _applicationUserRepository.GetApplicationUserByLoginId(request.LoginId);
                if(trangloUser != null)
                {
                    return Result.Failure<AddTrangloStaffOutputDTO>(
                            $"Application User {request.LoginId} Already Exists."
                        );
                }
                var fullName = FullName.Create(request.AddTrangloStaff.Name);
                var email = Email.Create(request.AddTrangloStaff.Email);
                
                TrangloStaff trangloStaff = new TrangloStaff(request.LoginId, fullName.Value, email.Value);

               // var getSG = Domain.Common.TimezoneConversion.ConvertFromUTC("Asia/Singapore", languageCode, dateTime);
                trangloStaff.Timezone = request.AddTrangloStaff.Timezone;
                trangloStaff.SetSecurityStamp(Guid.NewGuid().ToString());
                await _applicationUserRepository.AddTrangloUser(trangloStaff);

                foreach (var i in request.AddTrangloStaff.trangloStaffEntity)
                {
                    TrangloEntity trangloEntity = TrangloEntity.GetByEntityByTrangloId(i.TrangloEntityId);
                    TrangloDepartment trangloDepartment = Enumeration.FindById<TrangloDepartment>(i.TrangloDepartmentCode);
                    var trangloRole = await _trangloRoleRepository.GetTrangloRoleByCodeAsync(i.TrangloRoleCode);

                    if (trangloEntity == null)
                    {
                        return Result.Failure<AddTrangloStaffOutputDTO>($"Tranglo Entity {i.TrangloEntityId} does not exist");
                    }
                    if (trangloDepartment == null)
                    {
                        return Result.Failure<AddTrangloStaffOutputDTO>($"Tranglo Department Code {i.TrangloDepartmentCode} does not exist");
                    }
                    if (trangloRole == null)
                    {
                        return Result.Failure<AddTrangloStaffOutputDTO>($"Role {trangloRole} does not exist");
                    }
                    trangloStaff.AssignToTrangloEntityAssignment(trangloEntity, trangloDepartment, trangloRole);
                    trangloStaff.AssignToTrangloStaffEntityAssignment(trangloEntity);
                }
                 var result = await _applicationUserRepository.SaveTrangloUserChanges(trangloStaff);

                AddTrangloStaffOutputDTO addTrangloStaffOutput = new AddTrangloStaffOutputDTO
                {
                    LoginId = trangloStaff.LoginId
                };
                if (result.IsFailure)
                {
                    return Result.Failure<AddTrangloStaffOutputDTO>("Error inserting record into table");
                }

                return Result.Success<AddTrangloStaffOutputDTO>(addTrangloStaffOutput);
            }
        }
    }
}
