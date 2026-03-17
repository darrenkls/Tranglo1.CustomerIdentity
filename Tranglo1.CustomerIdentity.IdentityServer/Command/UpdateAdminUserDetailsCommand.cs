using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class UpdateAdminUserDetailsCommand : BaseCommand<Result<SingleAdminOutputDTO>>
    {
        public string TrangloStaffLoginId { get; set; }
        public SingleAdminUserInputDTO SingleAdminUserInputDTO { get; set; }

        public override Task<string> GetAuditLogAsync(Result<SingleAdminOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Update Admin User for Tranglo Staff Login ID: [{this.TrangloStaffLoginId}]";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
        internal class UpdateAdminUserDetailsCommandHandler : IRequestHandler<UpdateAdminUserDetailsCommand, Result<SingleAdminOutputDTO>>
        {
            private readonly IMapper _mapper;
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly ILogger<SingleAdminOutputDTO> _logger;
            public UpdateAdminUserDetailsCommandHandler(IMapper mapper, IApplicationUserRepository applicationUserRepository, ILogger<SingleAdminOutputDTO> logger)
            {
                _mapper = mapper;
                _applicationUserRepository = applicationUserRepository;
                _logger = logger;
            }


            public async Task<Result<KYCSummaryFeedbackOutputDTO>> Handle(UpdateAdminUserDetailsCommand request, CancellationToken cancellationToken)
            {
                var _applicationUser = await _applicationUserRepository.GetTrangloStaffAssignmentsByLoginId(request.TrangloStaffLoginId);

                if (_applicationUser = null)
                {


                }
            }
            private async Task<Result<TrangloStaffAssignment>> UpdateCOInformation(UpdateAdminUserDetailsCommand request, TrangloStaffAssignment trangloStaffAssignment)
            {
                Result<TrangloStaffAssignment> UpdateCOInformationResp = await _applicationUserRepository.UpdateTrangloStaffAssignmentByLoginIdAsync(trangloStaffAssignment);
                if (UpdateCOInformationResp.IsFailure)
                {
                    _logger.LogError("UpdateComplianceOfficers", UpdateCOInformationResp.Error);

                    return Result.Failure<TrangloStaffAssignment>(
                                $"Update Compliance Officers Info failed for {request.TrangloStaffLoginId}."
                            );
                }

                return Result.Success(UpdateCOInformationResp.Value);
            }
        }
    }
}
