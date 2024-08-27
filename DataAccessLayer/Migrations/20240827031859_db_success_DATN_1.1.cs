using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_DATN_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "76393af2-7ae8-44e4-acaf-f47516499aa8");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1ac5cda0-b2a4-4c66-a0a2-b7f655873aef"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("20dc31bc-84e6-45bc-87a1-233a6f66422b"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("52a0db55-7e82-4dad-a8e6-e4366fa61774"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("69219117-f1eb-4d2f-95bf-138ecc850123"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("c9097d4f-efaf-4330-8f46-bb12be39bb91"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("4b93f0f9-660f-40bf-bd1c-ef2effd82398"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("55971d42-131e-4252-8ea8-a4dc9cbcc7db"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("765212d0-935b-4cbf-a789-bf8712fca64c"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("88f94b89-4ffd-4e7a-805d-0e8eebb13265"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("8fd4e720-bf10-4eeb-b798-42b742a9072a"));

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Sizes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BarCode",
                table: "ProductDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cotsts",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "7f18afd6-2241-4e8a-b010-bfeb1a27f501", 0, "ba8196c2-35a4-4419-aa27-df3a1dc4ed78", "IdentityUser", "admin@gmail.com", true, false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEO1n8dPO5FE8gYYQdELr77r4zsLHtMAbHO4HLOrjNEXx8/KwV7xOziInyq2GX/1mTw==", null, false, "025003b8-e9eb-4770-a570-f31d63e80234", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("66378141-9ab2-4488-bcd2-d735365a7c25"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1684), null, null, "", null, null, "Black", 1 },
                    { new Guid("98d9c323-c60a-4acd-9590-750b1c6d496c"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1703), null, null, "", null, null, "Green", 1 },
                    { new Guid("9de7d9e9-2fd6-41b2-b7a6-d610e061a3fd"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1670), null, null, "", null, null, "White", 1 },
                    { new Guid("9fc3bbb1-6475-414f-b1aa-a070b97e1da6"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1686), null, null, "", null, null, "Red", 1 },
                    { new Guid("e063f0f3-3e71-40cd-bf14-8885f641ea81"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1701), null, null, "", null, null, "Blue", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("21874a93-657e-4992-ba55-cf2a7eda75cb"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1856), null, null, "", null, null, "M", 1 },
                    { new Guid("284fd8b0-7713-4b5c-9f69-48b36d244e20"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1852), null, null, "", null, null, "XS", 1 },
                    { new Guid("6c7a124b-67b5-4190-ba06-2a67221530d5"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1857), null, null, "", null, null, "L", 1 },
                    { new Guid("c6f3aa95-0eb4-4102-86fc-d82d40b8483e"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1859), null, null, "", null, null, "XL", 1 },
                    { new Guid("dc3b365d-fa14-4530-915d-3514ae52651d"), "", new DateTime(2024, 8, 27, 10, 18, 59, 664, DateTimeKind.Local).AddTicks(1854), null, null, "", null, null, "S", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7f18afd6-2241-4e8a-b010-bfeb1a27f501");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("66378141-9ab2-4488-bcd2-d735365a7c25"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("98d9c323-c60a-4acd-9590-750b1c6d496c"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9de7d9e9-2fd6-41b2-b7a6-d610e061a3fd"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9fc3bbb1-6475-414f-b1aa-a070b97e1da6"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("e063f0f3-3e71-40cd-bf14-8885f641ea81"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("21874a93-657e-4992-ba55-cf2a7eda75cb"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("284fd8b0-7713-4b5c-9f69-48b36d244e20"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("6c7a124b-67b5-4190-ba06-2a67221530d5"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("c6f3aa95-0eb4-4102-86fc-d82d40b8483e"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("dc3b365d-fa14-4530-915d-3514ae52651d"));

            migrationBuilder.DropColumn(
                name: "BarCode",
                table: "ProductDetails");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Sizes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cotsts",
                table: "Order",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "76393af2-7ae8-44e4-acaf-f47516499aa8", 0, "2312a78f-dcda-4081-a6e0-dc4bea7f19b3", "IdentityUser", "admin@gmail.com", true, false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEDhq4bZUrlE9H4/hzUqCNogng+p/Sh7p5NBacPcJm5JKHeHy5wnLhv54U/pDJCwF3Q==", null, false, "7ceebe18-d7f6-4382-a37d-441ab0fb9249", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1ac5cda0-b2a4-4c66-a0a2-b7f655873aef"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(13), null, null, "", null, null, "White", 1 },
                    { new Guid("20dc31bc-84e6-45bc-87a1-233a6f66422b"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(28), null, null, "", null, null, "Black", 1 },
                    { new Guid("52a0db55-7e82-4dad-a8e6-e4366fa61774"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(34), null, null, "", null, null, "Green", 1 },
                    { new Guid("69219117-f1eb-4d2f-95bf-138ecc850123"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(32), null, null, "", null, null, "Blue", 1 },
                    { new Guid("c9097d4f-efaf-4330-8f46-bb12be39bb91"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(30), null, null, "", null, null, "Red", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("4b93f0f9-660f-40bf-bd1c-ef2effd82398"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(206), null, null, "", null, null, "M", 1 },
                    { new Guid("55971d42-131e-4252-8ea8-a4dc9cbcc7db"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(204), null, null, "", null, null, "S", 1 },
                    { new Guid("765212d0-935b-4cbf-a789-bf8712fca64c"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(208), null, null, "", null, null, "L", 1 },
                    { new Guid("88f94b89-4ffd-4e7a-805d-0e8eebb13265"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(199), null, null, "", null, null, "XS", 1 },
                    { new Guid("8fd4e720-bf10-4eeb-b798-42b742a9072a"), "", new DateTime(2024, 7, 28, 11, 12, 45, 331, DateTimeKind.Local).AddTicks(210), null, null, "", null, null, "XL", 1 }
                });
        }
    }
}
