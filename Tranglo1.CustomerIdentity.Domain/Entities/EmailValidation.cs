using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class EmailValidation
    {
        public string AdminPortalSuccessURL { get; set; }
        public string AdminPortalFailedURL { get; set; }
        public string ConnectPortalSuccessURL { get; set; }
        public string ConnectPortalFailedURL { get; set; }
    }
}
