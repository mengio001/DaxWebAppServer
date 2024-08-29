using QuizTowerPlatform.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.Context.ViewPartials
{
    public partial class DbContextViewDefinitions
    {
        // Note: Don't forget to add the method name to the ConfigureAllViews() method in main DbContextViewDefinitions partial class!
        public void DefineTeamView()
        {
            // Note: To avoid errors caused by special characters or reserved keywords,
            // always enclose schema, table, and column names in square brackets.
            const string vwName = $"[{Constants.Schemas.QuizTowerPlatform}].[{Constants.ViewPrefix + nameof(Team)}]";

            const string createViewSql = $@"
                CREATE VIEW {vwName} WITH SCHEMABINDING 
                AS
                SELECT 
	                t.ID As Id,
	                'TeamBlue' AS TeamTypeString,
	                t.UserId 
                FROM [TOQ].[UserTeam] t
            ";

            const string dropViewSql = $"DROP VIEW IF EXISTS {vwName}";

            AddView(Regex.Replace(vwName, @"\[(.*?)\]", "$1").Split('.')[1], createViewSql, dropViewSql);
        }
    }
}