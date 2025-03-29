
using Diginsight.Options;

namespace SampleWebAPI.Configuration;

public class FeatureFlagOptions : IDynamicallyConfigurable
{
    public bool TraceRequestBody { get; set; }
    public bool TraceResponseBody { get; set; }

}
