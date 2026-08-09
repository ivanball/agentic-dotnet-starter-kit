# Agentic .NET Starter Kit

The take-home from the ADC 2026 precon "Build a Modular Monolith with an AI Pair".
Five files to copy into your own repo, in adoption order:

1. `CLAUDE.template.md` -> your repo's `CLAUDE.md` (fill in the blanks; 30 minutes)
2. `.claude/settings.json` -> deny rules for your scariest commands
3. `.claude/hooks/` -> two example hooks (already wired in settings.json)
4. `fitness-tests/LayerRulesTests.cs` -> adapt the namespaces, add to your test suite
5. `.claude/commands/slice.md` -> a model for codifying your most-repeated request

Workshop solution with all snapshot branches: `<workshop repo link>`

## Notes

- The hooks prefer `jq` for precise JSON parsing and fall back to matching the raw hook
  input when it is missing, so they work out of the box. Installing jq is still
  recommended: `winget install jqlang.jq` (Windows), `brew install jq` (macOS),
  `sudo apt install jq` (Debian/Ubuntu).
- Deny rules in `settings.json` catch the common spellings of dangerous commands; the hooks
  catch rewordings the pattern list misses. Belt and suspenders: keep both.
- `confirm-merge.sh` is the strict variant taught in the workshop: the agent never merges,
  a human does, after reviewing checks. Teams that want agent-assisted merges can relax it;
  start strict.
- The fitness tests are decoration until you prove each one can fail: temporarily break the
  rule it guards, watch the red run, restore.

MIT licensed: copy without asking.
