using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.Types
{
    public static class TeamTypeExtensions
    {
        public static TeamType ToTeamType(this string asString)
        {
            return (TeamType)Enum.Parse(typeof(TeamType), asString, true);
        }
    }

    // LET OP: moet synchroon lopen met enum.KlantType in de database!
    // Verander NOOIT een volgnummer zonder conversie van de database!
    public enum TeamType : byte
    {
        TeamRed = 1,
        TeamBlue = 2,
        TeamYellow = 3,
        TeamGreen = 4
    }
}
