# Agentic .NET Starter Kit

The take-home from the precon **Build an AI-Ready .NET App with an AI Pair: Clean
Architecture, MCP, and Agentic Workflows**.

This is not an application and it will not build. It is the set of files that made the
workshop application work with an agent rather than in spite of one: the rules the agent
reads, the guardrails that hold when it ignores them, the skills that codify the requests
you make every week, the tests that make a layer rule executable, the pipeline templates,
the orchestration and MCP seeds, and the eval that tells you whether any of it is still
true six months from now. Every file here came out of the workshop solution, so the kit
and the solution cannot disagree with each other. Copy what you need, rename the
placeholders, delete the rest.

## What to copy, in adoption order

1. **Context files.** `CLAUDE.template.md` and `AGENTS.template.md` become `CLAUDE.md`
   and `AGENTS.md` at your repository root. Same rules, one file: fill in `AGENTS.md`,
   and make `CLAUDE.md` the one line `@AGENTS.md`, because Claude Code skips `AGENTS.md`
   whenever a `CLAUDE.md` exists. Fill in the blanks: half an hour, and it is the single
   highest-return file in the list.
2. **Settings and hooks.** `.claude/settings.json` carries the allow list, the deny list,
   the permission mode and the sandbox block, and wires the two hooks in
   `.claude/hooks/`. Allow rules are for read-only and reversible work; deny rules and
   hooks are for what must never happen regardless of who asks. The sandbox applies on
   macOS, Linux and WSL2; on native Windows commands run unsandboxed, so the deny rules
   and hooks are the only layer there.
3. **Skills.** `.claude/skills/` holds three: `slice` (add one use case end to end the
   way this codebase already does it), `spec-first-change` (spec, plan, approve,
   implement, verify), and `ci-diagnose` (find the first failed step, classify it, never
   weaken a gate to go green). A skill fires on its description, so write the description
   as the situation in the words you would actually type.
4. **Fitness test seed.** `fitness-tests/LayerRulesTests.cs` is six architecture rules as
   xUnit tests. Substitute the placeholder names, add the project to your solution, and
   put it in the required check.
5. **CI and release.** `templates/github/` has `ci.yml`, `release.yml` and
   `agent-review.yml`, plus `templates/Dockerfile` and `templates/Directory.Build.props`
   (the lock files and audit that CI and the Dockerfile rely on). Each carries a header
   naming what to change. The agent review workflow needs an API key secret; skip it
   without one. It is advisory: never make it a required check.
6. **Aspire seed.** `aspire/` is an app host and service defaults pair, with the
   liveness and readiness wiring, plus `aspire/README.md` for dropping it in and
   `aspire/MCP-SETUP.md` for pointing an agent at the running app over MCP.
7. **Rubric and agents.** `evals/rubric.md` is a five-rule architecture rubric scored
   0 to 2 per rule, and `.claude/agents/` holds the scorer and the adversarial verifier
   that grade a repository against it. Run scorer, then verifier, then a human.
8. **Token checklist.** `TOKEN-DISCIPLINE.md` is one page on what to keep out of the
   window, what to keep out of the model, and the six fields that make a subagent brief
   executable.
9. **MCP host.** `mcp-host/` is the tool class pattern and the registration for serving
   your own use cases to an agent over streamable HTTP from inside the app you have.

Workshop solution, with a snapshot branch per lab: `<workshop repo link: TODO before the
conference>`

## Verify

Each piece has a cheap check. Run it once; a guardrail nobody has watched work is a
guardrail nobody knows is wired.

- **Hooks fire.** Ask the agent to run `git push --force`. It should be refused by the
  hook, with the hook's own message. If it is not, the hook path in `settings.json` is
  wrong or the script is not executable. A hook with a carriage return in its shebang
  fails silently on some shells, which is why `.gitattributes` pins `*.sh` to LF.
- **Deny rules fire.** Ask for `rm -rf` on a relative path. The hook lets it through
  (its pattern only covers absolute paths), and the deny rule refuses it. That proves
  the deny layer on its own. Keep both layers: the deny list catches the common
  spellings, the hook catches the rewordings the pattern list misses.
- **A skill fires.** Start a fresh session and describe the situation in your own words
  without naming the skill: "CI went red on my branch, what happened?" should reach
  `ci-diagnose`. If it does not, the description is wrong, not your phrasing.
- **The fitness test compiles and can fail.** Point the placeholder types at your own
  assemblies, build, and watch the six tests pass. Then break one rule on purpose, watch
  the matching test go red, and restore it. A fitness test that has never failed is
  decoration.
- **CI protects the branch.** Open a pull request that breaks a layer rule but compiles.
  The failing check should be the fitness test step, and the message should name the
  offending type.
- **The rubric runs.** Score your own repository against `evals/rubric.md`, then run the
  verifier over the scorer's claims and read every FLAG yourself. A verifier that confirms
  everything on its first run was handed the scorer's reasoning by mistake.

## Notes

- The hooks prefer `jq` for precise JSON parsing and fall back to matching the raw hook
  input when it is missing, so they work out of the box. Installing jq is still
  recommended: `winget install jqlang.jq` (Windows), `brew install jq` (macOS),
  `sudo apt install jq` (Debian/Ubuntu).
- `confirm-merge.sh` is the strict variant taught in the workshop: the agent never merges,
  a human does, after reviewing checks. Teams that want agent-assisted merges can relax
  it; start strict.
- Gate on reversibility and on whether an action leaves your machine, not on how weighty
  it feels. Prompting on every `git commit` teaches everyone to click through the one
  prompt that mattered.
- Nothing here contains a secret, and nothing here should. The one password in the
  templates belongs to a database container that lives for the length of one CI job;
  replace it anyway, and never let a real one reach a file in the repository.

MIT licensed: copy without asking.
