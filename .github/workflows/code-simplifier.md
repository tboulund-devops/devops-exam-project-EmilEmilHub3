---
name: Code Simplifier
description: Reviews recently changed .NET code and creates a PR with safe simplifications that improve clarity without changing behavior.
on:
  
  workflow_dispatch:
permissions:
  contents: read
  issues: read
  pull-requests: read
tracker-id: code-simplifier
strict: true
safe-outputs:
  create-pull-request:
    title-prefix: "[code-simplifier] "
    labels: [refactoring, code-quality, automation]
    expires: 1d
tools:
  github:
    toolsets: [default]
  edit:
  bash:
    - "git"
    - "find SimpleShop.Api SimpleShop.Tests -type f | sort"
    - "dotnet build SimpleShop.sln --no-restore"
    - "dotnet test SimpleShop.sln --no-build --no-restore"
    - "dotnet format --verify-no-changes SimpleShop.sln || true"
timeout-minutes: 45
---
# Code Simplifier Agent
