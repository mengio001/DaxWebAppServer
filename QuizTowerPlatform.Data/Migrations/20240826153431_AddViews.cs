using Microsoft.EntityFrameworkCore.Migrations;
using QuizTowerPlatform.Data.Context.ViewPartials;

#nullable disable

namespace QuizTowerPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var viewDefinitions = new DbContextViewDefinitions();
            viewDefinitions.ConfigureAllViews();

            foreach (var createViewSql in viewDefinitions.CreateViewSqlQueryList.Values)
            {
                migrationBuilder.Sql(createViewSql);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var viewDefinitions = new DbContextViewDefinitions();
            viewDefinitions.ConfigureAllViews();

            foreach (var dropViewSql in viewDefinitions.DropViewSqlQueryList.Values)
            {
                migrationBuilder.Sql(dropViewSql);
            }
        }
    }
}