using CSharpFunctionalExtensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Entities.PartnerManagement;
using Tranglo1.CustomerIdentity.Domain.Entities.Specifications.CustomerUserBusinessProfiles;
using Tranglo1.CustomerIdentity.Domain.Repositories;

namespace Tranglo1.CustomerIdentity.Domain.DomainServices
{
    public class ApplicationUserService
    {
        private readonly IPartnerRepository partnerRepository;
        private readonly IBusinessProfileRepository businessProfileRepository;
        private readonly IApplicationUserRepository applicationUserRepository;
        private readonly BusinessProfileService businessProfileService;
        private readonly IConfiguration configuration;

        protected IApplicationUserRepository Repository => applicationUserRepository;
        public ApplicationUserService(
            IPartnerRepository partnerRepository,
             IBusinessProfileRepository businessProfileRepository,
             IApplicationUserRepository applicationUserRepository,
            BusinessProfileService businessProfileService,
            IConfiguration config)
        {
            this.partnerRepository = partnerRepository;
            this.businessProfileRepository = businessProfileRepository;
            this.businessProfileService = businessProfileService;
            this.applicationUserRepository = applicationUserRepository;
            this.configuration = config;
        }

        public async Task<Result<List<TrangloStaffEntityAssignment>>> GetTrangloStaffEntityAssignmentAsync(
           TrangloStaff staff, string entityCode)
        {
            var trangloStaffEntity = await applicationUserRepository.GetTrangloStaffEntityAssignmentByUserId(staff.Id);
            foreach(var item in trangloStaffEntity)
            {
                if(item.TrangloEntity == entityCode)
                {
                    return Result.Success(trangloStaffEntity);
                }
            }
            return Result.Failure<List<TrangloStaffEntityAssignment>>("Tranglo Entity does not match.");
        }

        public async Task<bool> UserHasTrangloEntity(TrangloStaff trangloStaff, string entityCode)
        {
            var trangloStaffEntity = await this.applicationUserRepository.GetTrangloStaffEntityAssignmentById(trangloStaff.LoginId);


            if (trangloStaff != null)
            {
                //var check = trangloStaffEntity.Where(x => x.TrangloEntity == trangloEntityByPartner);
                foreach (var item in trangloStaffEntity)
                {
                    if (item.TrangloEntity.Trim().ToLower() == entityCode.Trim().ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
