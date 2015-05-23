#!/bin/bash

# git
git pull

# nuget restore
nuget restore

# build
#xbuild /p:Configuration=Release Core.sln
#xbuild /p:Configuration=Debug Core.sln

# git
git add --all
git commit -a
git push
