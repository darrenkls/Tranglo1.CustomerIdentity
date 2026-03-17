using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Partner;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.PartnerAPISetting, UACAction.Edit)]
    internal class UpdateAPIPartnerSettingsCommand : BaseCommand<Result<PartnerAPISettingsOutputDTO>>
    {
        public long PartnerCode { get; set; }
        public PartnerAPISettings Staging { get; set; }
        public PartnerAPISettings Production { get; set; }

        public override Task<string> GetAuditLogAsync(Result<PartnerAPISettingsOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Update API Settings for Partner Code: [{this.PartnerCode}]";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class UpdateAPIPartnerSettingsCommandHandler : IRequestHandler<UpdateAPIPartnerSettingsCommand, Result<PartnerAPISettingsOutputDTO>>
    {
        private readonly PartnerService _partnerService;
        private readonly ILogger<UpdateAPIPartnerSettingsCommandHandler> _logger;

        public UpdateAPIPartnerSettingsCommandHandler(PartnerService partnerService, ILogger<UpdateAPIPartnerSettingsCommandHandler> logger)
        {
            _partnerService = partnerService;
            _logger = logger;
        }

        public async Task<Result<PartnerAPISettingsOutputDTO>> Handle(UpdateAPIPartnerSettingsCommand request, CancellationToken cancellationToken)
        {
            //try
            //{
            //    var checkResult = await _partnerService.GetHelloSignDocumentByHelloSignDocumentIdAsync(request.HelloSignDocumentId);
            //    if (checkResult != null)
            //    {
            //        checkResult.PartnerCode = request.PartnerCode;
            //        checkResult.IsRemoved = true;
                    var result = new PartnerAPISettingsOutputDTO();
                    return Result.Success(result);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("UpdateAPIPartnerSettingsCommand", ex.Message);
            //}
            //return Result.Failure<HelloSignDocument>(
            //                    $"Update API Settings failed for {request.PartnerCode}."
            //                );

        }
    }
}
