using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tranglo1.CustomerIdentity.Domain.Entities
{
    public class FullName: ValueObject
	{
        public FullName(): base()
        {

        }

		public const int MaxLength = 300;
		public string Value { get; }

		private FullName(string value)
		{
			this.Value = value;
		}

		public static Result<FullName> Create(string value)
		{
			value = value?.Trim();

			if( !string.IsNullOrEmpty(value) )
			{
				value = Regex.Replace(value, @"\s+", " "); //Replace multiple white spaces to a single whitespace using Regex
			}

			if (string.IsNullOrEmpty(value))
				return Result.Failure<FullName>("Full Name cannot be blank");


			if (value.Length > MaxLength)
				return Result.Failure<FullName>($"Full Name cannot be longer than {MaxLength} characters.");

			/*
			if ( !Regex.IsMatch(value, @"^[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*$"))
				return Result.Failure<FullName>($"Full Name format is incorrect");
			*/
			return Result.Success<FullName>(new FullName(value));
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return Value;
		}

	}
}
