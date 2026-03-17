using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    public class TrangloUserGroupingOutputDTO
    {
        public string FullName { get; set; }

        public string Email { get; set; }
        public string LoginId { get; set; }
        public string AccountStatus { get; set; }
        public string Timezone { get; set; }
        public List<TrangloUserEntitiesGroupingOutputDTO> Entities { get; set; } = new List<TrangloUserEntitiesGroupingOutputDTO>();

        public class TrangloUserEntitiesGroupingOutputDTO
        {
            public string TrangloEntity { get; set; }
            public string TrangloEntityId { get; set; }
            public List<TrangloUserRoleGroupingOutputDTO> RoleGrouping { get; set; } = new List<TrangloUserRoleGroupingOutputDTO>();

            public class TrangloUserRoleGroupingOutputDTO
            {
                public string TrangloDepartment { get; set; }
                public string TrangloRole { get; set; }
                public string BlockStatus { get; set; }
            }
        }
       
    }
}
