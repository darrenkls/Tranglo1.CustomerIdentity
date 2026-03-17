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
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.AdminManagement;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.TrangloUser, UACAction.View)]
    internal class GetTrangloStaffByLoginIdQuery : BaseQuery<Result<IEnumerable<TrangloStaffOutputDTO>>>
    {
        public string LoginId { get; set; }
        public override Task<string> GetAuditLogAsync(Result<IEnumerable<TrangloStaffOutputDTO>> result)
        {
            if (result.IsSuccess) 
            {
                return Task.FromResult("Viewed User Details");
            }

            return base.GetAuditLogAsync(result);
        }

        private class TrangloStaffView
        {
            public string FullName { get; set; }
            public string LoginId { get; set; }
            public string Email { get; set; }
            public string TrangloRoleCode { get; set; }
            public string TrangloRoleDesc { get; set; }
            public string TrangloEntityId { get; set; }
            public string TrangloEntityDesc{ get; set; }
            public string Timezone { get; set; }
            public int TrangloDepartmentCode { get; set; }
            public string TrangloDepartmentDesc { get; set; }
            public string AccountStatus { get; set; }
            public long AccountStatusId { get; set; }
        }
        public class GetTrangloStaffByLoginIdQueryHandler : IRequestHandler<GetTrangloStaffByLoginIdQuery, Result<IEnumerable<TrangloStaffOutputDTO>>>
        {
            private readonly IConfiguration _config;

            public GetTrangloStaffByLoginIdQueryHandler(IConfiguration config)
            {
                _config = config;
            }

            public async Task<Result<IEnumerable<TrangloStaffOutputDTO>>> Handle(GetTrangloStaffByLoginIdQuery request, CancellationToken cancellationToken)
            {
                IEnumerable<TrangloStaffOutputDTO> result;
                var outputDTO = new List<TrangloStaffOutputDTO>();
                var _connectionString = _config.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var reader = await connection.QueryMultipleAsync(
                        "GetTrangloStaffById",
                        new
                        {
                            LoginId = request.LoginId
                        },
                        null, null, CommandType.StoredProcedure);
                   var results = await reader.ReadAsync<TrangloStaffView>();

                   var userResultGroup = results.GroupBy(x => new { x.LoginId}).Select(y => y.First()).ToList();

                    foreach(var u in userResultGroup)
                    {
                        var singleTrangloStaff = new TrangloStaffOutputDTO
                        {
                            AccountStatus = u.AccountStatus,
                            Email = u.Email,
                            FullName = u.FullName,
                            LoginId = u.LoginId,
                            Timezone = u.Timezone,
                            AccountStatusId = u.AccountStatusId
                        };

                        var entitiesGroup = results.Where(x => x.LoginId == u.LoginId).ToList();
                        foreach (var r in entitiesGroup)
                        {
                            var trangloUserRoles = new TrangloStaffOutputDTO.TrangloStaffEntities
                            {
                                TrangloDepartmentCode = r.TrangloDepartmentCode,
                                TrangloDepartmentDesc = r.TrangloDepartmentDesc,
                                TrangloRoleCode = r.TrangloRoleCode,
                                TrangloRoleDesc = r.TrangloRoleDesc,
                                TrangloEntityId = r.TrangloEntityId,
                                TrangloEntityDesc = r.TrangloEntityDesc
                            };
                            singleTrangloStaff.TrangloStaffEntity.Add(trangloUserRoles);
                        }

                        outputDTO.Add(singleTrangloStaff);
                    }
                    result = outputDTO;
                }
                return Result.Success<IEnumerable<TrangloStaffOutputDTO>>(result);
            }
        }
    }
}
