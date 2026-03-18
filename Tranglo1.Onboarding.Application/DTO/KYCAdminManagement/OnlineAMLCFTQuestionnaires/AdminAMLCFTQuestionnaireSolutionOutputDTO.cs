using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.KYCAdminManagement.OnlineAMLCFTQuestionnaires
{
    public class AdminAMLCFTQuestionnaireSolutionOutputDTO
    {
        public long? SolutionCode { get; set; }
        public AdminAMLCFTQuestionnaireOutputDTO QuestionnaireOutputDTO { get; set; }

    }
}
