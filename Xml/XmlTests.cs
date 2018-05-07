using System;
using System.Xml;
using NUnit.Framework;

namespace Xml
{
	[TestFixture]
	public class XmlTests
	{
		[Test]
		public void XmlParseWithDtd()
		{
			string x = @"<?xml version='1.0' encoding='utf-8'?>
<Project ToolsVersion='4.0' DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
	<PropertyGroup>
		<Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
		<Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
	</PropertyGroup>
	<ItemGroup>
		<ProjectReference Include=""..\Features\ChangeTracking\BusinessLayer\PETIT_PROJET.Features.ChangeTracking.BusinessLayer.csproj"">
			<Project><[PETIT_PROJET.Features.ChangeTracking.BusinessLayer.GUID]></Project>
			<Name>PETIT_PROJET.Features.ChangeTracking.BusinessLayer</Name>
		</ProjectReference>
	</ItemGroup>
</Project>
";
			var xmlDocument = new XmlDocument { InnerXml = x };
			Assert.True(true);
		}
	}
}