using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.Base
{
    public interface IConcurrencyAware
    {
        [ConcurrencyCheck]
        string ConcurrencyStamp { get; set; }
    }
}
