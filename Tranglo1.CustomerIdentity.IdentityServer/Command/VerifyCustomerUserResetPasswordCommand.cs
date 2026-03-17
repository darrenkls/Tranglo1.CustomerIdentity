using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
    public class VerifyCustomerUserResetPasswordCommand : IRequest<IdentityResult>
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string NewPassword { get; set; }

        public class RecentPasswordOutputDTO 
        {
            public long userId { get; set; }
            public string PasswordHash { get; set; }
            public DateTime SysStartTime { get; set; }
            public DateTime SysEndTime { get; set; }
        }

        public class VerifyCustomerUserResetPasswordCommandHandler : IRequestHandler<VerifyCustomerUserResetPasswordCommand, IdentityResult>
        {
            private readonly TrangloUserManager _userManager;
            private readonly IBusinessProfileRepository _businessProfileRepository;
            private readonly IConfiguration _config;
            public VerifyCustomerUserResetPasswordCommandHandler(TrangloUserManager userManager,
                IBusinessProfileRepository businessProfileRepository,
                IConfiguration config)
            {
                _userManager = userManager;
                _businessProfileRepository = businessProfileRepository;
                _config = config;
            }

            public async Task<IdentityResult> Handle(VerifyCustomerUserResetPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.Email);
                string errorMessage = "";
                var tokenString = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

                if (user != null && user is CustomerUser)
                {

                    var _connectionString = _config.GetConnectionString("DefaultConnection");


                    IEnumerable<RecentPasswordOutputDTO> RecentPasswordOutputDTOs;

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        var reader = await connection.QueryMultipleAsync(
                            "GetRecentPasswordHistories",
                            new
                            {
                                UserId = user.Id
                            },
                            null, null, CommandType.StoredProcedure);

                        // read as IEnumerable<dynamic>
                        RecentPasswordOutputDTOs = await reader.ReadAsync<RecentPasswordOutputDTO>();


                    }

                    foreach (var old in RecentPasswordOutputDTOs)
                    {
                        var passwordHasher = new PasswordHasher<ApplicationUser>();
                        if (passwordHasher.VerifyHashedPassword((CustomerUser)user, old.PasswordHash, request.NewPassword)
                            == PasswordVerificationResult.Success)
                        {
                            errorMessage = $"Must not reuse the past 6 passwords.";
                            return IdentityResult.Failed(
                                new IdentityError
                                {
                                    Description = errorMessage
                                });
                        }
                    }

                    var result = await _userManager.ResetPasswordAsync(user, tokenString, request.NewPassword);

                    //Add additional checking on reset password checking
                    if (!result.Succeeded)
                    {
                        errorMessage = String.Join(",", result.Errors.Select(x => x.Description).ToList());
                    }
                    else
                    {
                        var customerUser = (CustomerUser)user;
                        customerUser.setIsTPNUser(false);
                        var editResult = await _businessProfileRepository.EditIsTPNUser(customerUser);
                        if (!editResult.IsSuccess)
                        {
                            errorMessage = $"ValidateUserPasswordCommand - Update isTpnUser failed";
                        }
                    }

                }
                else
                {
                    errorMessage = $"Email: {request.Email} is not a valid customer user email.";
                }

                if (errorMessage == "")
                    return IdentityResult.Success;

                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = errorMessage
                    });
            }
        }
    }
}
