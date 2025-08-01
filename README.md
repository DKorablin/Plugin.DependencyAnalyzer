# Plugin.DependencyAnalyzer
[![Auto build](https://github.com/DKorablin/Plugin.DependencyAnalyzer/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/Plugin.DependencyAnalyzer/releases/latest)

Plugin.DependencyAnalyzer is a diagnostic plugin designed to inspect and visualize binary-level dependencies in .NET Framework and native Windows applications.

It analyzes assemblies and native modules to uncover both direct and transitive dependencies — including AssemblyRef, DllImport, GAC-resolved references, and system libraries. With recursive analysis support, it maps full reference chains and renders interactive graphs using Microsoft’s Automatic Graph Layout engine.

Additionally, the plugin can identify unused public members in shared libraries by scanning how dependent assemblies access them. This feature helps reduce binary bloat, isolate dead code, and guide API refactoring.

With configurable options for recursion depth, search scope, and visual styling, Plugin.DependencyAnalyzer provides developers and architects a practical way to audit and optimize large .NET projects with complex interdependencies.