using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("cfb29f05-83eb-41db-b0b7-ae54f8b15863")]
[assembly: System.CLSCompliant(false)]

[assembly: AssemblyDescription(".NET Assembly reference analyzer with Microsoft Automatic Graph Layout and PEReader")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2016-2025")]

/*if $(ConfigurationName) == Release (
..\..\..\..\ILMerge.exe /v4  "/out:$(ProjectDir)..\bin\$(TargetFileName)" "$(TargetPath)" "$(TargetDir)PEReader.dll" "$(TargetDir)Microsoft.Msagl.dll" "$(TargetDir)Microsoft.Msagl.Drawing.dll" "$(TargetDir)Microsoft.Msagl.GraphViewerGdi.dll" "/lib:..\..\..\SAL\bin"
)*/

//.NET Core Runtime package store
//https://learn.microsoft.com/en-us/dotnet/core/deploying/runtime-store