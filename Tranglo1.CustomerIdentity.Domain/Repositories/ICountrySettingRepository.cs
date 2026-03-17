using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Events;

namespace Tranglo1.CustomerIdentity.Domain.Repositories
{
    public interface ICountrySettingRepository
    {
        Task<Result<CountrySetting>> SaveCountrySettingAsync(CountrySetting countrySettingInfo);
        Task<Result<CountrySetting>> UpdateCountrySettingAsync(CountrySetting countrySettingInfo);
        Task<Result<CountrySetting>> DeleteCountrySettingAsync(CountrySetting countrySettingInfo);
        Task<CountrySetting> GetCountrySettingByCodeAsync(int countrySettingCode);
        Task<List<CountryMeta>> GetIsDisplayCountriesAsync();
        Task<bool> IsCountrySettingExistAsync(string countryISO2);
        Task<List<CountryMeta>> GetNoHighRiskNoSanctionCountriesAsync();
        Task<CountrySettingsChangedEvent> AddCountrySettingChangedEventAsync(CountrySettingsChangedEvent countrySettingsChangedEvent);
    }
}
