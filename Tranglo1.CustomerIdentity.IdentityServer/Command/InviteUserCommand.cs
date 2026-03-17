using MediatR;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using CSharpFunctionalExtensions;
using Tranglo1.UserAccessControl;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using static Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerUser.InviteUserInputDTO;
using System.Collections.Generic;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    //[Permission(PermissionGroupCode.PartnerUser, UACAction.Create)]
    [Permission(Permission.ManagePartnerUser.Action_InviteUser_Code,
        new int[] { (int)PortalCode.Admin, (int)PortalCode.Connect, (int)PortalCode.Business },
        new string[] { Permission.ManagePartnerUser.Action_View_Code, Permission.ManagePartnerUser.Action_ViewDetail_Code })]
    public class InviteUserCommand : IRequest<IdentityResult>
    {
        public int UserEnvironmentCode { get; set; }
        public string InviterEmail { get; set; }
        public int BusinessProfileCode { get; set; }
        public string InviteeFullName { get; set; }
        public string InviteeEmail { get; set; }
        public int InviteeRoleCode { get; set; }
        public List<string> InviteeRoleCodeList { get; set; }
        public string LoginId { get; set; }
        public string Timezone { get; set; } // Used in TB portal invite user
        public int SolutionCode { get; set; }
    }

    public class InviteUserCommandHandler : IRequestHandler<InviteUserCommand, IdentityResult>
    {
        private readonly TrangloUserManager _userManager;
        private readonly ILogger<InviteUserCommandHandler> _logger;

        public InviteUserCommandHandler(
            TrangloUserManager userManager,
            ILogger<InviteUserCommandHandler> logger
            )
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityResult> Handle(InviteUserCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser inviter = await _userManager.FindByIdAsync(request.LoginId) as ApplicationUser;
            if (inviter == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "500",
                        Description = $"Inviter email [{request.InviterEmail}] does not exist."
                    });
            }

            // Validate if email provided is in the correct email format
            Result<Email> emailResult = Email.Create(request.InviteeEmail);
            if (emailResult.IsFailure)
            {
                _logger.LogError($"[InviteUserCommand] Email Result. {emailResult.Error}");
                return IdentityResult.Failed(new IdentityError() { Description = emailResult.Error });
            }

            string generateTemporaryPassword = GeneratePassword();
            _logger.LogInformation($"Generated temporary password: {generateTemporaryPassword} for user [{emailResult.Value.Value}]");

            // Mock data
            //Country mockCountry = await _countryRepository.GetCountryByISO2Async("US");
            //UserType mockUserType = UserType.Business;
            //Solution mockSolution = Solution.Connect;

            var inviteUserResult = await _userManager.InviteAsync(
                                                    inviter,
                                                    FullName.Create(request.InviteeFullName).Value,
                                                    emailResult.Value,
                                                    request.InviteeRoleCodeList,
                                                    generateTemporaryPassword,
                                                    request.BusinessProfileCode,
                                                    request.Timezone,
                                                    request.SolutionCode
                                                );

            if (inviteUserResult.IsFailure)
            {
                _logger.LogError($"[InviteUserCommand] Invite user [{emailResult.Value.Value}] failed. {inviteUserResult.Error}.");
                return IdentityResult.Failed(new IdentityError() { Description = $"Invite user [{emailResult.Value.Value}] failed. {inviteUserResult.Error}." });
            }

            return IdentityResult.Success;
        }

        private string GeneratePassword()
        {
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }
    }
}
