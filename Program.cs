using System.IO;
using System.Linq;

namespace EasyTransformConfig
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory());
            var csprojPath = files.FirstOrDefault(x => x.Contains(".csproj")) ?? "";

            if (string.IsNullOrWhiteSpace(csprojPath))
                // write an error message
                return;

            if (!File.Exists("App.config"))
                File.WriteAllText("App.config", TextProvider.configContent);
            if (!File.Exists("App.Release.config"))
                File.WriteAllText("App.Release.config", TextProvider.configContent);
            if (!File.Exists("App.Debug.config"))
                File.WriteAllText("App.Debug.config", TextProvider.configContent);

            var csproj = File.ReadAllText(csprojPath);
            var backupName = csprojPath.Insert(csprojPath.LastIndexOf('.'), "_backup");
            File.WriteAllText(backupName, csproj);


            var AppConfig = "<None Include=\"App.config\"";
            if (!csproj.Contains(AppConfig))
                csproj = csproj.Insert(csproj.LastIndexOf("<Import"), TextProvider.appConfig);

            csproj = csproj.Insert(csproj.LastIndexOf("</Project>"), TextProvider.targets);

            File.WriteAllText(csprojPath, csproj);
        }

        private class TextProvider
        {
            internal static readonly string appConfig = @"
    <ItemGroup>
           <None Include=""App.config"" />
               <None Include=""App.Debug.config"">
               <DependentUpon>App.config</DependentUpon>
           </None>
           <None Include=""App.Release.config"">
               <DependentUpon>App.config</DependentUpon>
           </None>
    </ItemGroup>
    ";

            internal static readonly string targets = @"
    <UsingTask TaskName=""TransformXml"" AssemblyFile=""$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\Web\Microsoft.Web.Publishing.Tasks.dll"" />
    <Target Name=""BeforeBuild"" Condition=""Exists('App.$(Configuration).config')"">
      <!-- Generate transformed app config and replace it: will get the <runtime> node and assembly bindings properly populated -->
      <TransformXml Source=""App.config"" Destination=""App.config"" Transform=""App.$(Configuration).config"" />
    </Target>
    <Target Name=""AfterBuild"" Condition=""Exists('App.$(Configuration).config')"">
      <!-- Generate transformed app config in the intermediate directory: this will transform sections such as appSettings -->
      <TransformXml Source=""App.config"" Destination=""$(IntermediateOutputPath)$(TargetFileName).config"" Transform=""App.$(Configuration).config"" />
      <!-- Force build process to use the transformed configuration file from now on.-->
      <ItemGroup>
          <AppConfigWithTargetPath Remove=""App.config"" />
          <AppConfigWithTargetPath Include=""$(IntermediateOutputPath)$(TargetFileName).config"">
              <TargetPath>$(TargetFileName).config</TargetPath>
          </AppConfigWithTargetPath>
      </ItemGroup>
    </Target>
    ";

            internal static readonly string configContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
    <appSettings>
    </appSettings>
</configuration>";
        }
    }
}