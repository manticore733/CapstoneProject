using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APCapstoneProject.Migrations
{
    /// <inheritdoc />
    public partial class addedBankRemark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankRemark",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankRemark",
                table: "Transactions");
        }
    }
}
