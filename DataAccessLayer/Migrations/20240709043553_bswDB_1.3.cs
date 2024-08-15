using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class bswDB_13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "146544a3-3319-4a25-9097-aa9cef9ea397");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46bce938-7805-4603-b5f6-e85738a83606");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d6a2f90-5271-43fb-bcd3-4c32c23056d5");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1538926c-7152-4282-b774-cd53d45c192b"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("2363ff2c-eefd-46b6-a9da-84d8afb70659"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("7b6c1e90-d350-4e77-b115-4134aa0ea373"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9d1e212f-0a3d-4c79-abdb-61d35e2c300c"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("fa72afb1-f46a-4455-804b-0933d933af32"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("17f022fc-3b7a-4706-a4a4-d06514102e9c"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("8f033c72-1bbd-401c-b8e6-3b8ba0026bb7"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("c1e9f213-b557-4c5f-af5c-8f1aa3081232"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("d3b8c370-2ca4-4404-88a7-ab432051894a"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("f35e5133-5d7e-40bf-b590-f0340f1e130d"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0e734ced-7c1d-4c38-9d7c-f2bde0e2df44", null, "Client", "Client" },
                    { "75c4c5c7-b22d-4841-9eb6-f0b1c8a76c56", null, "Admin", "Admin" },
                    { "922e2d85-af0e-44a8-a56f-37edf00adc2d", null, "Staff", "Staff" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "56056ec6-16c2-4563-9419-dd90b7c9376a", 0, "57c10f89-0ba7-49ea-92c6-075d525ce28a", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEO2X2RJzKmeLvNngE5/XMDng9Uq/nLpiGZb9ZWWP4x1hPVNEHoVeBQw9U/+Ptxr+2w==", null, false, "a3660571-a9fb-4b7c-90d0-14198ed7c757", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1386eb22-b20e-4b00-ab8a-2f287e292469"), "", new DateTime(2024, 7, 9, 11, 35, 53, 255, DateTimeKind.Local).AddTicks(1604), null, null, "", null, null, "Black", 1 },
                    { new Guid("1921ede7-75b6-4e80-b048-92bc4f245f3d"), "", new DateTime(2024, 7, 9, 11, 35, 53, 255, DateTimeKind.Local).AddTicks(1570), null, null, "", null, null, "White", 1 },
                    { new Guid("2f9df112-c65c-4da8-9437-761151ad16fd"), "", new DateTime(2024, 7, 9, 11, 35, 53, 255, DateTimeKind.Local).AddTicks(1608), null, null, "", null, null, "Blue", 1 },
                    { new Guid("a4911ddb-2ea9-4ad7-9d9e-a082ef98c4c1"), "", new DateTime(2024, 7, 9, 11, 35, 53, 255, DateTimeKind.Local).AddTicks(1610), null, null, "", null, null, "Green", 1 },
                    { new Guid("a5c3f724-a042-4949-907a-9f4c9fad6a33"), "", new DateTime(2024, 7, 9, 11, 35, 53, 255, DateTimeKind.Local).AddTicks(1606), null, null, "", null, null, "Red", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("54b1cb82-0329-4e40-b0d2-163ce0ed5e0d"), "", new DateTime(2024, 7, 9, 11, 35, 53, 318, DateTimeKind.Local).AddTicks(1873), null, null, "", null, null, "M", 1 },
                    { new Guid("75c4c11f-c30a-47e9-bade-76fd90582cd7"), "", new DateTime(2024, 7, 9, 11, 35, 53, 318, DateTimeKind.Local).AddTicks(1878), null, null, "", null, null, "XL", 1 },
                    { new Guid("bd5c9071-a730-451d-8d1e-03c60f914626"), "", new DateTime(2024, 7, 9, 11, 35, 53, 318, DateTimeKind.Local).AddTicks(1852), null, null, "", null, null, "XS", 1 },
                    { new Guid("c2977e7d-25c6-420c-a95a-8222049ad4d0"), "", new DateTime(2024, 7, 9, 11, 35, 53, 318, DateTimeKind.Local).AddTicks(1871), null, null, "", null, null, "S", 1 },
                    { new Guid("d43aefac-3e06-44c1-a41b-4ef61c4cb86e"), "", new DateTime(2024, 7, 9, 11, 35, 53, 318, DateTimeKind.Local).AddTicks(1876), null, null, "", null, null, "L", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e734ced-7c1d-4c38-9d7c-f2bde0e2df44");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "75c4c5c7-b22d-4841-9eb6-f0b1c8a76c56");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "922e2d85-af0e-44a8-a56f-37edf00adc2d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "56056ec6-16c2-4563-9419-dd90b7c9376a");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1386eb22-b20e-4b00-ab8a-2f287e292469"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1921ede7-75b6-4e80-b048-92bc4f245f3d"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("2f9df112-c65c-4da8-9437-761151ad16fd"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("a4911ddb-2ea9-4ad7-9d9e-a082ef98c4c1"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("a5c3f724-a042-4949-907a-9f4c9fad6a33"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("54b1cb82-0329-4e40-b0d2-163ce0ed5e0d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("75c4c11f-c30a-47e9-bade-76fd90582cd7"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("bd5c9071-a730-451d-8d1e-03c60f914626"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("c2977e7d-25c6-420c-a95a-8222049ad4d0"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("d43aefac-3e06-44c1-a41b-4ef61c4cb86e"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "146544a3-3319-4a25-9097-aa9cef9ea397", null, "Client", "Client" },
                    { "46bce938-7805-4603-b5f6-e85738a83606", null, "Admin", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6d6a2f90-5271-43fb-bcd3-4c32c23056d5", 0, "a5b99415-2473-4350-9ee1-6886cc9f1d59", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEIdjvN3iXSk9O7mqx44iD9Qk3SE0d9zcMfOsZIW+2AvWHUmlviUerLJb2kXhBqKosw==", null, false, "166bbda7-36e8-4084-9fbd-aa88e1812365", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1538926c-7152-4282-b774-cd53d45c192b"), "", new DateTime(2024, 7, 9, 11, 29, 23, 72, DateTimeKind.Local).AddTicks(5123), null, null, "", null, null, "Blue", 1 },
                    { new Guid("2363ff2c-eefd-46b6-a9da-84d8afb70659"), "", new DateTime(2024, 7, 9, 11, 29, 23, 72, DateTimeKind.Local).AddTicks(5119), null, null, "", null, null, "Black", 1 },
                    { new Guid("7b6c1e90-d350-4e77-b115-4134aa0ea373"), "", new DateTime(2024, 7, 9, 11, 29, 23, 72, DateTimeKind.Local).AddTicks(5121), null, null, "", null, null, "Red", 1 },
                    { new Guid("9d1e212f-0a3d-4c79-abdb-61d35e2c300c"), "", new DateTime(2024, 7, 9, 11, 29, 23, 72, DateTimeKind.Local).AddTicks(5152), null, null, "", null, null, "Green", 1 },
                    { new Guid("fa72afb1-f46a-4455-804b-0933d933af32"), "", new DateTime(2024, 7, 9, 11, 29, 23, 72, DateTimeKind.Local).AddTicks(5097), null, null, "", null, null, "White", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("17f022fc-3b7a-4706-a4a4-d06514102e9c"), "", new DateTime(2024, 7, 9, 11, 29, 23, 138, DateTimeKind.Local).AddTicks(377), null, null, "", null, null, "XS", 1 },
                    { new Guid("8f033c72-1bbd-401c-b8e6-3b8ba0026bb7"), "", new DateTime(2024, 7, 9, 11, 29, 23, 138, DateTimeKind.Local).AddTicks(402), null, null, "", null, null, "M", 1 },
                    { new Guid("c1e9f213-b557-4c5f-af5c-8f1aa3081232"), "", new DateTime(2024, 7, 9, 11, 29, 23, 138, DateTimeKind.Local).AddTicks(404), null, null, "", null, null, "L", 1 },
                    { new Guid("d3b8c370-2ca4-4404-88a7-ab432051894a"), "", new DateTime(2024, 7, 9, 11, 29, 23, 138, DateTimeKind.Local).AddTicks(406), null, null, "", null, null, "XL", 1 },
                    { new Guid("f35e5133-5d7e-40bf-b590-f0340f1e130d"), "", new DateTime(2024, 7, 9, 11, 29, 23, 138, DateTimeKind.Local).AddTicks(400), null, null, "", null, null, "S", 1 }
                });
        }
    }
}
