using System;
using System.Reflection;
using System.Text;

namespace Plugin.DependencyAnalyzer
{
	internal static class AssemblyNameExtensions
	{
		public static String GetPublicKeyTokenString(this AssemblyName assemblyName)
		{
			Byte[] token = assemblyName.GetPublicKeyToken();
			return token == null
				? null
				: ByteArrayToHexString(token);
		}
		private static String ByteArrayToHexString(Byte[] data)
		{
			StringBuilder result = new StringBuilder(data.Length * 2);
			const String HexAlphabet = "0123456789ABCDEF";

			foreach(Byte b in data)
			{
				result.Append(HexAlphabet[(Int32)(b >> 4)]);
				result.Append(HexAlphabet[(Int32)(b & 0xF)]);
			}

			return result.ToString();
		}
	}
}