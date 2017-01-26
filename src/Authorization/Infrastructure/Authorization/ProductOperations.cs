using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Authorization.Infrastructure.Authorization
{
    public static class ProductOperations
    {
        public static OperationAuthorizationRequirement Edit =
            new OperationAuthorizationRequirement { Name = "Edit" };

        public static OperationAuthorizationRequirement GiveDiscount(decimal amount)
        {
            return new DiscountOperationAuthorizationRequirement
            {
                Name = "GiveDiscount",
                Amount = amount
            };
        }
    }
}
