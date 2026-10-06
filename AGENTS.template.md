# <Project name> (agent instructions)

Codex, the Copilot coding agent and Cursor read AGENTS.md. Claude Code reads it too
(v2.1.277+), but only when there is no CLAUDE.md. If you keep both files, make CLAUDE.md
a one-line `@AGENTS.md` import so there is one set of rules, not two copies that drift,
and every agent and every human starts from the same contract.

<One paragraph: what this system is, its module/layer shape, where the composition root is.>

## Commands

- Build: <command>
- Test: <command> (<what has to be running first: a database container, a service>)
- Run: <command>
- Run (orchestrated): <command, if you have an app host that starts the dependencies too>
- Single test: <command>
- MCP: <endpoint and transport, if the app exposes one; delete this line if it does not>
- Ship: <path to your release doc, plus the one gotcha: what to regenerate after a package
  change, which restore mode CI uses>

## Layer rules (enforced by <path to fitness tests>)

- <Your domain-equivalent> references only <allowed>.
- <Who may reference infrastructure, and who may not>.
- <Module boundary rule if modular: where modules are allowed to meet>.

## Conventions

- Errors: <Result pattern / exceptions policy>. Never throw for business rule failures;
  exceptions are for bugs and infrastructure faults only.
- <Entity construction convention: private constructors, static factory methods.>
- <Handler/endpoint conventions: one command or query per file, handler beside it,
  internal visibility.>
- <Style: nullable, warnings-as-errors, file-scoped namespaces.>

## Never

- Never add a NuGet package without asking.
- Never modify tests to make them pass; fix the code or flag the test as wrong.
- Never touch the database schema without a migration.
- <The gotcha you tell every new hire.>
