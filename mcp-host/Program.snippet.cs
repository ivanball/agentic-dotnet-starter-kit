// SNIPPET, not a compiling file. Two blocks: the in-app HTTP server, which is what you
// want, and the stdio fallback host, which exists for clients that can only launch a
// process. Substitute your own tool class for TicketTools.

// ---------------------------------------------------------------------------
// 1. In the API host's Program.cs. Two calls, beside the ones already there.
// ---------------------------------------------------------------------------

using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTicketsModule(builder.Configuration);   // your existing registration

// MCP is one more adapter at the API layer: the tools in the Presentation assembly call
// the same handlers the HTTP endpoints call.
// Stateless because the 2026-07-28 specification drops the initialize handshake and the
// Mcp-Session-Id header from the wire, so every POST stands on its own and no instance
// has to remember a caller between requests.
builder.Services.AddMcpServer()
    .WithHttpTransport(options => options.SessionMode = HttpServerSessionMode.Stateless)
    .WithToolsFromAssembly(typeof(TicketTools).Assembly);

var app = builder.Build();

app.MapTicketEndpoints();   // your existing routes

// The MCP endpoint sits beside the HTTP routes, not in front of them or instead of them.
app.MapMcp("/mcp");

app.Run();

// ---------------------------------------------------------------------------
// 2. The stdio fallback: a separate console project, same tools, no web server.
// ---------------------------------------------------------------------------

var stdio = Host.CreateApplicationBuilder(args);

// On stdio the protocol owns stdout, so a log line printed there corrupts the stream.
stdio.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

// Same database the app uses; override with the ConnectionStrings__<Name> environment
// variable when pointing it somewhere else.
stdio.Configuration["ConnectionStrings:Tickets"] =
    stdio.Configuration.GetConnectionString("Tickets")
    ?? "Server=localhost,14330;Database=YourDatabase;User Id=sa;Password=<set this>;TrustServerCertificate=True";

stdio.Services.AddTicketsModule(stdio.Configuration);

stdio.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(typeof(TicketTools).Assembly);

await stdio.Build().RunAsync();
