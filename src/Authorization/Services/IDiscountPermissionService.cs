namespace Authorization.Services
{
    public interface IDiscountPermissionService
    {
        bool IsDiscountAllowed(int productId, decimal amount);
    }
}
