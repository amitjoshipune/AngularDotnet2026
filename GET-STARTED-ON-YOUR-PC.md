# How to get this code into AngularDotnet2026 on your PC

This folder contains the **full monorepo** (stock agent, docs, templates).
It was published here because the cloud agent could not push directly to AngularDotnet2026.

## Step-by-step (Windows — follow in order)

### Step 1 — Download this code to your PC

Open **PowerShell** or **Command Prompt** and run:

```powershell
cd C:\Users\YourName\source\repos
git clone https://github.com/amitjoshipune/angular15apps.git
cd angular15apps
git checkout export/angulardotnet2026-monorepo
```

You now have folder: `angular15apps\AngularDotnet2026\` with all the code.

### Step 2 — Create AngularDotnet2026 repo on your PC

Still in PowerShell:

```powershell
cd C:\Users\YourName\source\repos
git clone https://github.com/amitjoshipune/AngularDotnet2026.git
```

This creates an almost-empty `AngularDotnet2026` folder (only README today).

### Step 3 — Copy the code into it

```powershell
cd C:\Users\YourName\source\repos\AngularDotnet2026
xcopy /E /I /Y ..\angular15apps\AngularDotnet2026\* .
```

### Step 4 — Push to your GitHub

```powershell
git add .
git commit -m "Add monorepo: India Stock Report Agent and product structure"
git push origin main
```

Done. Your code is now on https://github.com/amitjoshipune/AngularDotnet2026

### Step 5 — Run the stock agent

```powershell
cd products\india-stock-report-agent
dotnet run
```

---

## What is a PR and do you need one?

A **Pull Request (PR)** is a GitHub web page where you review changes before merging a branch into `main`.

Because you pushed directly to `main` in Step 4, **you do NOT need a PR** for your first upload.

PRs are useful later when you work on a feature branch:

1. Go to https://github.com/amitjoshipune/AngularDotnet2026
2. GitHub may show a yellow bar: **"Compare & pull request"** — click it
3. Or: **Pull requests** tab → **New pull request** → pick your branch → **Create pull request**

---

## Even easier next time (optional)

GitHub → AngularDotnet2026 → **Settings** → **Collaborators** → add `cursor[bot]`  
Then the cloud agent can push directly and you skip manual copy.

---

## angular15apps PR #1

Close without merging: https://github.com/amitjoshipune/angular15apps/pull/1  
(That work is now in this folder instead.)
