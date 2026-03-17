using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
	public class RegistryCode : ValueObject
	{
		public RegistryCode() : base()
		{

		}

		public const int MaxLength = 150;
		public string Value { get; }

		private RegistryCode(string value)
		{
			this.Value = value;
		}

		public static Result<RegistryCode> Create(string value)
		{
			value = value?.Trim();
			value = Regex.Replace(value, @"\s+", " "); //Replace multiple white spaces to a single whitespace using Regex

			if (string.IsNullOrEmpty(value))
				return Result.Failure<RegistryCode>("Registry Code cannot be blank");

			if (value.Length > MaxLength)
				return Result.Failure<RegistryCode>($"Registry Code cannot be longer than {MaxLength} characters.");

			return Result.Success(new RegistryCode(value));
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Value;
		}

	}
}