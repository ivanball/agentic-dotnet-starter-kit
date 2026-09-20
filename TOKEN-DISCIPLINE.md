# Token and context discipline

One page. A session's bill is roughly **what is resident in the window times the number
of turns**, because every turn re-sends the whole conversation. The output you read is a
rounding error against that. Everything below follows from those two facts, and every
one of them buys quality as well as money: a bloated window is cheap-ish to re-read and
dilutes attention exactly as much.

## What to put in the window

- **Pass paths, not bodies.** A file body pasted into the conversation is re-sent on
  every later turn. A path is re-read only when it is needed, and it is never stale.
- **Admit a thing only if it will be read more than once and is still true.** Content
  that fails either test is charged twice: once in tokens, once in diluted attention on
  everything else.
- **Stale but on topic is worse than irrelevant.** A pre-edit copy of a file you have
  since changed, a build log from before the fix, an abandoned approach: these compete
  directly with the truth and often win. Off-topic noise is easy to ignore; a wrong copy
  of the right file is not.
- **Bound every read.** Search with a filter and a result limit to find the line, then
  read that range with an offset and a limit. Do not read a large file to check one
  claim, and never run a recursive shell search over a repository: most of a .NET tree
  by file count is build output.

## What to keep out of the model entirely

- **Deterministic work goes to a script.** Retrieval from a long context stays reliable;
  summing, counting, tallying and cross-referencing degrade first, and degrade fluently,
  so the wrong total arrives well formed and confident. Any derived number comes from a
  script and is checked by a re-runnable command. Never re-add a table by hand as the
  check: that is the failing operation auditing itself.
- **Let the structured output be the report.** Ask an agent for the artifact, not the
  conclusion: the command and its exit code, the `path:line`, the pasted failure line.
  Never "it builds" or "looks good". Then the return value is the report and nobody
  re-summarises it.
- **Push every check to the cheapest rung that settles it:** analyzers, then tests, then
  a script, then an adversarial second agent, and only then your own eyes on a diff.

## Model, effort and turns

- **Tier the model by the cost of being wrong, not by difficulty.** Cheap when a later
  gate catches the error (search, breadth passes, mechanical transformation), expensive
  when the output becomes another agent's premise or is irreversible (adjudication, a
  pattern forty files will copy). A cheap model with a sharp brief and a deterministic
  check beats an expensive model with a vague one.
- **Effort scales inference, not information.** More thinking helps only when the answer
  is derivable from what is already in the window. When the model simply did not know
  something, effort buys a longer, more confident, still wrong answer, and the fix is
  the brief. Do not raise effort on a fan-out: the cost multiplies by N.
- **Batch turns.** Three related questions in one message cost one re-send of the
  window; three messages cost three. Group independent tool calls into one turn.
- **Append, do not mutate.** Prompt caching holds a prefix, so editing an instruction
  file, a settings file or a skill description mid-session throws away the discount on
  every later turn. Batch changes to the harness to the end of a unit of work.

## Fan-out

- **Chunk it, and make it resumable.** A rate limit binds on throughput, not on total, so
  a wide fan-out is exactly the shape that trips one, and a tripped fan-out does not
  degrade, it dies, taking everything already spent with it. Run in chunks, null-check
  every return, track which units failed, and re-run only those.
- **Pick width by the narrowest resource in the chain**, never by the number of units:
  one workstation, the rate limit, your own attention at the review gate.
- **Fan out only when every brief can be written before seeing any result.** Otherwise it
  is a pipeline, and going wide means being wrong in parallel.

## Session lifecycle

- **Clear above roughly 300 K resident context.** Rebuilding a cold session costs one
  small read; not clearing costs a large warm read on every remaining turn. Below about
  100 K it is close to break-even and buys quality rather than money.
- **Write anything expensive to re-derive to disk the moment it is derived.** Compaction
  keeps narrative and drops specifics, so identifiers, counts and paths must be written
  down **before** it runs. A summary of a number is a rumour about a number.
- **One session per unit of work**, not per day. Half-finished work on something else is
  the related-but-wrong context that misleads hardest.
- **The degradation signals**: a file gets re-read, an earlier decision gets contradicted,
  or the human re-explains something they already said. At any of those, write a handoff
  note and start fresh.

## Brief a subagent in six fields

An agent cannot ask you a question. It is a competent stranger with perfect recall of
exactly what you wrote and no back channel, so every gap you leave is filled silently
with the most plausible default and you are never told it was filled.

1. **Goal.** What done looks like, in one sentence.
2. **Evidence path.** Where to look. Paths, not bodies.
3. **Negative space.** What not to do, what not to touch, what is out of scope.
4. **Return shape.** The exact format of the answer. Add this one if you add only one: a
   shape is checkable where a procedure is not, and asking for `path:line` in the return
   changes what the agent collects from the first file it opens.
5. **Completion criterion.** Done when. The most omitted field, and it fails both ways:
   without it an agent stops at the first plausible answer, or expands into scope nobody
   asked for.
6. **Verification method.** How you will know the result is right. If you cannot name the
   check before dispatching, the task is mis-specified, not ready to run.

A brief you have written three times is a skill. A brief you cannot write means the
thinking is not done.
