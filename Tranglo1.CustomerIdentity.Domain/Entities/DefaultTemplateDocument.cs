using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class DefaultTemplateDocument : Entity
    {
        // This entity is used to store any document templates that are not tied to Admin - Template Management
        // Steps to add a new template:
        // 1) Add a new template value in DefaultTemplate and add migration
        // 2) Use GET API "default-template/{defaultTemplateCode}" to upload the template document

        public DefaultTemplate DefaultTemplate { get; set; }
        public Guid? DocumentId { get; set; }

        private DefaultTemplateDocument() { }

        public DefaultTemplateDocument(DefaultTemplate defaultTemplate, Guid? documentId)
        {
            this.DefaultTemplate = defaultTemplate;
            this.DocumentId = documentId;
        }
    }
}