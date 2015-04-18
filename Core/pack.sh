xbuild /p:Configuration=Release Core.sln
rm -rf nuget-out
mkdir nuget-out
nuget pack Core.nuspec -Prop Configuration=Release -OutputDirectory nuget-out
nuget push nuget-out/*.nupkg

exit 0

call msbuild src\Commons.sln /t:Rebuild /p:Configuration=Release;TragetFrameworkVersion=4.5
call sn -R .\src\bin\Commons.Utils.dll .\src\FullKey.snk
call sn -R .\src\bin\Commons.Collections.dll .\src\FullKey.snk
call sn -R .\src\bin\Commons.Json.dll .\src\FullKey.snk
call sn -R .\src\bin\Commons.Pool.dll .\src\FullKey.snk
call src\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe src\bin\Test.Commons.dll /xml src\bin\UnitTestResult.xml
call nuget pack Commons.nuspec -OutputDirectory ".\src\bin"
