using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class ValidateEmailAuthenticationOTPQuery : BaseQuery<Result<bool>>
    {
        public string MFAOTP {  get; set; }
        public ApplicationUser user {  get; set; }

        public class ValidateEmailAuthenticationOTPQueryHandler : IRequestHandler<ValidateEmailAuthenticationOTPQuery, Result<bool>>
        {
            private readonly IConfiguration _config;
            private readonly ILogger<ValidateEmailAuthenticationOTPQuery> _logger;
            private readonly IApplicationUserRepository _applicationUserRepository;

            public ValidateEmailAuthenticationOTPQueryHandler(IConfiguration config, ILogger<ValidateEmailAuthenticationOTPQuery> logger,
                IApplicationUserRepository applicationUserRepository)
            {
                _config = config;
                _logger = logger;
                _applicationUserRepository = applicationUserRepository;
            }

            public async Task<Result<bool>> Handle(ValidateEmailAuthenticationOTPQuery request, CancellationToken cancellationToken)
            {
                if (request.user == null)
                {
                    return false;
                }
                var userInfo = await _applicationUserRepository.GetApplicationUserByUserId(request.user.Id);
                var mfaOTPChecker = await _applicationUserRepository.GetMFAEmailOTPByOTPAsync(request.MFAOTP);
                var mfaChecker = await _applicationUserRepository.GetMFAEmailOTPAsync(userInfo.LoginId);


                if (mfaOTPChecker.CreatedDate.AddMinutes(5) < DateTime.UtcNow)
                {
                    // More than 5 minutes have passed
                    return false;
                    _logger.LogInformation("MFA OTP is Expired");
                }

                if(userInfo.LoginId == mfaOTPChecker.LoginID && mfaChecker.OTP != request.MFAOTP)
                {
                    return false;
                    _logger.LogInformation("MFA OTP is different");
                }
                return true;
                //var _connectionString = _config.GetConnectionString("DefaultConnection");
                //using (var connection = new SqlConnection(_connectionString))
                //{
                //    await connection.OpenAsync();
                //    var result = (await connection.QueryAsync(
                //       "GetLatestMFAOTP",
                //           new
                //           {
                //               MFAOTP = request.MFAOTP,
                //               LoginId = request.user.LoginId
                //           },
                //       null, null, CommandType.StoredProcedure)).ToList();
                //    
                //    if (result.Count == 0)
                //    {
                //        return false;
                //    }
                //    else
                //    {
                //        return true;
                //    }
                //}

                
                    
            }
        }
    }
}
