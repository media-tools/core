nuget restore

"C:\Program Files (x86)\Msbuild\14.0\bin\msbuild" /p:Configuration=Release Core.sln


mkdir nuget-out
nuget pack Core.Common.nuspec -Prop Configuration=Release -OutputDirectory nuget-out
nuget pack Core.Common.nuspec -Prop Configuration=Release -OutputDirectory nuget-out
nuget pack Core.Calendar.Google.nuspec -Prop Configuration=Release -OutputDirectory nuget-out
nuget push nuget-out\*.nupkg
pause
