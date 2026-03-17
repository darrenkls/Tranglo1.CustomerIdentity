using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class GetKYCSubmissionStatusOutputDto
    {
        public long? SubmissionStatusCode { get; set; }

        public string SubmissionStatusDescription { get; set; }
    }
}
