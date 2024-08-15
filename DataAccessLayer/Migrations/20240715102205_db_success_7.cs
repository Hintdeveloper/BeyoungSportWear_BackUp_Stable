using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("58f09de6-5e76-4c24-b839-bba669a790f0"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("691fcd3a-8f0c-475d-a3e1-74fe16ee7856"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9335adb7-ca87-4b81-be65-ec08d6e597d1"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("b9e68e02-ecd0-4a0a-8e66-5a9610fdec19"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("e8929ea0-40ce-4545-9f85-6356497c698b"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("7e59728f-3fed-4ab3-b8a9-e27dbc07d01e"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("9500c568-f9b1-43bc-8d47-90364ace609a"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("a6bc016f-c014-4756-9b36-b688dba5ae1d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("bbb546cf-80bb-4d87-b841-ca4ddee884b0"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("c0fece64-b779-48e7-adb1-7a522417e8af"));

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Gmail",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Website",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gmail",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
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
                    { new Guid("58f09de6-5e76-4c24-b839-bba669a790f0"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7687), null, null, "", null, null, "Green", 1 },
                    { new Guid("691fcd3a-8f0c-475d-a3e1-74fe16ee7856"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7666), null, null, "", null, null, "Red", 1 },
                    { new Guid("9335adb7-ca87-4b81-be65-ec08d6e597d1"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7668), null, null, "", null, null, "Blue", 1 },
                    { new Guid("b9e68e02-ecd0-4a0a-8e66-5a9610fdec19"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7663), null, null, "", null, null, "Black", 1 },
                    { new Guid("e8929ea0-40ce-4545-9f85-6356497c698b"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7644), null, null, "", null, null, "White", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("7e59728f-3fed-4ab3-b8a9-e27dbc07d01e"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7912), null, null, "", null, null, "S", 1 },
                    { new Guid("9500c568-f9b1-43bc-8d47-90364ace609a"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7909), null, null, "", null, null, "XS", 1 },
                    { new Guid("a6bc016f-c014-4756-9b36-b688dba5ae1d"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7918), null, null, "", null, null, "XL", 1 },
                    { new Guid("bbb546cf-80bb-4d87-b841-ca4ddee884b0"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7914), null, null, "", null, null, "M", 1 },
                    { new Guid("c0fece64-b779-48e7-adb1-7a522417e8af"), "", new DateTime(2024, 7, 11, 23, 26, 50, 118, DateTimeKind.Local).AddTicks(7916), null, null, "", null, null, "L", 1 }
                });
        }
    }
}
