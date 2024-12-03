using AnimalCrossingAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace AnimalCrossingAPI.Auth
{
    public class CreatedByAuthorizationHandler : AuthorizationHandler<CreatedByRequirement, Fish>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedByRequirement requirement, Fish resource)
        {
            var scopeClaim = context.User.FindFirst("scope")?.Value;
            var userName = context.User.Identity.Name;
            Console.WriteLine("In CreatedByAuthorizationHandler!!");
            Console.WriteLine($"This is scope claim : {scopeClaim}");
            Console.WriteLine("This is username :" + userName);
            Console.WriteLine("This is who created the resource :" + resource.CreatedBy);

            if (scopeClaim != null)
            {
                if (userName == resource.CreatedBy)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
