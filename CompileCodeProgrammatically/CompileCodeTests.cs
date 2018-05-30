using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using NFluent;
using NUnit.Framework;

namespace CompileCodeProgrammatically
{
	[TestFixture]
	public class CompileCodeTests
	{
		private const string SOURCE_STRING_C_SHARP_5 = @"
using System;

namespace HelloWorld
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class HelloWorldClass
	{
		static void Main(string[] args)
		{
			var author=""Régis"";
			Console.WriteLine(""Hello World!"");
			Console.WriteLine("""");
			Console.WriteLine(""Programmatically compiled"");
			Console.WriteLine(""-------------------------"");
			Console.WriteLine(String.Format(""                 ---->{0}"", author));
			//Console.WriteLine($""                 ---->{author}"");
			Console.ReadLine();
		}
	}
}";
		private const string SOURCE_STRING_C_SHARP_6 = @"
using System;

namespace HelloWorld
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class HelloWorldClass
	{
		static void Main(string[] args)
		{
			var author=""Régis"";
			Console.WriteLine(""Hello World!"");
			Console.WriteLine("""");
			Console.WriteLine(""Programmatically compiled"");
			Console.WriteLine(""-------------------------"");
			//Console.WriteLine(String.Format(""                 ---->{0}"", author));
			Console.WriteLine($""                 ---->{author}"");
			Console.ReadLine();
		}
	}
}";

		[Test]
		//https://support.microsoft.com/en-us/help/304655/how-to-programmatically-compile-code-using-c-compiler
		public void CompileCodeProgrammaticallyTest1_BeforeCSharp6()
		{
			//V0 : avant c# 6
			/* //Obsolete way
			CSharpCodeProvider codeProvider = new CSharpCodeProvider();
			ICodeCompiler icc = codeProvider.CreateCompiler();
			*/
			CodeDomProvider compiler = CSharpCodeProvider.CreateProvider("CSharp"); 

			CompilerParameters parameters = new CompilerParameters();
			//parameters.GenerateExecutable = true;
			parameters.OutputAssembly = @"c:\tmp\helloWorld-compiled-programmatically.exe";
			parameters.GenerateExecutable = true;
			//parameters.CompilerOptions = "-langversion:6";
			parameters.IncludeDebugInformation = true;
			parameters.GenerateInMemory = false;
			parameters.TreatWarningsAsErrors = false;
			parameters.WarningLevel = 3;
			parameters.ReferencedAssemblies.Add("System.Runtime.dll");
			
			CompilerResults results = compiler.CompileAssemblyFromSource(parameters, SOURCE_STRING_C_SHARP_5);

			foreach (CompilerError error in results.Errors)
			{
				Console.WriteLine(error);
			}

			Check.That(results.PathToAssembly).IsEqualTo(@"c:\tmp\helloWorld-compiled-programmatically.exe");
			Check.That(results.Errors).CountIs(0);
			Check.That(results.Errors.HasErrors).IsFalse();
			Check.That(results.Errors.HasWarnings).IsFalse();
			Check.That(results.NativeCompilerReturnValue).IsEqualTo(0);
		}

		[Test]
		//https://stackoverflow.com/questions/31639602/using-c-sharp-6-features-with-codedomprovider-rosyln
		public void CompileCodeProgrammaticallyTest1_AfterCSharp6()
		{
			CompilerParameters parameters = new CompilerParameters();
			//parameters.GenerateExecutable = true;
			parameters.OutputAssembly = @"c:\tmp\helloWorld-compiled-programmatically.exe";
			parameters.GenerateExecutable = true;
			//parameters.CompilerOptions = "-langversion:6";
			parameters.IncludeDebugInformation = true;
			parameters.GenerateInMemory = false;
			parameters.TreatWarningsAsErrors = false;
			parameters.WarningLevel = 3;
			parameters.ReferencedAssemblies.Add("System.Runtime.dll");
			
			//V1 : après c#6 roslyn : v1
			//CodeDomProvider compiler = CodeProvider.Value;
			
			//V2 : après c#6 V2 : C:\Program Files (x86)\MSBuild\14.0\Bin\csc.exe
			//Set hardcoded environment variable to set the path to the library
			Environment.SetEnvironmentVariable("ROSLYN_COMPILER_LOCATION", @"C:\Program Files (x86)\MSBuild\14.0\Bin", EnvironmentVariableTarget.Process);
			//Create compiler object
			CSharpCodeProvider compiler = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
			//Clean up
			Environment.SetEnvironmentVariable("ROSLYN_COMPILER_LOCATION", null, EnvironmentVariableTarget.Process);


			CompilerResults results = compiler.CompileAssemblyFromSource(parameters, SOURCE_STRING_C_SHARP_6);

			foreach (CompilerError error in results.Errors)
			{
				Console.WriteLine(error);
			}

			Check.That(results.PathToAssembly).IsEqualTo(@"c:\tmp\helloWorld-compiled-programmatically.exe");
			Check.That(results.Errors).CountIs(0);
			Check.That(results.Errors.HasErrors).IsFalse();
			Check.That(results.Errors.HasWarnings).IsFalse();
			Check.That(results.NativeCompilerReturnValue).IsEqualTo(0);
		}

		//https://stackoverflow.com/questions/31639602/using-c-sharp-6-features-with-codedomprovider-rosyln
		/*
		static Lazy<CSharpCodeProvider> CodeProvider { get; }
			= new Lazy<CSharpCodeProvider>(() =>
			{
				var csc = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
				var settings = csc.GetType()
				                  .GetField("_compilerSettings", BindingFlags.Instance | BindingFlags.NonPublic)
				                  .GetValue(csc);

				var path = settings
				           .GetType()
				           .GetField("_compilerFullPath", BindingFlags.Instance | BindingFlags.NonPublic);

				path.SetValue(settings, ((string) path.GetValue(settings)).Replace(@"bin\roslyn\", @"roslyn\"));

				return csc;
			});
		*/
	}
}