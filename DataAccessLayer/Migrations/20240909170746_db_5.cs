using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("054236bb-75b7-46c0-a294-75d333fd053e"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("0f3acee9-05e1-4455-8397-b5862d418c7a"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("addf77ba-63fd-4e3c-9b34-f8b808e47cd1"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("d918bb68-58a7-4f42-a1f9-4e49101b3f3e"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("f0a1d40b-0246-4c44-b646-6e57c685a978"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("3aa8ef1c-e3ac-4238-9829-9f4baf9f4255"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("3f637a38-28ee-40ff-bad4-b4e66a934a2a"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("737fd23e-5a4a-421a-a13d-9c569ff3cf51"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("ac5bf5f6-8a92-4b45-9ce1-5678cb5d57a9"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("e5034648-1231-4a05-a12f-f97d27612141"));

            migrationBuilder.DropColumn(
                name: "BillOfLadingCode",
                table: "Order");

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1925fee1-058d-41a3-b6cd-74f54237aa5e"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1434), null, null, "", null, null, "Green", 1 },
                    { new Guid("66e4b917-9eed-4e12-97d1-ecaf1f3b1680"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1411), null, null, "", null, null, "White", 1 },
                    { new Guid("8e20a739-9a94-4994-9f50-d6b5e4a3bd87"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1432), null, null, "", null, null, "Blue", 1 },
                    { new Guid("98e9f851-89a5-4f4e-9ac5-306c187a72ef"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1430), null, null, "", null, null, "Red", 1 },
                    { new Guid("c926f3fd-7872-4cee-8a10-6bc0a5790bc3"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1427), null, null, "", null, null, "Black", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1d355f61-b5a0-4576-bd48-a902a6858baf"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1608), null, null, "", null, null, "XL", 1 },
                    { new Guid("457c7eb8-a2b2-412c-b2ca-01150e4ccb6d"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1597), null, null, "", null, null, "S", 1 },
                    { new Guid("50e9ed1b-fbe7-4f1e-a563-dea08c90bc72"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1595), null, null, "", null, null, "XS", 1 },
                    { new Guid("c2c78799-c74d-4d11-b2c3-d2fe988f8a5f"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1601), null, null, "", null, null, "L", 1 },
                    { new Guid("f68d2b79-43b2-44bd-ab87-4d94f0209fc6"), "", new DateTime(2024, 9, 10, 0, 7, 46, 499, DateTimeKind.Local).AddTicks(1599), null, null, "", null, null, "M", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1925fee1-058d-41a3-b6cd-74f54237aa5e"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("66e4b917-9eed-4e12-97d1-ecaf1f3b1680"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("8e20a739-9a94-4994-9f50-d6b5e4a3bd87"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("98e9f851-89a5-4f4e-9ac5-306c187a72ef"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("c926f3fd-7872-4cee-8a10-6bc0a5790bc3"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("1d355f61-b5a0-4576-bd48-a902a6858baf"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("457c7eb8-a2b2-412c-b2ca-01150e4ccb6d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("50e9ed1b-fbe7-4f1e-a563-dea08c90bc72"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("c2c78799-c74d-4d11-b2c3-d2fe988f8a5f"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("f68d2b79-43b2-44bd-ab87-4d94f0209fc6"));

            migrationBuilder.AddColumn<string>(
                name: "BillOfLadingCode",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("054236bb-75b7-46c0-a294-75d333fd053e"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(848), null, null, "", null, null, "Red", 1 },
                    { new Guid("0f3acee9-05e1-4455-8397-b5862d418c7a"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(846), null, null, "", null, null, "Black", 1 },
                    { new Guid("addf77ba-63fd-4e3c-9b34-f8b808e47cd1"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(850), null, null, "", null, null, "Blue", 1 },
                    { new Guid("d918bb68-58a7-4f42-a1f9-4e49101b3f3e"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(852), null, null, "", null, null, "Green", 1 },
                    { new Guid("f0a1d40b-0246-4c44-b646-6e57c685a978"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(831), null, null, "", null, null, "White", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("3aa8ef1c-e3ac-4238-9829-9f4baf9f4255"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(1036), null, null, "", null, null, "XL", 1 },
                    { new Guid("3f637a38-28ee-40ff-bad4-b4e66a934a2a"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(1029), null, null, "", null, null, "S", 1 },
                    { new Guid("737fd23e-5a4a-421a-a13d-9c569ff3cf51"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(1027), null, null, "", null, null, "XS", 1 },
                    { new Guid("ac5bf5f6-8a92-4b45-9ce1-5678cb5d57a9"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(1030), null, null, "", null, null, "M", 1 },
                    { new Guid("e5034648-1231-4a05-a12f-f97d27612141"), "", new DateTime(2024, 9, 9, 23, 22, 44, 68, DateTimeKind.Local).AddTicks(1034), null, null, "", null, null, "L", 1 }
                });
        }
    }
}
