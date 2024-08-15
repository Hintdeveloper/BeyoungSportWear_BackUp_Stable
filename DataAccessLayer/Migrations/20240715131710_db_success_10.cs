using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { new Guid("8be90924-8249-4a94-841a-bba1d1c11b7f"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9331), null, null, "", null, null, "Red", 1 },
                    { new Guid("b0f4c077-aca3-44fb-8d37-1c2b1a0eaf59"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9328), null, null, "", null, null, "Black", 1 },
                    { new Guid("bbad3aa1-bad6-44eb-8110-4147b4ecf76e"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9309), null, null, "", null, null, "White", 1 },
                    { new Guid("ea48f76c-e536-4299-b90c-25d8af2e8a5c"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9352), null, null, "", null, null, "Green", 1 },
                    { new Guid("f59d38b2-91d5-439e-b80f-19c7615ab03e"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9349), null, null, "", null, null, "Blue", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("640c9db7-5ecb-41e8-b7fd-6fe525e93480"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9643), null, null, "", null, null, "XS", 1 },
                    { new Guid("9d418078-93c7-45ee-b302-3d0680d35b0b"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9650), null, null, "", null, null, "L", 1 },
                    { new Guid("d94bec56-cbba-449f-a9d2-ce96e01e6b1d"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9649), null, null, "", null, null, "M", 1 },
                    { new Guid("dacb1271-9086-4777-84bb-1504588afc41"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9653), null, null, "", null, null, "XL", 1 },
                    { new Guid("eaa905e4-dcbe-4710-847b-db5e452297aa"), "", new DateTime(2024, 7, 15, 20, 17, 10, 594, DateTimeKind.Local).AddTicks(9647), null, null, "", null, null, "S", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("8be90924-8249-4a94-841a-bba1d1c11b7f"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("b0f4c077-aca3-44fb-8d37-1c2b1a0eaf59"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("bbad3aa1-bad6-44eb-8110-4147b4ecf76e"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ea48f76c-e536-4299-b90c-25d8af2e8a5c"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("f59d38b2-91d5-439e-b80f-19c7615ab03e"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("640c9db7-5ecb-41e8-b7fd-6fe525e93480"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("9d418078-93c7-45ee-b302-3d0680d35b0b"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("d94bec56-cbba-449f-a9d2-ce96e01e6b1d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("dacb1271-9086-4777-84bb-1504588afc41"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("eaa905e4-dcbe-4710-847b-db5e452297aa"));

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
    }
}
