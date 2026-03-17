using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities.Meta
{
    public class SubmissionResult : Enumeration
    {
        public SubmissionResult() : base()
        {

        } 

        public SubmissionResult(int id,string name) : base(id, name)
        {

        }

        public static readonly SubmissionResult Processing = new SubmissionResult(1, "Processing.");
        public static readonly SubmissionResult Failed_Please_Resubmit = new SubmissionResult(2, "Failed.Please Resubmit.");
        public static readonly SubmissionResult Submitted = new SubmissionResult(3, "Submitted.");
        public static readonly SubmissionResult Service_Not_Available = new SubmissionResult(4, "Service not available. Please resubmit");

    }
}
