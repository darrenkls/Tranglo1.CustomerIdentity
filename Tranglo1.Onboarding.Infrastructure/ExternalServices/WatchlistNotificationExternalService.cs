using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Common.SingleScreening;
using Tranglo1.CustomerIdentity.Domain.ExternalServices.Watchlist;
using Tranglo1.CustomerIdentity.Domain.ExternalServices.Watchlist.Models.Requests;
using Tranglo1.CustomerIdentity.Infrastructure.Extensions;

namespace Tranglo1.CustomerIdentity.Infrastructure.ExternalServices
{
    public class WatchlistNotificationExternalService : IWatchlistNotificationExternalService
    {
        private readonly HttpClient _httpClient;

        public WatchlistNotificationExternalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<bool, string>> SendAsync(List<ChangeDTO> changeDTOs, bool isSingleProfileScreening, string singlePartnerName)
        {
            try
            {
                var request = new WatchlistNotificationRequest()
                {
                    ChangeDTOs = changeDTOs,
                    IsSingleProfileScreening = isSingleProfileScreening,
                    SinglePartnerName = singlePartnerName
                };

                var response = await _httpClient.PostAsJsonAsync("api/v1/watchlist/notification", request);
                var result = await response.ReadFromJsonAsync<bool>();
                return Result.Success<bool, string>(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool, string>(ex.ToString());
            }
        }
    }
}
