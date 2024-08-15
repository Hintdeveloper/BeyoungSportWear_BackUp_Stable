using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class bswDB_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a5d00a6-2548-4ff2-ae6e-c879cefff04f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36a9e232-f6d9-4a79-a91f-4ed702da8427");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "93b061ce-e8b6-443d-b9b5-2caa9fcfa954");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("122f1b58-2dd1-4cae-8838-9c480c18ec62"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("2b4c2db0-5591-4b4a-b5ae-cd69c6a6088f"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("306e80c6-ba39-4b0f-83dd-7704aa24138e"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("434a3028-6480-4df7-a6be-2d115dc2e7c4"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("5663da6b-f076-439f-bcf8-84d0e3a7bee0"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("4d1945da-9aeb-46d8-a151-879e88206164"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("697c2b8f-9a1b-44ee-8f80-63f831f1b998"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("91bdcc1a-ec5d-408b-8249-ba86c4d1922a"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("baa4abe1-9662-46fd-90aa-14ca8cc74442"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("d0a5b03b-7833-4929-9cb1-241d43c6b8a0"));

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Material",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Manufacturer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                table: "Material",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Manufacturer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Brand",
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
                    { "1a5d00a6-2548-4ff2-ae6e-c879cefff04f", null, "Admin", "Admin" },
                    { "36a9e232-f6d9-4a79-a91f-4ed702da8427", null, "Client", "Client" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "93b061ce-e8b6-443d-b9b5-2caa9fcfa954", 0, "4b2a27d6-18a3-43e8-b7f7-24e16e80c51a", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEPNuGHCIAzsWCM2EqTej14tkVKLQU1M7GWhhA87vR35cBkLyQLXW9Jc+87IYcitenQ==", null, false, "741ce30c-a35d-42da-8750-a2347b83ee9b", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("122f1b58-2dd1-4cae-8838-9c480c18ec62"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9742), null, null, "", null, null, "Red", 1 },
                    { new Guid("2b4c2db0-5591-4b4a-b5ae-cd69c6a6088f"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9718), null, null, "", null, null, "White", 1 },
                    { new Guid("306e80c6-ba39-4b0f-83dd-7704aa24138e"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9744), null, null, "", null, null, "Blue", 1 },
                    { new Guid("434a3028-6480-4df7-a6be-2d115dc2e7c4"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9746), null, null, "", null, null, "Green", 1 },
                    { new Guid("5663da6b-f076-439f-bcf8-84d0e3a7bee0"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9739), null, null, "", null, null, "Black", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("4d1945da-9aeb-46d8-a151-879e88206164"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6796), null, null, "", null, null, "XS", 1 },
                    { new Guid("697c2b8f-9a1b-44ee-8f80-63f831f1b998"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6834), null, null, "", null, null, "XL", 1 },
                    { new Guid("91bdcc1a-ec5d-408b-8249-ba86c4d1922a"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6827), null, null, "", null, null, "S", 1 },
                    { new Guid("baa4abe1-9662-46fd-90aa-14ca8cc74442"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6832), null, null, "", null, null, "L", 1 },
                    { new Guid("d0a5b03b-7833-4929-9cb1-241d43c6b8a0"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6830), null, null, "", null, null, "M", 1 }
                });
        }
    }
}
