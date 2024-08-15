using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("0cfb9f0c-91ad-4973-a673-3c6c60b2ab40"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("33ec58ef-a200-498b-a22d-c7e6a2be27fa"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("8b102a9a-2ecc-4fd4-872e-720dd11736eb"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ee65c063-960c-4d5d-994b-70ccca3cbbb2"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ee81a200-0532-4ace-a3a3-39be420bfe52"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("1955a5c8-066d-4879-b198-7f31f9dc6b52"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("1bdc9ff7-f8f8-403e-a5b3-a0f7beff7ee6"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("299441cb-091b-4729-9d6b-a1067644939d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("abfc2ce5-0982-4cc1-b963-9060e9593ac5"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("e2194e36-0d44-44e6-b19c-3cc3b7752b32"));

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Sizes",
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
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("2ef62fd3-7d62-4378-b3ad-fa4de9c3a25d"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(722), null, null, "", null, null, "Red", 1 },
                    { new Guid("48ee16ed-22a7-475f-abfd-1234f2b98979"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(720), null, null, "", null, null, "Black", 1 },
                    { new Guid("85ad9a2e-80b5-4224-b0f4-2d85a733a800"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(726), null, null, "", null, null, "Green", 1 },
                    { new Guid("dc1b6c56-09a1-4928-9333-60f3e90894ec"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(685), null, null, "", null, null, "White", 1 },
                    { new Guid("f2b6f63c-6618-4d6a-83e5-4c74236e68e4"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(724), null, null, "", null, null, "Blue", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("0ec36ef1-a167-4b6d-a5fc-d5d7720e2c2e"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(887), null, null, "", null, null, "S", 1 },
                    { new Guid("15e16259-7427-4fb4-92ed-973b2d32b7ab"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(892), null, null, "", null, null, "L", 1 },
                    { new Guid("6f1b0562-3684-465b-b856-b0de399709ea"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(885), null, null, "", null, null, "XS", 1 },
                    { new Guid("767c7ea9-177a-40b5-9238-62924c3e9035"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(896), null, null, "", null, null, "XL", 1 },
                    { new Guid("8704a78a-61fe-4f25-9540-47ca98f23b2c"), "", new DateTime(2024, 7, 15, 17, 51, 44, 830, DateTimeKind.Local).AddTicks(889), null, null, "", null, null, "M", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("2ef62fd3-7d62-4378-b3ad-fa4de9c3a25d"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("48ee16ed-22a7-475f-abfd-1234f2b98979"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("85ad9a2e-80b5-4224-b0f4-2d85a733a800"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("dc1b6c56-09a1-4928-9333-60f3e90894ec"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("f2b6f63c-6618-4d6a-83e5-4c74236e68e4"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("0ec36ef1-a167-4b6d-a5fc-d5d7720e2c2e"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("15e16259-7427-4fb4-92ed-973b2d32b7ab"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("6f1b0562-3684-465b-b856-b0de399709ea"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("767c7ea9-177a-40b5-9238-62924c3e9035"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("8704a78a-61fe-4f25-9540-47ca98f23b2c"));

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Sizes",
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
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("0cfb9f0c-91ad-4973-a673-3c6c60b2ab40"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1435), null, null, "", null, null, "Blue", 1 },
                    { new Guid("33ec58ef-a200-498b-a22d-c7e6a2be27fa"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1416), null, null, "", null, null, "Black", 1 },
                    { new Guid("8b102a9a-2ecc-4fd4-872e-720dd11736eb"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1418), null, null, "", null, null, "Red", 1 },
                    { new Guid("ee65c063-960c-4d5d-994b-70ccca3cbbb2"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1395), null, null, "", null, null, "White", 1 },
                    { new Guid("ee81a200-0532-4ace-a3a3-39be420bfe52"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1436), null, null, "", null, null, "Green", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1955a5c8-066d-4879-b198-7f31f9dc6b52"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1613), null, null, "", null, null, "S", 1 },
                    { new Guid("1bdc9ff7-f8f8-403e-a5b3-a0f7beff7ee6"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1616), null, null, "", null, null, "L", 1 },
                    { new Guid("299441cb-091b-4729-9d6b-a1067644939d"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1614), null, null, "", null, null, "M", 1 },
                    { new Guid("abfc2ce5-0982-4cc1-b963-9060e9593ac5"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1611), null, null, "", null, null, "XS", 1 },
                    { new Guid("e2194e36-0d44-44e6-b19c-3cc3b7752b32"), "", new DateTime(2024, 7, 15, 17, 22, 5, 242, DateTimeKind.Local).AddTicks(1618), null, null, "", null, null, "XL", 1 }
                });
        }
    }
}
