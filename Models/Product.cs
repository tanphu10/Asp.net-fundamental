using System.ComponentModel.DataAnnotations;

namespace DemoApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        //[StringLength(8, ErrorMessage = "SKUMinAndMaxLengthError", MinimumLength = 6)]
        [Required(ErrorMessage = "SKURequiredErrorMsg")]
        public string Sku { get; set; }
        [Required(ErrorMessage = "PriceRequiredErrorMsg")]
        public float Price { get; set; }
        public float? DiscountPrice { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
        public int ViewCount { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string SeoAlias { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SeoTitle { get; set; }
        public string CategoryIds { get; set; }
        public string CategoryName { get; set; }

    }
}
