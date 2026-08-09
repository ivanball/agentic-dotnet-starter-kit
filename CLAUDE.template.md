# <Project name>

<One paragraph: what this system is, its module/layer shape, where the composition root is.>

## Commands

- Build: <command>
- Test: <command>
- Run: <command>
- Single test: <command>

## Layer rules (enforced by <path to fitness tests>)

- <Your domain-equivalent> references only <allowed>.
- <Who may reference infrastructure>.
- <Module boundary rule if modular>.

## Conventions

- Errors: <Result pattern / exceptions policy>.
- <Entity construction convention>.
- <Handler/endpoint conventions>.
- <Style: nullable, warnings-as-errors, namespaces>.

## Never

- Never add a NuGet package without asking.
- Never modify tests to make them pass; fix the code or flag the test as wrong.
- Never touch the database schema without a migration.
- <The gotcha you tell every new hire.>
