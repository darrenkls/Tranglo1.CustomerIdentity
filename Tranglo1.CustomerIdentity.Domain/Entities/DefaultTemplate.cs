using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Common;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class DefaultTemplate : Enumeration
    {
        public DefaultTemplate() : base()
        {

        }

        public DefaultTemplate(int id, string name)
            : base(id, name)
        {

        }

        public static readonly DefaultTemplate F2FVerificationTemplateMY = new DefaultTemplate(1, "Face-To-Face Verification Form MY");
        public static readonly DefaultTemplate F2FVerificationTemplateNonMY = new DefaultTemplate(2, "Face-To-Face Verification Form Non MY");
    }
}