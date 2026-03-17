using AutoMapper;
using CSharpFunctionalExtensions;
using IdentityServer4.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.SignUpCodes;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Exceptions;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.SignUpCode;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR.Behaviours;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.PartnerSignUpCode, UACAction.Create)]
    [Permission(Permission.SignupCode.Action_GenerateNewCode_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] { Permission.SignupCode.Action_View_Code, Permission.RegisterNewPartner.Action_View_Code }
        )]
    class GenerateSignUpCodesCommand : BaseCommand<Result>
    {
        public SignUpCodesInputDTO SignUpCodes;

        public override Task<string> GetAuditLogAsync(Result result)
        {
            if (result.IsSuccess)
            {
                string _description = $"Generated sign-up code for a partner";
                return Task.FromResult(_description);
            }

            return Task.FromResult<string>(null);
        }
    }

    internal class GenerateSignUpCodesCommandHandler : IRequestHandler<GenerateSignUpCodesCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly BusinessProfileService _businessProfileService;
        private readonly ISignUpCodeRepository _signUpCodeRepository;
        private readonly ILogger<GenerateSignUpCodesCommandHandler> _logger;
        private readonly IPartnerRepository _partnerRepository;
        private readonly CustomerIdentity.Infrastructure.Services.IIdentityContext identityContext;
        private readonly TrangloUserManager _userManager;

        public GenerateSignUpCodesCommandHandler(IMapper mapper,
        IApplicationUserRepository applicationUserRepository,
        IBusinessProfileRepository businessProfileRepository,
        BusinessProfileService businessProfileService,
        ISignUpCodeRepository signUpCodeRepository,
        IPartnerRepository partnerRepository,
        IIdentityContext identityContext,
        TrangloUserManager userManager,
        ILogger<GenerateSignUpCodesCommandHandler> logger)

        {
            _partnerRepository = partnerRepository;
            _businessProfileRepository = businessProfileRepository;
            _signUpCodeRepository = signUpCodeRepository;
            _applicationUserRepository = applicationUserRepository;
            _businessProfileService = businessProfileService;
            _mapper = mapper;
            _logger = logger;
            this.identityContext = identityContext;
            _userManager = userManager;
        }

        public async Task<Result> Handle(GenerateSignUpCodesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var inputSignUpCode = request.SignUpCodes;

                //TODO: Refactor to remove PartnerCode from RequestDTO as PartnerCode should not be passed over from FrontEnd
                //// ( User can pass in invalid / different partner code ) so the detection of partner code logic should happen only at API logic
                var hasPartnerCode = request.SignUpCodes.partnerCode != 0; //Not in used anymore. 
                var applicationUser = await _applicationUserRepository.GetApplicationUserByLoginId(inputSignUpCode.AgentLoginId);
                var solutionCode = request.SignUpCodes.SolutionCode;
                //if (hasPartnerCode == true)
                //{
                //    var partnerInfo = await _partnerRepository.GetPartnerInfoByCodeForSignUpCode(inputSignUpCode.partnerCode);
                //    if (partnerInfo == null)
                //    {
                //        return Result.Failure($"Partner Code {inputSignUpCode.partnerCode} is not found.");
                //    }
                //}

                var leadsOrigin = Enumeration.FindById<LeadsOrigin>(inputSignUpCode.LeadsOriginCode);
                if (leadsOrigin == null)
                {
                    return Result.Failure($"Leads origin {inputSignUpCode.LeadsOriginCode} is not found.");
                }

                long generatedPartnerCode = 0;
                var isExistingPartnerName = await _businessProfileService.CheckIsExistingCompanyNameAsync(request.SignUpCodes.CompanyName);
                if (isExistingPartnerName.isInUsed)
                {
                    string trimmedCompanyName = request.SignUpCodes.CompanyName.Trim();

                    var signUpCodeList = await _signUpCodeRepository.GetFilteredSignUpCodeByCompanyNameAsync(trimmedCompanyName, solutionCode);

                    // SignUpCodeList is not null and is Active/Used
                    if (signUpCodeList != null && signUpCodeList.SolutionCode == solutionCode && (signUpCodeList.ExpireAt > DateTime.UtcNow || signUpCodeList?.Status.Id == 2))
                    {
                        switch (signUpCodeList?.Status.Id)
                        {
                            case 1: // Active
                                return Result.Failure($"Partner Name and Sign Up code already exists.");
                            case 2: // Used
                                //return Result.Failure($"Partner Name already exists and registration has been completed by Partner.");
                                return Result.Failure($"Partner account has already been verified. Please try with another partner name.");
                            default: // SignUpCodeList not found
                                return Result.Failure($"Partner Name already exists. Only unique naming is allowed.");
                        }
                    }
                    else if (signUpCodeList != null && isExistingPartnerName.isInCustomerUserRegistrationList && !isExistingPartnerName.isInSignUpCodeList && signUpCodeList.SolutionCode == solutionCode)
                    {
                        // The Company Name already registered in CustomerUserRegistration through self sign up, but not inside SignUpCode List
                        return Result.Failure($"Partner Name already exists. Only unique naming is allowed.");
                    }
                    else if (isExistingPartnerName.isInBusinessProfileList)
                    {
                        var businessProfileList = await _businessProfileRepository.GetBusinessProfileByCompanyNameAsync(trimmedCompanyName);
                        var customerUserBusinessProfile = await _businessProfileRepository.GetCustomerUserBusinessProfileByBusinessProfileCodeAsync(businessProfileList.Id);
                        if (customerUserBusinessProfile != null)
                        {
                            // The BusinessProfileCode of the Company Name already tie to user, which means the company name have been used/existed
                            return Result.Failure($"Partner Name already has user(s) tied to it");
                        }

                        var partnerInfo = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(businessProfileList.Id);// GetPartnerInfoByCodeForSignUpCode(inputSignUpCode.partnerCode);
                        if (partnerInfo == null)
                        {
                            generatedPartnerCode = 0;
                        }
                        else
                        {
                            generatedPartnerCode = partnerInfo.Id;
                        }
                    }
                }

                if (!isExistingPartnerName.isInBusinessProfileList)
                {
                    // perform Partner Registration
                    var _CurrentUser = identityContext.CurrentUser;
                    var _Sub = _CurrentUser.GetSubjectId();
                    ApplicationUser user = await _userManager.FindByIdAsync(_Sub.Value);
                    if (user is TrangloStaff trangloStaff)
                    {
                        var trangloEntities = await _applicationUserRepository.GetTrangloStaffEntityAssignmentByUserId(trangloStaff.Id);
                        var TrangloEntityResult = EntityVerificationBehavior.TrangloEntityChecking(trangloEntities, request.SignUpCodes.entity);
                        if (TrangloEntityResult.IsFailure)
                        {
                            throw new ForbiddenException($"User account: {_Sub.Value} is not authorized to access Tranglo Entity {request.SignUpCodes.entity}");
                        }
                    }

                    if (generatedPartnerCode == 0)
                    {
                        // create Business Profile & Partner Registration
                        var RegisterBusinessProfileResult = _businessProfileService.EnsurePartnerBusinessProfileAsync(null, null,
                        null, null, null, null, null, null, null, request.SignUpCodes.entity, inputSignUpCode.AgentLoginId, inputSignUpCode.CompanyName, null
                        , null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, false, null, null);


                        var bisinessProfileId = RegisterBusinessProfileResult.Result.Value.Id;
                        var partnerRegistrationResult = _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(bisinessProfileId);
                        generatedPartnerCode = partnerRegistrationResult.Result.Id;
                    }
                }

                SignUpCode newSignUpCode = new SignUpCode
                {
                    CompanyName = inputSignUpCode.CompanyName,
                    AgentLoginId = inputSignUpCode.AgentLoginId,
                    PartnerCode = generatedPartnerCode,
                    Status = SignUpAccountStatus.Active,
                    LeadsOrigin = leadsOrigin,
                    CreatedAt = DateTime.UtcNow,
                    ExpireAt = DateTime.UtcNow.AddDays(2),
                    SolutionCode = solutionCode
                };

                var addSignUpCode = await _signUpCodeRepository.AddSignUpCodesAsync(newSignUpCode);
                var signUpCodeInfo = await _signUpCodeRepository.GetSignUpCodesAsync(addSignUpCode.Id);
                string loginId = applicationUser.LoginId;
                int index = loginId.IndexOf("@");
                if (index >= 0)
                    loginId = loginId.Substring(0, index);
                var agentName = loginId.Replace(" ", "").Replace(".", "").ToUpper();
                var companyName = inputSignUpCode.CompanyName.Replace(" ", "").ToUpper();
                var generateCode = string.Concat(agentName, companyName, addSignUpCode.Id);
                signUpCodeInfo.Code = generateCode;

                var result = await _signUpCodeRepository.UpdateSignUpCodesAsync(signUpCodeInfo);
                return Result.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GenerateSignUpCodesCommand] {ex.Message}");
            }
            return Result.Failure<SignUpCode>(
                        $"Generate sign up code failed for {request.SignUpCodes.AgentLoginId}."
        );
        }
    }
}