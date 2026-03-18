using System.Collections.Generic;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.Documentation.AdminTemplateManagementInputDTO
{
    public class AdminTemplateManagementInputDTO
    {
        public long? QuestionnaireCode { get; set; }
        public long SolutionCode { get; set; }
        public List<TrangloEntity> TrangloEntities { get; set; }
        

    }

    public class TrangloEntity
    {
       public  long TrangloEntityCode { get; set; }
       public bool? IsChecked { get; set; }
    }
}
