using Microsoft.Extensions.Logging;
using Tachyon;
using Tachyon.Example;

#pragma warning disable CS0618 // Type or member is obsolete
using var factory = LoggerFactory.Create(builder => builder.AddConsole(options => options.IncludeScopes = true));
#pragma warning restore CS0618 // Type or member is obsolete
var logger = factory.CreateLogger("Program");
TachyonContext.Logger = logger;

#pragma warning disable CA1848 // Use the LoggerMessage delegates
logger.LogInformation("About to invoke...");

var result = Processor.Process(2);

#pragma warning disable CA1873 // Avoid potentially expensive logging
#pragma warning disable CA1727 // Use PascalCase for named placeholders
logger.LogInformation("Invocation complete, result is {result}", result);
