using System;
using System.Collections.Generic;
using System.Text;

namespace Tranglo1.CustomerIdentity.Domain.Events
{
    public class CountrySettingsChangedEvent
    {
        public long EventId { get; set; }
        public int CountryCode { get; set; }
        public string CountryISO2 { get; set; }
        public bool IsHighRisk { get; set; }
        public bool IsSanction { get; set; }
        public bool IsDisplay { get; set; }
        public bool IsRejectTransaction { get; set; }


        internal CountrySettingsChangedEvent()
        { }

        public CountrySettingsChangedEvent( int countryCode, string countryISO2, bool isHighRisk, bool isSanction, bool isDisplay, bool isRejectTransaction)
        {
            CountryCode = countryCode;
            CountryISO2 = countryISO2;
            IsHighRisk = isHighRisk;
            IsSanction = isSanction;
            IsDisplay = isDisplay;
            IsRejectTransaction = isRejectTransaction;
        }
    }

    
}
