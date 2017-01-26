using System.Threading.Tasks;
using Authorization.Models;
using Authorization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Authorization.Infrastructure.Authorization
{
    public class ProductAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Product>
    {
        private readonly IDiscountPermissionService _discountPermissionService;

        public ProductAuthorizationHandler(IDiscountPermissionService discountPermissionService)
        {
            _discountPermissionService = discountPermissionService;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Product product)
        {
            // Products can be handled only by sales people
            if (!context.User.HasClaim("department", "sales"))
            {
                return Task.CompletedTask;
            }

            // Special products can be edited only by senior sales people
            if (requirement == ProductOperations.Edit)
            {
                if (product.ProductType == ProductType.Special)
                {
                    if (!context.User.HasClaim("status", "senior"))
                    {
                        return Task.CompletedTask;
                    }
                }
            }

            // Discount operations needs an external service to validate
            // the discount ammount for a given product
            var discountRequirement = requirement as DiscountOperationAuthorizationRequirement;

            if (discountRequirement != null)
            {
                if (product.ProductType == ProductType.Special)
                {
                    if (!context.User.HasClaim("status", "senior"))
                    {
                        return Task.CompletedTask;
                    }
                }

                HandleDiscountOperation(context, discountRequirement, product);

                return Task.CompletedTask;
            }

            // Other operations are allowed
            context.Succeed(requirement);

            return Task.CompletedTask;
        }

        private void HandleDiscountOperation(
            AuthorizationHandlerContext context,
            DiscountOperationAuthorizationRequirement requirement,
            Product product)
        {
            var isAllowed = _discountPermissionService
                .IsDiscountAllowed(product.Id, requirement.Amount);

            if (isAllowed)
            {
                context.Succeed(requirement);
            }
        }
    }
}
