using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class ContactNumber : ValueObject
	{
		public ContactNumber() : base()
		{

		}
		public const int MaxLength = 50;
		public string CountryISO2Code { get; }
		public string DialCode { get; }
        public string Value { get; }

		private ContactNumber(string dialCode, string countryISO2Code, string value)
		{
			this.DialCode = dialCode;
			this.Value = value;
			this.CountryISO2Code = countryISO2Code;
		}

		public static Result<ContactNumber> Create(string dialCode, string countryISO2Code, string value)
		{
			dialCode = dialCode?.Trim();

			var countryMeta = CountryMeta.GetCountryByDialCodeAsync(dialCode);

			/*if ( String.IsNullOrEmpty(dialCode))
				return Result.Failure<ContactNumber>("Contact Number Dial Code cannot be blank");

			if (!String.IsNullOrEmpty(dialCode) && countryMeta == null)
				return Result.Failure<ContactNumber>("Contact Number Dial Code is invalid");

			if (dialCode.Length > MaxLength)
				return Result.Failure<ContactNumber>($"Contact Number Dial Code cannot be longer than {MaxLength} characters.");*/


			var countryISOMeta = CountryMeta.GetCountryByISO2Async(countryISO2Code);

			/*if (String.IsNullOrEmpty(countryISO2Code))
				return Result.Failure<ContactNumber>("Contact Number Country Code cannot be blank");

			if (!String.IsNullOrEmpty(countryISO2Code) && countryISOMeta == null)
				return Result.Failure<ContactNumber>("Contact Number Country Code is invalid");*/

			//if (countryMeta != countryISOMeta )
			//	return Result.Failure<ContactNumber>("Country code & Dial code not matching");

			value = value?.Trim();

		/*	if (string.IsNullOrEmpty(value))
				return Result.Failure<ContactNumber>("Contact Number cannot be blank");

			if (value.Length > MaxLength)
				return Result.Failure<ContactNumber>($"Contact Number cannot be longer than {MaxLength} characters.");

			//Expecting format only digit
			if (!Regex.IsMatch(value, @"^[0-9]*$"))
				return Result.Failure<ContactNumber>($"Contact Number format is incorrect. Only allow digit.");*/

			return Result.Success<ContactNumber>(new ContactNumber(dialCode, countryISO2Code, value));
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return DialCode + Value;
		}

	}
}
