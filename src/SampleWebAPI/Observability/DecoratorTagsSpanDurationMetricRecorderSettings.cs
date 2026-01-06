//using Diginsight.Diagnostics;
//using Microsoft.Extensions.Options;
//using System.Diagnostics;
//using System.Diagnostics.Metrics;

//namespace SampleWebAPI;

//internal sealed class CustomMetricRecordingEnricher : IMetricRecordingEnricher
//{
//    private readonly IOpenTelemetryOptions openTelemetryOptions;

//    public CustomMetricRecordingEnricher(
//        IOptions<OpenTelemetryOptions> openTelemetryOptions
//    )
//    {
//        this.openTelemetryOptions = openTelemetryOptions.Value;
//    }

//    public void Enrich(Activity activity, TagList tags)
//    {
//        // Extract tags from activity ancestors based on DurationMetricTags configuration
//        foreach (var tagKey in openTelemetryOptions.DurationMetricTags)
//        {
//            var value = activity.GetAncestors(true)
//                               .Select(a => a.GetTagItem(tagKey))
//                               .FirstOrDefault(static v => v is not null);

//            if (value is not null)
//            {
//                tags.Add(tagKey, value);
//            }
//        }
//    }

//    public IEnumerable<KeyValuePair<string, object?>> ExtractTags(Activity activity, Instrument instrument)
//    {
//        foreach (var tagKey in openTelemetryOptions.DurationMetricTags)
//        {
//            var value = activity.GetAncestors(true)
//                               .Select(a => a.GetTagItem(tagKey))
//                               .FirstOrDefault(static v => v is not null);

//            if (value is not null)
//            {
//                yield return new KeyValuePair<string, object?>(tagKey, value);
//            }
//        }
//    }
//}