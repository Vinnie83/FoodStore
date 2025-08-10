namespace FoodStore.GCommon
{
    public class ValidationConstants
    {
        public static class Product
        {
            public const int ProductNameMinLength = 2;
            public const int ProductNameMaxLength = 80;

            public const decimal ProductMinPrice = 0.05m;
            public const decimal ProductMaxPrice = 5000.00m;

            public const int ProductMinQuantity = 0;
            public const int ProductMaxQuantity = 3000;

            public const int ProductBarcodeMinLength = 8;
            public const int ProductBarcodeMaxLength = 20;
        }

        public static class Category
        {
            public const int CategoryNameMinLength = 3;
            public const int CategoryNameMaxLength = 50;
        }

        public static class Brand
        {
            public const int BrandNameMinLength = 2;
            public const int BrandNameMaxLength = 30;
        }

        public static class Supplier
        {
            public const int SupplierPhoneMinLength = 9;
            public const int SupplierPhoneMaxLength = 20;

            public const int SupplierEmailMinLength = 9;
            public const int SupplierEmailMaxLength = 100;
        }

        public const string NoImageUrl = "no-image.jpg";
        public const string CreatedOnFormat = "dd-MM-yyyy";

    }
}
