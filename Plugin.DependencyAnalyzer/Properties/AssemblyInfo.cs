using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("cfb29f05-83eb-41db-b0b7-ae54f8b15863")]
[assembly: System.CLSCompliant(false)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=121")]
#else

[assembly: AssemblyTitle("Plugin.DependencyAnalyzer")]
[assembly: AssemblyDescription(".NET Assembly reference analyzer with Microsoft Automatic Graph Layout and PEReader")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.DependencyAnalyzer")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2016-2024")]
#endif

/*if $(ConfigurationName) == Release (
..\..\..\..\ILMerge.exe /v4  "/out:$(ProjectDir)..\bin\$(TargetFileName)" "$(TargetPath)" "$(TargetDir)PEReader.dll" "$(TargetDir)Microsoft.Msagl.dll" "$(TargetDir)Microsoft.Msagl.Drawing.dll" "$(TargetDir)Microsoft.Msagl.GraphViewerGdi.dll" "/lib:..\..\..\SAL\bin"
)*/

//.NET Core Runtime package store
//https://learn.microsoft.com/en-us/dotnet/core/deploying/runtime-store