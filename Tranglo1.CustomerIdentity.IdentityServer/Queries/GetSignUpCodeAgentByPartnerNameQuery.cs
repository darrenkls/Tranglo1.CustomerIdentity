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
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.PartnerSignUpCode, UACAction.View)]
    internal class GetSignUpCodeAgentByPartnerNameQuery : BaseQuery<Result<SignUpCodesGetByPartnerNameOutputDTO>>
    {
        public string PartnerName { get; set; } 

        public override Task<string> GetAuditLogAsync(Result<SignUpCodesGetByPartnerNameOutputDTO> result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Get : [{this.PartnerName}]";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }
    internal class GetSignUpCodeAgentByPartnerNameQueryHandler : IRequestHandler<GetSignUpCodeAgentByPartnerNameQuery, Result<SignUpCodesGetByPartnerNameOutputDTO>>
    {
        private readonly ISignUpCodeRepository _signUpCodeRepository;
        private readonly BusinessProfileService _businessProfileService;
        private readonly IPartnerRepository _partnerRepository;
        private readonly ILogger<GetSignUpCodeAgentByPartnerNameQuery> _logger;
        private readonly IMapper _mapper;

        public GetSignUpCodeAgentByPartnerNameQueryHandler(ISignUpCodeRepository signUpCodeRepository,
                                             BusinessProfileService businessProfileService,
                                             IPartnerRepository partnerRepository,
                                             ILogger<GetSignUpCodeAgentByPartnerNameQuery> logger,
                                              IMapper mapper)
        {
            _signUpCodeRepository = signUpCodeRepository;
            _partnerRepository = partnerRepository;
            _businessProfileService = businessProfileService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<SignUpCodesGetByPartnerNameOutputDTO>> Handle(GetSignUpCodeAgentByPartnerNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PartnerName != "")
                {
                    var businessProfile = new Result<BusinessProfile>();
                    try
                    {
                        businessProfile = await _businessProfileService.GetBusinessProfileByCompanyName(request.PartnerName);
                        if (businessProfile.Value is null || businessProfile.IsFailure) //Throw exception if null or failure
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                       return Result.Failure<SignUpCodesGetByPartnerNameOutputDTO>($"Unable to generate the sign up code. Please register the partner.");
                    }

                    var partnerInfo = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(businessProfile.Value.Id);
                    var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionListAsync(partnerInfo.Id);
                    var isTrangloBusinessExist = partnerSubInfo.Any(x => x.Solution == Solution.Business);
                    var isTrangloConnectExist = partnerSubInfo.Any(x => x.Solution == Solution.Connect);

                    var codesInfo = new SignUpCodesGetByPartnerNameOutputDTO()
                    {
                        CompanyName = businessProfile.Value.CompanyName,
                        Agent = partnerInfo.AgentLoginId,
                        PartnerCode = partnerInfo.Id,
                        IsTrangloConnectExist = isTrangloConnectExist,
                        IsTrangloBusinessExist = isTrangloBusinessExist,
                        
                    };

                    return Result.Success(codesInfo);
                }
                else
                {
                    var codesInfo = new SignUpCodesGetByPartnerNameOutputDTO()
                    {
                        CompanyName = "",
                        Agent = ""
                    };

                    return Result.Success(codesInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetSignUpCodeAgentByPartnerNameQuery] {ex.Message}");
            }
            return Result.Failure<SignUpCodesGetByPartnerNameOutputDTO>(
                        $"Get sign up code info failed for {request.PartnerName}."
                    );
        }
        }
    }
    
