using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Watchlist;

namespace Tranglo1.CustomerIdentity.Domain.Common.SingleScreening
{
    public class SingleDBScreeningInputDTO
    {
        public string ClientReference { set; get; }
        public Guid? Reference { set; get; }
        public string Fullname { set; get; }
        public string CompanyName { set; get; }
        public Summary Summary { set; get; }
        public bool IsChange { set; get; }
        public List<ChangeDTO> Changes { set; get; } // Add this property


        public SingleDBScreeningInputDTO()
        {
            Changes = new List<ChangeDTO>();
        }
    }
}
