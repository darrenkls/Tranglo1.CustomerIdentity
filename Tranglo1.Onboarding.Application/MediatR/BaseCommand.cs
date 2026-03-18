using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.MediatR;

namespace Tranglo1.CustomerIdentity.IdentityServer.Command
{
	/// <summary>
	/// A marker class to represent a command.
	/// </summary>
	/// <typeparam name="TResponse"></typeparam>
	public abstract class BaseCommand<TResponse> : BaseRequest<TResponse>
	{

	}
}
