using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using Plugin.DependencyAnalyzer;

namespace ExecInfo.Dal.Documentation
{
	internal class XmlConfigBindingReader
	{
		public struct AssemblyIdentity
		{
			public String Name;
			public String PublicKeyToken;
			public String Culture;
		}

		public class BindingRedirect
		{
			public String VersionFrom;
			public Version VersionTo;
		}

		private readonly XmlDocument _configFile;
		private List<KeyValuePair<AssemblyIdentity, BindingRedirect>> _redirectCache;
		private Object _lock = new Object();

		public XmlConfigBindingReader(Assembly assembly)
		{
			if(assembly.GlobalAssemblyCache)
				throw new InvalidOperationException($"Assembly: {assembly} from GAC");

			String path = GetXmlPath(assembly.Location)
				?? GetXmlPath(new Uri(assembly.CodeBase).LocalPath)
				?? throw new FileNotFoundException(".config file not found for assembly");

			XmlDocument document = new XmlDocument();
			document.Load(path);
			_configFile = document;
		}

		public BindingRedirect FindBinding(AssemblyName assemblyName)
		{
			String publicKeyToken = assemblyName.GetPublicKeyTokenString();

			BindingRedirect result = this.FindBinding(assemblyName.Name, publicKeyToken);
			return result;
		}

		public BindingRedirect FindBinding(String name, String publicKeyToken)
		{
			if(_redirectCache == null)
				lock(_lock)
					if(_redirectCache == null)
						_redirectCache = XmlConfigBindingReader.ReadBindingRedirectSection(_configFile);

			foreach(var item in _redirectCache)
				if(item.Key.Name == name
					&& item.Key.PublicKeyToken == publicKeyToken)
					return item.Value;

			return null;
		}

		private static String GetXmlPath(String assemblyLocation)
		{
			String path = Path.GetDirectoryName(assemblyLocation);

			String appConfig = Path.Combine(path, Path.GetFileName(assemblyLocation) + ".config");
			String webConfig = Path.Combine(path, "web.config");

			Boolean isAppExists = File.Exists(appConfig);
			Boolean isWebExists = File.Exists(webConfig);

			if(isAppExists && isWebExists)
				return null;
			else if(isAppExists)
				return appConfig;
			else if(isWebExists)
				return webConfig;
			else
				return null;
		}

		private static List<KeyValuePair<AssemblyIdentity, BindingRedirect>> ReadBindingRedirectSection(XmlDocument document)
		{
			const String xNamespace = "urn:schemas-microsoft-com:asm.v1";
			XmlNamespaceManager nameSpaceManager = new XmlNamespaceManager(document.NameTable);
			nameSpaceManager.AddNamespace("ns", xNamespace);

			XPathNavigator navigator = document.CreateNavigator();
			XPathNodeIterator nodes = navigator.Select("/configuration/runtime/ns:assemblyBinding/ns:dependentAssembly", nameSpaceManager);
			List<KeyValuePair<AssemblyIdentity, BindingRedirect>> result = new List<KeyValuePair<AssemblyIdentity, BindingRedirect>>();

			if(nodes.Count > 0)
				foreach(XPathNavigator node in nodes)
				{
					XPathNavigator assemblyIdentity = node.SelectChildren("assemblyIdentity", xNamespace).Cast<XPathNavigator>().First();
					XPathNavigator bindingRedirect = node.SelectChildren("bindingRedirect", xNamespace).Cast<XPathNavigator>().First();

					AssemblyIdentity identity = new AssemblyIdentity()
					{
						Name = assemblyIdentity.GetAttribute("name", xNamespace),
						PublicKeyToken = assemblyIdentity.GetAttribute("publicKeyToken", xNamespace),
						Culture = assemblyIdentity.GetAttribute("culture", xNamespace),
					};
					BindingRedirect redirect = new BindingRedirect()
					{
						VersionFrom = bindingRedirect.GetAttribute("oldVersion", xNamespace),
						VersionTo = new Version(bindingRedirect.GetAttribute("newVersion", xNamespace)),
					};

					result.Add(new KeyValuePair<AssemblyIdentity, BindingRedirect>(identity, redirect));
				}

			return result;
		}
	}
}