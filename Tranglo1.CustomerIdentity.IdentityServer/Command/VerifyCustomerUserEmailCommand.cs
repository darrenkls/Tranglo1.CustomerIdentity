using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Microsoft.Extensions.Logging;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using System.Linq;

namespace Tranglo1.CustomerIdentity.IdentityServer.CustomerUserList.Commands
{
    public class VerifyCustomerUserEmailCommand : IRequest<IdentityResult>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }

    public class VerifyCustomerUserEmailCommandHandler : IRequestHandler<VerifyCustomerUserEmailCommand, IdentityResult>
    {
        private readonly ILogger<VerifyCustomerUserEmailCommandHandler> _logger;
        private readonly TrangloUserManager _userManager;
        private readonly IBusinessProfileRepository _businessProfileRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IPartnerRepository _partnerRepository;

        public VerifyCustomerUserEmailCommandHandler(
            TrangloUserManager userManager,
            ILogger<VerifyCustomerUserEmailCommandHandler> logger, IBusinessProfileRepository businessProfileRepository, IApplicationUserRepository applicationUserRepository, IPartnerRepository partnerRepository
            )
        {
            _userManager = userManager;
            _logger = logger;
            _businessProfileRepository = businessProfileRepository;
            _applicationUserRepository = applicationUserRepository;
            _partnerRepository = partnerRepository;
        }

        public async Task<IdentityResult> Handle(VerifyCustomerUserEmailCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser _User = await _userManager.FindByIdAsync(request.UserId);
            if (_User == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = $"Email {request.UserId} does not exist."
                    });
            }

            var latestToken = await _businessProfileRepository.FindUserTokenByEmailAsync(_User.Email.Value);
            if (latestToken.Token != request.Token)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = $"This verification link no longer valid. Kindly contact your BD representative for a new verification link."
                    });
            }
            //Get Solution by Customer User Registration
            var customerUserInfo = await _applicationUserRepository.GetCustomerUserRegistrationsByLoginIdAsync(request.UserId);
            IdentityResult ConfirmEmailResult = new IdentityResult();
            try
            {

                if (customerUserInfo.SolutionCode == null) // Partner Register by SignUp Code
                {
                    var partnerInfo = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(customerUserInfo.BusinessProfileCode);
                    var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionListAsync(partnerInfo.Id);
                    var isTrangloConnectExist = partnerSubInfo.Any(x => x.Solution == Solution.Connect);
                    var isTrangloBusinessExist = partnerSubInfo.Any(x => x.Solution == Solution.Business);
                    bool isMultipleSolution = (isTrangloConnectExist == true && isTrangloBusinessExist == true);

                    if (isMultipleSolution == true)
                    {
                        ConfirmEmailResult = await _userManager.ConfirmEmailAsync(_User, request.Token, 1, isMultipleSolution);
                        if (!ConfirmEmailResult.Succeeded)
                        {
                            return IdentityResult.Failed(
                                    new IdentityError
                                    {
                                        Description = $"Email confirmation for {request.UserId} failed."
                                    });
                        }
                    }
                    else
                    {
                        if (isTrangloConnectExist == true)
                        {
                            ConfirmEmailResult = await _userManager.ConfirmEmailAsync(_User, request.Token, 1, isMultipleSolution);
                            if (!ConfirmEmailResult.Succeeded)
                            {
                                return IdentityResult.Failed(
                                        new IdentityError
                                        {
                                            Description = $"Email confirmation for {request.UserId} failed."
                                        });
                            }
                        }
                        if (isTrangloBusinessExist == true)
                        {
                            ConfirmEmailResult = await _userManager.ConfirmEmailAsync(_User, request.Token, 2, isMultipleSolution);
                            if (!ConfirmEmailResult.Succeeded)
                            {
                                return IdentityResult.Failed(
                                        new IdentityError
                                        {
                                            Description = $"Email confirmation for {request.UserId} failed."
                                        });
                            }
                        }
                    }
                }
                else
                {

                    ConfirmEmailResult = await _userManager.ConfirmEmailAsync(_User, request.Token, customerUserInfo.SolutionCode, false);
                    if (!ConfirmEmailResult.Succeeded)
                    {
                        return IdentityResult.Failed(
                                new IdentityError
                                {
                                    Description = $"Email confirmation for {request.UserId} failed."
                                });
                    }
                }

                return ConfirmEmailResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
            

        }
    }
}