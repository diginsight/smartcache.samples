using Diginsight.Options;

namespace SampleWebAPI;

public class ConcurrencyOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public int? MaxConcurrency { get; set; }
}
