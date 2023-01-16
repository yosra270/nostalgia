using System.Diagnostics;
using System.Diagnostics.Metrics;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Datadog.Trace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetrySample.Services;
using OpenTelemetrySample.Settings;
using Orleans;
using Serilog.Context;
using Shared;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HelloController> _logger;
    private readonly IGrainFactory _grainFactory;
    private readonly IDynamoService _dynamoService;
    private readonly IOptionsMonitor<OtlpSettings> _monitor;

    public HomeController(ILogger<HelloController> logger, IGrainFactory grainFactory, IDynamoService dynamoService, IOptionsMonitor<OtlpSettings> monitor)
    {
        _logger = logger;
        _grainFactory = grainFactory;
        _dynamoService = dynamoService;
        _monitor = monitor;
    }

    [HttpGet]
    public async Souvenir Get(string name)
    {
        var clientIpAddress = request.HttpContext.Connection.RemoteIpAddress;

        Random rnd = new Random();
        int request_id  = rnd.Next(10000000000);

        using (LogContext.PushProperty("dd_trace_id", CorrelationIdentifier.TraceId.ToString()))
        using (LogContext.PushProperty("dd_span_id", CorrelationIdentifier.SpanId.ToString()))
        using (LogContext.PushProperty("client_id", clientIpAddress))
        using (LogContext.PushProperty("request_id", request_id))
        {
            using var activity = ActivitySourcesSetup.ActivitySource.StartActivity("100 ms delay");
            
            activity?.SetStartTime(DateTime.Now);
            activity?.SetStatus(ActivityStatusCode.Ok);
            
            await Task.Delay(100);
            _logger.LogInformation("{@activity}", activity);
            var helloGrain = _grainFactory.GetGrain<IHelloGrain>(name);
            await helloGrain.SayHello(name);

            activity?.SetTag("client_id", $"{clientIpAddress}");
            activity?.SetTag("request_id", $"{request_id}");
            
            if (_monitor.CurrentValue.UseDynamoDb)
            {
                await _dynamoService.GetItem(name);
            }

            activity?.SetEndTime(DateTime.Now);
            return await Task.FromResult($"{name}");
        }
    }

    [HttpGet("metrics")]
    public async Task<string> Get()
    {
        Random rnd = new Random();
        var meter = new Meter("HomeController.Get");

        var counter = meter.CreateCounter<int>("HomeController.Get.Requests");
        var histogram =
            meter.CreateHistogram<float>("HomeController.Get.RequestDuration", unit: "ms");

        using (LogContext.PushProperty("dd_trace_id", CorrelationIdentifier.TraceId.ToString()))
        using (LogContext.PushProperty("dd_span_id", CorrelationIdentifier.SpanId.ToString()))
        {
            var stopwatch = Stopwatch.StartNew();
            await Task.Delay(rnd.Next(100,5000));

            histogram.Record(stopwatch.ElapsedMilliseconds,
                tag: KeyValuePair.Create<string, object?>("Host", "otlptest"));
            return await Task.FromResult($"Successfully output metrics");
        }
    }
}
