using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tranglo1.CustomerIdentity.Domain.DomainServices;
using Tranglo1.CustomerIdentity.Domain.Entities;
using Tranglo1.CustomerIdentity.Domain.Repositories;
using Tranglo1.CustomerIdentity.IdentityServer.Common.Exceptions;
using Tranglo1.CustomerIdentity.IdentityServer.Queries;
using Tranglo1.CustomerIdentity.IdentityServer.Services.Identity;
using Tranglo1.CustomerIdentity.Infrastructure.Services;
using Tranglo1.UserAccessControl;

namespace Tranglo1.CustomerIdentity.IdentityServer.MediatR.Behaviours
{
    public class EntityVerificationBehavior
    {
        public static Result TrangloEntityChecking(List<TrangloStaffEntityAssignment> staff, string entityCode)
        {
            foreach (var item in staff)
            {
                if (item.TrangloEntity.Equals(entityCode))
                {
                    return Result.Success(staff);
                }
            }
            return Result.Failure("Entity list do not match");
        }
    }
}
