using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.DTO.Meta;

namespace Tranglo1.CustomerIdentity.IdentityServer.Models
{
    public class RegisterViewModel : RegisterInputModel
    {
        public IEnumerable<CountryListOutputDTO> Countries { get; set; }
        public IEnumerable<SolutionListOutputDTO> Solutions { get; set; }
        //public IEnumerable<UserTypeListOutputDTO> UserTypes { get; set; }
        public IEnumerable<CustomerTypeListOutputDTO> CustomerTypes { get; set; }

        public IEnumerable<PartnerRegistrationLeadsOriginOutputDTO> PartnerRegistrationLeadsOrigin { get; set; }

        public int FormType { get; set; }
        /*
            0 - RESET FORM
            1 - SELF SIGN UP - CONNECT
            2 - SELF SIGN UP - BUSINESS
            3 - SIGN UP WITH CODE - Connect / Business ( Non Individual )
            4 - SIGN UP WIT H CODE - Individual
        */
    }
}
