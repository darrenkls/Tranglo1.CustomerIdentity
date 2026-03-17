using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;

namespace Tranglo1.CustomerIdentity.IdentityServer.Services.UserProfile
{
    public class TrangloUserProfileService : DefaultProfileService
	{
		public TrangloUserProfileService(ILogger<TrangloUserProfileService> logger)
			: base(logger)
		{

		}

		public override Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			foreach (var identityResource in context.RequestedResources.Resources.IdentityResources)
			{
				foreach (var item in identityResource.UserClaims)
				{
					//Look for the requested claim type from subject claims (loaded from TrangloUserManager).
					//During the first login, the claim type will be Microsoft ClaimTypes, and 
					//when client is refreshing the access token, the claim type will be the shorter version
					//Example (the email clam):
					//		First login                    : ClaimTypes.Email
					//		Subsequence token refresh call : "email"
					//
					//Hence, the code below will tryt to look for "email" claim type. If no claim is found,
					//then the Microsoft claim type mapping is retrieve
					//from JwtSecurityTokenHandler.DefaultInboundClaimTypeMap

					var _ClaimFromSubject = context.Subject.Claims.FirstOrDefault(c => c.Type == item);
					if (_ClaimFromSubject == null)
					{
						if (JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.TryGetValue(item, out string _MicrosoftClaimType))
						{
							_ClaimFromSubject = context.Subject.Claims.FirstOrDefault(c => c.Type == _MicrosoftClaimType);
						}
					}

					if (_ClaimFromSubject != null)
					{
						context.IssuedClaims.Add(new Claim(item, _ClaimFromSubject.Value, _ClaimFromSubject.ValueType));
					}
				}
			}

			//collect all the wildcard claim type, the value will include "."
			List<string> _wildcardClaims = new List<string>();

            //The subject claims we are having here may contains claim type = ClaimTypes.Role.
            //We need to include them into ProfileDataRequestContext.IssuedClaims if 
            //ProfileDataRequestContext.RequestedClaimTypes contains type "role" (defined as constant JwtClaimTypes.Role)
            if (context.RequestedClaimTypes.Contains(JwtClaimTypes.Role))
            {
                var transformedRoles = from c in context.Subject.Claims
                                       where c.Type == ClaimTypes.Role
                                       select new Claim(JwtClaimTypes.Role, c.Value, c.ValueType);

                context.IssuedClaims.AddRange(transformedRoles);
            }

			var _solutionClaims = context.Subject.Claims.FirstOrDefault(c => c.Type == ClaimCode.Solution);
			if (_solutionClaims == null && context.Client.ClientId.ToLower().Contains(ClaimCode.Connect))
            {
				//Add connect claims
				context.IssuedClaims.Add(new Claim(ClaimCode.Solution, ClaimCode.Connect)); //TODO: Change solution to a constant
			}
			else if (_solutionClaims == null && context.Client.ClientId.ToLower().Contains(ClaimCode.Business))
			{
				//Add business claims
				context.IssuedClaims.Add(new Claim(ClaimCode.Solution, ClaimCode.Business));
			}


			//TODELETE
			var _UserIdClaim = from c in context.Subject.Claims
							   where string.Equals("userid", c.Type)
							   select c;

			if (_UserIdClaim.Any())
			{
				context.IssuedClaims.Add(_UserIdClaim.First());
			}

			var identityResources = context.RequestedResources.Resources.IdentityResources;
			foreach (var resource in identityResources)
			{
				foreach (var claimType in resource.UserClaims)
				{
					if (claimType.EndsWith("*"))
					{
						_wildcardClaims.Add(claimType.Replace("*", ""));
					}
				}
			}

			foreach (var wildcard in _wildcardClaims)
			{
				foreach (var claim in context.Subject.Claims)
				{
					if (_wildcardClaims.Any(wildcard => claim.Type.StartsWith(wildcard)))
					{
						context.IssuedClaims.Add(claim);
					}
				}
			}

			return Task.CompletedTask;
		}
	}
}
