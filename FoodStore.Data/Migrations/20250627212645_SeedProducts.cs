using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Danone");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "ImageUrl", "Name", "Price", "Quantity", "SupplierId" },
                values: new object[,]
                {
                    { 1, 1, 1, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-157.jpg", "Banana", 2.30m, 150, 5 },
                    { 2, 2, 1, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-109.jpg", "Avocado", 4.70m, 20, 5 },
                    { 3, 3, 1, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-395.jpg", "Potato", 3.20m, 200, 5 },
                    { 4, 2, 1, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-102.jpg", "Onion", 2.10m, 100, 5 },
                    { 5, 4, 2, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-11242.jpg", "Yogurt Meggle", 1.80m, 50, 4 },
                    { 6, 5, 2, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-10247.jpg", "Yogurt Activia Naturalna", 2.10m, 60, 4 },
                    { 7, 6, 2, "https://powellsnl.ca/media/uploads/gs1/.thumbnails/05680008218_1.png/05680008218_1-650x0-padded-%23fff.png", "Yogurt Danone Vanilla", 2.50m, 20, 4 },
                    { 8, 7, 2, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-11026.jpg", "Butter Sonnenweg", 4.10m, 35, 4 },
                    { 9, 8, 2, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-10608.jpg", "Milk Harmonica", 5.10m, 20, 4 },
                    { 10, 9, 3, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-2115.jpg", "Chicken Gradus", 5.10m, 30, 1 },
                    { 11, 10, 3, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-14157.jpg", "Minced meat Tandem", 8.50m, 50, 1 },
                    { 12, 11, 4, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-14062.jpg", "Salami Naroden Hamburgski", 4.30m, 30, 1 },
                    { 13, 12, 4, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-14289.jpg", "Ambarica Chichovtsi", 6.50m, 30, 1 },
                    { 14, 13, 4, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-14267.jpg", "Boni Sausage", 5.50m, 30, 1 },
                    { 15, 14, 5, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-30036.jpg", "Couscous Stella", 3.10m, 50, 2 },
                    { 16, 15, 5, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-30011.jpg", "Couscous Melissa", 3.30m, 30, 2 },
                    { 17, 16, 5, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-30662.jpg", "Bread Simid", 2.40m, 60, 2 },
                    { 18, 17, 5, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-30478.jpg", "Bread Mestemacher", 2.40m, 15, 2 },
                    { 19, 18, 6, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-55101.jpg", "Bake Rolls Spinach", 1.30m, 50, 2 },
                    { 20, 19, 6, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-159011.jpg", "Chocolate Benjamissimo Caramel", 6.40m, 22, 2 },
                    { 21, 20, 6, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-50242.jpg", "Biscuits Nestle Zhiten Dar", 2.40m, 30, 2 },
                    { 22, 21, 6, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-51979.jpg", "Biscuits Belvita wholegrain", 1.30m, 32, 2 },
                    { 23, 22, 6, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-51690.jpg", "Cookies Leibnitz butter", 2.80m, 32, 2 },
                    { 24, 23, 6, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-50015.jpg", "Biscuits Loacker Noisette", 5.60m, 21, 2 },
                    { 25, 24, 6, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-55311.jpg", "Chips Chio Salted", 3.70m, 30, 2 },
                    { 26, 25, 6, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-50419.jpg", "Chocolate Cookies Milka", 3.80m, 15, 2 },
                    { 27, 26, 7, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-31041.jpg", "Beans Krina Extra", 2.80m, 25, 2 },
                    { 28, 27, 8, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-40062.jpg", "Coca Cola Original", 2.40m, 80, 3 },
                    { 29, 28, 8, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-40124.jpg", "Fanta Orange", 3.20m, 54, 3 },
                    { 30, 29, 9, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-38038.jpg", "Baby Puree Hipp Pumpkin", 3.40m, 14, 3 },
                    { 31, 30, 10, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-90050.jpg", "Aluminium Foil Fino", 3.60m, 20, 3 },
                    { 32, 31, 10, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-90049.jpg", "Batteries duracell AAA", 8.90m, 25, 3 },
                    { 33, 32, 10, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-80275.jpg", "Washing-up Liquid Medix", 2.10m, 20, 3 },
                    { 34, 33, 10, "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-80055.jpg", "Washing-up Liquid Pur Apple", 3.15m, 18, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Activia");
        }
    }
}
