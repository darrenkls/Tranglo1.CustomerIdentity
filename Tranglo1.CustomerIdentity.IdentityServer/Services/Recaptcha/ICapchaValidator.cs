using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.Recaptcha
{
	public interface ICapchaValidator
	{
		Task<Result<bool>> ValidateAsync(string token);
	}
}
