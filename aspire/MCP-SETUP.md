# Pointing a coding agent at a running app

The Aspire CLI can expose the application it is running to a coding agent over MCP. The
agent can then list the resources, read their logs and read their traces from the run in
front of you, which closes the loop between running the app and changing it.

## 1. Check the CLI first

```sh
aspire --version
aspire agent --help
```

The `agent` command arrived on the 13.1 line. A CLI that predates it answers
`Unrecognized command or argument 'agent'`. Update before going further: everything
below depends on it, and the failure mode is a config file that looks right and a server
that never starts.

## 2. Write the server entry

```sh
aspire agent init
```

It asks which agent you use and writes the entry for it. For Claude Code that is
`.mcp.json` at the repository root:

```json
{
  "mcpServers": {
    "aspire": {
      "command": "aspire",
      "args": ["agent", "mcp"]
    }
  }
}
```

VS Code reads `.vscode/mcp.json` instead, in the same shape. The file is safe to commit:
it names a command, not a credential.

## 3. Restart and confirm

Restart the agent so it picks the server up, then list the connected servers (`/mcp` in
Claude Code) and confirm `aspire` is there and connected. An MCP server that failed to
start is usually silent, so confirm rather than assume.

## 4. What the agent can then read

With the app running under the app host, the agent can enumerate the resources, read a
resource's logs, and read traces. That is enough for the loop this exists for:

> The close endpoint returns 204 but the record reads back unchanged. Use the aspire MCP
> server to read the logs and traces from the running app, find the failure, and fix it.

Work that failed after the response was sent has no trace around it, and that is exactly
the class of bug a passing test suite does not see and a green trace does not show. The
agent reads the orphaned error line, follows the exception to the handler and proposes
the fix. Confirm the fix the way CI would, by running the test tier that covers it, not
by re-reading the dashboard.

## What to keep in mind

Each MCP server you connect is a dependency with two costs: the tool definitions sit in
the agent's context on every turn, and whatever the server returns arrives in the same
window as your own instructions. Connect the ones the current work needs and disable the
rest, rather than accumulating them.
