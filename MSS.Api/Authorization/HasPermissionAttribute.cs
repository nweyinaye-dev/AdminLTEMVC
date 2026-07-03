using Microsoft.AspNetCore.Authorization;
namespace MSS.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Permission = permission;
        Policy = $"Permission:{permission}";
    }

    public string Permission { get; }
}






