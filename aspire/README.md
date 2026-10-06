# Aspire seed

Four seed files, not a project. They are the smallest orchestration that gives you one
command to start the database and the app together, a dashboard with traces that reach
into the database, and the two health endpoints wired the way a container platform
expects them.

## Drop them in

1. Create two projects in your solution: an app host and a service defaults library.
   The app host uses the Aspire app host SDK; the service defaults library is a plain
   class library with `IsAspireSharedProject` set. Use `aspire new` or the Aspire
   templates if you prefer to start from those and then paste over the contents.
2. `AppHost.cs` and `AppHost.csproj` become your app host project, renamed. Change the
   `UserSecretsId` to a fresh GUID, and repoint the `ProjectReference` at your own host.
3. `ServiceDefaults.Extensions.cs` becomes `Extensions.cs` in the service defaults
   project. Keep its namespace as `Microsoft.Extensions.Hosting`: that is what makes
   `builder.AddServiceDefaults()` resolve everywhere without an extra `using`.
4. Every service project references the service defaults project. In each service's
   `Program.cs`, call `builder.AddServiceDefaults()` before anything else and
   `app.MapDefaultEndpoints()` after `Build()`.
5. Run it: `aspire run --project <path to your app host>`. The console prints a
   dashboard URL with a token in it. Open the printed link rather than typing the host.

## The three rules worth keeping

**The resource name is the connection string name.** `sql.AddDatabase("Tickets", ...)`
names the resource `Tickets`, and `WithReference` injects it as
`ConnectionStrings:Tickets`. That is the whole mechanism: your code keeps reading the
configuration key it already read, and the app host decides what is behind it. The
second argument is the physical database name and does not have to match. Getting this
backwards is the usual first hour lost: a resource renamed for readability silently
changes the key your service reads.

**`WaitFor` gates on a dependency's liveness, never on your own readiness.** Two health
endpoints, two different questions. `/alive` is liveness: the process is up and
answering, and it checks nothing else on purpose, because a platform restarts anything
that fails it. `/health` is readiness: every dependency this instance needs is
reachable, which is what a load balancer asks before sending traffic. So the app host
holds the API until the database resource reports healthy, and nothing ever waits on the
API's own `/health`, because readiness only goes green after the schema is in place and
the schema only arrives after the service starts. Wait on what you depend on, report
your own readiness, never wait on yourself.

**Migrating on startup is a switch, not a guess.** The app host sets
`Migrations__ApplyOnStartup=true` and nothing else does, so the orchestrated run
migrates and every other way of starting the app leaves the schema alone. An explicit
switch beats sniffing the environment name: you can read it in one line, a developer
running the host directly keeps migrating by hand, and integration tests that create
their own database never migrate twice.

## What the dashboard is for

Open **Traces** after one request. A create call is the HTTP span with the database span
(the INSERT) directly under it. Add an ActivitySource to your handlers if you want a
handler level in between. The database span is why the
service defaults project turns on SQL client instrumentation; without it the database
time is invisible and reads as an unexplained gap inside the HTTP span. **Structured
logs** are joined to traces by trace id, so you can go from a slow request to the log
lines written during it without searching for anything.

The tell worth teaching: an error log for a request whose trace already ended green ran
after the caller had its answer. That is how work that escaped the request scope shows
itself.

See `MCP-SETUP.md` for pointing a coding agent at this dashboard.
