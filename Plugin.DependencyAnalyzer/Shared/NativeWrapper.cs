using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Plugin.DependencyAnalyzer.Shared
{
	internal static class NativeWrapper
	{
		/// <summary>Search file</summary>
		/// <param name="fileName">File name to search</param>
		/// <returns>Path to the found file or null if the file is not found</returns>
		public static String SearchPath(String fileName)
		{
			const Int32 MAX_PATH = 255;
			Int32 bufferLength=MAX_PATH;
			String result = SearchPath(fileName, ref bufferLength);

			if(result == null && bufferLength > MAX_PATH)
				result = SearchPath(fileName, ref bufferLength);

			return result;
		}

		/// <summary>Search file</summary>
		/// <param name="fileName">File name to search for</param>
		/// <param name="bufferLength">Buffer size</param>
		/// <returns>Path to file or null if file not found</returns>
		private static String SearchPath(String fileName, ref Int32 bufferLength)
		{
			StringBuilder buffer = new StringBuilder(bufferLength);

			UInt32 hResult = Native.SearchPath(null, fileName, null, buffer.Capacity, buffer, out IntPtr dummy);
			if(hResult == 0)//Not found
			{
				bufferLength = 0;
				return null;
			} else if(hResult > bufferLength)
			{
				bufferLength = (Int32)hResult;
				return null;
			} else
				return buffer.ToString();
		}

		internal static class Native
		{
			/// <summary>Searches for a specified file in a specified path</summary>
			/// <remarks>
			/// https://docs.microsoft.com/en-us/windows/win32/api/processenv/nf-processenv-searchpathw
			/// 
			/// If the lpPath parameter is NULL, SearchPath searches for a matching file based on the current value of the following registry value:
			/// HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\SafeProcessSearchMode
			/// When the value of this REG_DWORD registry value is set to 1, SearchPath first searches the folders that are specified in the system path, and then searches the current working folder.
			/// When the value of this registry value is set to 0, the computer first searches the current working folder, and then searches the folders that are specified in the system path. The system default value for this registry key is 0.
			/// The search mode used by the SearchPath function can also be set per-process by calling the SetSearchPathMode function.
			/// </remarks>
			/// <param name="lpPath">
			/// The path to be searched for the file.
			/// If this parameter is NULL, the function searches for a matching file using a registry-dependent system search path.
			/// For more information, see the Remarks section.
			/// </param>
			/// <param name="lpFileName">The name of the file for which to search.</param>
			/// <param name="lpExtension">
			/// The extension to be added to the file name when searching for the file.
			/// The first character of the file name extension must be a period (.).
			/// The extension is added only if the specified file name does not end with an extension.
			/// If a file name extension is not required or if the file name contains an extension, this parameter can be NULL.
			/// </param>
			/// <param name="nBufferLength">The size of the buffer that receives the valid path and file name (including the terminating null character), in TCHARs.</param>
			/// <param name="lpBuffer">A pointer to the buffer to receive the path and file name of the file found. The string is a null-terminated string.</param>
			/// <param name="lpFilePart">A pointer to the variable to receive the address (within lpBuffer) of the last component of the valid path and file name, which is the address of the character immediately following the final backslash () in the path.</param>
			/// <returns>
			/// If the function succeeds, the value returned is the length, in TCHARs, of the string that is copied to the buffer, not including the terminating null character.
			/// If the return value is greater than nBufferLength, the value returned is the size of the buffer that is required to hold the path, including the terminating null character.
			/// </returns>
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern UInt32 SearchPath(String lpPath,
				String lpFileName,
				String lpExtension,
				Int32 nBufferLength,
				[MarshalAs(UnmanagedType.LPTStr)]
				StringBuilder lpBuffer,
				out IntPtr lpFilePart);

			/// <summary>The search mode to use</summary>
			[Flags]
			public enum BASE_SEARCH_PATH : UInt32
			{
				/// <summary>Enable safe process search mode for the process.</summary>
				ENABLE_SAFE_SEARCHMODE = 0x00000001,
				/// <summary>Disable safe process search mode for the process.</summary>
				DISABLE_SAFE_SEARCHMODE = 0x00010000,
				/// <summary>Optional flag to use in combination with BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE to make this mode permanent for this process</summary>
				/// <remarks>This is done by bitwise OR operation:
				/// (BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE | BASE_SEARCH_PATH_PERMANENT)
				/// This flag cannot be combined with the BASE_SEARCH_PATH_DISABLE_SAFE_SEARCHMODE flag.</remarks>
				PERMANENT = 0x00008000,
			}

			/// <summary>Sets the per-process mode that the SearchPath function uses when locating files</summary>
			/// <remarks>
			/// https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setsearchpathmode
			/// 
			/// If the <c>SetSearchPathMode</c> function has not been successfully called for the current process, the search mode used by the <c>SearchPath</c> function is obtained from the system registry.
			/// For more information, see <c>SearchPath</c>.
			/// After the <c>SetSearchPathMode</c> function has been successfully called for the current process, the setting in the system registry is ignored in favor of the mode most recently set successfully.
			/// If the SetSearchPathMode function has been successfully called for the current process with Flags set to (BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE | BASE_SEARCH_PATH_PERMANENT), safe mode is set permanently for the calling process.
			/// Any subsequent calls to the SetSearchPathMode function from within that process that attempt to change the search mode will fail with ERROR_ACCESS_DENIED from the GetLastError function.
			/// </remarks>
			/// <param name="dwFlags">The search mode to use</param>
			/// <returns>
			/// If the operation completes successfully, the <c>SetSearchPathMode</c> function returns a nonzero value.
			/// If the operation fails, the <c>SetSearchPathMode</c> function returns zero. To get extended error information, call the <c>GetLastError</c> function.
			/// If the <c>SetSearchPathMode</c> function fails because a parameter value is not valid, the value returned by the <c>GetLastError</c> function will be ERROR_INVALID_PARAMETER.
			/// If the <c>SetSearchPathMode</c> function fails because the combination of current state and parameter value is not valid, the value returned by the <c>GetLastError</c> function will be ERROR_ACCESS_DENIED. For more information, see the Remarks section.
			/// </returns>
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern Boolean SetSearchPathMode(BASE_SEARCH_PATH dwFlags);
		}
	}
}