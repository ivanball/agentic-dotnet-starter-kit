---
name: slice
description: Use when asked to add a new command or query slice to a module, wire up a new endpoint, or implement a new use case end to end through Application, Presentation and registration.
---

# Add a vertical slice

Add one command or query, its handler, its validator, its endpoint and its registration.
One use case per slice. Follow CLAUDE.md and AGENTS.md exactly; they are the contract.

Replace `<App>`, `<Module>` and `<Aggregate>` below with your own names once, then delete
this line. The paths are the only part of this skill that is project specific.

## Steps

1. Read CLAUDE.md, then read the nearest existing slice in
   `src/Modules/<Module>/<App>.<Module>.Application/<Aggregate>/` and copy its shape.
   The existing code is the specification; do not invent a second way of doing this.
2. Create the Application file `<UseCase>.cs` beside its siblings. It holds, in order:
   the command or query record implementing `ICommand<TResponse>` or `IQuery<TResponse>`,
   the handler (internal, sealed, thin: load or create through the domain, call the domain
   behavior, save through `IUnitOfWork`), and any response records the query returns.
3. Add a validator implementing `IValidator<T>` in the same file when the input has a
   shape to check. Validators check input shape only. Business rules belong in the domain
   entity and are returned as a `Result` failure, never thrown.
4. Add the minimal-API endpoint in
   `src/Modules/<Module>/<App>.<Module>.Presentation/<Aggregate>Endpoints.cs`.
   Map the `Result` the handler returns: success to 201 Created for a create, 204 No Content
   for a state change, 200 OK for a query; failure to 404 when the error code ends in
   `NotFound`, and 400 Bad Request with the error code and message otherwise.
5. Register the handler in the module's existing registration extension
   (`<Module>Application.cs`). Do not add new infrastructure, a new DI container, or a new
   registration file.
6. Add a test. Domain rules go in `tests/<App>.<Module>.Domain.Tests`. Status-code mapping
   goes in `tests/<App>.ApiTests`, which drives the real host against the database
   container.
7. Verify: start the database container, then run the solution build and the full test
   command from CLAUDE.md. Show the output. Warnings are errors, so a build that reports
   any warning is a failure.

## Constraints

- Never modify the shared kernel, the Domain layer, or an existing test to make a slice pass.
- Never add a NuGet package. Never change the database schema without a migration.
- If a step conflicts with an existing convention, stop and ask rather than improvising.
