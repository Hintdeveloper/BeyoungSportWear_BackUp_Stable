using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("64b208a5-1346-42bd-a9f5-e50157d9fe34"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("64fbc4c5-0c06-455f-9ce9-09efeb6d7c2c"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("83e9f982-562d-4271-bda1-3010f95c06ea"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("bd332fb4-6a72-41f9-b102-863cebb75c71"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ee5b5400-45af-48f6-a199-7a8d0d0e8679"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("33cf93a6-411a-48fb-9972-6648d55f4be9"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("4193f484-d151-484b-aa9a-86a30fd7de02"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("59f86a65-f9f4-4587-87d9-7cc284a4942f"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("74b2b2b6-4edd-40b1-a730-2d6b55d79e80"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("a812a124-7160-442f-8a87-32b386dffbf7"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("64b208a5-1346-42bd-a9f5-e50157d9fe34"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9262), null, null, "", null, null, "Green", 1 },
                    { new Guid("64fbc4c5-0c06-455f-9ce9-09efeb6d7c2c"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9257), null, null, "", null, null, "Red", 1 },
                    { new Guid("83e9f982-562d-4271-bda1-3010f95c06ea"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9255), null, null, "", null, null, "Black", 1 },
                    { new Guid("bd332fb4-6a72-41f9-b102-863cebb75c71"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9259), null, null, "", null, null, "Blue", 1 },
                    { new Guid("ee5b5400-45af-48f6-a199-7a8d0d0e8679"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9232), null, null, "", null, null, "White", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("33cf93a6-411a-48fb-9972-6648d55f4be9"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9597), null, null, "", null, null, "M", 1 },
                    { new Guid("4193f484-d151-484b-aa9a-86a30fd7de02"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9589), null, null, "", null, null, "S", 1 },
                    { new Guid("59f86a65-f9f4-4587-87d9-7cc284a4942f"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9599), null, null, "", null, null, "L", 1 },
                    { new Guid("74b2b2b6-4edd-40b1-a730-2d6b55d79e80"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9586), null, null, "", null, null, "XS", 1 },
                    { new Guid("a812a124-7160-442f-8a87-32b386dffbf7"), "", new DateTime(2024, 7, 11, 18, 51, 41, 887, DateTimeKind.Local).AddTicks(9602), null, null, "", null, null, "XL", 1 }
                });
        }
    }
}
