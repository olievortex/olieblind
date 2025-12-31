#!/bin/sh
basePath=~/source/repos/olieblind/src/olieblind.api
pubPath=/opt/olieblind.api
sourceFile=~/olieblind/sourceOlieBlind.sh
logPath=/var/log/olieblind
set -e

echo create folders
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

echo git pull
cd ${basePath}
git pull

echo dotnet clean
rm -rf ${basePath}/bin
rm -rf ${basePath}/obj
cd ${basePath}/..
dotnet clean

echo dotnet build
dotnet build --configuration Release

echo dotnet test
dotnet test --configuration Release --no-restore --no-build

echo dotnet publish
dotnet publish olieblind.api/olieblind.api.csproj --configuration Release --no-restore --no-build

echo deploy
cd ${basePath}/bin/Release/net10.0/publish
tar -cf ../publish.tar *
cd ${pubPath}
rm -rf *
tar -xf ${basePath}/bin/Release/net10.0/publish.tar

echo stop API
~/start/stopOlieBlind.Api.sh

echo start API
~/start/startOlieBlind.Api.sh
