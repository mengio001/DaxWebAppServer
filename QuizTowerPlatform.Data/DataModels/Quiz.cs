using QuizTowerPlatform.Data.Base;
using QuizTowerPlatform.Data.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table(nameof(Quiz), Schema = Constants.Schemas.QuizTowerPlatform)]
    public class Quiz : AuditableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public QuizGenre Category { get; set; }

        [InverseProperty("Quiz")]
        public List<Question> QuizQuestions { get; set; } = new List<Question>();

        public string QuizLogoUrl { get; set; }

    }
}
