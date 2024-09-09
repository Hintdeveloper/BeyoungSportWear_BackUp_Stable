using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0be40e8d-3f4b-4707-a01f-050f797a9af8");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("01822efe-a344-4aa9-a74c-cc06742ae69f"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("2e4ade72-b296-4a56-a97b-969d423c1a6d"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("3e52dde2-59ba-46e9-9979-27da78c9a150"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("497aa989-4631-4d01-893e-13d177b769c9"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("db74d7aa-6f82-482a-bed1-701a5db639db"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("2e976485-b970-4379-b2a5-9f72cc7b9130"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("32c89139-e8ea-4081-8282-ea96d42da20e"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("73aec868-0a80-44f8-a23a-5d4f25a648f7"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("a6c201de-6554-4b3c-aca9-2a575fa9982d"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("ccabe5ba-2126-412f-9f7d-f316b90a65e2"));

            migrationBuilder.AddColumn<string>(
                name: "BillOfLadingCode",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Gmail",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstAndLastName",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            //migrationBuilder.InsertData(
            //    table: "AspNetUsers",
            //    columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
            //    values: new object[] { "164e6801-6d74-4784-81e3-e4e4759501da", 0, "f7a3c25d-e347-41b2-bb1c-e0c8c88bf3d6", "IdentityUser", "admin@gmail.com", true, false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEN/TWxhLYmYWbXjVzCw3mXAYyUxzu0Aao4vL4tzf+BH7GTVuLBuo8roVdXBWa36TSw==", null, false, "ede7e57c-19c4-405e-8aad-85176577db8b", false, "admin" });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "BillOfLadingCode",
                table: "Order");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gmail",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstAndLastName",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0be40e8d-3f4b-4707-a01f-050f797a9af8", 0, "cdfedf23-6fae-4f89-a37a-454f73b8040a", "IdentityUser", "admin@gmail.com", true, false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAENY6u5Na7oHyospdduqZ5F2Ozd41KH+3xLoFvMXp8mEMbdyBF+J1vcr6GBnJzdcJvQ==", null, false, "5926012e-3326-4788-a91c-466ba6653f03", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("01822efe-a344-4aa9-a74c-cc06742ae69f"), "", new DateTime(2024, 9, 3, 3, 8, 55, 350, DateTimeKind.Local).AddTicks(9876), null, null, "", null, null, "White", 1 },
                    { new Guid("2e4ade72-b296-4a56-a97b-969d423c1a6d"), "", new DateTime(2024, 9, 3, 3, 8, 55, 350, DateTimeKind.Local).AddTicks(9915), null, null, "", null, null, "Black", 1 },
                    { new Guid("3e52dde2-59ba-46e9-9979-27da78c9a150"), "", new DateTime(2024, 9, 3, 3, 8, 55, 350, DateTimeKind.Local).AddTicks(9918), null, null, "", null, null, "Red", 1 },
                    { new Guid("497aa989-4631-4d01-893e-13d177b769c9"), "", new DateTime(2024, 9, 3, 3, 8, 55, 350, DateTimeKind.Local).AddTicks(9920), null, null, "", null, null, "Blue", 1 },
                    { new Guid("db74d7aa-6f82-482a-bed1-701a5db639db"), "", new DateTime(2024, 9, 3, 3, 8, 55, 350, DateTimeKind.Local).AddTicks(9921), null, null, "", null, null, "Green", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("2e976485-b970-4379-b2a5-9f72cc7b9130"), "", new DateTime(2024, 9, 3, 3, 8, 55, 351, DateTimeKind.Local).AddTicks(148), null, null, "", null, null, "XL", 1 },
                    { new Guid("32c89139-e8ea-4081-8282-ea96d42da20e"), "", new DateTime(2024, 9, 3, 3, 8, 55, 351, DateTimeKind.Local).AddTicks(142), null, null, "", null, null, "M", 1 },
                    { new Guid("73aec868-0a80-44f8-a23a-5d4f25a648f7"), "", new DateTime(2024, 9, 3, 3, 8, 55, 351, DateTimeKind.Local).AddTicks(140), null, null, "", null, null, "S", 1 },
                    { new Guid("a6c201de-6554-4b3c-aca9-2a575fa9982d"), "", new DateTime(2024, 9, 3, 3, 8, 55, 351, DateTimeKind.Local).AddTicks(144), null, null, "", null, null, "L", 1 },
                    { new Guid("ccabe5ba-2126-412f-9f7d-f316b90a65e2"), "", new DateTime(2024, 9, 3, 3, 8, 55, 351, DateTimeKind.Local).AddTicks(137), null, null, "", null, null, "XS", 1 }
                });
        }
    }
}
