#!/bin/sh
basePath=~/source/repos/olieblind/src/olieblind.api
pubPath=/opt/olieblind.api
sourceFile=~/olieblind/source.sh
logPath=/var/log/olieblind
set -e

if [ ! -d ${logPath} ]; then
    echo "The directory ${logPath} doesn't exist"
    exit 1
fi
if [ ! -d ${pubPath} ]; then
    echo "The directory ${pubPath} doesn't exist"
    exit 1
fi
mkdir -p ${pubPath} 
if [ ! -x ${sourceFile} ]; then
    echo "The sourcing file ${sourceFile} doesn't exist"
    exit 1
fi

echo olieblind.api - dotnet clean
cd ${basePath}
rm -rf ${basePath}/bin
rm -rf ${basePath}/obj
cd ${basePath}/..
dotnet clean

echo olieblind.api - dotnet build
dotnet build --configuration Release

echo olieblind.api - dotnet test
dotnet test --configuration Release --no-restore --no-build

echo olieblind.api - dotnet publish
dotnet publish olieblind.api/olieblind.api.csproj --configuration Release --no-restore --no-build

echo olieblind.api - stop website
~/olieblind/stopOlieBlind.Api.sh

echo olieblind.api - deploy
cd ${basePath}/bin/Release/net10.0/publish
tar -cf ../publish.tar *
cd ${pubPath}
rm -rf *
tar -xf ${basePath}/bin/Release/net10.0/publish.tar

echo olieblind.api - start website
~/olieblind/startOlieBlind.Api.sh
