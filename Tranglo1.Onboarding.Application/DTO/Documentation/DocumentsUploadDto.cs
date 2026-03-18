using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class DocumentsUploadDto
    {
        public Stream File { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
