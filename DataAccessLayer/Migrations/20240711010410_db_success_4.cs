using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartProductDetails_Cart_IDCart",
                table: "CartProductDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartProductDetails_Options_IDOptions",
                table: "CartProductDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartProductDetails",
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

            migrationBuilder.RenameTable(
                name: "CartProductDetails",
                newName: "CartOptions");

            migrationBuilder.RenameIndex(
                name: "IX_CartProductDetails_IDCart",
                table: "CartOptions",
                newName: "IX_CartOptions_IDCart");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartOptions",
                table: "CartOptions",
                columns: new[] { "IDOptions", "IDCart" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2bd71676-c8d7-48ec-9bd7-df52db146618", null, "Admin", "Admin" },
                    { "ab81ed2e-3358-44df-b3da-d16091e3131f", null, "Client", "Client" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6f01f4a4-c64e-4c29-8923-fcfa39e94630", 0, "673c65eb-2e38-4c0c-8032-821ab6776c89", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEJbcv/g0upAAcZX07e+fEYUkUOhQK+Y0Akd/J53G3PjhYtywzRogRBIl4RK2TkJZag==", null, false, "3c948df0-3e4c-4a3f-b086-6660e5574595", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1390cde8-b1bb-431f-8fe2-d0ba6355efa4"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9127), null, null, "", null, null, "White", 1 },
                    { new Guid("140ed46a-eda9-43b4-9178-60079212cf79"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9148), null, null, "", null, null, "Blue", 1 },
                    { new Guid("9051ccbf-08ae-4714-8706-db4c014c8d55"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9143), null, null, "", null, null, "Black", 1 },
                    { new Guid("b07bb846-5143-4fc6-99d8-8b89760bc93d"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9146), null, null, "", null, null, "Red", 1 },
                    { new Guid("e76bb74b-eaff-4751-bb55-dd44dcca54f5"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9150), null, null, "", null, null, "Green", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("0b8b6da6-38f7-4bc0-a776-1439eaf87c41"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2853), null, null, "", null, null, "S", 1 },
                    { new Guid("4069a60c-0cfb-41c7-81d3-2eeb8ccaa04c"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2865), null, null, "", null, null, "XL", 1 },
                    { new Guid("715a6392-e348-4c83-8e86-b5138eac1e61"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2830), null, null, "", null, null, "XS", 1 },
                    { new Guid("8aab1550-e062-470a-b920-c0dfa1821b71"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2857), null, null, "", null, null, "L", 1 },
                    { new Guid("f44d9902-0a92-4556-9fc7-ce2a99126f87"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2855), null, null, "", null, null, "M", 1 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CartOptions_Cart_IDCart",
                table: "CartOptions",
                column: "IDCart",
                principalTable: "Cart",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartOptions_Options_IDOptions",
                table: "CartOptions",
                column: "IDOptions",
                principalTable: "Options",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartOptions_Cart_IDCart",
                table: "CartOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_CartOptions_Options_IDOptions",
                table: "CartOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartOptions",
                table: "CartOptions");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2bd71676-c8d7-48ec-9bd7-df52db146618");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ab81ed2e-3358-44df-b3da-d16091e3131f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6f01f4a4-c64e-4c29-8923-fcfa39e94630");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1390cde8-b1bb-431f-8fe2-d0ba6355efa4"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("140ed46a-eda9-43b4-9178-60079212cf79"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9051ccbf-08ae-4714-8706-db4c014c8d55"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("b07bb846-5143-4fc6-99d8-8b89760bc93d"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("e76bb74b-eaff-4751-bb55-dd44dcca54f5"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("0b8b6da6-38f7-4bc0-a776-1439eaf87c41"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("4069a60c-0cfb-41c7-81d3-2eeb8ccaa04c"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("715a6392-e348-4c83-8e86-b5138eac1e61"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("8aab1550-e062-470a-b920-c0dfa1821b71"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("f44d9902-0a92-4556-9fc7-ce2a99126f87"));

            migrationBuilder.RenameTable(
                name: "CartOptions",
                newName: "CartProductDetails");

            migrationBuilder.RenameIndex(
                name: "IX_CartOptions_IDCart",
                table: "CartProductDetails",
                newName: "IX_CartProductDetails_IDCart");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartProductDetails",
                table: "CartProductDetails",
                columns: new[] { "IDOptions", "IDCart" });

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
                name: "FK_CartProductDetails_Cart_IDCart",
                table: "CartProductDetails",
                column: "IDCart",
                principalTable: "Cart",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartProductDetails_Options_IDOptions",
                table: "CartProductDetails",
                column: "IDOptions",
                principalTable: "Options",
                principalColumn: "ID");
        }
    }
}
