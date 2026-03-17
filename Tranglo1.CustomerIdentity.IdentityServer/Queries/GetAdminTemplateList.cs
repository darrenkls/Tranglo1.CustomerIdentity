using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Documentation;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using Tranglo1.UserAccessControl;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Documentation.AdminTemplateOutputDTO;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    //[Permission(PermissionGroupCode.KYCTemplatesManagement, UACAction.View)]
    [Permission(Permission.KYCAdministrationTemplatesManagement.Action_View_Code,
        new int[] { (int)PortalCode.Admin },
        new string[] {})]
    internal class GetAdminTemplateList : BaseQuery<IEnumerable<AdminTemplateOutputDTO>>
    {
     public int SolutionCode { get; set; }
    
    }

    internal class GetAdminTemplateListHandler : IRequestHandler<GetAdminTemplateList, IEnumerable<AdminTemplateOutputDTO>>
    {
        private readonly IConfiguration _config;

        public GetAdminTemplateListHandler(IConfiguration config)
        {
            _config = config;
        }

      
        public async Task<IEnumerable<AdminTemplateOutputDTO>> Handle(GetAdminTemplateList request, CancellationToken cancellationToken)
        {
            var _connectionString = _config.GetConnectionString("DefaultConnection");

            IEnumerable<AdminTemplateOutputDTO> DocumentDataDtos;
            IEnumerable<TrangloEntity> TrangloEntities;


            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var reader = await connection.QueryMultipleAsync(
                    "GetAdminTemplateList",
                    new
                    { 
                        SolutionCode = request.SolutionCode
                    },

                   null,null, CommandType.StoredProcedure);

                DocumentDataDtos = await reader.ReadAsync<AdminTemplateOutputDTO>();
                TrangloEntities = await reader.ReadAsync<TrangloEntity>();

            }

            foreach (var template in DocumentDataDtos)
            {
                template.TrangloEntities = TrangloEntities.Where(x => x.CategoryId == template.CategoryId && x.QuestionnaireCode == template.QuestionnaireCode && x.isChecked).OrderBy(x => x.TrangloEntityCode).ToList();
                foreach (var te in template.TrangloEntities)
                {
                    _ = te.TrangloEntityCode;
                }
            }
            return DocumentDataDtos;
        }
    }
}
