using Microsoft.EntityFrameworkCore.Migrations;

namespace AssetTrackerIDP.Data
{
    public partial class UserEnableLockout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "AspNetUsers");
        }
    }
}
