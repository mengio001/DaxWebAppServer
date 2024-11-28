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
                SELECT acc.UserId As Id,
	                999 AS TeamId,
	                acc.UserName,
	                null AS Initials,
	                upd.FirstName,
	                upd.MiddleName,
	                upd.LastName,
	                upd.Email,
	                upd.PhoneNumber,
	                upd.DateOfBirth,
	                acc.UserId As UserId
                FROM [Identity].[AspNetUsers] asp
                JOIN [TOQ].[AccountLinkPath] acc ON asp.Id = acc.AspNetUserId AND [asp].[Subject] = acc.SubjectId
                LEFT JOIN [TOQ].[UserPersonalDetails] upd ON acc.UserId = upd.ID AND acc.UserName = upd.UserName
                LEFT JOIN [QuizTowerPlatform].[TotalAchievementPoints] tap ON acc.UserId = tap.UserId
            ";

            const string dropViewSql = $"DROP VIEW IF EXISTS {vwName}";

            AddView(Regex.Replace(vwName, @"\[(.*?)\]", "$1").Split('.')[1], createViewSql, dropViewSql);
        }
    }
}
