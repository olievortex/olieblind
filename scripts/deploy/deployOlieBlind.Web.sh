#!/bin/sh
basePath=~/source/repos/olieblind/src/olieblind.web
pubPath=/opt/olieblind.web
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

echo olieblind.web - dotnet clean
cd ${basePath}
rm -rf ${basePath}/bin
rm -rf ${basePath}/obj
cd ${basePath}/..
dotnet clean

echo olieblind.web - dotnet build
dotnet build --configuration Release

echo olieblind.web - dotnet test
dotnet test --configuration Release --no-restore --no-build

echo olieblind.web - dotnet publish
dotnet publish olieblind.web/olieblind.web.csproj --configuration Release --no-restore --no-build

echo olieblind.web - stop website
~/olieblind/stop_web.sh

echo olieblind.web - deploy
cd ${basePath}/bin/Release/net10.0/publish
tar -cf ../publish.tar *
cd ${pubPath}
rm -rf *
tar -xf ${basePath}/bin/Release/net10.0/publish.tar

echo solieblind.web - tart website
~/olieblind/start_web.sh
