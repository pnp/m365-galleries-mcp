using ModelContextProtocol.Server;
using System.ComponentModel;
using SampleGalleriesMCPServer;

var builder = WebApplication.CreateBuilder(args);

// Configure the Samples API settings
builder.Services.Configure<SamplesApiConfiguration>(
    builder.Configuration.GetSection(SamplesApiConfiguration.SectionName));

// Add HTTP client for the Samples API
builder.Services.AddHttpClient<SamplesTools>();

// Add the MCP services: the transport to use (HTTP) and the tools to register.
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();
    
var app = builder.Build();

// Configure the application to use the MCP server
app.MapMcp();

// Run the application
// This will start the MCP server and listen for incoming requests.
app.Run();