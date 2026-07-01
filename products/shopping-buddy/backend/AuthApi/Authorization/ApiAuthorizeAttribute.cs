using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Authorization;

public sealed class ApiAuthorizeAttribute : TypeFilterAttribute
{
    public ApiAuthorizeAttribute() : base(typeof(ApiAuthorizeFilter))
    {
    }
}
