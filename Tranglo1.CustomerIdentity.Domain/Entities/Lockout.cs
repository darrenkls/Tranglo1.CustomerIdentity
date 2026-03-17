using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class Lockout
    {
        public int MaxFailedAccessAttempts { get; set; }
    }
}
