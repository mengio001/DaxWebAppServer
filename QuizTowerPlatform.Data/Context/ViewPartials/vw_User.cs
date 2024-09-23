using QuizTowerPlatform.Data.DataModels;
using System.Text.RegularExpressions;

namespace QuizTowerPlatform.Data.Context.ViewPartials
{
    public partial class DbContextViewDefinitions
    {
        // Note: Don't forget to add the method name to the ConfigureAllViews() method in main DbContextViewDefinitions partial class!
        public void DefineUserView()
        {
            // Note: To avoid errors caused by special characters or reserved keywords,
            // always enclose schema, table, and column names in square brackets.
            const string vwName = $"[{Constants.Schemas.QuizTowerPlatform}].[{Constants.ViewPrefix + nameof(User)}]";

            const string createViewSql = $@"
                CREATE VIEW {vwName} WITH SCHEMABINDING 
                AS
                SELECT 
	                u.ID As Id,
	                999 AS TeamId,
	                upd.UserName,
	                null AS Initials,
	                upd.FirstName,
	                upd.MiddleName,
	                upd.LastName,
	                upd.Email,
	                upd.PhoneNumber,
	                upd.DateOfBirth,
	                tap.UserId
                FROM [TOQ].[User] u
                JOIN [TOQ].[UserPersonalDetails] upd ON u.ID = upd.ID
                LEFT JOIN [QuizTowerPlatform].[TotalAchievementPoints] tap ON u.ID = tap.UserId
            ";

            const string dropViewSql = $"DROP VIEW IF EXISTS {vwName}";

            AddView(Regex.Replace(vwName, @"\[(.*?)\]", "$1").Split('.')[1], createViewSql, dropViewSql);
        }
    }
}
