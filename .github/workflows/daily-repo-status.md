---
name: Daily Repo Status
description: Creates a daily GitHub issue with a short status report about recent repository activity.
on:
  schedule: daily
  workflow_dispatch:
permissions:
  contents: read
  issues: read
  pull-requests: read
network: defaults
tools:
  github:
    lockdown: false
    min-integrity: none
safe-outputs:
  mentions: false
  allowed-github-references: []
  create-issue:
    title-prefix: "[repo-status] "
    labels: [report, daily-status]
    close-older-issues: true
---
# Daily Repo Status

Create an upbeat but practical daily status report for the repository as a GitHub issue.

## Repository context
- Repository: `${{ github.repository }}`
- Project: SimpleShop, a small ASP.NET Core / EF Core / MySQL e-commerce backend
- Main workflows already present: CI and Delivery Pipeline

## What to include
- Recent repository activity from roughly the last 24 hours:
  - commits
  - open and newly updated pull requests
  - open and newly updated issues
  - workflow failures or notable workflow activity when visible
- Brief progress summary tied to the current state of the repo
- Short list of practical next steps for the maintainer

## Focus points for this repository
Pay special attention to:
- API feature progress in `SimpleShop.Api/`
- test health in `SimpleShop.Tests/`
- workflow and deployment health in `.github/workflows/`
- whether README feature plan and implemented code seem aligned

## Style
- Positive, concise, and helpful
- Use light emoji only when it improves readability
- Do not invent activity that you cannot verify
- If activity is low, say so plainly and keep the report short

## Process
1. Gather recent repository activity.
2. Read enough repository context to understand what changed.
3. Summarize the current state in a useful maintainer-facing report.
4. Create a new GitHub issue with the report.
