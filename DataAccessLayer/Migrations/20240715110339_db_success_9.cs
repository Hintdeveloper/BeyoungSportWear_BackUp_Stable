using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("0530e7e6-7afb-4029-8789-98489c143e71"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4036), null, null, "", null, null, "White", 1 },
                    { new Guid("902a1ae7-4878-4f54-840d-2cdf464c4075"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4056), null, null, "", null, null, "Blue", 1 },
                    { new Guid("a68f599d-9ad2-4bec-8ac8-c1e295d3675b"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4058), null, null, "", null, null, "Green", 1 },
                    { new Guid("e580acad-c550-4c3e-a13e-6c8ddc2ddb25"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4053), null, null, "", null, null, "Black", 1 },
                    { new Guid("ef2e44aa-1e36-4422-bf31-df7f1a49a0c6"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4055), null, null, "", null, null, "Red", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("0b418c54-3342-4e4e-a369-772742919002"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4241), null, null, "", null, null, "M", 1 },
                    { new Guid("47910bf5-2a04-427b-9ba7-7adabf4c53aa"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4244), null, null, "", null, null, "XL", 1 },
                    { new Guid("5a90633b-0dba-4291-ab84-6151c66b3c10"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4243), null, null, "", null, null, "L", 1 },
                    { new Guid("786c41aa-6842-4552-82fe-95e18feb37bf"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4239), null, null, "", null, null, "S", 1 },
                    { new Guid("da04f285-a1da-4a93-b441-903f2d3ff532"), "", new DateTime(2024, 7, 15, 18, 3, 38, 827, DateTimeKind.Local).AddTicks(4236), null, null, "", null, null, "XS", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("0530e7e6-7afb-4029-8789-98489c143e71"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("902a1ae7-4878-4f54-840d-2cdf464c4075"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("a68f599d-9ad2-4bec-8ac8-c1e295d3675b"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("e580acad-c550-4c3e-a13e-6c8ddc2ddb25"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ef2e44aa-1e36-4422-bf31-df7f1a49a0c6"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("0b418c54-3342-4e4e-a369-772742919002"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("47910bf5-2a04-427b-9ba7-7adabf4c53aa"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("5a90633b-0dba-4291-ab84-6151c66b3c10"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("786c41aa-6842-4552-82fe-95e18feb37bf"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("da04f285-a1da-4a93-b441-903f2d3ff532"));

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
    }
}
