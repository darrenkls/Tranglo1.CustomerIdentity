using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.TransactionEvaluation
{
    public class TransactionEvaluationInputDTO : SharedTransactionEvaluationDTO
    {
        public List<QuestionInputDTO> Questions { get; set; }
    }

    public class QuestionInputDTO : SharedQuestionDTO
    {
        public List<AnswerInputDTO> Answers { get; set; }
    }

    public class AnswerInputDTO : SharedAnswerDTO
    {
        public bool IsRemoved { get; set; }
    }
}
