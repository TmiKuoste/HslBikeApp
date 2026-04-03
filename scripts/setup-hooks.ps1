# Setup Git Hooks
# Run this script once after cloning the repository to enable pre-commit hooks.

Write-Host "Configuring git to use .githooks directory..."
git config core.hooksPath .githooks
Write-Host "Git hooks configured. Pre-commit hook will run build + tests before each commit."
