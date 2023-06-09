﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PeopleSearch.Services.Intarfaces.Models;

namespace PeopleSearchAPI.Helpers;

/// <summary>
/// Specifies that the class or method that this attribute is applied to don't requires the specified authorization
/// </summary>

[AttributeUsage(AttributeTargets.Method)]
public class LoginAttribute : Attribute, IAuthorizationFilter
{
    /// <inheritdoc/>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (UserModel?)context.HttpContext.Items["User"];

        if (user != null)
        {
            context.Result = new JsonResult(new { message = "Already authorized" })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
