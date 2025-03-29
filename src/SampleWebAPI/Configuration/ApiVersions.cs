using Microsoft.AspNetCore.Mvc;

namespace SampleWebAPI.Configuration;

public static class ApiVersions
{
    public static class V_2024_04_26
    {
        public const string Name = "2024-04-26";
        public static ApiVersion Version = new ApiVersion(new DateTime(2024, 04, 26));
    }
}