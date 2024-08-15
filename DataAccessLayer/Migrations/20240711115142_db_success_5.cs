using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2bd71676-c8d7-48ec-9bd7-df52db146618");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ab81ed2e-3358-44df-b3da-d16091e3131f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6f01f4a4-c64e-4c29-8923-fcfa39e94630");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1390cde8-b1bb-431f-8fe2-d0ba6355efa4"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("140ed46a-eda9-43b4-9178-60079212cf79"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9051ccbf-08ae-4714-8706-db4c014c8d55"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("b07bb846-5143-4fc6-99d8-8b89760bc93d"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("e76bb74b-eaff-4751-bb55-dd44dcca54f5"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("0b8b6da6-38f7-4bc0-a776-1439eaf87c41"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("4069a60c-0cfb-41c7-81d3-2eeb8ccaa04c"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("715a6392-e348-4c83-8e86-b5138eac1e61"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("8aab1550-e062-470a-b920-c0dfa1821b71"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("f44d9902-0a92-4556-9fc7-ce2a99126f87"));

            migrationBuilder.AlterColumn<string>(
                name: "SpecificAddress",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DistrictCounty",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Commune",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "SpecificAddress",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DistrictCounty",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Commune",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2bd71676-c8d7-48ec-9bd7-df52db146618", null, "Admin", "Admin" },
                    { "ab81ed2e-3358-44df-b3da-d16091e3131f", null, "Client", "Client" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6f01f4a4-c64e-4c29-8923-fcfa39e94630", 0, "673c65eb-2e38-4c0c-8032-821ab6776c89", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEJbcv/g0upAAcZX07e+fEYUkUOhQK+Y0Akd/J53G3PjhYtywzRogRBIl4RK2TkJZag==", null, false, "3c948df0-3e4c-4a3f-b086-6660e5574595", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1390cde8-b1bb-431f-8fe2-d0ba6355efa4"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9127), null, null, "", null, null, "White", 1 },
                    { new Guid("140ed46a-eda9-43b4-9178-60079212cf79"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9148), null, null, "", null, null, "Blue", 1 },
                    { new Guid("9051ccbf-08ae-4714-8706-db4c014c8d55"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9143), null, null, "", null, null, "Black", 1 },
                    { new Guid("b07bb846-5143-4fc6-99d8-8b89760bc93d"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9146), null, null, "", null, null, "Red", 1 },
                    { new Guid("e76bb74b-eaff-4751-bb55-dd44dcca54f5"), "", new DateTime(2024, 7, 11, 8, 4, 9, 844, DateTimeKind.Local).AddTicks(9150), null, null, "", null, null, "Green", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("0b8b6da6-38f7-4bc0-a776-1439eaf87c41"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2853), null, null, "", null, null, "S", 1 },
                    { new Guid("4069a60c-0cfb-41c7-81d3-2eeb8ccaa04c"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2865), null, null, "", null, null, "XL", 1 },
                    { new Guid("715a6392-e348-4c83-8e86-b5138eac1e61"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2830), null, null, "", null, null, "XS", 1 },
                    { new Guid("8aab1550-e062-470a-b920-c0dfa1821b71"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2857), null, null, "", null, null, "L", 1 },
                    { new Guid("f44d9902-0a92-4556-9fc7-ce2a99126f87"), "", new DateTime(2024, 7, 11, 8, 4, 9, 955, DateTimeKind.Local).AddTicks(2855), null, null, "", null, null, "M", 1 }
                });
        }
    }
}
