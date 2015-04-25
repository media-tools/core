# build
xbuild /p:Configuration=Release Core.sln

# nuget version
for x in *.nuspec
do
	perl -pi.bak -e 's/<version>([0-9]+)\.([0-9]+)\.([0-9]+)<\/version>/"<version>$1.$2.".($3+1)."<\/version>"/ge;' "$x"
done

# pack
rm -rf nuget-out
mkdir nuget-out
for x in *.nuspec
do
	nuget pack "$x" -Prop Configuration=Release -OutputDirectory nuget-out
done

# push
nuget push nuget-out/*.nupkg

