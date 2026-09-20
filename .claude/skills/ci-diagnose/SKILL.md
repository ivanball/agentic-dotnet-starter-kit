---
name: ci-diagnose
description: Use when CI is red, a check failed, or the user asks why the pipeline is failing, why the build broke, or what is wrong with the run, and proactively right after a push or a pull request is opened.
---

# Diagnose a red pipeline

Find the first thing that broke, name it, and propose the smallest fix. A pipeline reports
its failures in cascade order, so the last red line is usually a consequence and the first
one is the cause.

## Steps

1. List the recent runs and find the failing one:

   ```sh
   gh run list --limit 10
   ```

2. Read only the failed portion of the log. Never download the whole log:

   ```sh
   gh run view <run-id> --log-failed
   ```

3. Name the **first** failed step, by its step name, with the first error line under it.
   Say the step name out loud before saying anything about a cause. If three steps are red,
   the other two are almost certainly downstream of the first.

4. Classify the failure into exactly one of these, because the fix differs by class:

   - **Build.** A compiler error, or a warning promoted to an error by warnings-as-errors.
     The fix is in the code the error names. Read that file at that line.
   - **Test.** An assertion failed. Read the assertion message first: a good one names the
     offender. Decide whether the test is right and the code is wrong, which is the usual
     case, or the requirement moved, which needs a human to confirm.
   - **Restore or lock drift.** A locked-mode restore refusing a package the committed lock
     files do not list. Somebody changed a package version without regenerating the locks.
     The fix is to regenerate them and commit the result, not to drop locked mode.
   - **Audit.** A package with a published advisory. The fix is to bump the package, or to
     pin a fixed version of a transitive one by naming it directly. Never suppress it.
   - **Infrastructure.** A service container that never went healthy, a runner that timed
     out, a registry that was unreachable, a step that failed in under a few seconds with
     no output at all. This class is not a code defect: re-run the failed jobs once and
     say that is what you did.

5. Propose the smallest fix that addresses the class you named, and say which acceptance
   the fix restores. One fix per red run. If the fix is not obvious from the log, say what
   you would need to see rather than guessing.

6. Re-verify locally where you can, then push and watch the same check go green. Report the
   run id and the check name, not "it should be fine now".

## Never

- Never weaken a test, delete an assertion, add a skip attribute, lower a threshold,
  narrow an architecture rule, or suppress an analyzer to make a run go green. Those are
  all valid solutions to the problem "make CI pass" and none of them is a solution to the
  problem that made it fail. If you believe a gate is genuinely wrong, say so, leave it
  red, and hand the decision to a human.
- Never disable a workflow, remove a required check, or merge past a red one.
- Never claim a run is green without the run id and the check name to point at.
