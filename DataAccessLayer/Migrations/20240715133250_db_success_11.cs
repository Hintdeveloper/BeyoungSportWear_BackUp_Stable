using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("126c42ee-5468-458c-bd70-0a139157de31"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4434), null, null, "", null, null, "Blue", 1 },
                    { new Guid("35d2f393-1061-4b60-a750-bf260bd51cfd"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4419), null, null, "", null, null, "Red", 1 },
                    { new Guid("9088f8f0-5d94-4f10-a367-4b74302e0ea2"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4436), null, null, "", null, null, "Green", 1 },
                    { new Guid("a78ba250-65b0-476e-8a9c-045aa1b15992"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4400), null, null, "", null, null, "White", 1 },
                    { new Guid("bf98a0cc-35b9-4db1-8695-0e8ea1d6f042"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4417), null, null, "", null, null, "Black", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("043652df-3c67-4575-9758-0af06392ce1f"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4652), null, null, "", null, null, "S", 1 },
                    { new Guid("05a65d39-cb1c-4678-93ca-f427ebe003e1"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4653), null, null, "", null, null, "M", 1 },
                    { new Guid("2b07b913-b46d-427d-901a-1703acd61ccf"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4649), null, null, "", null, null, "XS", 1 },
                    { new Guid("a3a9b744-e30e-44d9-b897-c2e4d03ca4f2"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4657), null, null, "", null, null, "XL", 1 },
                    { new Guid("aff8fa74-60e0-4434-b57e-f07d85784c4a"), "", new DateTime(2024, 7, 15, 20, 32, 50, 388, DateTimeKind.Local).AddTicks(4656), null, null, "", null, null, "L", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("126c42ee-5468-458c-bd70-0a139157de31"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("35d2f393-1061-4b60-a750-bf260bd51cfd"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9088f8f0-5d94-4f10-a367-4b74302e0ea2"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("a78ba250-65b0-476e-8a9c-045aa1b15992"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("bf98a0cc-35b9-4db1-8695-0e8ea1d6f042"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("043652df-3c67-4575-9758-0af06392ce1f"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("05a65d39-cb1c-4678-93ca-f427ebe003e1"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("2b07b913-b46d-427d-901a-1703acd61ccf"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("a3a9b744-e30e-44d9-b897-c2e4d03ca4f2"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("aff8fa74-60e0-4434-b57e-f07d85784c4a"));

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
    }
}
