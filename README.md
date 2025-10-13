# Plugin.DependencyAnalyzer
[![Auto build](https://github.com/DKorablin/Plugin.DependencyAnalyzer/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/Plugin.DependencyAnalyzer/releases/latest)

Plugin.DependencyAnalyzer helps you quickly understand what your Windows or .NET application truly depends on without digging through code or configuration files.

What it does for you:
- Lists every library your app loads (managed assemblies and native DLLs), including indirect (transitive) ones
- Flags missing, unexpected, or version‑mismatched dependencies early
- Highlights unused public members in shared libraries so you can slim deployments
- Visualizes dependency chains as an interactive graph that is easy to navigate
- Distinguishes system, GAC, third‑party, and solution assemblies for clarity

Why it’s useful:
- Speeds up troubleshooting “File not found” or binding failures
- Reduces application size by identifying dead, unused API surface
- Assists planning library refactors or modularization
- Increases confidence before shipping or migrating to a newer .NET version
- Makes audits and dependency reviews straightforward

How you use it:
1. Point the plugin at your application root, a folder of assemblies, or a single DLL / EXE.
2. Choose recursion depth (just direct references or full chains).
3. View a structured list or open the generated dependency graph.
4. Inspect which assemblies call which members.
5. Export results for documentation or review.

Typical scenarios:
- Cleaning up old helper libraries
- Preparing for a .NET upgrade
- Resolving deployment issues on customer machines
- Verifying third‑party component impact
- Tracing why a large framework DLL is being pulled in

Results you get:
- Clear dependency map
- Unused public member report
- Categorized reference list
- Visual graph for presentations or analysis

Benefit summary:
Less guesswork. Leaner binaries. Faster diagnostics. Clear insight.

No code changes required—pure inspection. Just run, review, act.
