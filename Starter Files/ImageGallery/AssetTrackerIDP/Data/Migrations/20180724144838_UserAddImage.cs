using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetTrackerIDP.Data
{
    public partial class UserAddImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "UserImage",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserImage",
                table: "AspNetUsers");
        }
    }
}
