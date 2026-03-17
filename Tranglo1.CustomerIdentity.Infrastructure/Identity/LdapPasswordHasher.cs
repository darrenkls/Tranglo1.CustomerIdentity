using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;

namespace Tranglo1.CustomerIdentity.Infrastructure.Identity
{
	public sealed class LdapConfiguration
	{
		public LdapConfiguration()
		{

		}

		public LdapConfiguration(string ldapServer, string ldapDomain)
		{
			LdapServer = ldapServer;
			LdapDomain = ldapDomain;
		}

		public string LdapServer { get; set; }
		public string LdapDomain { get; set; }
	}


	class LdapPasswordHasher : PasswordHasher<ApplicationUser>
	{
		public LdapPasswordHasher(LdapConfiguration ldapConfiguration,
			LdapConnectionOptions ldapConnectionOptions, ILogger<LdapPasswordHasher> logger)
		{
			LdapConfiguration = ldapConfiguration;
			LdapConnectionOptions = ldapConnectionOptions;
			Logger = logger;
		}

		public LdapConfiguration LdapConfiguration { get; }
		public LdapConnectionOptions LdapConnectionOptions { get; }
		public ILogger<LdapPasswordHasher> Logger { get; }

		public override PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
		{
			if (user is TrangloStaff)
			{
				// Validate with AD
				return ValidateDomainCredential(user.LoginId, providedPassword);
			}

			return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
		}

		private PasswordVerificationResult ValidateDomainCredential(string userId, string password)
		{
			Logger.LogInformation($"Performing Tranglo Ldap authentication for [{userId}].");

			try
			{
				var username = $"{userId}@{LdapConfiguration.LdapDomain}";

				using (var connection = new LdapConnection(LdapConnectionOptions))
				{
					if (LdapConnectionOptions.Ssl)
					{
						connection.Connect(LdapConfiguration.LdapServer, LdapConnection.DefaultSslPort);
					}
					else
					{
						connection.Connect(LdapConfiguration.LdapServer, LdapConnection.DefaultPort);
					}

					connection.Bind(username, password);

					if (connection.Bound)
					{
						Logger.LogInformation($"Password validation success for [{userId}].");
						return PasswordVerificationResult.Success;
					}
					else
					{
						Logger.LogWarning($"Password validation failed for [{userId}].");
						return PasswordVerificationResult.Failed;
					}
				}
			}
			catch (LdapException ex)
			{
				switch (ex.ResultCode)
				{
					case LdapException.InvalidCredentials:
						return PasswordVerificationResult.Failed;

					case LdapException.InsufficientAccessRights:
						Logger.LogCritical($"Application do not has sufficient access rights to query LDAP server.");
						return PasswordVerificationResult.Failed;
					default:
						break;
				}

				Logger.LogError(ex.ToString());
				return PasswordVerificationResult.Failed;
			}
		}
	}
}
