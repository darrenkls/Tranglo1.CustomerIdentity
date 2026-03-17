using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.ExternalUserRole;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    ////[Permission(PermissionGroupCode.PartnerManageExternalRole, UACAction.View)]
    internal class GetExternalUserRoleByClaimQuery : IRequest<Result<ExternalUserRoleByClaimOutputDTO>>
    {
        public string LoginId { get; set; }
        public class GetExternalUserRoleByClaimQueryHandler : IRequestHandler<GetExternalUserRoleByClaimQuery, Result<ExternalUserRoleByClaimOutputDTO>>
        {
            private readonly IConfiguration _config;
            private readonly IApplicationUserRepository _applicationUserRepository;
            private readonly IBusinessProfileRepository _businessProfileRepository;
            private readonly IPartnerRepository _partnerRepository;

            public GetExternalUserRoleByClaimQueryHandler(IConfiguration config, IApplicationUserRepository applicationUserRepository, IBusinessProfileRepository businessProfileRepository, IPartnerRepository partnerRepository)
            {
                _config = config;
                _applicationUserRepository = applicationUserRepository;
                _businessProfileRepository = businessProfileRepository;
                _partnerRepository = partnerRepository;
            }

            public async Task<Result<ExternalUserRoleByClaimOutputDTO>> Handle(GetExternalUserRoleByClaimQuery request, CancellationToken cancellationToken)
            {
                var applicationUserInfo = await _applicationUserRepository.GetApplicationUserByLoginId(request.LoginId);
                var customerUserBusinessProfileInfo = await _businessProfileRepository.GetCustomerUserBusinessProfilesByUserIdAsync(applicationUserInfo.Id);
                var partnerInfo = await _partnerRepository.GetPartnerRegistrationByBusinessProfileCodeAsync(customerUserBusinessProfileInfo.BusinessProfileCode);
                var partnerSubInfo = await _partnerRepository.GetPartnerSubscriptionListAsync(partnerInfo.Id);
                var isConnectExist = partnerSubInfo.Any(x => x.Solution == Solution.Connect);
                var isBusinessExist = partnerSubInfo.Any(x => x.Solution == Solution.Business);


                //if (partnerSubInfo.Count > 0)
                //{
                //    if (isConnectExist == true && isBusinessExist == true)
                //    {
                //        var businessinactiveuser = partnerSubInfo.Any(x => x.PartnerAccountStatusType == PartnerAccountStatusType.Inactive && x.Solution == Solution.Business);

                //        if (businessinactiveuser == true)
                //        {
                //            isDisabled = true;
                //        }
                //    }
                //    else if (isConnectExist == true)
                //    {
                //        var activeUser = partnerSubInfo.Any(x => x.PartnerAccountStatusType == PartnerAccountStatusType.Active);
                //        var inactiveUser = partnerSubInfo.Any(x => x.PartnerAccountStatusType == PartnerAccountStatusType.Inactive);

                //        if (activeUser == false)
                //        {
                //            isDisabled = true;
                //        }
                //    }
                //    else if (isBusinessExist == true)
                //    {
                //        var activeUser = partnerSubInfo.Any(x => x.PartnerAccountStatusType == PartnerAccountStatusType.Active);
                //        var inactiveUser = partnerSubInfo.Any(x => x.PartnerAccountStatusType == PartnerAccountStatusType.Inactive);

                //        if (activeUser == false)
                //        {
                //            isDisabled = true;
                //        }
                //    }
                //}
                IEnumerable<ExternalUserRoleByClaimOutputDTO> result;
                IEnumerable<UserSolution> userSolutionResults;
                IEnumerable<ExternalUser> externalUserResults;
                IEnumerable<ExternalUserRoles> externalUserRoleResults;

                var outputDTO = new ExternalUserRoleByClaimOutputDTO();

                var _connectionString = _config.GetConnectionString("DefaultConnection");
                //request.LoginId = "AliceSmith@email.com";
                string timezone = String.Empty;
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                        "GetExternalUserRoleByClaim",
                        new
                        {
                            LoginId = request.LoginId
                        },
                        null, null, CommandType.StoredProcedure);
                    result = await reader.ReadAsync<ExternalUserRoleByClaimOutputDTO>();
                    userSolutionResults = await reader.ReadAsync<UserSolution>();
                    externalUserResults = await reader.ReadAsync<ExternalUser>();
                    externalUserRoleResults = await reader.ReadAsync<ExternalUserRoles>();

                    var userSolutions = new List<UserSolution>();

                    foreach (var us in userSolutionResults)
                    {
                        var externalUsers = new List<ExternalUser>();

                        foreach (var ex in externalUserResults)
                        {
                            var externalUserRoles = new List<ExternalUserRoles>();

                            foreach (var eur in externalUserRoleResults)
                            {
                                if (us.SolutionCode == eur.SolutionCode && ex.BusinessProfileCode == eur.BusinessProfileCode)
                                {
                                    var externalUserRole = new ExternalUserRoles
                                    {
                                        SolutionCode = eur.SolutionCode,
                                        RoleCode = eur.RoleCode,
                                        RoleName = eur.RoleName,
                                    };
                                    externalUserRoles.Add(externalUserRole);
                                }
                            }

                            if (us.SolutionCode == ex.SolutionCode)
                            {
                                var externalUser = new ExternalUser
                                {
                                    BusinessProfileCode = ex.BusinessProfileCode,
                                    PartnerCode = ex.PartnerCode,
                                    isDisabled = ex.isDisabled,
                                    CompanyAccountStatusCode = ex.CompanyAccountStatusCode,
                                    BlockStatusCode = ex.BlockStatusCode,
                                    UserAccountStatusCode = ex.UserAccountStatusCode,
                                    CompanyName = ex.CompanyName,
                                    ExternalUserRoles = externalUserRoles,
                                    SolutionCode = ex.SolutionCode,
                                    CustomerTypeCode = ex.CustomerTypeCode,
                                    CustomerTypeDescription = ex.CustomerTypeDescription,
                                    PartnerTypeCode = ex.PartnerTypeCode,
                                    PartnerTypeDescription = ex.PartnerTypeDescription,
                                    BusinessDeclarationStatusCode = ex.BusinessDeclarationStatusCode,
                                    BusinessDeclarationStatusDescription = ex.BusinessDeclarationStatusDescription,
                                    CustomerVerificationCode = ex.CustomerVerificationCode,
                                    EKYCVerificationStatusCode = ex.EKYCVerificationStatusCode,
                                    EKYCVerificationStatusDescription = ex.EKYCVerificationStatusDescription,
                                    F2FVerificationStatusCode = ex.F2FVerificationStatusCode,
                                    F2FVerificationStatusDescription = ex.F2FVerificationStatusDescription,
                                    TrangloEntityCode = ex.TrangloEntityCode,
                                    RegistrationDate = ex.RegistrationDate,
                                    PartnerAccountStatusCode = ex.PartnerAccountStatusCode,
                                    PartnerAccountStatusDescription = ex.PartnerAccountStatusDescription
                                };

                                externalUsers.Add(externalUser);
                            }

                            //var externalUser = new ExternalUser
                            //{
                            //    BusinessProfileCode = ex.BusinessProfileCode,
                            //    PartnerCode = ex.PartnerCode,
                            //    CompanyAccountStatusCode = ex.CompanyAccountStatusCode,
                            //    BlockStatusCode = ex.BlockStatusCode,
                            //    UserAccountStatusCode = ex.UserAccountStatusCode,
                            //    CompanyName = ex.CompanyName,
                            //    ExternalUserRoles = externalUserRoles,
                            //    SolutionCode = ex.SolutionCode,
                            //};

                            //// remove duplicate external users in response
                            //if (!externalUsers.Any(x => x.BusinessProfileCode == ex.BusinessProfileCode && x.PartnerCode == ex.PartnerCode))
                            //{
                            //    externalUsers.Add(externalUser);
                            //}
                        }

                        var solution = new UserSolution
                        {
                            SolutionCode = us.SolutionCode,
                            SolutionDescription = us.SolutionDescription,
                            ExternalUser = externalUsers
                        };

                        userSolutions.Add(solution);
                    }

                    outputDTO.Timezone = result.FirstOrDefault().Timezone;
                    outputDTO.UserSolution = userSolutions;
                }

                return Result.Success(outputDTO);
            }
        }
    }
}
