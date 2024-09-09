using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("32940775-50ab-4634-bb3e-6612d43ff33a"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5128), null, null, "", null, null, "Green", 1 },
                    { new Guid("5526c8a4-d6a2-488f-bdbc-bd16d3e6cfce"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5126), null, null, "", null, null, "Blue", 1 },
                    { new Guid("a4c2b98a-f047-40d1-9b42-c3ec8ad14e4f"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5109), null, null, "", null, null, "Black", 1 },
                    { new Guid("e307daf3-1ef3-4e00-b8a6-9215df6c13c1"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5123), null, null, "", null, null, "Red", 1 },
                    { new Guid("ffd04e06-c2d2-44fe-b8fc-42c799329540"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5093), null, null, "", null, null, "White", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("20fd0478-197a-4d16-9150-a74b0b3fece1"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5353), null, null, "", null, null, "M", 1 },
                    { new Guid("22022e15-01b6-435c-82ad-f938c3d2c452"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5355), null, null, "", null, null, "L", 1 },
                    { new Guid("2a575096-2e6c-4107-aa58-b039605190a3"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5351), null, null, "", null, null, "S", 1 },
                    { new Guid("400dc2ba-8d34-4eac-aeb3-b124ba857a79"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5356), null, null, "", null, null, "XL", 1 },
                    { new Guid("c90d5a97-9fb3-49e1-a0a5-2e63ce31c036"), "", new DateTime(2024, 9, 10, 0, 11, 41, 846, DateTimeKind.Local).AddTicks(5349), null, null, "", null, null, "XS", 1 }
                });

            migrationBuilder.AddColumn<string>(
                name: "BillOfLadingCode",
                table: "OrderHistory",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("32940775-50ab-4634-bb3e-6612d43ff33a"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("5526c8a4-d6a2-488f-bdbc-bd16d3e6cfce"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("a4c2b98a-f047-40d1-9b42-c3ec8ad14e4f"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("e307daf3-1ef3-4e00-b8a6-9215df6c13c1"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ffd04e06-c2d2-44fe-b8fc-42c799329540"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("20fd0478-197a-4d16-9150-a74b0b3fece1"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("22022e15-01b6-435c-82ad-f938c3d2c452"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("2a575096-2e6c-4107-aa58-b039605190a3"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("400dc2ba-8d34-4eac-aeb3-b124ba857a79"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("c90d5a97-9fb3-49e1-a0a5-2e63ce31c036"));

            migrationBuilder.DropColumn(
                name: "BillOfLadingCode",
                table: "OrderHistory");

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
    }
}
