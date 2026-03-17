// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Constant;
using static System.Net.WebRequestMethods;

namespace Tranglo1.CustomerIdentity.IdentityServer
{
    /// <summary>
    /// Refer to this :
    /// https://leastprivilege.com/2016/12/01/new-in-identityserver4-resource-based-configuration/
    /// 
    /// In latest IdentityServer4, ApiScope is no longer applicable. We will only deal with
    /// <seealso cref="IdentityResource"/> and <seealso cref="ApiResource"/>
    /// </summary>
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),

                new IdentityResource()
                {
                    //TODELETE : Remove business_profile as separated into connect & business
					Name = "business_profile",
                    UserClaims = { "connect.*","business.*","blck_stat.*", "role.*", "act_stat.*", "comp*", "type","solution"},
                    DisplayName = "Business Profile",
                    Description = "Company Name (aka Business profile, only applicable to customer user)",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },
                new IdentityResource()
                {
                    Name = "connect_solution_business_profile",
                    UserClaims = { "connect.*", "type","solution"},
                    DisplayName = "Business Profile - Connect Solution",
                    Description = "Company Name (aka Business profile, only applicable to customer user for connect)",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },
                new IdentityResource()
                {
                    Name = "business_solution_business_profile",
                    UserClaims = { "business.*", "type","solution"},
                    DisplayName = "Business Profile - Business Solution",
                    Description = "Company Name (aka Business profile, only applicable to customer user for business)",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },

                new IdentityResource()
                {
                    //TODELETE : Remove company_name.* and account_status.* only
					Name = "tranglo_profile",
                    UserClaims = { "entity*", "account_status.*","role.*", "type", "blck_stat.*", "dept." },
                    DisplayName = "Entity",
                    Description = "Entity Name (aka Tranglo profile, only applicable to Tranglo user)",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },

                new IdentityResource()
                {
                    Name = "account_status",
                    UserClaims = { "account_status.*" }
                },

                //TODELETE
                new IdentityResource()
                {
                    Name = "roles",
                    UserClaims = { "role", "department" },
                    Description = "This can let system know your are external or internal user.",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },
                new IdentityResource()
                {
                    Name = "dynamicroles",
                    UserClaims = { "role.*" },
                    DisplayName = "Dynamic Roles",
                    Description = "Lists of user roles",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },
                new IdentityResource()
                {
                    Name = "company_name",
                    UserClaims = { "company_name.*" },
                    DisplayName = "Company Name",
                    Description = "Company Name",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },
                new IdentityResource()
                {
                    Name = "types",
                    UserClaims = { "type*" },
                    DisplayName = "client types",
                    Description = "This is to determine if a client is internal or external user",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },
                new IdentityResource()
                {
                    Name = "userid",
                    UserClaims = { "userid*" },
                    DisplayName = "user id",
                    Description = "Application user ID",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                },
                new IdentityResource()
                {
                    Name = "solution",
                    UserClaims = { "solution*" },
                    DisplayName = "Solution Id",
                    Description = "Solution ID",
                    Required = false,
                    Emphasize = false,
                    ShowInDiscoveryDocument = true
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                //TODELETE
                new ApiResource
                {
                    Name = "TransactionAPI",
                    DisplayName = "Dummy transaction API",
                    Scopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        IdentityServerConstants.StandardScopes.Email
                        //"api1.read", "api1.write", "api1.delete"
                    },
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    UserClaims = { "name", "role" }
                },
                //TODELETE
                new ApiResource
                {
                    Name = "api1",
                    DisplayName = "API #1",
                    Description = "Allow the application to access API #1 on your behalf",
                    Scopes = new List<string> {"api1.read", "api1.write", IdentityServerConstants.LocalApi.ScopeName},
                    ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                    UserClaims =  new List<string> {"role"}
                },

                new ApiResource
                {
                    Name = "business_profile",
                    DisplayName = "Onboading process Api",
                    Scopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "business_profile",
                        IdentityServerConstants.StandardScopes.Email
                    },
                    UserClaims = { "type" }
                },
                new ApiResource
                {
                    Name = "pricing_api",
                    DisplayName = "Pricing Api Microservice",
                    Scopes =  {"pricing.read", "pricing.write", IdentityServerConstants.LocalApi.ScopeName},
                    ApiSecrets =  {new Secret("secret".Sha256())},
                    UserClaims =   {"type","role"}
                },
                new ApiResource
                {
                    Name = "tb.customer-user",
                    //UserClaims =
                    //    {
                    //        JwtClaimTypes.Email,
                    //        JwtClaimTypes.Audience,
                    //        JwtClaimTypes.Issuer,
                    //        JwtClaimTypes.JwtId
                    //    },
                    Scopes = { "tb.customer-user" }
                }
            };

        /// <summary>
        /// No applicable in IdentityServer4 anymore. Please use "resouces"
        /// </summary>
        /// TODELETE
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1.read", "Read transaction")
                {
                    UserClaims =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new ApiScope("api1.write", "Create transaction")
                {
                    Required = false
                },
                new ApiScope("api1.delete", "Delete transaction")
                {
                    Required = false
                },
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName, "Identity Server Api")
                {
                    Required = false
                },
                new ApiScope("pricing.write", "Write Pricing")
                {
                    Required = false
                },
                new ApiScope("pricing.read", "Read Pricing")
                {
                    Required = false
                },
                new ApiScope("tb.customer-user", "(Tranglo Business) Customer Users")
                {
                    //UserClaims =
                    //{
                    //    IdentityServerConstants.StandardScopes.OpenId,
                    //    IdentityServerConstants.StandardScopes.Profile
                    //}
                }
            };


        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "api-client",
                    ClientSecrets = { new Secret("secret".Sha256()) }, 
                    //AccessTokenType = AccessTokenType.Reference,

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        IdentityServerConstants.StandardScopes.Email
                    }
                },

                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "API for MVC portal" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },
                    
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:7001/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:7001/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,

                        "roles",
                        "types",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = ".NET Core MVC client",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //connect-client production
                new Client
                {
                    ClientId = "connect-client",
                    ClientName = "Connect Angular Client Application Production",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Connect Angular Client Application Production" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "https://connect.tranglo.com/signin-oidc",
                        "https://connect-api.tranglo.com/swagger/oauth2-redirect.html",
                        "https://identity.tranglo.com/swagger/oauth2-redirect.html",
                        "https://pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://forex.tranglo.com/swagger/oauth2-redirect.html",
                        "https://product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://internal-compliance.tranglo.net/swagger/oauth2-redirect.html",

                        //TODO : Production TC Application link
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html"
                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://connect.tranglo.com",
                        "https://connect-api.tranglo.com",
                        "https://identity.tranglo.com",
                        "https://pricing.tranglo.net",
                        "https://notification.tranglo.net",
                        "https://forex.tranglo.net",
                        "https://wallet.tranglo.net",
                        "https://product.tranglo.net",
                        "https://global.tranglo.net",
                        "https://internal-compliance.tranglo.net",
                        "https://reporting.tranglo.net:1000",
                        "https://reporting.tranglo.net:1001",
                        "https://reporting.tranglo.net:1002",
                        "https://reporting.tranglo.net:1003",
                        "https://businesspayment.tranglo.net:1000",
                        "https://businesspayment.tranglo.net:1001",
                        "https://businesspayment.tranglo.net:1002",
                        "https://businesspayment.tranglo.net:1003",

                        //TODO : Production TC Application link
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://connect.tranglo.com/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile", //to remove
                        ClaimCode.Connect_BusinessProfile, //"connect_solution_business_profile",
                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Connect Angular Client Application Production",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //connect-client-uat
                new Client
                {
                    ClientId = "connect-client-uat",
                    ClientName = "Connect Angular Client Application UAT",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Connect Angular Client Application UAT" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:5004/signin-oidc",

                        "https://uat-connect.tranglo.net/signin-oidc",
                        "https://uat-identity.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-connect-api.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:1003/swagger/oauth2-redirect.html",

                        "https://uat-connect.tranglo.net:5000/signin-oidc",
                        // new connect portal uri
                        "https://uat-connect.tranglo.com:5555/signin-oidc",

                        "https://uat-identity.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-notification.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-connect-api.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",

                        "https://uat-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://twdofmixms01.tranglo.net:5001",
                        "https://twdofmixms01.tranglo.net:5002",
                        "https://twdofmixms01.tranglo.net:5004",
                        "https://twdofmixms01.tranglo.net:5006",
                        "https://twdofmixms01.tranglo.net:5007",
                        "https://twdofmixms01.tranglo.net:5009",
                        "https://twdofmixms01.tranglo.net:5013",
                        "https://twdofmixms01.tranglo.net:5014",
                        "https://twdofmixms01.tranglo.net:5020",
                        "https://twdofmixms01.tranglo.net:5010",
                        "https://twdofmixms01.tranglo.net:5011",
                        "http://localhost:4200",

                        "https://uat-identity.tranglo.net",
                        "https://uat-pricing.tranglo.net",
                        "https://uat-connect.tranglo.net",
                        "https://uat-connect-api.tranglo.net",
                        "https://uat-notification.tranglo.net",
                        "https://uat-forex.tranglo.net",
                        "https://uat-transaction.tranglo.net",
                        "https://uat-product.tranglo.net",
                        "https://uat-internal-compliance.tranglo.net",
                        "https://uat-global.tranglo.net",
                        "https://uat-reporting.tranglo.net:1000",
                        "https://uat-reporting.tranglo.net:1001",
                        "https://uat-reporting.tranglo.net:1002",
                        "https://uat-reporting.tranglo.net:1003",

                        "https://uat-identity.tranglo.net:5000",
                        "https://uat-pricing.tranglo.net:5000",
                        "https://uat-connect.tranglo.net:5000",
                        // new connect portal uri
                        "https://uat-connect.tranglo.com:5555",

                        "https://uat-connect-api.tranglo.net:5000",
                        "https://uat-notification.tranglo.net:5000",
                        "https://uat-forex.tranglo.net:5000",
                        "https://uat-transaction.tranglo.net:5000",
                        "https://uat-product.tranglo.net:5000",
                        "https://uat-internal-compliance.tranglo.net:5000",
                        "https://uat-global.tranglo.net:5000",
                        "https://uat-reporting.tranglo.net:5000",
                        "https://uat-reporting.tranglo.net:5001",
                        "https://uat-reporting.tranglo.net:5002",
                        "https://uat-reporting.tranglo.net:5003",
                        "https://uat-connectpayment.tranglo.net:5000",
                        "https://uat-connectpayment.tranglo.net:5001",
                        "https://uat-connectpayment.tranglo.net:5002",
                        "https://uat-connectpayment.tranglo.net:5003",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://localhost:4200/signout-callback-oidc",
                        "https://twdofmixms01.tranglo.net:5004/signout-callback-oidc",
                        "https://uat-connect.tranglo.net/signout-callback-oidc",
                        "https://uat-connect.tranglo.net:5000/signout-callback-oidc",
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile",
                        ClaimCode.Connect_BusinessProfile,
                        //TODELETE
                        "roles",
                        "api1.read",
                        "IdentityServerApi",
                        "dynamicroles",
                        "company_name",
                        "userid",
                        "types",
                        "account_status",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Connect Angular Client Application UAT",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //connect-client-dev
                new Client
                {
                    ClientId = "connect-client-dev",
                    ClientName = "Connect Angular Client Application Development",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Connect Angular Client Application Development" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "http://localhost:4200/signin-oidc",
                        "https://twdofmixms01.tranglo.net:6004/signin-oidc",
                        "https://twdofmixms01.tranglo.net:6001/swagger/oauth2-redirect.html",
                        "https://twdofmixms01.tranglo.net:6002/swagger/oauth2-redirect.html",
                        "https://twdofmixms01.tranglo.net:6007/swagger/oauth2-redirect.html",

                        "https://connect.dev.tranglo.aws/signin-oidc",
                        "https://connect-api.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://identity.dev.tranglo.aws/swagger/oauth2-redirect.html",

                        "https://dev-connect.tranglo.net/signin-oidc",
                        "https://dev-identity.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-connect-api.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-wallet.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1003/swagger/oauth2-redirect.html",

                        "https://dev-connect.tranglo.net:5000/signin-oidc",
                        "https://dev-identity.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-notification.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-connect-api.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-wallet.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",

                        "https://dev-connect.tranglo.net:5200/signin-oidc", //tb
                        "https://dev-business.tranglo.net:5200/signin-oidc", //tb
                        "https://dev-identity.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-pricing.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-connect-api.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-forex.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-transaction.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-product.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-internal-compliance.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-global.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5200/swagger/oauth2-redirect.html",
                        // new connect portal uri
                        "https://dev-connect.tranglo.net:5555/signin-oidc",

                        "https://dev-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "http://localhost:4200",
                        "https://twdofmixms01.tranglo.net:6001",
                        "https://twdofmixms01.tranglo.net:6002",
                        "https://twdofmixms01.tranglo.net:6004",
                        "https://twdofmixms01.tranglo.net:6006",
                        "https://twdofmixms01.tranglo.net:6007",
                        "https://connect.dev.tranglo.aws",
                        "https://connect-api.dev.tranglo.aws",
                        "https://identity.dev.tranglo.aws",

                        "https://dev-identity.tranglo.net",
                        "https://dev-pricing.tranglo.net",
                        "https://dev-connect.tranglo.net",
                        "https://dev-connect-api.tranglo.net",
                        "https://dev-notification.tranglo.net",
                        "https://dev-forex.tranglo.net",
                        "https://dev-wallet.tranglo.net",
                        "https://dev-transaction.tranglo.net",
                        "https://dev-product.tranglo.net",
                        "https://dev-internal-compliance.tranglo.net",
                        "https://dev-global.tranglo.net",
                        "https://dev-reporting.tranglo.net:1000",
                        "https://dev-reporting.tranglo.net:1001",
                        "https://dev-reporting.tranglo.net:1002",
                        "https://dev-reporting.tranglo.net:1003",

                        "https://dev-pricing.tranglo.net:5000",
                        "https://dev-connect.tranglo.net:5000",
                        "https://dev-connect-api.tranglo.net:5000",
                        "https://dev-notification.tranglo.net:5000",
                        "https://dev-forex.tranglo.net:5000",
                        "https://dev-wallet.tranglo.net:5000",
                        "https://dev-transaction.tranglo.net:5000",
                        "https://dev-product.tranglo.net:5000",
                        "https://dev-internal-compliance.tranglo.net:5000",
                        "https://dev-global.tranglo.net:5000",
                        "https://dev-reporting.tranglo.net:5000",
                        "https://dev-reporting.tranglo.net:5001",
                        "https://dev-reporting.tranglo.net:5002",
                        "https://dev-reporting.tranglo.net:5003",

                        "https://dev-pricing.tranglo.net:5200", //tb
                        "https://dev-connect.tranglo.net:5200",
                        "https://dev-business.tranglo.net:5200",
                        "https://dev-connect-api.tranglo.net:5200",
                        "https://dev-notification.tranglo.net:5200",
                        "https://dev-forex.tranglo.net:5200",
                        "https://dev-transaction.tranglo.net:5200",
                        "https://dev-product.tranglo.net:5200",
                        "https://dev-internal-compliance.tranglo.net:5200",
                        "https://dev-global.tranglo.net:5200",
                        "https://dev-reporting.tranglo.net:5200",
                        // new connect portal uri
                        "https://dev-connect.tranglo.net:5555",

                        "https://dev-connectpayment.tranglo.net:5000",
                        "https://dev-connectpayment.tranglo.net:5001",
                        "https://dev-connectpayment.tranglo.net:5002",
                        "https://dev-connectpayment.tranglo.net:5003",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:6004/signout-callback-oidc",
                        "https://connect.dev.tranglo.aws/signout-callback-oidc",
                        "https://dev-connect.tranglo.net/signout-callback-oidc",
                        "https://dev-connect.tranglo.net:5000/signout-callback-oidc",

                        "https://dev-connect.tranglo.net:5200/signout-callback-oidc"    //tb 
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile",
                        ClaimCode.Connect_BusinessProfile,
                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,


                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Connect Angular Client Application Development",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //connect-client-local
                new Client
                {
                    ClientId = "connect-client-local",
                    ClientName = "Connect Angular Client Local Application",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Connect Angular Client Local Application" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },
                    
                    // where to redirect to after login
                    RedirectUris = {
                        "http://localhost:4200/signin-oidc",
                        "http://localhost:4300/signin-oidc",
                        "https://localhost:5001/swagger/oauth2-redirect.html",
                        "https://localhost:5002/swagger/oauth2-redirect.html",
                        "https://localhost:5006/swagger/oauth2-redirect.html",
                        "https://localhost:5007/swagger/oauth2-redirect.html",
                        "https://localhost:5008/swagger/oauth2-redirect.html",
                        "https://localhost:5009/swagger/oauth2-redirect.html",
                        "https://localhost:5013/swagger/oauth2-redirect.html",
                        "https://localhost:5014/swagger/oauth2-redirect.html",
                        "https://localhost:5020/swagger/oauth2-redirect.html",
                        "https://localhost:5010/swagger/oauth2-redirect.html",
                        "https://localhost:5011/swagger/oauth2-redirect.html",
                        "https://localhost:5021/swagger/oauth2-redirect.html",
                        "https://localhost:5022/swagger/oauth2-redirect.html",
                        "https://localhost:5023/swagger/oauth2-redirect.html",

                        //TC
                        "https://localhost:5041/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "http://localhost:4200",
                        "http://localhost:4300",
                        "https://localhost:5001",
                        "https://localhost:5002",
                        "https://localhost:5006",
                        "https://localhost:5007",
                        "https://localhost:5008",
                        "https://localhost:5009",
                        "https://localhost:5013",
                        "https://localhost:5014",
                        "https://localhost:5020",
                        "https://localhost:5010",
                        "https://localhost:5011",
                        "https://localhost:5021",
                        "https://localhost:5022",
                        "https://localhost:5023",

                         //TC
                        "https://localhost:5041",
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        "http://localhost:4200/signout-callback-oidc",
                        "http://localhost:4300/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile",
                        ClaimCode.Connect_BusinessProfile,
                        //TODELETE
                        "roles",
                        "api1.read",
                        "IdentityServerApi",
                        "dynamicroles",
                        "company_name",
                        "userid",
                        "types",
                        "account_status",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Connect Angular Client Local Application",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //connect-client-qa
                new Client
                {
                    ClientId = "connect-client-qa",
                    ClientName = "Angular Connect Client Application QA",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Angular Client Application QA" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "http://localhost:4200/signin-oidc",
                        "https://twdofmixms01.tranglo.net:7004/signin-oidc",
                        "https://twdofmixms01.tranglo.net:7001/swagger/oauth2-redirect.html",
                        "https://twdofmixms01.tranglo.net:7002/swagger/oauth2-redirect.html",
                        "https://twdofmixms01.tranglo.net:7007/swagger/oauth2-redirect.html",
                        "https://connect.dev.tranglo.aws/signin-oidc",
                        "https://connect-api.dev.tranglo.aws/swagger/oauth2-redirect.html",

                        "https://identity.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://qa-connect.tranglo.net/signin-oidc",
                        "https://qa-identity.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-connect-api.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-wallet.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1003/swagger/oauth2-redirect.html",

                        "https://qa-connect.tranglo.net:5000/signin-oidc",
                        
                        // new connect portal uri
                        "https://qa-connect.tranglo.net:5555/signin-oidc",

                        "https://qa-identity.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-connect-api.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-notification.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-wallet.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",

                        "https://qa-businesspayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-businesspayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://qa-businesspayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://qa-businesspayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                        "https://qa-connect.tranglo.net:5200/signin-oidc",  //tb
                        "https://qa-business.tranglo.net:5200/signin-oidc",  //tb
                        "https://qa-identity.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-pricing.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-connect-api.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-notification.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-wallet.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-forex.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-product.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-internal-compliance.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-global.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5200/swagger/oauth2-redirect.html",

                        "https://qa-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "http://localhost:4200",
                        "https://twdofmixms01.tranglo.net:7001",
                        "https://twdofmixms01.tranglo.net:7002",
                        "https://twdofmixms01.tranglo.net:7004",
                        "https://twdofmixms01.tranglo.net:7006",
                        "https://twdofmixms01.tranglo.net:7007",
                        "https://connect.dev.tranglo.aws",
                        "https://connect-api.dev.tranglo.aws",
                        "https://identity.dev.tranglo.aws",

                        "https://qa-identity.tranglo.net",
                        "https://qa-pricing.tranglo.net",
                        "https://qa-connect.tranglo.net",
                        "https://qa-business.tranglo.net",
                        "https://qa-connect-api.tranglo.net",
                        "https://qa-notification.tranglo.net",
                        "https://qa-wallet.tranglo.net",
                        "https://qa-forex.tranglo.net",
                        "https://qa-transaction.tranglo.net",
                        "https://qa-product.tranglo.net",
                        "https://qa-internal-compliance.tranglo.net",
                        "https://qa-global.tranglo.net",
                        "https://qa-reporting.tranglo.net:1000",
                        "https://qa-reporting.tranglo.net:1001",
                        "https://qa-reporting.tranglo.net:1002",
                        "https://qa-reporting.tranglo.net:1003",

                        "https://qa-businesspayment.tranglo.net:5000",
                        "https://qa-businesspayment.tranglo.net:5001",
                        "https://qa-businesspayment.tranglo.net:5002",
                        "https://qa-businesspayment.tranglo.net:5003",

                        "https://qa-identity.tranglo.net:5000",
                        "https://qa-pricing.tranglo.net:5000",
                        "https://qa-connect.tranglo.net:5000",
                        
                        // new connect portal uri
                        "https://qa-connect.tranglo.net:5555",

                        "https://qa-business.tranglo.net:5000",
                        "https://qa-connect-api.tranglo.net:5000",
                        "https://qa-notification.tranglo.net:5000",
                        "https://qa-wallet.tranglo.net:5000",
                        "https://qa-forex.tranglo.net:5000",
                        "https://qa-transaction.tranglo.net:5000",
                        "https://qa-product.tranglo.net:5000",
                        "https://qa-internal-compliance.tranglo.net:5000",
                        "https://qa-global.tranglo.net:5000",
                        "https://qa-reporting.tranglo.net:5000",
                        "https://qa-reporting.tranglo.net:5001",
                        "https://qa-reporting.tranglo.net:5002",
                        "https://qa-reporting.tranglo.net:5003",
                        "https://qa-identity.tranglo.net:5200", //tb
                        "https://qa-pricing.tranglo.net:5200",
                        "https://qa-connect.tranglo.net:5200",
                        "https://qa-business.tranglo.net:5200",
                        "https://qa-connect-api.tranglo.net:5200",
                        "https://qa-notification.tranglo.net:5200",
                        "https://qa-forex.tranglo.net:5200",
                        "https://qa-transaction.tranglo.net:5200",
                        "https://qa-product.tranglo.net:5200",
                        "https://qa-internal-compliance.tranglo.net:5200",
                        "https://qa-global.tranglo.net:5200",
                        "https://qa-reporting.tranglo.net:5200",

                        "https://qa-connectpayment.tranglo.net:5000",
                        "https://qa-connectpayment.tranglo.net:5001",
                        "https://qa-connectpayment.tranglo.net:5002",
                        "https://qa-connectpayment.tranglo.net:5003",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:7004/signout-callback-oidc",
                        "https://connect.dev.tranglo.aws/signout-callback-oidc",
                        "https://qa-connect.tranglo.net/signout-callback-oidc",
                        "https://qa-business.tranglo.net/signout-callback-oidc",
                        "https://qa-connect.tranglo.net:5000/signout-callback-oidc",

                        "https://qa-connect.tranglo.net:5200/signout-callback-oidc" // tb
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile",
                        ClaimCode.Connect_BusinessProfile,
                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Angular Connect Client Application QA",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },


                //business-client production
                new Client
                {
                    ClientId = "business-client",
                    ClientName = "Business Angular Client Application Production",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Business Angular Client Application Production" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "https://tb.tranglo.com/signin-oidc",
                        "https://connect-api.tranglo.com/swagger/oauth2-redirect.html",
                        "https://identity.tranglo.com/swagger/oauth2-redirect.html",
                        "https://pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://forex.tranglo.com/swagger/oauth2-redirect.html",
                        "https://product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://internal-compliance.tranglo.net/swagger/oauth2-redirect.html",

                        //TODO : Production TC Link
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://tb.tranglo.com",
                        "https://connect-api.tranglo.com",
                        "https://identity.tranglo.com",
                        "https://pricing.tranglo.net",
                        "https://notification.tranglo.net",
                        "https://forex.tranglo.net",
                        "https://wallet.tranglo.net",
                        "https://product.tranglo.net",
                        "https://global.tranglo.net",
                        "https://internal-compliance.tranglo.net",
                        "https://reporting.tranglo.net:1000",
                        "https://reporting.tranglo.net:1001",
                        "https://reporting.tranglo.net:1002",
                        "https://reporting.tranglo.net:1003",
                        "https://businesspayment.tranglo.net:1000",
                        "https://businesspayment.tranglo.net:1001",
                        "https://businesspayment.tranglo.net:1002",
                        "https://businesspayment.tranglo.net:1003",

                        //TODO : Production TC Link
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://tb.tranglo.com/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile", //to remove
                        ClaimCode.Business_BusinessProfile, //"business_solution_business_profile",
                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Business Angular Client Application Production",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },

                new Client
                {
                    ClientId = "business-client-uat",
                    ClientName = "Business Angular Client Application UAT",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Business Angular Client Application UAT" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:5004/signin-oidc",

                        "https://uat-business.tranglo.com/signin-oidc",
                        "https://uat-identity.tranglo.com/swagger/oauth2-redirect.html",
                        "https://uat-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-connect-api.tranglo.com/swagger/oauth2-redirect.html",
                        "https://uat-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:1003/swagger/oauth2-redirect.html",

                        "https://uat-businesspayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                        "https://uat-business.tranglo.com:5000/signin-oidc",
                        "https://uat-identity.tranglo.com:5000/swagger/oauth2-redirect.html",
                        "https://uat-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-connect-api.tranglo.com:5000/swagger/oauth2-redirect.html",
                        "https://uat-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",

                        "https://uat-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://twdofmixms01.tranglo.net:5001",
                        "https://twdofmixms01.tranglo.net:5002",
                        "https://twdofmixms01.tranglo.net:5004",
                        "https://twdofmixms01.tranglo.net:5006",
                        "https://twdofmixms01.tranglo.net:5007",
                        "https://twdofmixms01.tranglo.net:5009",
                        "https://twdofmixms01.tranglo.net:5013",
                        "https://twdofmixms01.tranglo.net:5014",
                        "https://twdofmixms01.tranglo.net:5020",
                        "https://twdofmixms01.tranglo.net:5010",
                        "https://twdofmixms01.tranglo.net:5011",
                        "http://localhost:4200",

                        "https://uat-identity.tranglo.com",
                        "https://uat-pricing.tranglo.net",
                        "https://uat-business.tranglo.com",
                        "https://uat-connect-api.tranglo.com",
                        "https://uat-notification.tranglo.net",
                        "https://uat-forex.tranglo.net",
                        "https://uat-transaction.tranglo.net",
                        "https://uat-product.tranglo.net",
                        "https://uat-internal-compliance.tranglo.net",
                        "https://uat-global.tranglo.net",
                        "https://uat-reporting.tranglo.net:1000",
                        "https://uat-reporting.tranglo.net:1001",
                        "https://uat-reporting.tranglo.net:1002",
                        "https://uat-reporting.tranglo.net:1003",

                        "https://uat-identity.tranglo.com:5000",
                        "https://uat-pricing.tranglo.net:5000",
                        "https://uat-businesspayment.tranglo.net:5000",
                        "https://uat-businesspayment.tranglo.net:5001",
                        "https://uat-businesspayment.tranglo.net:5002",
                        "https://uat-businesspayment.tranglo.net:5003",
                        "https://uat-business.tranglo.com:5000",
                        "https://uat-connect-api.tranglo.com:5000",
                        "https://uat-notification.tranglo.net:5000",
                        "https://uat-forex.tranglo.net:5000",
                        "https://uat-transaction.tranglo.net:5000",
                        "https://uat-product.tranglo.net:5000",
                        "https://uat-internal-compliance.tranglo.net:5000",
                        "https://uat-global.tranglo.net:5000",
                        "https://uat-reporting.tranglo.net:5000",
                        "https://uat-reporting.tranglo.net:5001",
                        "https://uat-reporting.tranglo.net:5002",
                        "https://uat-reporting.tranglo.net:5003",

                        "https://uat-connectpayment.tranglo.net:5000",
                        "https://uat-connectpayment.tranglo.net:5001",
                        "https://uat-connectpayment.tranglo.net:5002",
                        "https://uat-connectpayment.tranglo.net:5003",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://localhost:4200/signout-callback-oidc",
                        "https://twdofmixms01.tranglo.net:5004/signout-callback-oidc",
                        "https://uat-business.tranglo.net/signout-callback-oidc",
                        "https://uat-business.tranglo.net:5000/signout-callback-oidc",
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile",
                        ClaimCode.Business_BusinessProfile,
                        //TODELETE
                        "roles",
                        "api1.read",
                        "IdentityServerApi",
                        "dynamicroles",
                        "company_name",
                        "userid",
                        "types",
                        "account_status",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Business Angular Client Application UAT",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //business-client-dev
                new Client
                {
                    ClientId = "business-client-dev",
                    ClientName = "Business Angular Client Application Development",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Business Angular Client Application Development" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "http://localhost:4200/signin-oidc",
                        "https://twdofmixms01.tranglo.net:6004/signin-oidc",
                        "https://twdofmixms01.tranglo.net:6001/swagger/oauth2-redirect.html",
                        "https://twdofmixms01.tranglo.net:6002/swagger/oauth2-redirect.html",
                        "https://twdofmixms01.tranglo.net:6007/swagger/oauth2-redirect.html",
                        "https://business.dev.tranglo.aws/signin-oidc",
                        "https://connect-api.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://identity.dev.tranglo.aws/swagger/oauth2-redirect.html",

                        "https://dev-business.tranglo.net/signin-oidc",
                        "https://dev-identity.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-connect-api.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-wallet.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1003/swagger/oauth2-redirect.html",

                        "https://dev-business.tranglo.net:5000/signin-oidc",
                        "https://dev-identity.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-connect-api.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-wallet.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",

                        "https://dev-business.tranglo.net:5200/signin-oidc", //tb
                        "https://dev-identity.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-pricing.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-connect-api.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-forex.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-transaction.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-product.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-internal-compliance.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-global.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5200/swagger/oauth2-redirect.html",

                        "https://dev-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "http://localhost:4200",
                        "https://twdofmixms01.tranglo.net:6001",
                        "https://twdofmixms01.tranglo.net:6002",
                        "https://twdofmixms01.tranglo.net:6004",
                        "https://twdofmixms01.tranglo.net:6006",
                        "https://twdofmixms01.tranglo.net:6007",
                        "https://business.dev.tranglo.aws",
                        "https://connect-api.dev.tranglo.aws",
                        "https://identity.dev.tranglo.aws",

                        "https://dev-identity.tranglo.net",
                        "https://dev-pricing.tranglo.net",
                        "https://dev-business.tranglo.net",
                        "https://dev-connect-api.tranglo.net",
                        "https://dev-notification.tranglo.net",
                        "https://dev-forex.tranglo.net",
                        "https://dev-wallet.tranglo.net",
                        "https://dev-transaction.tranglo.net",
                        "https://dev-product.tranglo.net",
                        "https://dev-internal-compliance.tranglo.net",
                        "https://dev-global.tranglo.net",
                        "https://dev-reporting.tranglo.net:1000",
                        "https://dev-reporting.tranglo.net:1001",
                        "https://dev-reporting.tranglo.net:1002",
                        "https://dev-reporting.tranglo.net:1003",

                        "https://dev-pricing.tranglo.net:5000",
                        "https://dev-business.tranglo.net:5000",
                        "https://dev-connect-api.tranglo.net:5000",
                        "https://dev-notification.tranglo.net:5000",
                        "https://dev-forex.tranglo.net:5000",
                        "https://dev-wallet.tranglo.net:5000",
                        "https://dev-transaction.tranglo.net:5000",
                        "https://dev-product.tranglo.net:5000",
                        "https://dev-internal-compliance.tranglo.net:5000",
                        "https://dev-global.tranglo.net:5000",
                        "https://dev-reporting.tranglo.net:5000",
                        "https://dev-reporting.tranglo.net:5001",
                        "https://dev-reporting.tranglo.net:5002",
                        "https://dev-reporting.tranglo.net:5003",

                        "https://dev-pricing.tranglo.net:5200", //tb
                        "https://dev-business.tranglo.net:5200",
                        "https://dev-connect-api.tranglo.net:5200",
                        "https://dev-notification.tranglo.net:5200",
                        "https://dev-forex.tranglo.net:5200",
                        "https://dev-transaction.tranglo.net:5200",
                        "https://dev-product.tranglo.net:5200",
                        "https://dev-internal-compliance.tranglo.net:5200",
                        "https://dev-global.tranglo.net:5200",
                        "https://dev-reporting.tranglo.net:5200",

                        "https://dev-connectpayment.tranglo.net:5000",
                        "https://dev-connectpayment.tranglo.net:5001",
                        "https://dev-connectpayment.tranglo.net:5002",
                        "https://dev-connectpayment.tranglo.net:5003",

                    },

                    PostLogoutRedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:6004/signout-callback-oidc",
                        "https://business.dev.tranglo.aws/signout-callback-oidc",
                        "https://dev-business.tranglo.net/signout-callback-oidc",
                        "https://dev-business.tranglo.net/signout-callback-oidc:5000",

                        "https://dev-business.tranglo.net/signout-callback-oidc:5200"    //tb 
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile",
                        ClaimCode.Business_BusinessProfile,

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,


                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Business Angular Client Application Development",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //business-client-local
                new Client
                {
                    ClientId = "business-client-local",
                    ClientName = "Business Angular Client Local Application",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Business Angular Client Local Application" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },
                    
                    // where to redirect to after login
                    RedirectUris = {
                        "http://localhost:4200/signin-oidc",
                        "http://localhost:4300/signin-oidc",
                        "https://localhost:5001/swagger/oauth2-redirect.html",
                        "https://localhost:5002/swagger/oauth2-redirect.html",
                        "https://localhost:5007/swagger/oauth2-redirect.html",
                        "https://localhost:5008/swagger/oauth2-redirect.html",
                        "https://localhost:5009/swagger/oauth2-redirect.html",
                        "https://localhost:5013/swagger/oauth2-redirect.html",
                        "https://localhost:5014/swagger/oauth2-redirect.html",
                        "https://localhost:5020/swagger/oauth2-redirect.html",
                        "https://localhost:5010/swagger/oauth2-redirect.html",
                        "https://localhost:5011/swagger/oauth2-redirect.html",
                        "https://localhost:5015/swagger/oauth2-redirect.html",
                        "https://localhost:5021/swagger/oauth2-redirect.html",
                        "https://localhost:5022/swagger/oauth2-redirect.html",
                        "https://localhost:5023/swagger/oauth2-redirect.html",

                        //TC
                        "https://localhost:5041/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "http://localhost:4200",
                        "http://localhost:4300",
                        "https://localhost:5001",
                        "https://localhost:5002",
                        "https://localhost:5006",
                        "https://localhost:5007",
                        "https://localhost:5008",
                        "https://localhost:5009",
                        "https://localhost:5013",
                        "https://localhost:5014",
                        "https://localhost:5020",
                        "https://localhost:5010",
                        "https://localhost:5011",
                        "https://localhost:5015",
                        "https://localhost:5021",
                        "https://localhost:5022",
                        "https://localhost:5023",

                        //TC
                        "https://localhost:5041",
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        "http://localhost:4200/signout-callback-oidc",
                        "http://localhost:4300/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile",
                        ClaimCode.Business_BusinessProfile,

                        //TODELETE
                        "roles",
                        "api1.read",
                        "IdentityServerApi",
                        "dynamicroles",
                        "company_name",
                        "userid",
                        "types",
                        "account_status",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Business Angular Client Local Application",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //business-client-qa
                new Client
                {
                    ClientId = "business-client-qa",
                    ClientName = "Angular Business Client Application QA",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Angular Client Application QA" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "http://localhost:4200/signin-oidc",
                        "https://twdofmixms01.tranglo.net:7004/signin-oidc",
                        "https://twdofmixms01.tranglo.net:7001/swagger/oauth2-redirect.html",
                        "https://twdofmixms01.tranglo.net:7002/swagger/oauth2-redirect.html",
                        "https://twdofmixms01.tranglo.net:7007/swagger/oauth2-redirect.html",
                        "https://business.dev.tranglo.aws/signin-oidc",
                        "https://connect-api.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://identity.dev.tranglo.aws/swagger/oauth2-redirect.html",

                        "https://qa-business.tranglo.net/signin-oidc",
                        "https://qa-identity.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-connect-api.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-wallet.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1003/swagger/oauth2-redirect.html",

                        "https://qa-business.tranglo.net:5000/signin-oidc",
                        "https://qa-identity.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-connect-api.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-wallet.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",

                        "https://qa-business.tranglo.net:5200/signin-oidc",  //tb
                        "https://qa-business.tranglo.net:5200/signin-oidc",  //tb
                        "https://qa-identity.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-pricing.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-connect-api.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-wallet.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-forex.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-product.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-internal-compliance.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-global.tranglo.net:5200/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5200/swagger/oauth2-redirect.html",

                        "https://qa-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "http://localhost:4200",
                        "https://twdofmixms01.tranglo.net:7001",
                        "https://twdofmixms01.tranglo.net:7002",
                        "https://twdofmixms01.tranglo.net:7004",
                        "https://twdofmixms01.tranglo.net:7006",
                        "https://twdofmixms01.tranglo.net:7007",
                        "https://business.dev.tranglo.aws",
                        "https://connect-api.dev.tranglo.aws",
                        "https://identity.dev.tranglo.aws",

                        "https://qa-identity.tranglo.net",
                        "https://qa-pricing.tranglo.net",
                        "https://qa-business.tranglo.net",
                        "https://qa-business.tranglo.net",
                        "https://qa-connect-api.tranglo.net",
                        "https://qa-notification.tranglo.net",
                        "https://qa-wallet.tranglo.net",
                        "https://qa-forex.tranglo.net",
                        "https://qa-transaction.tranglo.net",
                        "https://qa-product.tranglo.net",
                        "https://qa-internal-compliance.tranglo.net",
                        "https://qa-global.tranglo.net",
                        "https://qa-reporting.tranglo.net:1000",
                        "https://qa-reporting.tranglo.net:1001",
                        "https://qa-reporting.tranglo.net:1002",
                        "https://qa-reporting.tranglo.net:1003",

                        "https://qa-identity.tranglo.net:5000",
                        "https://qa-pricing.tranglo.net:5000",
                        "https://qa-business.tranglo.net:5000",
                        "https://qa-business.tranglo.net:5000",
                        "https://qa-connect-api.tranglo.net:5000",
                        "https://qa-notification.tranglo.net:5000",
                        "https://qa-wallet.tranglo.net:5000",
                        "https://qa-forex.tranglo.net:5000",
                        "https://qa-transaction.tranglo.net:5000",
                        "https://qa-product.tranglo.net:5000",
                        "https://qa-internal-compliance.tranglo.net:5000",
                        "https://qa-global.tranglo.net:5000",
                        "https://qa-reporting.tranglo.net:5000",
                        "https://qa-reporting.tranglo.net:5001",
                        "https://qa-reporting.tranglo.net:5002",
                        "https://qa-reporting.tranglo.net:5003",

                        "https://qa-identity.tranglo.net:5200", //tb
                        "https://qa-pricing.tranglo.net:5200",
                        "https://qa-business.tranglo.net:5200",
                        "https://qa-business.tranglo.net:5200",
                        "https://qa-connect-api.tranglo.net:5200",
                        "https://qa-notification.tranglo.net:5200",
                        "https://qa-wallet.tranglo.net:5200",
                        "https://qa-forex.tranglo.net:5200",
                        "https://qa-transaction.tranglo.net:5200",
                        "https://qa-product.tranglo.net:5200",
                        "https://qa-internal-compliance.tranglo.net:5200",
                        "https://qa-global.tranglo.net:5200",
                        "https://qa-reporting.tranglo.net:5200",

                        "https://qa-connectpayment.tranglo.net:5000",
                        "https://qa-connectpayment.tranglo.net:5001",
                        "https://qa-connectpayment.tranglo.net:5002",
                        "https://qa-connectpayment.tranglo.net:5003",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:7004/signout-callback-oidc",
                        "https://business.dev.tranglo.aws/signout-callback-oidc",
                        "https://qa-business.tranglo.net/signout-callback-oidc",
                        "https://qa-business.tranglo.net:5000/signout-callback-oidc",

                        "https://qa-business.tranglo.net:5200/signout-callback-oidc" // tb
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "business_profile",
                        ClaimCode.Business_BusinessProfile,

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Angular Business Client Application QA",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },


                //admin-client production
                new Client
                {
                    ClientId = "admin-client",
                    ClientName = "Admin Angular Client Application Production",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Admin Angular Client Application Production" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "https://identity.tranglo.com/swagger/oauth2-redirect.html",
                        "https://admin.tranglo.com/signin-oidc",
                        "https://identity.tranglo.com/swagger/oauth2-redirect.html",
                        "https://pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://connect-api.tranglo.com/swagger/oauth2-redirect.html",
                        "https://wallet.tranglo.net/swagger/oauth2-redirect.html",
                        "https://forex.tranglo.com/swagger/oauth2-redirect.html",
                        "https://transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://reporting.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://businesspayment.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://internal-compliance.tranglo.net/swagger/oauth2-redirect.html",

                        //TODO : Production TC Link
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                        "https://connectpayment.tranglo.com/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://admin.tranglo.com",
                        "https://identity.tranglo.com",
                        "https://pricing.tranglo.net",
                        "https://notification.tranglo.net",
                        "https://connect-api.tranglo.com",
                        "https://forex.tranglo.net",
                        "https://wallet.tranglo.net",
                        "https://transaction.tranglo.net",
                        "https://product.tranglo.net",
                        "https://global.tranglo.net",
                        "https://reporting.tranglo.net:1000",
                        "https://reporting.tranglo.net:1001",
                        "https://reporting.tranglo.net:1002",
                        "https://reporting.tranglo.net:1003",
                        "https://businesspayment.tranglo.net:1000",
                        "https://businesspayment.tranglo.net:1001",
                        "https://businesspayment.tranglo.net:1002",
                        "https://businesspayment.tranglo.net:1003",
                        "https://internal-compliance.tranglo.net",

                        //TODO : Production TC Link
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                        "https://connectpayment.tranglo.com",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://admin.tranglo.com/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "tranglo_profile",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Admin Angular Client Application Production",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },         
                //admin-client-uat
                new Client
                {
                    ClientId = "admin-client-uat",
                    ClientName = "Admin Angular Client Application UAT",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Admin Angular Client Application UAT" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:5005/signin-oidc",

                        "https://uat-admin.tranglo.net/signin-oidc",
                        "https://uat-identity.tranglo.com/swagger/oauth2-redirect.html",
                        "https://uat-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-connect-api.tranglo.com/swagger/oauth2-redirect.html",
                        "https://uat-wallet.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net/swagger/oauth2-redirect.html:1000",
                        "https://uat-reporting.tranglo.net/swagger/oauth2-redirect.html:1001",
                        "https://uat-reporting.tranglo.net/swagger/oauth2-redirect.html:1002",
                        "https://uat-reporting.tranglo.net/swagger/oauth2-redirect.html:1003",
                        "https://uat-businesspayment.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://uat-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",

                        "https://uat-admin.tranglo.net:5000/signin-oidc",
                        "https://uat-identity.tranglo.com:5000/swagger/oauth2-redirect.html",
                        "https://uat-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-notification.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-connect-api.tranglo.com:5000/swagger/oauth2-redirect.html",
                        "https://uat-wallet.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://uat-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://uat-businesspayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                        "https://uat-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",

                        "https://uat-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://uat-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://twdofmixms01.tranglo.net:5001",
                        "https://twdofmixms01.tranglo.net:5002",
                        "https://twdofmixms01.tranglo.net:5005",
                        "https://twdofmixms01.tranglo.net:5006",
                        "https://twdofmixms01.tranglo.net:5007",
                        "https://twdofmixms01.tranglo.net:5009",
                        "https://twdofmixms01.tranglo.net:5013",
                        "https://twdofmixms01.tranglo.net:5014",
                        "https://twdofmixms01.tranglo.net:5020",
                        "https://twdofmixms01.tranglo.net:5010",
                        "https://twdofmixms01.tranglo.net:5011",

                        "https://uat-identity.tranglo.com",
                        "https://uat-pricing.tranglo.net",
                        "https://uat-transaction.tranglo.net",
                        "https://uat-admin.tranglo.net",
                        "https://uat-notification.tranglo.net",
                        "https://uat-connect-api.tranglo.com",
                        "https://uat-wallet.tranglo.net",
                        "https://uat-forex.tranglo.net",
                        "https://uat-transaction.tranglo.net",
                        "https://uat-product.tranglo.net",
                        "https://uat-global.tranglo.net",
                        "https://uat-reporting.tranglo.net:1000",
                        "https://uat-reporting.tranglo.net:1001",
                        "https://uat-reporting.tranglo.net:1002",
                        "https://uat-reporting.tranglo.net:1003",
                        "https://uat-businesspayment.tranglo.net:1000",
                        "https://uat-businesspayment.tranglo.net:1001",
                        "https://uat-businesspayment.tranglo.net:1002",
                        "https://uat-businesspayment.tranglo.net:1003",
                        "https://uat-internal-compliance.tranglo.net",

                        "https://uat-identity.tranglo.com:5000",
                        "https://uat-pricing.tranglo.net:5000",
                        "https://uat-transaction.tranglo.net:5000",
                        "https://uat-admin.tranglo.net:5000",
                        "https://uat-notification.tranglo.net:5000",
                        "https://uat-connect-api.tranglo.com:5000",
                        "https://uat-wallet.tranglo.net:5000",
                        "https://uat-forex.tranglo.net:5000",
                        "https://uat-transaction.tranglo.net:5000",
                        "https://uat-product.tranglo.net:5000",
                        "https://uat-global.tranglo.net:5000",
                        "https://uat-reporting.tranglo.net:5000",
                        "https://uat-reporting.tranglo.net:5001",
                        "https://uat-reporting.tranglo.net:5002",
                        "https://uat-reporting.tranglo.net:5003",
                        "https://uat-businesspayment.tranglo.net:5000",
                        "https://uat-businesspayment.tranglo.net:5001",
                        "https://uat-businesspayment.tranglo.net:5002",
                        "https://uat-businesspayment.tranglo.net:5003",
                        "https://uat-internal-compliance.tranglo.net:5000",
                        "http://localhost:4200",

                        "https://uat-connectpayment.tranglo.net:5000",
                        "https://uat-connectpayment.tranglo.net:5001",
                        "https://uat-connectpayment.tranglo.net:5002",
                        "https://uat-connectpayment.tranglo.net:5003",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://localhost:4200/signout-callback-oidc",
                        "https://twdofmixms01.tranglo.net:5005/signout-callback-oidc",
                        "https://uat-admin.tranglo.net/signout-callback-oidc",
                        "https://uat-admin.tranglo.net:5000/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "tranglo_profile",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Admin Angular Client Application UAT",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //admin-client-dev
                new Client
                {
                    ClientId = "admin-client-dev",
                    ClientName = "Admin Angular Client Application Development",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Admin Angular Client Application Development" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "http://localhost:4500/signin-oidc",
                        "https://twdofmixms01.tranglo.net:6005/signin-oidc",
                        "https://twdofmixms01:6001/swagger/oauth2-redirect.html",
                        "https://twdofmixms01:6002/swagger/oauth2-redirect.html",
                        "https://twdofmixms01:6007/swagger/oauth2-redirect.html",
                        "https://identity.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://admin.dev.tranglo.aws/signin-oidc",
                        "https://identity.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://pricing.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://connect-api.dev.tranglo.aws/swagger/oauth2-redirect.html",

                        "https://dev-admin.tranglo.net/signin-oidc",
                        "https://dev-identity.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-connect-api.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-wallet.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://dev-businesspayment.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://dev-businesspayment.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://dev-businesspayment.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://dev-businesspayment.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://dev-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",

                        "https://dev-admin.tranglo.net:5000/signin-oidc",
                        "https://dev-identity.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-notification.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-connect-api.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-wallet.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://dev-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",
                        "https://dev-businesspayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-businesspayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://dev-businesspayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://dev-businesspayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                        "https://dev-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",

                        "https://dev-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://dev-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://twdofmixms01.tranglo.net:6001",
                        "https://twdofmixms01.tranglo.net:6002",
                        "https://twdofmixms01.tranglo.net:6005",
                        "https://twdofmixms01.tranglo.net:6006",
                        "https://twdofmixms01.tranglo.net:6007",
                        "http://localhost:4500",
                        "https://admin.dev.tranglo.aws",
                        "https://identity.dev.tranglo.aws",
                        "https://pricing.dev.tranglo.aws",
                        "https://connect-api.dev.tranglo.aws",

                        "https://dev-identity.tranglo.net",
                        "https://dev-pricing.tranglo.net",
                        "https://dev-admin.tranglo.net",
                        "https://dev-notification.tranglo.net",
                        "https://dev-connect-api.tranglo.net",
                        "https://dev-wallet.tranglo.net",
                        "https://dev-forex.tranglo.net",
                        "https://dev-transaction.tranglo.net",
                        "https://dev-product.tranglo.net",
                        "https://dev-global.tranglo.net",
                        "https://dev-reporting.tranglo.net:1000",
                        "https://dev-reporting.tranglo.net:1001",
                        "https://dev-reporting.tranglo.net:1002",
                        "https://dev-reporting.tranglo.net:1003",
                        "https://dev-businesspayment.tranglo.net:1000",
                        "https://dev-businesspayment.tranglo.net:1001",
                        "https://dev-businesspayment.tranglo.net:1002",
                        "https://dev-businesspayment.tranglo.net:1003",
                        "https://dev-internal-compliance.tranglo.net",

                        "https://dev-identity.tranglo.net:5000",
                        "https://dev-pricing.tranglo.net:5000",
                        "https://dev-admin.tranglo.net:5000",
                        "https://dev-notification.tranglo.net:5000",
                        "https://dev-connect-api.tranglo.net:5000",
                        "https://dev-wallet.tranglo.net:5000",
                        "https://dev-forex.tranglo.net:5000",
                        "https://dev-transaction.tranglo.net:5000",
                        "https://dev-product.tranglo.net:5000",
                        "https://dev-global.tranglo.net:5000",
                        "https://dev-reporting.tranglo.net:5000",
                        "https://dev-reporting.tranglo.net:5001",
                        "https://dev-reporting.tranglo.net:5002",
                        "https://dev-reporting.tranglo.net:5003",
                        "https://dev-businesspayment.tranglo.net:5000",
                        "https://dev-businesspayment.tranglo.net:5001",
                        "https://dev-businesspayment.tranglo.net:5002",
                        "https://dev-businesspayment.tranglo.net:5003",
                        "https://dev-internal-compliance.tranglo.net:5000",

                        "https://dev-connectpayment.tranglo.net:5000",
                        "https://dev-connectpayment.tranglo.net:5001",
                        "https://dev-connectpayment.tranglo.net:5002",
                        "https://dev-connectpayment.tranglo.net:5003",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:6005/signout-callback-oidc",
                        "https://admin.dev.tranglo.aws/signout-callback-oidc",
                        "https://dev-admin.tranglo.net/signout-callback-oidc",
                        "https://dev-admin.tranglo.net:5000/signout-callback-oidc",
                        "https://dev-admin.tranglo.net:5200/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "tranglo_profile",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Admin Angular Client Application Development",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //admin-client-local
                new Client
                {
                    ClientId = "admin-client-local",
                    ClientName = "Admin Angular Client Local Application",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Admin Angular Client Local Application" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },
                    
                    // where to redirect to after login
                    RedirectUris = {
                        "http://localhost:4500/signin-oidc",
                        "https://localhost:5002/signin-oidc",
                        "https://localhost:5001/swagger/oauth2-redirect.html",
                        "https://localhost:5002/swagger/oauth2-redirect.html",
                        "https://localhost:5006/swagger/oauth2-redirect.html",
                        "https://localhost:5007/swagger/oauth2-redirect.html",
                        "https://localhost:5009/swagger/oauth2-redirect.html",
                        "https://localhost:5013/swagger/oauth2-redirect.html",
                        "https://localhost:5014/swagger/oauth2-redirect.html",
                        "https://localhost:5008/swagger/oauth2-redirect.html",
                        "https://localhost:5009/swagger/oauth2-redirect.html",
                        "https://localhost:5015/swagger/oauth2-redirect.html",
                        "https://localhost:5016/swagger/oauth2-redirect.html",
                        "https://localhost:5017/swagger/oauth2-redirect.html",
                        "https://localhost:5018/swagger/oauth2-redirect.html",
                        "https://localhost:5020/swagger/oauth2-redirect.html",
                        "https://localhost:5010/swagger/oauth2-redirect.html",
                        "https://localhost:5011/swagger/oauth2-redirect.html",
                        "https://localhost:5021/swagger/oauth2-redirect.html",
                        "https://localhost:5022/swagger/oauth2-redirect.html",
                        "https://localhost:5023/swagger/oauth2-redirect.html",

                        //TC
                        "https://localhost:5041/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "http://localhost:4500",
                        "https://localhost:5001",
                        "https://localhost:5002",
                        "https://localhost:5006",
                        "https://localhost:5007",
                        "https://localhost:5008",
                        "https://localhost:5009",
                        "https://localhost:5013",
                        "https://localhost:5014",
                        "https://localhost:5015", //BusinessPayment TSB
                        "https://localhost:5016", //BusinessPayment TPL
                        "https://localhost:5017", //BusinessPayment TEL
                        "https://localhost:5018", //BusinessPayment PTT
                        "https://localhost:5020",
                        "https://localhost:5010",
                        "https://localhost:5011", //Reporting TSB
                        "https://localhost:5021", //Reporting TPL
                        "https://localhost:5022", //Reporting TEL
                        "https://localhost:5023", //Reporting PTT

                        //TC
                        "https://localhost:5041",
                    },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        "http://localhost:4500/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "tranglo_profile",
                        "pricing.read",
                        "pricing.write",
                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Admin Angular Client Local Application",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //admin-client-qa
                new Client
                {
                    ClientId = "admin-client-qa",
                    ClientName = "Admin Angular Client Application QA",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Admin Angular Client Application QA" }
                    },

                    //https://docs.identityserver.io/en/release/topics/clients.html
                    AllowedGrantTypes = { GrantType.AuthorizationCode },

                    RedirectUris =
                    {
                        "http://localhost:4500/signin-oidc",
                        "https://twdofmixms01.tranglo.net:7005/signin-oidc",
                        "https://twdofmixms01:7001/swagger/oauth2-redirect.html",
                        "https://twdofmixms01:7002/swagger/oauth2-redirect.html",
                        "https://twdofmixms01:7007/swagger/oauth2-redirect.html",
                        "https://identity.dev.tranglo.aws/swagger/oauth2-redirect.html",

                        "https://admin.dev.tranglo.aws/signin-oidc",
                        "https://identity.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://pricing.dev.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://connect-api.dev.tranglo.aws/swagger/oauth2-redirect.html",

                        "https://qa-admin.tranglo.net/signin-oidc",
                        "https://qa-identity.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-pricing.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-notification.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-connect-api.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-wallet.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-forex.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-product.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-global.tranglo.net/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1001/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1002/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:1003/swagger/oauth2-redirect.html",
                        "https://qa-internal-compliance.tranglo.net/swagger/oauth2-redirect.html",

                        "https://qa-admin.tranglo.net:5000/signin-oidc",
                        "https://qa-identity.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-pricing.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-notification.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-connect-api.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-wallet.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-forex.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-transaction.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-product.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-global.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://qa-reporting.tranglo.net:5003/swagger/oauth2-redirect.html",
                        "https://qa-businesspayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-businesspayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://qa-businesspayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://qa-businesspayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                        "https://qa-internal-compliance.tranglo.net:5000/swagger/oauth2-redirect.html",

                        "https://qa-connectpayment.tranglo.net:5000/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5001/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5002/swagger/oauth2-redirect.html",
                        "https://qa-connectpayment.tranglo.net:5003/swagger/oauth2-redirect.html",
                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://twdofmixms01.tranglo.net:7001",
                        "https://twdofmixms01.tranglo.net:7002",
                        "https://twdofmixms01.tranglo.net:7005",
                        "https://twdofmixms01.tranglo.net:7006",
                        "https://twdofmixms01.tranglo.net:7007",
                        "http://localhost:4500",
                        "https://admin.dev.tranglo.aws",
                        "https://identity.dev.tranglo.aws",
                        "https://pricing.dev.tranglo.aws",
                        "https://connect-api.dev.tranglo.aws",

                        "https://qa-identity.tranglo.net",
                        "https://qa-pricing.tranglo.net",
                        "https://qa-admin.tranglo.net",
                        "https://qa-transaction.tranglo.net",
                        "https://qa-notification.tranglo.net",
                        "https://qa-connect-api.tranglo.net",
                        "https://qa-wallet.tranglo.net",
                        "https://qa-forex.tranglo.net",
                        "https://qa-transaction.tranglo.net",
                        "https://qa-product.tranglo.net",
                        "https://qa-global.tranglo.net",
                        "https://qa-reporting.tranglo.net:1000",
                        "https://qa-reporting.tranglo.net:1001",
                        "https://qa-reporting.tranglo.net:1002",
                        "https://qa-reporting.tranglo.net:1003",
                        "https://qa-internal-compliance.tranglo.net",

                        "https://qa-identity.tranglo.net:5000",
                        "https://qa-pricing.tranglo.net:5000",
                        "https://qa-admin.tranglo.net:5000",
                        "https://qa-notification.tranglo.net:5000",
                        "https://qa-connect-api.tranglo.net:5000",
                        "https://qa-wallet.tranglo.net:5000",
                        "https://qa-forex.tranglo.net:5000",
                        "https://qa-transaction.tranglo.net:5000",
                        "https://qa-product.tranglo.net:5000",
                        "https://qa-global.tranglo.net:5000",
                        "https://qa-reporting.tranglo.net:5000",
                        "https://qa-reporting.tranglo.net:5001",
                        "https://qa-reporting.tranglo.net:5002",
                        "https://qa-reporting.tranglo.net:5003",
                        "https://qa-businesspayment.tranglo.net:5000",
                        "https://qa-businesspayment.tranglo.net:5001",
                        "https://qa-businesspayment.tranglo.net:5002",
                        "https://qa-businesspayment.tranglo.net:5003",
                        "https://qa-internal-compliance.tranglo.net:5000",

                        "https://qa-connectpayment.tranglo.net:5000",
                        "https://qa-connectpayment.tranglo.net:5001",
                        "https://qa-connectpayment.tranglo.net:5002",
                        "https://qa-connectpayment.tranglo.net:5003",
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://twdofmixms01.tranglo.net:7005/signout-callback-oidc",
                        "https://admin.dev.tranglo.aws/signout-callback-oidc",
                        "https://qa-admin.tranglo.net/signout-callback-oidc",
                        "https://qa-admin.tranglo.net:5000/signout-callback-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "tranglo_profile",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    RequirePkce = true, 
                    
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,

                    UpdateAccessTokenClaimsOnRefresh = true,

                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireConsent = false,

                    Description = "Admin Angular Client Application QA",

                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,

                    RequireClientSecret = false,

                    AccessTokenType = AccessTokenType.Jwt//, AlwaysIncludeUserClaimsInIdToken = true
                },
                //testClient
                new Client
                {
                    ClientId = "testClient",
                    ClientName = "Test Client Application",
                    ClientSecrets = new List<Secret> {new Secret("SuperSecretPassword".Sha256())}, // change me!
                    
                    //AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "api1.read",

                        //This is needed to allow this client to use refresh token
                        IdentityServerConstants.StandardScopes.OfflineAccess,

                        IdentityServerConstants.LocalApi.ScopeName
                    },

                    RequirePkce = true,
                    AllowPlainTextPkce = false,
                    RequireClientSecret = true,
                    
                    // where to redirect to after login
                    RedirectUris = new List<string> {
                        "http://localhost:4200"
                    },
                    AllowedCorsOrigins = new List<string> {
                        "http://localhost:4200",
                        "https://twdofmixms01.tranglo.net:5004",
                        "https://twdofmixms01.tranglo.net:5006"
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    //set to true to enable refresh token
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(1).TotalSeconds,
                    AccessTokenType = AccessTokenType.Jwt
                },
                //pricing api
                new Client
                {
                    ClientId = "pricing_api",
                    ClientSecrets = { new Secret("secret".Sha256()) }, 
                    //AccessTokenType = AccessTokenType.Reference,

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "type",
                        "pricing.read",
                        "pricing.write",
                        IdentityServerConstants.StandardScopes.Email
                    },

                     RedirectUris = {
                        "https://localhost:5002/signin-oidc",
                        "https://localhost:5002/swagger/oauth2-redirect.html",
                        "https://pricing.tranglo.aws/signin-oidc",
                        "https://pricing.tranglo.aws/swagger/oauth2-redirect.html",
                        "https://pricing.dev.tranglo.aws/signin-oidc",
                        "https://pricing.dev.tranglo.aws/swagger/oauth2-redirect.html"

                    },

                    AllowedCorsOrigins = new List<string> {
                        "https://localhost:5002",
                        "https://pricing.tranglo.aws",
                        "https://pricing.dev.tranglo.aws"
                    },
                },

                new Client
                {
                    ClientId = "WorkerA",
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string>
                    {
                        "tb.customer-user"
                    },
                    AlwaysSendClientClaims = true
                },

                new Client
                {
                    ClientId = "TB-AdminWeb",
                    ClientName = "Tranglo Business Admin Portal",
                    ClientSecrets =
                    {
                            new Secret("secret".Sha256()) { Description = "Tranglo Business Admin Portal" }
                    },

                    RedirectUris =
                    {
                        "https://localhost:3443/admins/signin-callback-oidc/",
                        "https://dev-collection.tranglo.net:9444/admins/signin-callback-oidc"
                    },

                    PostLogoutRedirectUris =
                    {
                        "https://localhost:3443/admins",
                        "https://dev-collection.tranglo.net:9444/admins"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    AllowAccessTokensViaBrowser = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AlwaysSendClientClaims = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(20).TotalSeconds,
                    AllowOfflineAccess = true,

                    Description = "Tranglo Business Admin Portal",

                    RefreshTokenExpiration= TokenExpiration.Sliding,
                    RefreshTokenUsage = TokenUsage.ReUse,

                    RequireClientSecret = true,
                    RequireConsent = false,
                    RequirePkce = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                }
            };
    }
}
