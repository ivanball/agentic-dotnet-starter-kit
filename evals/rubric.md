# The five-rule architecture mini rubric

An eval for a codebase rather than for a model. Five rules, scored 0, 1 or 2 each, out
of a possible 10. It exists so that "is this codebase still the shape we said it was"
has an answer somebody can check, rather than an answer somebody remembers.

Two properties make it work, and both are easy to lose. **Every score is anchored to a
`path:line` read in this run**, so a score is a pointer rather than an opinion. And
**a second agent tries to break every score the first one produced**, because a single
pass grades the argument it just wrote.

## The scale, identical for all five rules

| Score | Meaning |
| --- | --- |
| 0 | The rule is broken somewhere in the codebase, or there is no evidence it is followed. |
| 1 | The rule is followed everywhere you looked, but nothing would stop a change that broke it. Convention only. |
| 2 | The rule is followed AND a check enforces it on every merge: a test in the required CI job, an analyzer at error severity, or a build that fails. |

The line between 1 and 2 is a merge gate, not effort. A rule checked by a nightly job,
a script somebody remembers to run, or a code review is a 1. Open the workflow file and
read which check is required before scoring a 2.

## Rule 1: dependency direction

The domain layer depends on nothing but the shared kernel and the base class library.
No persistence package, no web package, no messaging package anywhere below the
application layer.

**Evidence that counts.** The domain project file, showing which package and project
references it actually has. A fitness test asserting the allowed dependency set, and
the CI step that runs it. **Evidence that does not count.** A statement in a README, a
namespace that looks clean, or the absence of a `using` in the one file you opened.

## Rule 2: failures are returned, not thrown

A broken business rule comes back as a failed result carrying a code and a message.
Exceptions are for defects and infrastructure faults only.

**Evidence that counts.** The result type itself, one aggregate method returning a
failure for a rule it refuses, and the endpoint or adapter that maps that failure onto
a status code. A test asserting the failing path returns rather than throws. **Does not
count.** A try/catch that converts an exception into a result at the boundary: that is
the pattern this rule exists to replace, and it scores 0.

## Rule 3: handlers are internal, one use case per file

Each command or query lives in its own file with its handler beside it, and the handler
is not public. Nothing outside the application assembly can reach past the abstraction
and call a handler directly.

**Evidence that counts.** A fitness test asserting that every type implementing the
handler interfaces is non-public, and the file listing of one aggregate folder showing
one use case per file. **Does not count.** Most handlers being internal. One public
handler is the one the next endpoint will bind to, which skips every decorator in the
pipeline.

## Rule 4: a test tier per layer

Three tiers exist and all three run in the required check: unit tests over the domain,
fitness tests over the architecture rules, and API tests that drive the real host
against a real database.

**Evidence that counts.** Three test projects, and the CI steps that run each of them
inside the job that protects the branch. **Does not count.** A high coverage number. A
tier that exists in the repository but is not in the required check is a 1 at best,
because nothing stops a merge that breaks it.

## Rule 5: modules meet only through contracts

No assembly of one module references another module's domain, application,
infrastructure or presentation assembly. Cross-module traffic goes through a contracts
project or an event.

**Evidence that counts.** A fitness test naming the forbidden assemblies, and one real
cross-module interaction implemented through the contracts project. **Does not count.**
Modules that do not talk to each other yet. That is a codebase with no evidence, which
is a 0, not a 2: the rule has never been tested by a real need.

## The scoring procedure

Three steps, in this order, and none of them is optional.

1. **Score.** Dispatch the `rubric-scorer` agent with the repository path and the five
   rules. It reads the evidence itself and returns, per rule, a score, a one-line
   justification, a `path:line`, and CONFIRMED or FLAG. It does not total the score and
   it writes nothing.

2. **Verify.** Dispatch the `evidence-verifier` agent with the scorer's claims and the
   paths, **never the scorer's reasoning and never the file bodies**. Its job is to
   disprove each `path:line`. It returns CONFIRMED, DRIFTED or FLAG with counter
   evidence. A verifier that received the argument for a score will confirm the
   argument, which is how this gate turns into a rubber stamp.

3. **Adjudicate.** A human reads every FLAG and every DRIFTED item and decides. The
   verifier's CONFIRMED means "I found nothing", not "there is nothing", so it
   authorizes nothing on its own. Total the scores yourself, or with a script: adding
   five numbers is the one step in this procedure that a model should not be doing.

Run it on a schedule you can keep, record the five scores and the date, and treat a
score that moved down without anybody deciding to move it as the finding it is.

## Using it on your own codebase

Rewrite the five rules to be your rules. The rules above are worth exactly as much as
they describe your system, which is probably not much. What transfers is the shape:
five rules and no more, a scale where the top mark requires a merge gate, evidence
rules written down per rule so the score is checkable, a scorer, an adversarial
verifier that never sees the scorer's reasoning, and a human at the end.
