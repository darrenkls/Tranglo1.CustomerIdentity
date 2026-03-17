using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetTrangloStaffByClaimsQuery : IRequest<Result<TrangloStaffByClaimsOutputDTO>>
    {
        public string LoginId { get; set; }
        public class GetTrangloStaffByClaimsQueryHandler : IRequestHandler<GetTrangloStaffByClaimsQuery, Result<TrangloStaffByClaimsOutputDTO>>
        {
            private readonly IConfiguration _config;
            private class GetTrangloStaffView
            {         
                public string Timezone { get; set; }
                public string LoginId { get; set; }
                public string RoleName { get; set; }
                public string EntityName { get; set; }
                public string RoleCode { get; set; }
                public string EntityId { get; set; }
                public int BlockStatusCode { get; set; }
                public int UserAccountStatusCode { get; set; }
                public int AuthorityLevelCode { get; set; }
                public int IsSuperApprover { get; set; }
            }
            public GetTrangloStaffByClaimsQueryHandler(IConfiguration config)
            {
                _config = config;
            }
            public async Task<Result<TrangloStaffByClaimsOutputDTO>> Handle(GetTrangloStaffByClaimsQuery request, CancellationToken cancellationToken)
            {
                var outputDTO = new List<TrangloStaffs>();
                TrangloStaffByClaimsOutputDTO outputDTOs = new TrangloStaffByClaimsOutputDTO();
                var _connectionString = _config.GetConnectionString("DefaultConnection");
                string timezone = String.Empty;
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                        "GetTrangloStaffByClaims",
                        new
                        {
                            LoginId = request.LoginId
                        },
                        null, null, CommandType.StoredProcedure);
                    var results = await reader.ReadAsync<GetTrangloStaffView>();
                    var userEntityGrouping = results.GroupBy(x => new { x.EntityId }).Select(x => x.First()).ToList();
                    
                    foreach(var item in userEntityGrouping)
                    {
                        timezone = item.Timezone;
                        List<TrangloStaffRoles> trangloStaffRoles = new List<TrangloStaffRoles>();
                        var userRolesResult = results.Where(x => x.EntityId == item.EntityId);
                        foreach (var items in userRolesResult)
                        {
                            bool b = Convert.ToBoolean(items.IsSuperApprover);

                            var trangloStaffRole = new TrangloStaffRoles
                            {
                                RoleCode = items.RoleCode,
                                RoleName = items.RoleName,
                                AuthorityLevelCode = items.AuthorityLevelCode,
                                IsSuperApprover = b
                            };
                            trangloStaffRoles.Add(trangloStaffRole);
                        }
                        var trangloStaffByClaim = new TrangloStaffs
                        {
                            UserAccountStatusCode = item.UserAccountStatusCode,
                            EntityId = item.EntityId,
                            EntityName = item.EntityName,
                            BlockStatusCode = item.BlockStatusCode,
                            TrangloStaffRole = trangloStaffRoles
                        };
                        outputDTO.Add(trangloStaffByClaim);
                    }
                }
                outputDTOs.Timezone = timezone;
                outputDTOs.TrangloStaffs = outputDTO;
                return Result.Success(outputDTOs);
            }
        }
    }
}
