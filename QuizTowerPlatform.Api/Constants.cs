namespace QuizTowerPlatform.Api
{
    public static class Constants
    {
        public const string RoutePrefix = "api/";
        public const string DefaultValueString = " ";
        public const int DefaultValueInt = -1;

        /// <summary>
        /// Detects the use of the default value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string? StringOrNull(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public static int? IntOrNull(this int? value)
        {
            return value == DefaultValueInt ? null : value;
        }
    }
}
