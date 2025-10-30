using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simple_ATM.Migrations
{
    /// <inheritdoc />
    public partial class CardPinIsNowNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CardPin",
                table: "Users",
                type: "TEXT",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CardPin",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 4);
        }
    }
}
