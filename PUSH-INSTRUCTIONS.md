# Push this repo to GitHub

The Cursor cloud agent prepared this folder as a **standalone git repository** ready for [AngularDotnet2026](https://github.com/amitjoshipune/AngularDotnet2026).

## One-time push (from your PC)

```powershell
cd AngularDotnet2026
git push -u origin cursor/india-stock-report-agent-0250
```

Then open a PR on GitHub: `cursor/india-stock-report-agent-0250` → `main`

## If remote URL is wrong

```powershell
git remote set-url origin https://github.com/amitjoshipune/AngularDotnet2026.git
git push -u origin cursor/india-stock-report-agent-0250
```

## angular15apps cleanup

1. **Do not merge** https://github.com/amitjoshipune/angular15apps/pull/1
2. Close that PR with comment: "Moved to AngularDotnet2026 monorepo"
3. Keep ShoppingBuddy work in angular15apps until you migrate it to `products/shopping-buddy/`

## Grant Cursor access (optional)

Repo → Settings → Collaborators → add `cursor[bot]` so future cloud agents can push directly.
