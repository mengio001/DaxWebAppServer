using QuizTowerPlatform.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.DataModels
{
    [Table(nameof(Question), Schema = Constants.Schemas.QuizTowerPlatform)]
    public class Question : AuditableEntity
    {
        public string QuestionName { get; set; }

        public string FirstOption { get; set; }

        public string SecondOption { get; set; }

        public string ThirdOption { get; set; }

        public string FourthOption { get; set; }

        public string CorrectAnswer { get; set; }

        public int CorrectAnswerPoints { get; set; }

        [Required]
        public int QuizId { get; set; }

        [ForeignKey(nameof(QuizId))]
        public Quiz Quiz { get; set; }
    }
}
