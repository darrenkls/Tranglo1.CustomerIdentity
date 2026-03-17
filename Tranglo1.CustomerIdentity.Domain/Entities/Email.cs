using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
	public class Email : ValueObject
	{
        public Email(): base()
        {

        }
		public const int MaxLength = 320;
		public string Value { get; }

		private Email(string value)
		{
			this.Value = value;
		}

		public static Result<Email> Create(string value)
		{
			value = value?.Trim();

			if (string.IsNullOrEmpty(value))
				return Result.Failure<Email>("Email cannot be blank");

			if (value.Length > MaxLength)
				return Result.Failure<Email>($"Email cannot be longer than {MaxLength} characters.");

			try
			{
				MailAddress address = new MailAddress(value);
			}
			catch (FormatException ex)
			{
				return Result.Failure<Email>(ex.Message);
			}

			return Result.Success<Email>(new Email(value));
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Value;
		}
	}
}
