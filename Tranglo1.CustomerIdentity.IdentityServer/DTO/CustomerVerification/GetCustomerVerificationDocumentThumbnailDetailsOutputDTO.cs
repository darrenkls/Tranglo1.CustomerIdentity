using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.DTO.CustomerVerification
{
    public class GetCustomerVerificationDocumentThumbnailDetailsOutputDTO
    {
        public byte[] FileData { get; set; }
        public Stream File { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
    }
}
