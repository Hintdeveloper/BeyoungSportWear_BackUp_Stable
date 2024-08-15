using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartProductDetails_ProductDetails_IDProductDetails",
                table: "CartProductDetails");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e807850-997b-4921-96bb-d840334d96bd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "14db4e5b-a157-425a-ae9d-6b0f765734d2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "799b8227-6165-4125-9e90-aecc6cfa88f9");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("41075a90-d820-4972-a6f3-59c41178a6ed"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("41a913a7-c724-4cda-94a8-79ec8abffc0d"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("49ff60eb-9018-447e-be5a-effdb43f72aa"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("b642c481-f496-462c-8e36-e72be35f1c4b"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("de5debfa-d474-46f6-a0b0-9468b61c225d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("6384848b-cba8-4ece-8254-884cc524af92"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("695a64fe-8eab-41dd-8688-b58bc62a85a9"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("82ff7d5e-3504-40be-8805-1169d6290685"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("8755a195-25ef-45e6-bcc6-f7f73384ccba"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("f2c5ecb0-e92b-4ff9-98b8-bd2bca8fbf53"));

            migrationBuilder.RenameColumn(
                name: "IDProductDetails",
                table: "CartProductDetails",
                newName: "IDOptions");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8e3b4167-7e4c-4123-99d8-c7492952094f", null, "Admin", "Admin" },
                    { "8f1693b5-3a1f-4dba-9a7c-f00764cc9954", null, "Client", "Client" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "3a1cf6d6-a9e3-4958-b1fb-074f22a1bbd8", 0, "3376cd8a-2ea7-493b-bf0c-0ec6a169a8ea", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEFtRsA/aCDS0IkMHsRWd5WCdkGI4MRhgqnF9CWYYBXnBpcA7xyIKCoW0e9d+D29+jg==", null, false, "757d8d22-90c7-4c21-a4ef-55abab87428f", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("05634f11-cab3-42b4-9f0a-4e3e593a3d79"), "", new DateTime(2024, 7, 11, 7, 57, 53, 124, DateTimeKind.Local).AddTicks(2278), null, null, "", null, null, "White", 1 },
                    { new Guid("0d8f1fd8-4798-4c2c-b2c7-bc4fcd7739c6"), "", new DateTime(2024, 7, 11, 7, 57, 53, 124, DateTimeKind.Local).AddTicks(2299), null, null, "", null, null, "Red", 1 },
                    { new Guid("10a19714-da2b-4293-a508-43883bb831e1"), "", new DateTime(2024, 7, 11, 7, 57, 53, 124, DateTimeKind.Local).AddTicks(2316), null, null, "", null, null, "Green", 1 },
                    { new Guid("7c75c0f8-6696-452f-8b1a-88376256e588"), "", new DateTime(2024, 7, 11, 7, 57, 53, 124, DateTimeKind.Local).AddTicks(2297), null, null, "", null, null, "Black", 1 },
                    { new Guid("e43ef521-1477-489f-8d68-c52e260a4913"), "", new DateTime(2024, 7, 11, 7, 57, 53, 124, DateTimeKind.Local).AddTicks(2301), null, null, "", null, null, "Blue", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("41cae779-19ef-4669-b88a-18fc36a9d5d7"), "", new DateTime(2024, 7, 11, 7, 57, 53, 192, DateTimeKind.Local).AddTicks(7578), null, null, "", null, null, "XS", 1 },
                    { new Guid("4e74b952-09ac-4992-99a2-3cdba15a7586"), "", new DateTime(2024, 7, 11, 7, 57, 53, 192, DateTimeKind.Local).AddTicks(7598), null, null, "", null, null, "S", 1 },
                    { new Guid("9fcad8ec-8926-4386-8a0e-75595630c666"), "", new DateTime(2024, 7, 11, 7, 57, 53, 192, DateTimeKind.Local).AddTicks(7603), null, null, "", null, null, "XL", 1 },
                    { new Guid("aa03e7ed-274b-4ee6-927e-af3e7fa785ee"), "", new DateTime(2024, 7, 11, 7, 57, 53, 192, DateTimeKind.Local).AddTicks(7601), null, null, "", null, null, "L", 1 },
                    { new Guid("fe89bd12-4d32-4600-a288-5a0c1fed1add"), "", new DateTime(2024, 7, 11, 7, 57, 53, 192, DateTimeKind.Local).AddTicks(7600), null, null, "", null, null, "M", 1 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CartProductDetails_Options_IDOptions",
                table: "CartProductDetails",
                column: "IDOptions",
                principalTable: "Options",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartProductDetails_Options_IDOptions",
                table: "CartProductDetails");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e3b4167-7e4c-4123-99d8-c7492952094f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f1693b5-3a1f-4dba-9a7c-f00764cc9954");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3a1cf6d6-a9e3-4958-b1fb-074f22a1bbd8");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("05634f11-cab3-42b4-9f0a-4e3e593a3d79"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("0d8f1fd8-4798-4c2c-b2c7-bc4fcd7739c6"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("10a19714-da2b-4293-a508-43883bb831e1"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("7c75c0f8-6696-452f-8b1a-88376256e588"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("e43ef521-1477-489f-8d68-c52e260a4913"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("41cae779-19ef-4669-b88a-18fc36a9d5d7"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("4e74b952-09ac-4992-99a2-3cdba15a7586"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("9fcad8ec-8926-4386-8a0e-75595630c666"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("aa03e7ed-274b-4ee6-927e-af3e7fa785ee"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("fe89bd12-4d32-4600-a288-5a0c1fed1add"));

            migrationBuilder.RenameColumn(
                name: "IDOptions",
                table: "CartProductDetails",
                newName: "IDProductDetails");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0e807850-997b-4921-96bb-d840334d96bd", null, "Admin", "Admin" },
                    { "14db4e5b-a157-425a-ae9d-6b0f765734d2", null, "Client", "Client" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "799b8227-6165-4125-9e90-aecc6cfa88f9", 0, "09b40c49-fd54-42fa-83aa-cde4649192ef", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEMy3erALzqJMHfdqmSPeop1CZMXfep9/lKZ7BH/bRIFdg0WHqBZ9oCLRZiUUb19sPw==", null, false, "bcfe3d17-238b-47ed-b46c-59571d7a7dd6", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("41075a90-d820-4972-a6f3-59c41178a6ed"), "", new DateTime(2024, 7, 11, 7, 50, 25, 273, DateTimeKind.Local).AddTicks(3183), null, null, "", null, null, "Green", 1 },
                    { new Guid("41a913a7-c724-4cda-94a8-79ec8abffc0d"), "", new DateTime(2024, 7, 11, 7, 50, 25, 273, DateTimeKind.Local).AddTicks(3145), null, null, "", null, null, "White", 1 },
                    { new Guid("49ff60eb-9018-447e-be5a-effdb43f72aa"), "", new DateTime(2024, 7, 11, 7, 50, 25, 273, DateTimeKind.Local).AddTicks(3175), null, null, "", null, null, "Black", 1 },
                    { new Guid("b642c481-f496-462c-8e36-e72be35f1c4b"), "", new DateTime(2024, 7, 11, 7, 50, 25, 273, DateTimeKind.Local).AddTicks(3177), null, null, "", null, null, "Red", 1 },
                    { new Guid("de5debfa-d474-46f6-a0b0-9468b61c225d"), "", new DateTime(2024, 7, 11, 7, 50, 25, 273, DateTimeKind.Local).AddTicks(3180), null, null, "", null, null, "Blue", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("6384848b-cba8-4ece-8254-884cc524af92"), "", new DateTime(2024, 7, 11, 7, 50, 25, 360, DateTimeKind.Local).AddTicks(2195), null, null, "", null, null, "L", 1 },
                    { new Guid("695a64fe-8eab-41dd-8688-b58bc62a85a9"), "", new DateTime(2024, 7, 11, 7, 50, 25, 360, DateTimeKind.Local).AddTicks(2189), null, null, "", null, null, "M", 1 },
                    { new Guid("82ff7d5e-3504-40be-8805-1169d6290685"), "", new DateTime(2024, 7, 11, 7, 50, 25, 360, DateTimeKind.Local).AddTicks(2186), null, null, "", null, null, "S", 1 },
                    { new Guid("8755a195-25ef-45e6-bcc6-f7f73384ccba"), "", new DateTime(2024, 7, 11, 7, 50, 25, 360, DateTimeKind.Local).AddTicks(2148), null, null, "", null, null, "XS", 1 },
                    { new Guid("f2c5ecb0-e92b-4ff9-98b8-bd2bca8fbf53"), "", new DateTime(2024, 7, 11, 7, 50, 25, 360, DateTimeKind.Local).AddTicks(2200), null, null, "", null, null, "XL", 1 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CartProductDetails_ProductDetails_IDProductDetails",
                table: "CartProductDetails",
                column: "IDProductDetails",
                principalTable: "ProductDetails",
                principalColumn: "ID");
        }
    }
}
