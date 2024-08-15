using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0d1f74b2-a31e-4e62-a39d-e843daede0ed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1e0fe679-b074-44ce-9860-284e09ca4eaa");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "964f8eff-eef7-4c48-8197-e244f7ca6e81");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1c0e12f8-b6d4-4616-8667-ca51f29477e9"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("38ac12cb-4980-4613-b7eb-131a8721cb6e"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("979ae9be-b2cb-4717-b335-7bc14f194bf2"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9cdf8ae4-4dcd-4856-a731-7f403d8dbfe2"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ec3d7d90-5b8c-4e51-979d-f417ed937834"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("104e3493-aeac-4f20-a4ad-9edee0f486ee"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("650170f8-849e-46b7-8162-c8478f325b72"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("9861c859-8a6b-46cd-98ae-0e9214638b2b"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("a0d987c5-5a72-4676-9f2c-f7a1d6e4d125"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("df3464e4-6c23-4ad6-86e0-1c7155071d4f"));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CartProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "CartProductDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "CartProductDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CartProductDetails");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "CartProductDetails");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "CartProductDetails");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0d1f74b2-a31e-4e62-a39d-e843daede0ed", null, "Client", "Client" },
                    { "1e0fe679-b074-44ce-9860-284e09ca4eaa", null, "Admin", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "964f8eff-eef7-4c48-8197-e244f7ca6e81", 0, "ac6baede-e69d-4c5b-90ad-2dcab102eec9", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAENLAmKDZ8znHal3LHJIwPlrD+BJsmInSFN3mOalH04X46C3PxA5wlcmrg3Ex9BNlXg==", null, false, "62d506af-4de0-4dfc-98ca-04410ded4783", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1c0e12f8-b6d4-4616-8667-ca51f29477e9"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1351), null, null, "", null, null, "White", 1 },
                    { new Guid("38ac12cb-4980-4613-b7eb-131a8721cb6e"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1386), null, null, "", null, null, "Red", 1 },
                    { new Guid("979ae9be-b2cb-4717-b335-7bc14f194bf2"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1388), null, null, "", null, null, "Blue", 1 },
                    { new Guid("9cdf8ae4-4dcd-4856-a731-7f403d8dbfe2"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1384), null, null, "", null, null, "Black", 1 },
                    { new Guid("ec3d7d90-5b8c-4e51-979d-f417ed937834"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1390), null, null, "", null, null, "Green", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("104e3493-aeac-4f20-a4ad-9edee0f486ee"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4314), null, null, "", null, null, "S", 1 },
                    { new Guid("650170f8-849e-46b7-8162-c8478f325b72"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4331), null, null, "", null, null, "XL", 1 },
                    { new Guid("9861c859-8a6b-46cd-98ae-0e9214638b2b"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4316), null, null, "", null, null, "M", 1 },
                    { new Guid("a0d987c5-5a72-4676-9f2c-f7a1d6e4d125"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4290), null, null, "", null, null, "XS", 1 },
                    { new Guid("df3464e4-6c23-4ad6-86e0-1c7155071d4f"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4318), null, null, "", null, null, "L", 1 }
                });
        }
    }
}
