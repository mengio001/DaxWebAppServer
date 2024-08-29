using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Model
{
    public class StartQuizModel
    {
        public int QuizId { get; set; }
        public List<AnswerModel> Answers { get; set; }
    }
}
