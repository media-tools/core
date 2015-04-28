"C:\Program Files\Mono\bin\xbuild" /p:Configuration=Release Core.sln


mkdir nuget-out
nuget pack Core.Common.nuspec -Prop Configuration=Release -OutputDirectory nuget-out
nuget pack Core.Common.nuspec -Prop Configuration=Release -OutputDirectory nuget-out
nuget pack Core.Calendar.Google.nuspec -Prop Configuration=Release -OutputDirectory nuget-out
nuget push nuget-out\*.nupkg
pause
