using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class bswDB_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c3597db3-270b-46db-880a-7b16afc7b150");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dc5c8145-248c-42f4-834d-d0321677cde2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b4cda764-c4b1-405b-a6f0-119ec8db19cc");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("2bf82db7-f60b-40f7-a733-6f841aa83ca1"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("3e02c600-07d9-4ab1-9ee8-802e8a4daa83"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("89b99055-1a08-4acd-9368-3eb25f4a6556"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("d1619b2d-3d30-4ee2-aefd-408473eb9a1d"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ec3eb58f-3aba-47c4-b66a-e0048035ddeb"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("14dd51a5-f6a3-4732-bd80-1e6cf88cc865"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("46b52335-0384-4258-b350-216dde4bd5d0"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("9b6f6d98-6027-43af-819f-06d7ebfb3eb9"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("af0b5fff-b412-4573-99bf-c873220a33b0"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("ee57f50f-8e95-41d2-a27a-b1dcd5c63f3b"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShipDate",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1a5d00a6-2548-4ff2-ae6e-c879cefff04f", null, "Admin", "Admin" },
                    { "36a9e232-f6d9-4a79-a91f-4ed702da8427", null, "Client", "Client" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "93b061ce-e8b6-443d-b9b5-2caa9fcfa954", 0, "4b2a27d6-18a3-43e8-b7f7-24e16e80c51a", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEPNuGHCIAzsWCM2EqTej14tkVKLQU1M7GWhhA87vR35cBkLyQLXW9Jc+87IYcitenQ==", null, false, "741ce30c-a35d-42da-8750-a2347b83ee9b", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("122f1b58-2dd1-4cae-8838-9c480c18ec62"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9742), null, null, "", null, null, "Red", 1 },
                    { new Guid("2b4c2db0-5591-4b4a-b5ae-cd69c6a6088f"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9718), null, null, "", null, null, "White", 1 },
                    { new Guid("306e80c6-ba39-4b0f-83dd-7704aa24138e"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9744), null, null, "", null, null, "Blue", 1 },
                    { new Guid("434a3028-6480-4df7-a6be-2d115dc2e7c4"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9746), null, null, "", null, null, "Green", 1 },
                    { new Guid("5663da6b-f076-439f-bcf8-84d0e3a7bee0"), "", new DateTime(2024, 7, 9, 11, 10, 52, 789, DateTimeKind.Local).AddTicks(9739), null, null, "", null, null, "Black", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("4d1945da-9aeb-46d8-a151-879e88206164"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6796), null, null, "", null, null, "XS", 1 },
                    { new Guid("697c2b8f-9a1b-44ee-8f80-63f831f1b998"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6834), null, null, "", null, null, "XL", 1 },
                    { new Guid("91bdcc1a-ec5d-408b-8249-ba86c4d1922a"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6827), null, null, "", null, null, "S", 1 },
                    { new Guid("baa4abe1-9662-46fd-90aa-14ca8cc74442"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6832), null, null, "", null, null, "L", 1 },
                    { new Guid("d0a5b03b-7833-4929-9cb1-241d43c6b8a0"), "", new DateTime(2024, 7, 9, 11, 10, 52, 842, DateTimeKind.Local).AddTicks(6830), null, null, "", null, null, "M", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a5d00a6-2548-4ff2-ae6e-c879cefff04f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36a9e232-f6d9-4a79-a91f-4ed702da8427");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "93b061ce-e8b6-443d-b9b5-2caa9fcfa954");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("122f1b58-2dd1-4cae-8838-9c480c18ec62"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("2b4c2db0-5591-4b4a-b5ae-cd69c6a6088f"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("306e80c6-ba39-4b0f-83dd-7704aa24138e"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("434a3028-6480-4df7-a6be-2d115dc2e7c4"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("5663da6b-f076-439f-bcf8-84d0e3a7bee0"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("4d1945da-9aeb-46d8-a151-879e88206164"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("697c2b8f-9a1b-44ee-8f80-63f831f1b998"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("91bdcc1a-ec5d-408b-8249-ba86c4d1922a"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("baa4abe1-9662-46fd-90aa-14ca8cc74442"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("d0a5b03b-7833-4929-9cb1-241d43c6b8a0"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShipDate",
                table: "Order",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c3597db3-270b-46db-880a-7b16afc7b150", null, "Client", "Client" },
                    { "dc5c8145-248c-42f4-834d-d0321677cde2", null, "Admin", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b4cda764-c4b1-405b-a6f0-119ec8db19cc", 0, "9b88d660-65d9-4e2b-9d44-ced5ed1b79d9", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAENrOqCmJu6v1p3KWx1ToHqsAtVs3PlAiV2bj1JItf497jLn8AW97PirpmS+QI7oEfQ==", null, false, "a3efb35e-8f3b-457e-97ab-ed4ceab9c4b6", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("2bf82db7-f60b-40f7-a733-6f841aa83ca1"), "", new DateTime(2024, 7, 6, 19, 8, 13, 95, DateTimeKind.Local).AddTicks(5262), null, null, "", null, null, "White", 1 },
                    { new Guid("3e02c600-07d9-4ab1-9ee8-802e8a4daa83"), "", new DateTime(2024, 7, 6, 19, 8, 13, 95, DateTimeKind.Local).AddTicks(5284), null, null, "", null, null, "Red", 1 },
                    { new Guid("89b99055-1a08-4acd-9368-3eb25f4a6556"), "", new DateTime(2024, 7, 6, 19, 8, 13, 95, DateTimeKind.Local).AddTicks(5282), null, null, "", null, null, "Black", 1 },
                    { new Guid("d1619b2d-3d30-4ee2-aefd-408473eb9a1d"), "", new DateTime(2024, 7, 6, 19, 8, 13, 95, DateTimeKind.Local).AddTicks(5309), null, null, "", null, null, "Green", 1 },
                    { new Guid("ec3eb58f-3aba-47c4-b66a-e0048035ddeb"), "", new DateTime(2024, 7, 6, 19, 8, 13, 95, DateTimeKind.Local).AddTicks(5307), null, null, "", null, null, "Blue", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("14dd51a5-f6a3-4732-bd80-1e6cf88cc865"), "", new DateTime(2024, 7, 6, 19, 8, 13, 199, DateTimeKind.Local).AddTicks(3574), null, null, "", null, null, "XL", 1 },
                    { new Guid("46b52335-0384-4258-b350-216dde4bd5d0"), "", new DateTime(2024, 7, 6, 19, 8, 13, 199, DateTimeKind.Local).AddTicks(3519), null, null, "", null, null, "XS", 1 },
                    { new Guid("9b6f6d98-6027-43af-819f-06d7ebfb3eb9"), "", new DateTime(2024, 7, 6, 19, 8, 13, 199, DateTimeKind.Local).AddTicks(3566), null, null, "", null, null, "S", 1 },
                    { new Guid("af0b5fff-b412-4573-99bf-c873220a33b0"), "", new DateTime(2024, 7, 6, 19, 8, 13, 199, DateTimeKind.Local).AddTicks(3571), null, null, "", null, null, "L", 1 },
                    { new Guid("ee57f50f-8e95-41d2-a27a-b1dcd5c63f3b"), "", new DateTime(2024, 7, 6, 19, 8, 13, 199, DateTimeKind.Local).AddTicks(3569), null, null, "", null, null, "M", 1 }
                });
        }
    }
}
