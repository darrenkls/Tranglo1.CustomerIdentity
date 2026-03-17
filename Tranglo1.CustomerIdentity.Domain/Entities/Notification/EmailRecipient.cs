using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
   public class EmailRecipient : Entity
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public NotificationTemplate NotificationTemplate { get; set; }
        public RecipientType RecipientType { get; set; }
        public CollectionTier CollectionTier { get; set; }
        public AuthorityLevel AuthorityLevel { get; set; }

    }
}
