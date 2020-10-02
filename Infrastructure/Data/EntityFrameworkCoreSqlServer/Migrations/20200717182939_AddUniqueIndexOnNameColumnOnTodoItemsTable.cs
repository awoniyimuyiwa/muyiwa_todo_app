using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer.Migrations
{
    public partial class AddUniqueIndexOnNameColumnOnTodoItemsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TodoItems",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_Name",
                table: "TodoItems",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_Name",
                table: "TodoItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
