---
name: spec-first-change
description: Use when asked to add a feature, change behaviour, or start any change bigger than a one-line fix, and proactively when a request names an outcome but not the files it touches.
---

# Write the spec before the code

A request is not a specification. Turn it into one, get it approved, then build against it.
The spec is what the verification step checks against at the end, so write the acceptance
checks before you know how you will satisfy them.

## Steps

1. Write the spec first, in the chat or in a file under `docs/specs/` if the change spans
   more than one session. Four headings, nothing else:
   - **Goal.** One sentence, in the language of the people who asked, naming the observable
     outcome rather than the implementation.
   - **Scope.** The files, modules and layers you expect to touch. Name them before you
     open them; a file you did not expect is a finding later.
   - **Out of scope.** What you are deliberately not doing. Write this even when it feels
     obvious. It is the field that stops a small change from growing a refactor.
   - **Acceptance checks.** Numbered, each one a command, an assertion or an observation
     that can come back true or false. "Works correctly" is not a check. "`POST /tickets`
     with an empty title returns 400 and error code `Tickets.TitleRequired`" is.
2. Ask the questions now. Anything ambiguous in the request goes back to the human before
   any code is written, not halfway through. This is the last point at which asking is cheap.
3. Enter plan mode and produce the plan: the ordered edits, the test that proves each one,
   and the order you will run them in. Do not edit anything yet.
4. Get the plan approved. If the human changes the goal, go back to step 1 and rewrite the
   spec. A plan that no longer matches its spec is worse than no spec.
5. Implement in the order the plan gives. Write the test before the code it covers, and
   never write a test and the implementation it checks in the same pass: a test written
   alongside its code describes the code instead of the requirement and passes by
   construction.
6. Verify against the acceptance checks, one at a time, by running them. Paste the command
   and its output. A check you cannot run is a check that failed.
7. Summarise what changed: the files touched against the files the scope predicted, each
   acceptance check with its result, and anything in the spec you did not do and why.

## Constraints

- If an acceptance check cannot be made to pass, say so and stop. Never relax the check,
  delete it, or rewrite it to describe what the code does instead.
- Anything you build that is not in the scope list is scope creep, however defensible the
  reason. Raise it, do not absorb it.
- If you cannot write the acceptance checks, the change is not understood yet. Say that
  and ask, rather than starting and discovering it in the third file.
