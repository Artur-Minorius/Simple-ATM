using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple_ATM.Migrations
{
    /// <inheritdoc />
    public partial class UserCardAmountNowCalculated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardAmount",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CardAmount",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
