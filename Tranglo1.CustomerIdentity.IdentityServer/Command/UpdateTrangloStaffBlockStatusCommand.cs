using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    [Permission(Permission.ManageAdminUser.Action_Block_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.ManageAdminUser.Action_View_Code })]
    internal class UpdateTrangloStaffBlockStatusCommand : BaseCommand<Result<TrangloEntityBlockStatusOutputDTO>>
    {
        public string LoginId { get; set; }
        public string TrangloEntity { get; set; }
        public TrangloEntityBlockStatusInputDTO BlockStatusInput { get; set; }
        public override Task<string> GetAuditLogAsync(Result<TrangloEntityBlockStatusOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                var status = Enumeration.FindById<CompanyUserBlockStatus>(BlockStatusInput.BlockStatusCode);
                if (status.Equals(CompanyUserBlockStatus.Block)) 
                {
                    return Task.FromResult("Blocked User Account");
                }
            }

            return Task.FromResult<string>(null);
        }
        public class UpdateTrangloStaffBlockStatusCommandHandler : IRequestHandler<UpdateTrangloStaffBlockStatusCommand, Result<TrangloEntityBlockStatusOutputDTO>>
        {
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ITrangloRoleRepository _trangloRoleRepository;
            private readonly ILogger<UpdateTrangloStaffBlockStatusCommandHandler> _logger;
            public IUnitOfWork UnitOfWork { get; }

            public UpdateTrangloStaffBlockStatusCommandHandler(
                    IApplicationUserRepository applicationUserRepository,
                    ILogger<UpdateTrangloStaffBlockStatusCommandHandler> logger,
                    IUnitOfWork unitOfWork,
                    ITrangloRoleRepository trangloRoleRepository
                )
            {
                _applicationUserRepository = applicationUserRepository;
                _logger = logger;
                UnitOfWork = unitOfWork;
                _trangloRoleRepository = trangloRoleRepository;
            }
            public async Task<Result<TrangloEntityBlockStatusOutputDTO>> Handle(UpdateTrangloStaffBlockStatusCommand request, CancellationToken cancellationToken)
            {
                var trangloStaffEntityAssignment = await _applicationUserRepository.GetTrangloStaffEntityAssignment(request.LoginId, request.TrangloEntity);
                if(trangloStaffEntityAssignment == null)
                {
                    return Result.Failure<TrangloEntityBlockStatusOutputDTO>(
                           $"Tranglo Staff Entity Assignment {request.LoginId} Does Not Exist."
                       );
                }

                trangloStaffEntityAssignment.BlockStatus = Enumeration.FindById<CompanyUserBlockStatus>(request.BlockStatusInput.BlockStatusCode);

                await _applicationUserRepository.UpdateTrangloStaffEntityAssignment(trangloStaffEntityAssignment);

                TrangloEntityBlockStatusOutputDTO trangloEntityBlockStatusOutput = new TrangloEntityBlockStatusOutputDTO
                {
                    BlockStatusCode = trangloStaffEntityAssignment.BlockStatus.Id,
                    BlockStatusDescription = trangloStaffEntityAssignment.BlockStatus.Name
                };
                return Result.Success<TrangloEntityBlockStatusOutputDTO>(trangloEntityBlockStatusOutput);
            }
        }
    }
}
