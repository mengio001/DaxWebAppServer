using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTowerPlatform.Data.Types
{
    public static class GenderExtensions
    {
        public static readonly string MaleString = Gender.Male.ToString();
        public static readonly string FemaleString = Gender.Female.ToString();

        public static string DisplayName(this Gender? type)
        {
            if (type == null)
                return "Niet bekend";
            return Items[type.Value];
        }

        public static string DisplayName(this Gender type)
        {
            return Items[type];
        }

        public static Gender? ToBinaryGender(this int? binaryGenderAsInt)
        {
            return binaryGenderAsInt.HasValue ? (Gender?)binaryGenderAsInt : null;
        }

        public static Gender? ToBinaryGender(this bool? binaryGenderAsBool)
        {
            return binaryGenderAsBool.HasValue ? (Gender?)(binaryGenderAsBool.Value ? Gender.Male : Gender.Female) : null;
        }

        public static Gender? ToBinaryGender(this string enumText)
        {
            return !string.IsNullOrEmpty(enumText) ? (Gender)Enum.Parse(typeof(Gender), enumText, true) : (Gender?)null;
        }

        public static string Salutation(this Gender? type)
        {
            return type.HasValue && SalutationTitle.ContainsKey(type.Value)
                ? SalutationTitle[type.Value]
                : "De heer/mevrouw";
        }

        private static Dictionary<Gender, string> Items => new Dictionary<Gender, string>
        {
            {Gender.Male, "Man"},
            {Gender.Female, "Vrouw"},
            {Gender.Other, "Mx."}
        };

        private static Dictionary<Gender, string> SalutationTitle => new Dictionary<Gender, string>
        {
            {Gender.Male, "De heer"},
            {Gender.Female, "Mevrouw"},
            {Gender.Other, "Geachte"}
        };
    }

    /// <summary>
    /// Currently using BinaryGender with the option Other Gender for non-binary identities,
    /// without including other specific genders for Non-Binary.
    /// </summary>
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3

        // NonBinary = 4,
        // Genderqueer = 5,
        // Agender = 6,
        // Genderfluid = 7,
    }
}
