using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.Types
{
    public static class QuizGenreExtensions
    {
        public static QuizGenre ToQuizGenre(this string asString)
        {
            return !string.IsNullOrEmpty(asString) ? (QuizGenre)Enum.Parse(typeof(QuizGenre), asString, true) : QuizGenre.Other;
        }

        // Note: I like to implement a fallback option to "Other Genre" if no specific genre is selected.
        //public static QuizGenre? ToQuizGenre(this string asString)
        //{
        //    return !string.IsNullOrEmpty(asString) ? (QuizGenre)Enum.Parse(typeof(QuizGenre), asString, true) : (QuizGenre?)null;
        //}

        public static string DisplayName(this QuizGenre? type)
        {
            return type.HasValue ? Items[type.Value] : string.Empty;
        }

        public static string DisplayName(this QuizGenre type)
        {
            return Items[type];
        }

        public static IReadOnlyDictionary<QuizGenre, string> Items => new Dictionary<QuizGenre, string>
            {
                {QuizGenre.Other, "Other"},
                {QuizGenre.Sports, "Sports"},
                {QuizGenre.Movies, "Movies"},
                {QuizGenre.Music, "Music"}
            };
    }

    public enum QuizGenre
    {
        Sports = 1,
        Movies = 2,
        Music = 3,
        Other = 4
    }
}
