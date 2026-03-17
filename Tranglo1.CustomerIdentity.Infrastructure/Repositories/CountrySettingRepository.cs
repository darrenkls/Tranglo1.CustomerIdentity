using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.Infrastructure.Persistence;
using Tranglo1.CustomerIdentity.Domain.Entities;
using CSharpFunctionalExtensions;
using System.Data; 
using Dapper;
using Microsoft.Data.SqlClient;
using Tranglo1.CustomerIdentity.Domain.Events;

namespace Tranglo1.CustomerIdentity.Infrastructure.Repositories
{
    public class CountrySettingRepository : ICountrySettingRepository
    {
        private readonly CountrySettingDbContext dbContext;
        private readonly IConfiguration _config;


        public CountrySettingRepository(CountrySettingDbContext dbContext, IConfiguration config)
        {
            this.dbContext = dbContext;
            _config = config;
        }

        public async Task<Result<CountrySetting>> SaveCountrySettingAsync(CountrySetting countrySettingInfo)
        {
            this.dbContext.CountrySettings.Attach(countrySettingInfo);
            if (countrySettingInfo.Id != 0)
            {
                this.dbContext.Entry(countrySettingInfo).State = EntityState.Modified;
            }
            await this.dbContext.SaveChangesAsync();
            return countrySettingInfo;
        }

        public async Task<Result<CountrySetting>> UpdateCountrySettingAsync(CountrySetting countrySettingInfo)
        {
            if (countrySettingInfo.Id != 0)
            {
                this.dbContext.Entry(countrySettingInfo).State = EntityState.Modified;
            }
            await this.dbContext.SaveChangesAsync();
            return countrySettingInfo;
        }

        public async Task<Result<CountrySetting>> DeleteCountrySettingAsync(CountrySetting countrySettingInfo)
        {
            this.dbContext.CountrySettings.Remove(countrySettingInfo);
            await this.dbContext.SaveChangesAsync();
            return countrySettingInfo;
        }

        public async Task<CountrySetting> GetCountrySettingByCodeAsync(int countrySettingCode)
        {
            return await dbContext.CountrySettings
                         .Where(c => c.Id == countrySettingCode)
                         .Include(c => c.Country)
                         .FirstOrDefaultAsync();

        }
        public async Task<List<CountryMeta>> GetIsDisplayCountriesAsync()
        {
            List<CountryMeta> result = new List<CountryMeta>();
            var connectionString = _config.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var reader = await connection.QueryMultipleAsync(
                    "GetDisplayedCountryMetas",
                    new { },
                    null, null, CommandType.StoredProcedure);

                result = (List<CountryMeta>)await reader.ReadAsync<CountryMeta>();
            }
            return result;
        }

        public async Task<bool> IsCountrySettingExistAsync(string countryISO2)
        {
            return await dbContext.CountrySettings
                .AnyAsync(x => x.Country.CountryISO2 == countryISO2);
        }

        public async Task<List<CountryMeta>> GetNoHighRiskNoSanctionCountriesAsync()
        {
            List<CountryMeta> result = new List<CountryMeta>();
            var connectionString = _config.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var reader = await connection.QueryMultipleAsync(
                    "GetNoHighRiskNoSanctionCountryMetas",
                    new { },
                    null, null, CommandType.StoredProcedure);

                result = (List<CountryMeta>)await reader.ReadAsync<CountryMeta>();
            }
            return result;
        }

        public async Task<CountrySettingsChangedEvent> AddCountrySettingChangedEventAsync(CountrySettingsChangedEvent countrySettingsChangedEvent)
        {
            this.dbContext.CountrySettingsChangedEvents.Add(countrySettingsChangedEvent);
            await this.dbContext.SaveChangesAsync();
            return countrySettingsChangedEvent;
        }
    }
}

