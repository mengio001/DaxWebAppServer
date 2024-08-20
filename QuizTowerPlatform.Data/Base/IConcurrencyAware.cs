using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.Base
{
    public interface IConcurrencyAware
    {
        string ConcurrencyStamp { get; set; }
    }
}
