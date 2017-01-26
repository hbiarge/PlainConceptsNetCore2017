using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Authorization.Infrastructure.Authorization
{
    public class DiscountOperationAuthorizationRequirement 
        : OperationAuthorizationRequirement
    {
        public decimal Amount { get; set; }
    }
}