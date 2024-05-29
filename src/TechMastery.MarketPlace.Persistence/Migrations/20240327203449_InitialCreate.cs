using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechMastery.MarketPlace.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1b4ef115-c58b-49ab-bc68-05fe2e7eb586"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("25167eea-d0a1-4642-9f02-8d29b1e5531c"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b2e65058-8ca3-4848-9a5c-99037addc804"));

            migrationBuilder.DeleteData(
                table: "Frameworks",
                keyColumn: "Id",
                keyValue: new Guid("c3865618-5efc-404d-adf2-02f467998492"));

            migrationBuilder.DeleteData(
                table: "Frameworks",
                keyColumn: "Id",
                keyValue: new Guid("e082bfc7-946c-4203-9655-cf91ef7b22a1"));

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("6b980fad-c7b5-4768-80d6-af693d30b345"));

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("7233be19-5a9c-449f-9701-fbd50b78db53"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("2a714d14-221a-4409-bd98-92ef99d91507"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("361b968b-66c8-4195-8126-a63f9924f6d9"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("5fbec092-4525-4d46-824a-52f9cf5a9206"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("9e126f0a-c47f-40c3-adc0-fd5cf32d83fc"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("a6af28a0-e425-477c-8f4e-02a305acf9fc"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("d90ed3fd-9574-416f-9379-8ddf3a627aa3"));

            migrationBuilder.DeleteData(
                table: "Platforms",
                keyColumn: "Id",
                keyValue: new Guid("17430e7d-9db7-4b61-adeb-8d763a683a26"));

            migrationBuilder.DeleteData(
                table: "Platforms",
                keyColumn: "Id",
                keyValue: new Guid("76de0e98-10fa-4903-95fc-4af610a2e6db"));

            migrationBuilder.DeleteData(
                table: "ProductStatus",
                keyColumn: "Id",
                keyValue: new Guid("0ee20767-3857-4521-ae96-62143061aaa1"));

            migrationBuilder.DeleteData(
                table: "ProductStatus",
                keyColumn: "Id",
                keyValue: new Guid("1beaac76-5e45-43a0-9cc7-49df2e80fcc3"));

            migrationBuilder.DeleteData(
                table: "ProductStatus",
                keyColumn: "Id",
                keyValue: new Guid("892cb0bc-c75c-428b-a5d9-31778b5bc50b"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { new Guid("7cdc42da-e3d6-4795-aa8c-8fb55eacbddd"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5990), null, null, "AI/ML", null },
                    { new Guid("7f2b9300-95d8-4a30-9930-28a31aff7d74"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5990), null, null, "Devops", null },
                    { new Guid("9e83d6dd-8746-4cf1-b3f2-adb2e872ca83"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5990), null, null, "Web", null }
                });

            migrationBuilder.InsertData(
                table: "Frameworks",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "Version" },
                values: new object[,]
                {
                    { new Guid("d3eb5a02-34f5-4186-bb0f-75aeb6ead11b"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5920), null, null, "React", "16.13.1" },
                    { new Guid("ffe1cf7f-8a78-4997-a657-ae96b63d7fe3"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5910), null, null, ".NET Core", "3.1" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "Version" },
                values: new object[,]
                {
                    { new Guid("3489f832-7d05-419e-aa14-a44637b63e5f"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5810), null, null, "C#", "" },
                    { new Guid("b3522427-e816-4f32-b248-1e906be3972e"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5810), null, null, "JavaScript", "" }
                });

            migrationBuilder.InsertData(
                table: "PaymentStatus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "StatusEnum" },
                values: new object[,]
                {
                    { new Guid("22c19cf7-10bb-4c22-88b6-647c44559553"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5680), null, null, "Successful", 0 },
                    { new Guid("410c5964-ec24-4669-9d7d-668666a10b9e"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5670), null, null, "Completed", 0 },
                    { new Guid("7cdd786a-e61a-4841-9a5a-c80b6c0518d6"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5680), null, null, "Refunded", 0 },
                    { new Guid("7ce711a4-b7b3-4cd2-bd3f-8f7704980a51"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5680), null, null, "Disputed", 0 },
                    { new Guid("b968d573-a7be-414c-be4a-559c0d4df52e"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5570), null, null, "Pending", 0 },
                    { new Guid("dc96fcd9-c517-4fbf-b4b3-867a55842572"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5670), null, null, "Failed", 0 }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "Version" },
                values: new object[,]
                {
                    { new Guid("93a5b0a8-3ca1-4b13-a5d2-e51e00a8f702"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5940), null, null, "Linux", "" },
                    { new Guid("cd78acba-b6dd-4e21-89c0-d584519e1308"), null, new DateTime(2024, 3, 27, 20, 34, 49, 649, DateTimeKind.Utc).AddTicks(5940), null, null, "Windows", "" }
                });

            migrationBuilder.InsertData(
                table: "ProductStatus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "StatusEnum" },
                values: new object[,]
                {
                    { new Guid("627b19ec-6555-43e6-b1d9-873756abf18f"), null, new DateTime(2024, 3, 27, 20, 34, 49, 648, DateTimeKind.Utc).AddTicks(5930), null, null, "InReview", 0 },
                    { new Guid("b812da0b-39cd-408a-a02e-a1f7cb9a15f5"), null, new DateTime(2024, 3, 27, 20, 34, 49, 648, DateTimeKind.Utc).AddTicks(5860), null, null, "NewlyListed", 0 },
                    { new Guid("d2cfb1f6-714e-4ba7-aa51-c0f0965a4555"), null, new DateTime(2024, 3, 27, 20, 34, 49, 648, DateTimeKind.Utc).AddTicks(5930), null, null, "ReadyForSale", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7cdc42da-e3d6-4795-aa8c-8fb55eacbddd"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("7f2b9300-95d8-4a30-9930-28a31aff7d74"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9e83d6dd-8746-4cf1-b3f2-adb2e872ca83"));

            migrationBuilder.DeleteData(
                table: "Frameworks",
                keyColumn: "Id",
                keyValue: new Guid("d3eb5a02-34f5-4186-bb0f-75aeb6ead11b"));

            migrationBuilder.DeleteData(
                table: "Frameworks",
                keyColumn: "Id",
                keyValue: new Guid("ffe1cf7f-8a78-4997-a657-ae96b63d7fe3"));

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("3489f832-7d05-419e-aa14-a44637b63e5f"));

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("b3522427-e816-4f32-b248-1e906be3972e"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("22c19cf7-10bb-4c22-88b6-647c44559553"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("410c5964-ec24-4669-9d7d-668666a10b9e"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("7cdd786a-e61a-4841-9a5a-c80b6c0518d6"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("7ce711a4-b7b3-4cd2-bd3f-8f7704980a51"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("b968d573-a7be-414c-be4a-559c0d4df52e"));

            migrationBuilder.DeleteData(
                table: "PaymentStatus",
                keyColumn: "Id",
                keyValue: new Guid("dc96fcd9-c517-4fbf-b4b3-867a55842572"));

            migrationBuilder.DeleteData(
                table: "Platforms",
                keyColumn: "Id",
                keyValue: new Guid("93a5b0a8-3ca1-4b13-a5d2-e51e00a8f702"));

            migrationBuilder.DeleteData(
                table: "Platforms",
                keyColumn: "Id",
                keyValue: new Guid("cd78acba-b6dd-4e21-89c0-d584519e1308"));

            migrationBuilder.DeleteData(
                table: "ProductStatus",
                keyColumn: "Id",
                keyValue: new Guid("627b19ec-6555-43e6-b1d9-873756abf18f"));

            migrationBuilder.DeleteData(
                table: "ProductStatus",
                keyColumn: "Id",
                keyValue: new Guid("b812da0b-39cd-408a-a02e-a1f7cb9a15f5"));

            migrationBuilder.DeleteData(
                table: "ProductStatus",
                keyColumn: "Id",
                keyValue: new Guid("d2cfb1f6-714e-4ba7-aa51-c0f0965a4555"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "ParentCategoryId" },
                values: new object[,]
                {
                    { new Guid("1b4ef115-c58b-49ab-bc68-05fe2e7eb586"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3350), null, null, "Web", null },
                    { new Guid("25167eea-d0a1-4642-9f02-8d29b1e5531c"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3360), null, null, "Devops", null },
                    { new Guid("b2e65058-8ca3-4848-9a5c-99037addc804"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3360), null, null, "AI/ML", null }
                });

            migrationBuilder.InsertData(
                table: "Frameworks",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "Version" },
                values: new object[,]
                {
                    { new Guid("c3865618-5efc-404d-adf2-02f467998492"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3200), null, null, ".NET Core", "3.1" },
                    { new Guid("e082bfc7-946c-4203-9655-cf91ef7b22a1"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3200), null, null, "React", "16.13.1" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "Version" },
                values: new object[,]
                {
                    { new Guid("6b980fad-c7b5-4768-80d6-af693d30b345"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3080), null, null, "JavaScript", "" },
                    { new Guid("7233be19-5a9c-449f-9701-fbd50b78db53"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3080), null, null, "C#", "" }
                });

            migrationBuilder.InsertData(
                table: "PaymentStatus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "StatusEnum" },
                values: new object[,]
                {
                    { new Guid("2a714d14-221a-4409-bd98-92ef99d91507"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(2920), null, null, "Completed", 0 },
                    { new Guid("361b968b-66c8-4195-8126-a63f9924f6d9"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(2930), null, null, "Failed", 0 },
                    { new Guid("5fbec092-4525-4d46-824a-52f9cf5a9206"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(2940), null, null, "Successful", 0 },
                    { new Guid("9e126f0a-c47f-40c3-adc0-fd5cf32d83fc"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(2930), null, null, "Refunded", 0 },
                    { new Guid("a6af28a0-e425-477c-8f4e-02a305acf9fc"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(2860), null, null, "Pending", 0 },
                    { new Guid("d90ed3fd-9574-416f-9379-8ddf3a627aa3"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(2930), null, null, "Disputed", 0 }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "Version" },
                values: new object[,]
                {
                    { new Guid("17430e7d-9db7-4b61-adeb-8d763a683a26"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3300), null, null, "Windows", "" },
                    { new Guid("76de0e98-10fa-4903-95fc-4af610a2e6db"), null, new DateTime(2024, 3, 27, 20, 4, 52, 992, DateTimeKind.Utc).AddTicks(3300), null, null, "Linux", "" }
                });

            migrationBuilder.InsertData(
                table: "ProductStatus",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name", "StatusEnum" },
                values: new object[,]
                {
                    { new Guid("0ee20767-3857-4521-ae96-62143061aaa1"), null, new DateTime(2024, 3, 27, 20, 4, 52, 991, DateTimeKind.Utc).AddTicks(3490), null, null, "InReview", 0 },
                    { new Guid("1beaac76-5e45-43a0-9cc7-49df2e80fcc3"), null, new DateTime(2024, 3, 27, 20, 4, 52, 991, DateTimeKind.Utc).AddTicks(3430), null, null, "NewlyListed", 0 },
                    { new Guid("892cb0bc-c75c-428b-a5d9-31778b5bc50b"), null, new DateTime(2024, 3, 27, 20, 4, 52, 991, DateTimeKind.Utc).AddTicks(3500), null, null, "ReadyForSale", 0 }
                });
        }
    }
}
