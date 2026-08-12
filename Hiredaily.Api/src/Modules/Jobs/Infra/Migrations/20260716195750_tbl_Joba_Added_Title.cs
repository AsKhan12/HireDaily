using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiredaily.Modules.Jobs.Infra.Migrations
{
    /// <inheritdoc />
    public partial class tbl_Joba_Added_Title : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Jobs");
        }
    }
}
