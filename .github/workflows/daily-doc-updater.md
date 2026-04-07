---
name: Daily Documentation Updater
description: Reviews recent code changes and updates the README so project documentation stays aligned with the actual repository state.
on:
  schedule: daily
  workflow_dispatch:
permissions:
  contents: read
  issues: read
  pull-requests: read
strict: true
network: defaults
safe-outputs:
  create-pull-request:
    expires: 1d
    title-prefix: "[docs] "
    labels: [documentation, automation]
    draft: false
tools:
  github:
    toolsets: [default]
  edit:
  bash:
    - "git"
    - "find . -maxdepth 4 -type f"
    - "cat README.md"
    - "grep -R Week SimpleShop.Api SimpleShop.Tests README.md || true"
    - "dotnet test SimpleShop.sln --no-restore"
timeout-minutes: 45
---
# Daily Documentation Updater
