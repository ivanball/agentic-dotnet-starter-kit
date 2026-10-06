# MCP host reference

Expose your application's use cases to an agent over MCP, hosted inside the app you
already have, over streamable HTTP. Two files here: `TicketTools.example.cs` is the tool
class pattern, `Program.snippet.cs` is the registration, in the API host and in a stdio
fallback.

The thesis is one sentence. **MCP is a protocol adapter at the API layer**, a sibling of
your HTTP endpoints, not a layer of its own and not a second route to the database. A
tool call lands on the same handler interface the endpoint injects, which means the same
validation, the same decorators and the same aggregate. Anything an HTTP caller cannot
do, a model cannot do either, because there is nothing else to call.

## Where the pieces go

- The tool class goes in your **Presentation** (API) project, beside the endpoints, and
  the protocol package goes in that project file and nowhere deeper. Application and
  domain stay package free, which is rule 1 of the fitness tests.
- The two registration lines go in the API host's `Program.cs`.
- The stdio host, if you want one, is a separate console project referencing the same
  Presentation assembly.

## The rules that matter

**DI parameters vanish from the schema.** A tool method's parameters are split: the ones
the container can resolve are injected, and the rest become the tool's input schema. So
`CreateTicketAsync(ICommandHandler<...> handler, string title, CancellationToken ct)`
publishes exactly one input, `title`. This is the feature that lets a tool be a thin
call into your existing handler rather than a second composition root.

**The descriptions are the only prose that matters.** `[Description]` on the method and
on each parameter is all a model has to decide with. Write what the tool does, what the
parameter is, and what makes the call fail. Then read them in MCP Inspector rather than
in the source, because that pane is what the model sees.

**A refused business rule is a tool error, not an exception.** Return a result with
`IsError` set and the same error code the HTTP endpoint puts in its body. The model
reads the refusal and stops. Throwing produces a generic message and tells it nothing.
Exceptions stay for defects and infrastructure faults, exactly as everywhere else.

**Run the server stateless.** Set the session mode to stateless. The 2026-07-28
specification removes the initialize handshake and the session header from the wire, so
each POST stands alone and nothing has to route a caller back to the instance that
remembers it. That is what makes the server survive more than one replica.

**Pin the SDK.** These files are written against **2.2.0**: `ModelContextProtocol`
for the client and the stdio host, `ModelContextProtocol.AspNetCore` for the in-app
server. The 2.0.0 release is the one that brought the SDK into stable alignment with the
2026-07-28 specification. Check which specification version your pin supports before
relying on a behaviour from it.

**Stdio is the fallback, not the default.** Some clients only know how to launch a
process. The stdio host is the same tools over a pipe, and it makes the point that the
transport is the only thing that changed. Logs go to stderr there, because the protocol
owns stdout.

## The tool surface is the blast radius

A record's title is text a stranger typed, and a read tool hands that text to a model.
So the model will read instructions written by someone who is not you. Three things stop
it acting on them, and none of them is the model's judgement:

- **There is no bulk operation.** "Close every ticket" has no tool. The close tool takes
  one identifier and nothing hands out identifiers. What the model can reach was fixed
  at design time by the method signatures in the tool class.
- **The aggregate still decides.** A rule that refuses a closed record refuses it on the
  tool path exactly as on the HTTP path. A tool call cannot argue with an invariant.
- **Every write goes through the same validated handler.** The validation decorator runs
  on both paths, so a malformed input is rejected before the aggregate is reached.

The defence is structural. Untrusted text reaching a model is a given; the question is
what the model can reach when it believes the text. Choose the tool list on that basis,
and keep read tools marked read-only.

## Connecting a client

- **MCP Inspector**, fastest for seeing the tool list:
  `npx @modelcontextprotocol/inspector@latest` (Node.js 22.19 or newer, 24 LTS
  recommended), transport Streamable HTTP, URL `http://localhost:<port>/mcp`. Keep the
  `@latest`: the bare name can reuse a cached old version.
- **VS Code**: a `.vscode/mcp.json` entry with `"type": "http"` and the same URL.
- **Claude Code**: `claude mcp add --transport http <name> http://localhost:<port>/mcp`,
  then `/mcp` to confirm it connected.

## What production adds

Nothing here is authenticated, which is fine for a workshop and not fine for anything
else. A production MCP server is an OAuth 2.1 resource server: it publishes protected
resource metadata, refuses an unauthenticated call with a 401 naming its authorization
server, and the client runs the authorization code flow with PKCE and retries with a
bearer token. Your server can map scopes to tools (the spec leaves that mapping to you),
so a read-only agent's token is refused on a write tool, and since 2025-11-25 the server
can ask for a wider scope on a later call through WWW-Authenticate. The SDK has the
server side of that, and
the endpoint `MapMcp` returns takes `RequireAuthorization()` like any other.

That is the honest end of the sentence this starts with: MCP is one more adapter at the
API layer, so it gets the same authentication story as the API layer, the same way.
