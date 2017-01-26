using System.ComponentModel.DataAnnotations;

namespace Authorization.ViewModels.Products
{
    public class ProductDiscountViewModel
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Range(1, 100)]
        public decimal Discount { get; set; }
    }
}