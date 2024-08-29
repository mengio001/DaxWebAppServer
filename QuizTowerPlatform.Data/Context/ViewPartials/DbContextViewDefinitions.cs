using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.Context.ViewPartials
{
    public partial class DbContextViewDefinitions
    {
        public Dictionary<string, string> CreateViewSqlQueryList { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> DropViewSqlQueryList { get; } = new Dictionary<string, string>();

        public void AddView(string viewName, string createSql, string dropSql)
        {
            CreateViewSqlQueryList[viewName] = createSql;
            DropViewSqlQueryList[viewName] = dropSql;
        }

        public void ConfigureAllViews()
        {
            DefineUserView();
            DefineTeamView();
        }
    }
}
