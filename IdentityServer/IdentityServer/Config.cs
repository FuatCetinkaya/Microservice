﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                       new IdentityResources.Email(),
                       new IdentityResources.OpenId(),
                       new IdentityResources.Profile(),
                       new IdentityResource(){ Name = "roles", DisplayName = "Roles", Description = "Kullanıcı rolleri", UserClaims = new[]{"role"}}
                   };

        public static IEnumerable<ApiResource> ApiResources =>
                   new ApiResource[]
                   {
                      new ApiResource("resource_catalog"){Scopes = {"catalog_fullpermission"}},
                      new ApiResource("resource_photo_stock"){Scopes = {"photo_stock_fullpermission"}},
                      new ApiResource("resource_basket"){Scopes = {"basket_fullpermission"}},
                      new ApiResource("resource_discount"){Scopes = {"discount_fullpermission"}},
                      new ApiResource("resource_payment"){Scopes = {"payment_fullpermission"}},
                      new ApiResource("resource_order"){Scopes = {"order_fullpermission"}},
                      new ApiResource("resource_gateway"){Scopes = {"gateway_fullpermission"}},
                      new ApiResource(IdentityServerConstants.LocalApi.ScopeName),

                   };
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog_fullpermission","Catalog Api için Full Erişim"),
                new ApiScope("photo_stock_fullpermission","Photo Stock Api için Full Erişim"),
                new ApiScope("basket_fullpermission","Basket Api için Full Erişim"),
                new ApiScope("discount_fullpermission","Discount Api için Full Erişim"),
                new ApiScope("payment_fullpermission","Payment Api için Full Erişim"),
                new ApiScope("order_fullpermission","Order Api için Full Erişim"),
                new ApiScope("gateway_fullpermission","Gateway için Full Erişim"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // Web MVC Client client credentials flow client
                new Client
                {
                    ClientId = "WebMVCClient",
                    ClientName = "Asp.Net Core MVC",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "catalog_fullpermission", "photo_stock_fullpermission", "gateway_fullpermission", IdentityServerConstants.LocalApi.ScopeName }
                },
                new Client
                {
                    ClientId = "WebMVCClientForUser",
                    ClientName = "Asp.Net Core MVC",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = {
                        "basket_fullpermission",
                        "discount_fullpermission",
                        "payment_fullpermission",
                        "order_fullpermission",
                        "gateway_fullpermission",
                        IdentityServerConstants.StandardScopes.Email, 
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile, 
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.LocalApi.ScopeName, 
                        "roles" 
                    },
                    AccessTokenLifetime = 1* 60* 60, // 1 saat
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AllowOfflineAccess= true,
                },

                //// interactive client using code flow + pkce
                //new Client
                //{
                //    ClientId = "interactive",
                //    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                //    AllowedGrantTypes = GrantTypes.Code,

                //    RedirectUris = { "https://localhost:44300/signin-oidc" },
                //    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                //    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                //    AllowOfflineAccess = true,
                //    AllowedScopes = { "openid", "profile", "scope2" }
                //},
            };
    }
}