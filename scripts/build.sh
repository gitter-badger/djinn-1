#!/bin/bash

startPath=`pwd`

# tabs to spaces
find ./ ! -type d ! -name _tmp_ -exec sh -c 'expand -t 4 {} > _tmp_ && mv _tmp_ {}' \;

# run build
xbuild "source/Sungiant.Djinn/Sungiant.Djinn.csproj" /p:Configuration=Release

cd $startPath

