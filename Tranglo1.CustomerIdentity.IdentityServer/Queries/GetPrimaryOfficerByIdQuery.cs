using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.PrimaryOfficer;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.KYCOwnershipAndManagementStructure, UACAction.View)]
    [Permission(Permission.KYCManagementOwnership.Action_View_Code,
        new int[] { (int)PortalCode.Admin, (int)PortalCode.Connect, (int)PortalCode.Business },
        new string[] { })]
    internal class GetPrimaryOfficerByIdQuery : BaseQuery<IEnumerable<PrimaryOfficerOutputDTO>>
    {
        public int BusinessProfileCode { get; set; }

        public override Task<string> GetAuditLogAsync(IEnumerable<PrimaryOfficerOutputDTO> result)
        {
            /*
            if (result.IsSuccess)
            {
                string _description = $"Get Primary Officer for Business Profile Code: [{this.BusinessProfileCode}]";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
            */

            string _description = $"Get Primary Officer for Business Profile Code: [{this.BusinessProfileCode}]";
            return Task.FromResult(_description);
        }

        public class GetPrimaryOfficerByIdQueryHandler : IRequestHandler<GetPrimaryOfficerByIdQuery, IEnumerable<PrimaryOfficerOutputDTO>>
        {

            private readonly IMapper _mapper;
            private readonly BusinessProfileService _businessProfileService;

            public GetPrimaryOfficerByIdQueryHandler(IMapper mapper, BusinessProfileService businessProfileService)
            {
                _mapper = mapper;
                _businessProfileService = businessProfileService;
            }

            public async Task<IEnumerable<PrimaryOfficerOutputDTO>> Handle(GetPrimaryOfficerByIdQuery query, CancellationToken cancellationToken)
            {

                var businessProfile = await _businessProfileService.GetBusinessProfileByBusinessProfileCodeAsync(query.BusinessProfileCode);

                var primary = await _businessProfileService.GetPrimaryOfficerByBusinessProfileCodeAsync(businessProfile.Value);

                var _isPrimaryOfficerCompleted = await _businessProfileService.IsOwnershipPrimaryOfficersCompleted(businessProfile.Value.Id);

                var primaryOfficerOutputDTO =  _mapper.Map<IEnumerable<PrimaryOfficer>, IEnumerable<PrimaryOfficerOutputDTO>>(primary.Value);

                for (int i = 0; i < primaryOfficerOutputDTO.Count(); i++)
                {
                    primaryOfficerOutputDTO.ElementAt(i).isCompleted = _isPrimaryOfficerCompleted[i];
                }

                foreach (var poOutput in primaryOfficerOutputDTO)
                {
                    // Set the concurrency token from BusinessProfile
                    poOutput.PrimaryOfficerConcurrencyToken = businessProfile.Value.PrimaryOfficerConcurrencyToken;
                }

                return primaryOfficerOutputDTO;

            }
        }
    }
}
