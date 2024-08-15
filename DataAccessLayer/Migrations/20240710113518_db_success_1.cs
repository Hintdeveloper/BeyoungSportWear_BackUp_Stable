using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class db_success_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8fcc5875-77e8-4c52-b69f-e716b63cf9b0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ddcb0f2b-3541-4af9-a1d2-1af905e50d6d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6c45835d-5a35-4e5d-9e73-74dfcb63366c");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("0516c173-fbc3-4a83-b172-2a793657851a"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1b3f6ad2-b4fa-4d94-b301-b8925c1b5df0"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("351b3f08-397f-4dd6-90cd-0734840ea872"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("a318d44d-88ac-4338-b758-fb2c6dbbd8d2"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("f2a93445-c59e-4d83-9d6b-cf0d46e199b5"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("3b07c3fb-ec34-4526-991f-265f08bda064"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("469fe9c1-e4eb-4128-a4db-3899aadb1539"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("4705a291-00ab-4cd8-9289-0f536a4982a6"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("5000411f-a740-428f-a56e-9835b13cdb9a"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("abb60e1d-1df7-438d-95f8-215a22904899"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Options",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0d1f74b2-a31e-4e62-a39d-e843daede0ed", null, "Client", "Client" },
                    { "1e0fe679-b074-44ce-9860-284e09ca4eaa", null, "Admin", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "964f8eff-eef7-4c48-8197-e244f7ca6e81", 0, "ac6baede-e69d-4c5b-90ad-2dcab102eec9", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAENLAmKDZ8znHal3LHJIwPlrD+BJsmInSFN3mOalH04X46C3PxA5wlcmrg3Ex9BNlXg==", null, false, "62d506af-4de0-4dfc-98ca-04410ded4783", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("1c0e12f8-b6d4-4616-8667-ca51f29477e9"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1351), null, null, "", null, null, "White", 1 },
                    { new Guid("38ac12cb-4980-4613-b7eb-131a8721cb6e"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1386), null, null, "", null, null, "Red", 1 },
                    { new Guid("979ae9be-b2cb-4717-b335-7bc14f194bf2"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1388), null, null, "", null, null, "Blue", 1 },
                    { new Guid("9cdf8ae4-4dcd-4856-a731-7f403d8dbfe2"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1384), null, null, "", null, null, "Black", 1 },
                    { new Guid("ec3d7d90-5b8c-4e51-979d-f417ed937834"), "", new DateTime(2024, 7, 10, 18, 35, 18, 326, DateTimeKind.Local).AddTicks(1390), null, null, "", null, null, "Green", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("104e3493-aeac-4f20-a4ad-9edee0f486ee"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4314), null, null, "", null, null, "S", 1 },
                    { new Guid("650170f8-849e-46b7-8162-c8478f325b72"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4331), null, null, "", null, null, "XL", 1 },
                    { new Guid("9861c859-8a6b-46cd-98ae-0e9214638b2b"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4316), null, null, "", null, null, "M", 1 },
                    { new Guid("a0d987c5-5a72-4676-9f2c-f7a1d6e4d125"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4290), null, null, "", null, null, "XS", 1 },
                    { new Guid("df3464e4-6c23-4ad6-86e0-1c7155071d4f"), "", new DateTime(2024, 7, 10, 18, 35, 18, 398, DateTimeKind.Local).AddTicks(4318), null, null, "", null, null, "L", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0d1f74b2-a31e-4e62-a39d-e843daede0ed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1e0fe679-b074-44ce-9860-284e09ca4eaa");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "964f8eff-eef7-4c48-8197-e244f7ca6e81");

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("1c0e12f8-b6d4-4616-8667-ca51f29477e9"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("38ac12cb-4980-4613-b7eb-131a8721cb6e"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("979ae9be-b2cb-4717-b335-7bc14f194bf2"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("9cdf8ae4-4dcd-4856-a731-7f403d8dbfe2"));

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "ID",
                keyValue: new Guid("ec3d7d90-5b8c-4e51-979d-f417ed937834"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("104e3493-aeac-4f20-a4ad-9edee0f486ee"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("650170f8-849e-46b7-8162-c8478f325b72"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("9861c859-8a6b-46cd-98ae-0e9214638b2b"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("a0d987c5-5a72-4676-9f2c-f7a1d6e4d125"));

            migrationBuilder.DeleteData(
                table: "Sizes",
                keyColumn: "ID",
                keyValue: new Guid("df3464e4-6c23-4ad6-86e0-1c7155071d4f"));

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Options");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8fcc5875-77e8-4c52-b69f-e716b63cf9b0", null, "Client", "Client" },
                    { "ddcb0f2b-3541-4af9-a1d2-1af905e50d6d", null, "Admin", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6c45835d-5a35-4e5d-9e73-74dfcb63366c", 0, "6164fb31-3218-4da8-8616-ffccb3a236b1", "IdentityUser", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin", "AQAAAAIAAYagAAAAEEaU44ENVgar03nPLXrNTYlt0Hbo6sVBGG6cB5iiKtcpB3z5pELEsFnCF1XiytQ2ow==", null, false, "83ba721a-d923-49c8-9a2d-145dbae29384", false, "admin" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("0516c173-fbc3-4a83-b172-2a793657851a"), "", new DateTime(2024, 7, 10, 17, 41, 52, 627, DateTimeKind.Local).AddTicks(4313), null, null, "", null, null, "Blue", 1 },
                    { new Guid("1b3f6ad2-b4fa-4d94-b301-b8925c1b5df0"), "", new DateTime(2024, 7, 10, 17, 41, 52, 627, DateTimeKind.Local).AddTicks(4309), null, null, "", null, null, "Black", 1 },
                    { new Guid("351b3f08-397f-4dd6-90cd-0734840ea872"), "", new DateTime(2024, 7, 10, 17, 41, 52, 627, DateTimeKind.Local).AddTicks(4311), null, null, "", null, null, "Red", 1 },
                    { new Guid("a318d44d-88ac-4338-b758-fb2c6dbbd8d2"), "", new DateTime(2024, 7, 10, 17, 41, 52, 627, DateTimeKind.Local).AddTicks(4316), null, null, "", null, null, "Green", 1 },
                    { new Guid("f2a93445-c59e-4d83-9d6b-cf0d46e199b5"), "", new DateTime(2024, 7, 10, 17, 41, 52, 627, DateTimeKind.Local).AddTicks(4294), null, null, "", null, null, "White", 1 }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "ID", "CreateBy", "CreateDate", "DeleteBy", "DeleteDate", "Description", "ModifiedBy", "ModifiedDate", "Name", "Status" },
                values: new object[,]
                {
                    { new Guid("3b07c3fb-ec34-4526-991f-265f08bda064"), "", new DateTime(2024, 7, 10, 17, 41, 52, 694, DateTimeKind.Local).AddTicks(6007), null, null, "", null, null, "XL", 1 },
                    { new Guid("469fe9c1-e4eb-4128-a4db-3899aadb1539"), "", new DateTime(2024, 7, 10, 17, 41, 52, 694, DateTimeKind.Local).AddTicks(5995), null, null, "", null, null, "S", 1 },
                    { new Guid("4705a291-00ab-4cd8-9289-0f536a4982a6"), "", new DateTime(2024, 7, 10, 17, 41, 52, 694, DateTimeKind.Local).AddTicks(6005), null, null, "", null, null, "L", 1 },
                    { new Guid("5000411f-a740-428f-a56e-9835b13cdb9a"), "", new DateTime(2024, 7, 10, 17, 41, 52, 694, DateTimeKind.Local).AddTicks(6003), null, null, "", null, null, "M", 1 },
                    { new Guid("abb60e1d-1df7-438d-95f8-215a22904899"), "", new DateTime(2024, 7, 10, 17, 41, 52, 694, DateTimeKind.Local).AddTicks(5977), null, null, "", null, null, "XS", 1 }
                });
        }
    }
}
