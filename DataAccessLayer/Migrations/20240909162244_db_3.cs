using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "164e6801-6d74-4784-81e3-e4e4759501da");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1b93b065-d4a8-443b-b99b-080750cb43fc"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("370fd0f4-f34f-4ab0-a8c3-b8e86061e598"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("8bc6f175-864c-4d78-8bc3-87f9590310c4"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("94d3a567-f0aa-4b91-944f-4ec2ac9213af"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("d496df53-4fe2-459a-8577-781ac9777f0d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("3be50a11-8dc2-465a-91a4-bda10495c65d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("51d6d5b4-5f85-4384-b74a-01e913635ad3"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("a1c3935a-e533-4551-8021-eb7410eeb969"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("d1ee37ae-72f5-4e4e-b002-2fb1609ee1b0"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("fc4dac15-f1ff-49dc-866b-30f87125bc03"));

            migrationBuilder.AlterColumn<string>(
                name: "BillOfLadingCode",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "BillOfLadingCode",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "164e6801-6d74-4784-81e3-e4e4759501da", 0, "f7a3c25d-e347-41b2-bb1c-e0c8c88bf3d6", "IdentityUser", "admin@gmail.com", true, false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEN/TWxhLYmYWbXjVzCw3mXAYyUxzu0Aao4vL4tzf+BH7GTVuLBuo8roVdXBWa36TSw==", null, false, "ede7e57c-19c4-405e-8aad-85176577db8b", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1b93b065-d4a8-443b-b99b-080750cb43fc"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(1827), null, null, "", null, null, "Black", 1 },
                    { new Guid("370fd0f4-f34f-4ab0-a8c3-b8e86061e598"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(1831), null, null, "", null, null, "Blue", 1 },
                    { new Guid("8bc6f175-864c-4d78-8bc3-87f9590310c4"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(1808), null, null, "", null, null, "White", 1 },
                    { new Guid("94d3a567-f0aa-4b91-944f-4ec2ac9213af"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(1830), null, null, "", null, null, "Red", 1 },
                    { new Guid("d496df53-4fe2-459a-8577-781ac9777f0d"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(1833), null, null, "", null, null, "Green", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("3be50a11-8dc2-465a-91a4-bda10495c65d"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(2006), null, null, "", null, null, "S", 1 },
                    { new Guid("51d6d5b4-5f85-4384-b74a-01e913635ad3"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(2013), null, null, "", null, null, "XL", 1 },
                    { new Guid("a1c3935a-e533-4551-8021-eb7410eeb969"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(2012), null, null, "", null, null, "L", 1 },
                    { new Guid("d1ee37ae-72f5-4e4e-b002-2fb1609ee1b0"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(2004), null, null, "", null, null, "XS", 1 },
                    { new Guid("fc4dac15-f1ff-49dc-866b-30f87125bc03"), "", new DateTime(2024, 9, 9, 21, 59, 14, 414, DateTimeKind.Local).AddTicks(2008), null, null, "", null, null, "M", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }
    }
}
