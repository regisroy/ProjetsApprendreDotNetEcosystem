using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace H3C.Tests.BusinessLayer.Common
{
	public class TestHelper
	{
		public static readonly string DATE_FORMAT = "yyyy/MM/dd";

		public static DateTime? GetDate(string yyyyMMdd)
		{
			if (yyyyMMdd == null)
			{
				return null;
			}

			return DateTime.ParseExact(yyyyMMdd, DATE_FORMAT, CultureInfo.InvariantCulture);
		}

		public static string GetFullFilePathInAssembly(string relativeFilePathInAssembly, bool createIfDoesntExist = false)
		{
			var dirExe = GetExecutionDir();
			var fullFilePath = Path.Combine(dirExe, relativeFilePathInAssembly);
			if (createIfDoesntExist)
			{
				var fileInfo = new FileInfo(fullFilePath);
				if (!fileInfo.Exists)
				{
					fileInfo.Create();
				}
			}

			return fullFilePath;
		}

		public static string GetExecutionDir()
		{
			var uriExe = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
			var dirExe = Path.GetDirectoryName(uriExe.LocalPath);
			Debug.Assert(dirExe != null, "dirExe != null");
			return dirExe;
		}

		public static void ChangeConfigurationManagerFile(string relativeFilePathInAssembly)
		{
			var fullFilePathInAssembly = GetFullFilePathInAssembly(relativeFilePathInAssembly);

			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var appSettings = (AppSettingsSection) config.GetSection("appSettings");
			appSettings.Settings.Clear();
			config.AppSettings.File = fullFilePathInAssembly;
			config.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection("appSettings");
		}

		public static DirectoryInfo CreateTmpDir()
		{
			var executionDir = GetExecutionDir();
			var tmpDir = new DirectoryInfo(Path.Combine(executionDir, Guid.NewGuid().ToString()));
			tmpDir.Create();

			return tmpDir;
		}

		public static string CreateFile(string tmpDir, string fileName, DateTime creationDate = default(DateTime))
		{
			var fullFileName = Path.Combine(tmpDir, fileName);
			{
				File.Create(fullFileName).Close();
			}
			if (creationDate != default(DateTime))
			{
				File.SetCreationTime(fullFileName, creationDate);
			}

			return fullFileName;
		}
	}
}