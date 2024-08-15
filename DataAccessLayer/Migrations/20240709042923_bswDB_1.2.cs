using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class bswDB_12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1e979495-3e52-48c6-b92f-9aa4c3ed7908");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4d274d3a-ab13-43d3-9472-b6b2c6bdafdf");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a76b9a0f-73bb-4f9a-8233-c5074336dd25");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("3738576a-5151-4677-9c7d-560ebb9630b3"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("76494ec6-5a62-467f-bfe1-2da3ca622c6a"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("7e16458d-bfb5-4e87-9e01-9be91f1adea4"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("b93e2f4c-f18f-4a60-a278-80e975e02dba"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("fca2c288-ce9a-4b7e-9c4b-f874b5360b40"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("1f08c6bc-f1e7-4dbd-ae41-aaaa531375c0"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("755eb9c7-ad34-405e-9b8c-c939a4fe6598"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("925d3b32-0549-4484-a949-07ad234b15ef"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("cad7735b-5be3-43a0-b147-c8922d530629"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("e13bb112-baf9-4e91-b8b5-680293e71f70"));

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Sizes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Sizes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1e979495-3e52-48c6-b92f-9aa4c3ed7908", null, "Admin", "Admin" },
                    { "4d274d3a-ab13-43d3-9472-b6b2c6bdafdf", null, "Client", "Client" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a76b9a0f-73bb-4f9a-8233-c5074336dd25", 0, "dba83d9c-09e1-46c2-a917-ed290c3a47ec", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEK6OyANqdhdu6up4220JUOibl5deRcd/k32d2WgVK1srRztu0xRU8qYTzGciNMw+fg==", null, false, "29ff3605-b4be-408f-ae0d-f2b1effd2007", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("3738576a-5151-4677-9c7d-560ebb9630b3"), "", new DateTime(2024, 7, 9, 11, 27, 40, 97, DateTimeKind.Local).AddTicks(4870), null, null, "", null, null, "Blue", 1 },
                    { new Guid("76494ec6-5a62-467f-bfe1-2da3ca622c6a"), "", new DateTime(2024, 7, 9, 11, 27, 40, 97, DateTimeKind.Local).AddTicks(4885), null, null, "", null, null, "Green", 1 },
                    { new Guid("7e16458d-bfb5-4e87-9e01-9be91f1adea4"), "", new DateTime(2024, 7, 9, 11, 27, 40, 97, DateTimeKind.Local).AddTicks(4866), null, null, "", null, null, "Black", 1 },
                    { new Guid("b93e2f4c-f18f-4a60-a278-80e975e02dba"), "", new DateTime(2024, 7, 9, 11, 27, 40, 97, DateTimeKind.Local).AddTicks(4846), null, null, "", null, null, "White", 1 },
                    { new Guid("fca2c288-ce9a-4b7e-9c4b-f874b5360b40"), "", new DateTime(2024, 7, 9, 11, 27, 40, 97, DateTimeKind.Local).AddTicks(4868), null, null, "", null, null, "Red", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1f08c6bc-f1e7-4dbd-ae41-aaaa531375c0"), "", new DateTime(2024, 7, 9, 11, 27, 40, 150, DateTimeKind.Local).AddTicks(8902), null, null, "", null, null, "L", 1 },
                    { new Guid("755eb9c7-ad34-405e-9b8c-c939a4fe6598"), "", new DateTime(2024, 7, 9, 11, 27, 40, 150, DateTimeKind.Local).AddTicks(8898), null, null, "", null, null, "S", 1 },
                    { new Guid("925d3b32-0549-4484-a949-07ad234b15ef"), "", new DateTime(2024, 7, 9, 11, 27, 40, 150, DateTimeKind.Local).AddTicks(8875), null, null, "", null, null, "XS", 1 },
                    { new Guid("cad7735b-5be3-43a0-b147-c8922d530629"), "", new DateTime(2024, 7, 9, 11, 27, 40, 150, DateTimeKind.Local).AddTicks(8900), null, null, "", null, null, "M", 1 },
                    { new Guid("e13bb112-baf9-4e91-b8b5-680293e71f70"), "", new DateTime(2024, 7, 9, 11, 27, 40, 150, DateTimeKind.Local).AddTicks(8904), null, null, "", null, null, "XL", 1 }
                });
        }
    }
}
