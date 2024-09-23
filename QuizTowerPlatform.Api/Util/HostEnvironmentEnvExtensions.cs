namespace QuizTowerPlatform.Api.Util
{
    public static class HostEnvironmentEnvExtensions
    {
        public static bool IsTest(this IHostEnvironment hostingEnvironment)
        {
            return hostingEnvironment.IsEnvironment("Test");
        }
    }
}
