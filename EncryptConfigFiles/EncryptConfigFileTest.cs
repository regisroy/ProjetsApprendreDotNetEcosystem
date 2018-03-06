using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Xml;
using H3C.Tests.BusinessLayer.Common;
using NFluent;
using NUnit.Framework;

namespace EncryptConfigFiles
{
	[TestFixture]
	public class EncryptConfigFileTest
	{

		[Test]
		[Ignore("testing ideas")]
		public void EncryptConfigFileAppsettings()
		{
			XmlDocument doc;
			var confilePath = TestHelper.GetFullFilePathInAssembly("files/web.safe.config");

			var config = GetConfigFromPath(confilePath);

			//ProtectSection("appSettings", "RSAProtectedConfigurationProvider", config);      //WORKING
			//ProtectSection("safe", "RSAProtectedConfigurationProvider", config);               //WORKING
			//ProtectSection("appSettings/safe", "RSAProtectedConfigurationProvider", config); //NOT WORKING

			//doc = LoadXmlDocument(confilePath);
			//doc.CheckNodeInnerXmlStartWith("/configuration/appSettings", "<EncryptedData ");
//			doc.CheckNodeInnerXmlStartWith("/configuration/system.web/machineKey", "<EncryptedData ");
//			FreeFile();

		}
		
		[Test]
		public void EncryptConfigFile()
		{
			XmlDocument doc;
			var confilePath = TestHelper.GetFullFilePathInAssembly("files/web.config");

			doc = LoadXmlDocument(confilePath);
			doc.CheckNodeInnerXmlStartWith("/configuration/connectionStrings", "<add name=");
			doc.CheckNodeHasAttribute("/configuration/system.web/machineKey", "validationKey");
			FreeFile();

			//Configuration webConfig = GetWebConfig("C:\inetpub\wwwroot\GenerativeObjectsApplications\Sandbox");
			//var config = GetConfigFromPath(@"C:\inetpub\wwwroot\GenerativeObjectsApplications\Sandbox\Web.config");
			var config = GetConfigFromPath(confilePath);
			//Console.WriteLine(config.FilePath);

			ProtectSection("connectionStrings", "RSAProtectedConfigurationProvider", config);
			ProtectSection("system.web/machineKey", "RSAProtectedConfigurationProvider", config);

			doc = LoadXmlDocument(confilePath);
			doc.CheckNodeInnerXmlStartWith("/configuration/connectionStrings", "<EncryptedData ");
			doc.CheckNodeInnerXmlStartWith("/configuration/system.web/machineKey", "<EncryptedData ");
			FreeFile();

			UnProtectSection("system.web/machineKey", config);
			UnProtectSection("connectionStrings", config);

			doc = LoadXmlDocument(confilePath);
			doc.CheckNodeInnerXmlStartWith("/configuration/connectionStrings", "<add name=");
			doc.CheckNodeHasAttribute("/configuration/system.web/machineKey", "validationKey");
		}
		
		[Test]
		public void DecryptConfigFile_fromAnotherServer()
		{
			XmlDocument doc;
			var confilePath = TestHelper.GetFullFilePathInAssembly("files/web.encrypt.config");

			doc = LoadXmlDocument(confilePath);
			doc.CheckNodeInnerXmlStartWith("/configuration/connectionStrings", "<EncryptedData ");
			FreeFile();

			var config = GetConfigFromPath(confilePath);

			UnProtectSection("connectionStrings", config);

			doc = LoadXmlDocument(confilePath);
			doc.CheckNodeInnerXmlStartWith("/configuration/connectionStrings", "<add name=");
		}

		private static XmlDocument LoadXmlDocument(string confilePath)
		{
			XmlDocument doc;
			doc = new XmlDocument();
			doc.Load(confilePath);
			return doc;
		}

		private static void FreeFile()
		{
			XmlDocument doc;
			doc = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		private void ProtectSection(string sectionName, string provider, Configuration config)
		{
			ConfigurationSection section = config.GetSection(sectionName);

			//if (section != null && !section.SectionInformation.IsProtected)
			if (section != null)
			{
				section.SectionInformation.ProtectSection(provider);
				config.Save();
			}
		}

		private void UnProtectSection(string sectionName, Configuration config)
		{
			ConfigurationSection section = config.GetSection(sectionName);

//			if (section != null && section.SectionInformation.IsProtected)
			if (section != null)
			{
				section.SectionInformation.UnprotectSection();
				config.Save();
			}
		}

		private static Configuration GetConfigFromPath(string configFilePath)
		{
			ExeConfigurationFileMap map = new ExeConfigurationFileMap {ExeConfigFilename = configFilePath};
			Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			return config;
		}

		public static Configuration GetWebConfig(string wwwPath)
		{
			var webConfigFile = new FileInfo("Web.config");
			var vdm = new VirtualDirectoryMapping(wwwPath, true, webConfigFile.Name);
			var wcfm = new WebConfigurationFileMap();
			wcfm.VirtualDirectories.Add("/", vdm);
			var siteName = "Sandbox";
			return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/", siteName);
		}
	}

	public static class Extensions
	{
		public static XmlElement GetNode(this XmlDocument doc, string nodePath)
		{
			XmlElement node = doc.SelectSingleNode(nodePath) as XmlElement;
			return node;
		}

		public static void CheckNodeHasAttribute(this XmlDocument doc, string nodePath, string attribute)
		{
			var node = doc.GetNode(nodePath);
			Check.That(node).IsNotNull();
			Check.That(node.HasAttribute(attribute)).IsTrue();
		}

		public static void CheckNodeHasNoAttribute(this XmlDocument doc, string nodePath, string attribute)
		{
			var node = doc.GetNode(nodePath);
			Check.That(node).IsNotNull();
			Check.That(node.HasAttribute(attribute)).IsFalse();
		}

		public static void CheckNodeInnerXmlStartWith(this XmlDocument doc, string nodePath, string expected)
		{
			var node = doc.GetNode(nodePath);
			Check.That(node).IsNotNull();
			Check.That(node.InnerXml).StartsWith(expected);
		}
	}
}