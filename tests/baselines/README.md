# Baselines

Committed captures of the catalog scenarios in `regression-check.yml`, recorded with
`python .claude/skills/regression-check/scripts/capture.py` on a reviewed commit.
Participant captures go to a scratch directory (`--out /tmp/rc/...`); this directory
changes only through a pull request carrying the `baseline-change` label, which the
`baseline-guard` workflow enforces.
