# git
git pull

# build
xbuild /p:Configuration=Release Core.sln

# nuget version
for x in *.nuspec
do
	perl -pi.bak -e 's/<version>([0-9]+)\.([0-9]+)\.([0-9]+)<\/version>/"<version>$1.$2.".($3+1)."<\/version>"/ge;' "$x"
done
MAIN_VERSION=$(grep "<version>" Core.Common.nuspec | perl -n -e 's/[^0-9.]+//g; print')

# pack
rm -rf nuget-out
mkdir nuget-out
for x in *.nuspec
do
	nuget pack "$x" -Prop Configuration=Release -OutputDirectory nuget-out
done

# git
git add --all
git commit -a -m "nuget release $MAIN_VERSION"
git push

# push
for x in nuget-out/*.nupkg
do
	nuget push "$x"
done

