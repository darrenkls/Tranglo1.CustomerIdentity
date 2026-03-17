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
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.PartnerSignUpCode, UACAction.View)]
    internal class GetSignUpCodeByCodeQuery : BaseQuery<Result<SignUpCodesGetByIdOutputDTO>>
    {
        public long PartnerCode { get; set; } 

        public override Task<string> GetAuditLogAsync(Result<SignUpCodesGetByIdOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Get : [{this.PartnerCode}]";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }
    internal class GetSignUpCodeByCodeQueryHandler : IRequestHandler<GetSignUpCodeByCodeQuery, Result<SignUpCodesGetByIdOutputDTO>>
    {
        private readonly ISignUpCodeRepository _signUpCodeRepository;
        private readonly BusinessProfileService _businessProfileService;
        private readonly IPartnerRepository _partnerRepository;
        private readonly ILogger<GetSignUpCodeByCodeQuery> _logger;
        private readonly IMapper _mapper;

        public GetSignUpCodeByCodeQueryHandler(ISignUpCodeRepository signUpCodeRepository,
                                             BusinessProfileService businessProfileService,
                                             IPartnerRepository partnerRepository,
                                             ILogger<GetSignUpCodeByCodeQuery> logger,
                                              IMapper mapper)
        {
            _signUpCodeRepository = signUpCodeRepository;
            _partnerRepository = partnerRepository;
            _businessProfileService = businessProfileService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<SignUpCodesGetByIdOutputDTO>> Handle(GetSignUpCodeByCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PartnerCode != 0)
                {
                    var partnerInfo = await _partnerRepository.GetPartnerInfoByCodeForSignUpCode(request.PartnerCode);
                    var businessProfile = await _businessProfileService.GetBusinessProfileByBusinessProfileCodeAsync(partnerInfo.BusinessProfileCode);

                    var codesInfo = new SignUpCodesGetByIdOutputDTO()
                    {
                        CompanyName = businessProfile.Value.CompanyName,
                        Email = partnerInfo.Email.Value
                    };

                    return Result.Success(codesInfo);
                }
                else
                {
                    var codesInfo = new SignUpCodesGetByIdOutputDTO()
                    {
                        CompanyName = "",
                        Email = ""
                    };

                    return Result.Success(codesInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetSignUpCodeByCodeQuery] {ex.Message}");
            }
            return Result.Failure<SignUpCodesGetByIdOutputDTO>(
                        $"Get sign up code info failed for {request.PartnerCode}."
                    );
        }
        }
    }
    
