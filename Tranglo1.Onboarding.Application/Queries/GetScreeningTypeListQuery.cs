using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities.ScreeningAggregate;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;

namespace Tranglo1.CustomerIdentity.IdentityServer.Queries
{
    internal class GetScreeningTypeListQuery : BaseQuery<IEnumerable<ScreeningDetailsCategoryOutputDTO>>
    {
        public override Task<string> GetAuditLogAsync(IEnumerable<ScreeningDetailsCategoryOutputDTO> result)
        {
            return base.GetAuditLogAsync(result);
        }

        public class GetScreeningTypeListQueryHandler : IRequestHandler<GetScreeningTypeListQuery, IEnumerable<ScreeningDetailsCategoryOutputDTO>>
        {
            public GetScreeningTypeListQueryHandler()
            {

            }

            public Task<IEnumerable<ScreeningDetailsCategoryOutputDTO>> Handle(GetScreeningTypeListQuery query, CancellationToken cancellationToken)
            {
                var screeningDetailCategories = new List<ScreeningDetailsCategory>()
                {
                    ScreeningDetailsCategory.Sanctions,
                    ScreeningDetailsCategory.PEP,
                    ScreeningDetailsCategory.SOE,
                    ScreeningDetailsCategory.Adverse_Media,
                    ScreeningDetailsCategory.Enforcement
                };

                var result = screeningDetailCategories.ConvertAll(category => new ScreeningDetailsCategoryOutputDTO
                {
                    ScreeningTypeListCode = (int)category.Id,
                    Description = category.Name
                });

                return Task.FromResult<IEnumerable<ScreeningDetailsCategoryOutputDTO>>(result);
            }
        }
    }
}
