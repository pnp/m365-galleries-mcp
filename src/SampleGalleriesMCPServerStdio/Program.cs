using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleGalleriesMCPServer;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Configure the Samples API settings
builder.Services.Configure<SamplesApiConfiguration>(
    builder.Configuration.GetSection(SamplesApiConfiguration.SectionName));

// Add HTTP client for the Samples API
builder.Services.AddHttpClient<SamplesTools>();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<SamplesTools>();

// Run the application
// This will start the MCP server and listen for incoming requests.
await builder.Build().RunAsync();
