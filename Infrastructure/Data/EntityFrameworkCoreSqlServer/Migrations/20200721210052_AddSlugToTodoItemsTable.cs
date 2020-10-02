using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.EntityFrameworkCoreSqlServer.Migrations
{
    public partial class AddSlugToTodoItemsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "TodoItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_Slug",
                table: "TodoItems",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_Slug",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "TodoItems");
        }
    }
}
