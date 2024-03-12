using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebAPICoreDapper.Constants;

namespace DemoApi.Filters
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        private readonly FunctionCode _function;
        private readonly ActionCode _action;
        public ClaimRequirementFilter(FunctionCode function, ActionCode action)
        {
            _function = function;
            _action = action;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var permissionClaim = context.HttpContext.User.Claims.SingleOrDefault(c => c.Type == SystemConstants.UserClaim.Permissions);
            var functionArr = _function.ToString().Split('_');
            string functionId = string.Join(".", functionArr);
            if (permissionClaim != null)
            {
                var permissions = JsonConvert.DeserializeObject<List<string>>(permissionClaim.Value);
                if (!permissions.Contains(_function + "_" + _action))
                {
                    context.Result = new ForbidResult();
                }
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
