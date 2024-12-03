using Microsoft.AspNetCore.Authorization;

namespace AnimalCrossingAPI.Auth
{
    public class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
        {

            Console.WriteLine("In scopeAuthorizationHandler");

            var scopeClaim = context.User.FindFirst("scope")?.Value;

            if (scopeClaim != null)
            {
                var scopes = scopeClaim.Split(' ');
                if (scopes.Contains(requirement.RequiredScope))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
