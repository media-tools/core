xbuild /p:Configuration=Release Core.sln
mkdir nuget-out
nuget pack Core.nuspec -Prop Configuration=Release -OutputDirectory nuget-out
nuget push nuget-out\*.nupkg
