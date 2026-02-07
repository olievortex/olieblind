#!/bin/sh
basePath=~/source/repos/olieblind/src/olieblind.cli
pubPath=/opt/olieblind.cli
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
if [ ! -x ${sourceFile} ]; then
    echo "The sourcing file ${sourceFile} doesn't exist"
    exit 1
fi

echo olieblind.cli - dotnet clean
cd ${basePath}
rm -rf ${basePath}/bin
rm -rf ${basePath}/obj
cd ${basePath}/..
dotnet clean

echo olieblind.cli - dotnet build
dotnet build --configuration Release

echo olieblind.cli - dotnet test
dotnet test --configuration Release --no-restore --no-build

echo olieblind.cli - dotnet publish
dotnet publish olieblind.cli/olieblind.cli.csproj --no-restore --no-build

echo olieblind.cli - stop satelliterequest process
~/olieblind/stop_cli.sh

echo olieblind.cli - deploy
cd ${basePath}/bin/Release/net10.0/publish
tar -cf ../publish.tar *
cd ${pubPath}
rm -rf *
tar -xf ${basePath}/bin/Release/net10.0/publish.tar
