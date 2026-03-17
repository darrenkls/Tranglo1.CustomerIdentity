using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic; 
using System.Security.Cryptography;
using System.Text;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Novell.Directory.Ldap;


namespace Tranglo1.CustomerIdentity.Infrastructure.Identity
{
    public class TrangloPasswordHasher : PasswordHasher<ApplicationUser>
    {
        public const string Domain = "TRANGLO";
        public const string DomainComponent = "NET";

        public override PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            if (user is TrangloStaff)
            {
                // Validate with AD
                return ValidateDomainCredential(user.LoginId, providedPassword);
            }

            return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }

        private PasswordVerificationResult ValidateDomainCredential(string userId, string password, int domainPortNumber = LdapConnection.DefaultPort)
        {
            try
            {
                var username = $"{userId}@{Domain}";
                var host = $"{Domain}.{DomainComponent}";
                using (var connection = new LdapConnection())
                {
                    connection.Connect(host, domainPortNumber);
                    connection.Bind(username, password);
                    return connection.Bound ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
                }
            }
            catch (LdapException ex)
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }
}
