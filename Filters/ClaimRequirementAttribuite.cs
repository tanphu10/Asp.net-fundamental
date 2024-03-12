using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Filters
{
    public class ClaimRequirementAttribuite : TypeFilterAttribute
    {
        public ClaimRequirementAttribuite(FunctionCode function, ActionCode action) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { function, action };
        }
    }
}
