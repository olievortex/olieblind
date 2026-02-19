#!/bin/sh
base_purple=~/source/repos/olievortex_purple/src
pub_purple=/opt/olievortex_purple
base_blind=~/source/repos/olieblind/src
base_blind_brown=${base_blind}/olieblind.brown
base_blind_cli=${base_blind}/olieblind.cli
base_blind_api=${base_blind}/olieblind.api
base_blind_web=${base_blind}/olieblind.web
pub_brown=/opt/olieblind.brown
pub_cli=/opt/olieblind.cli
pub_api=/opt/olieblind.api
pub_web=/opt/olieblind.web
sourceFile=~/olieblind/source.sh
logPath=/var/log/olieblind
set -e

# Pull code
echo olieblind- pull latest
cd ~/source/repos/olieblind
git pull

echo olievortex_purple- pull latest
cd ~/source/repos/olievortex_purple
git pull

# Sanity checks
if [ ! -d ${logPath} ]; then
    echo "The log directory ${logPath} doesn't exist"
    exit 1
fi
if [ ! -d ${pub_purple} ]; then
    echo "The purple directory ${pub_purple} doesn't exist"
    exit 1
fi
if [ ! -d ${pub_brown} ]; then
    echo "The brown directory ${pub_brown} doesn't exist"
    exit 1
fi
if [ ! -d ${pub_cli} ]; then
    echo "The cli directory ${pub_cli} doesn't exist"
    exit 1
fi
if [ ! -d ${pub_api} ]; then
    echo "The api directory ${pub_api} doesn't exist"
    exit 1
fi
if [ ! -d ${pub_web} ]; then
    echo "The web directory ${pub_web} doesn't exist"
    exit 1
fi
if [ ! -x ${sourceFile} ]; then
    echo "The sourcing file ${sourceFile} doesn't exist"
    exit 1
fi

echo
echo olievortex_purple - deploy
cd ${base_purple}
rm -rf ${pub_purple}/__pycache__
cp -r . ${pub_purple}

echo
echo olieblind.brown - deploy
cd ${base_blind_brown}
rm -rf ${pub_brown}/__pycache__
cp -r * ${pub_brown}

echo
echo olieblind - dotnet clean
cd ${base_blind}
dotnet clean
rm -rf ${base_blind_cli}/bin
rm -rf ${base_blind_cli}/obj
rm -rf ${base_blind_api}/bin
rm -rf ${base_blind_api}/obj
rm -rf ${base_blind_web}/bin
rm -rf ${base_blind_web}/obj

echo olieblind - dotnet build
dotnet build --configuration Release

echo olieblind - dotnet test
dotnet test --configuration Release --no-restore --no-build

echo olieblind.cli - dotnet publish
dotnet publish ${base_blind_cli}/olieblind.cli.csproj --configuration Release --no-restore --no-build

echo olieblind.api - dotnet publish
dotnet publish ${base_blind_api}/olieblind.api.csproj --configuration Release --no-restore --no-build

echo olieblind.web - dotnet publish
dotnet publish ${base_blind_web}/olieblind.web.csproj --configuration Release --no-restore --no-build

echo
echo olieblind.cli - stop satelliterequest process
~/olieblind/stop_satelliterequest.sh

echo olieblind.cli - deploy
cd ${base_blind_cli}/bin/Release/net10.0/publish
tar -cf ../publish.tar *
cd ${pub_cli}
rm -rf *
tar -xf ${base_blind_cli}/bin/Release/net10.0/publish.tar

echo
echo olieblind.cli - start satelliterequest process
~/olieblind/start_satelliterequest.sh

echo
echo olieblind.api - stop website
~/olieblind/stop_api.sh

echo olieblind.api - deploy
cd ${base_blind_api}/bin/Release/net10.0/publish
tar -cf ../publish.tar *
cd ${pub_api}
rm -rf *
tar -xf ${base_blind_api}/bin/Release/net10.0/publish.tar

echo olieblind.api - start website
~/olieblind/start_api.sh

echo
echo olieblind.web - stop website
~/olieblind/stop_web.sh

echo
echo olieblind.web - deploy
cd ${base_blind_web}/bin/Release/net10.0/publish
tar -cf ../publish.tar *
cd ${pub_web}
rm -rf *
tar -xf ${base_blind_web}/bin/Release/net10.0/publish.tar

echo
echo solieblind.web - start website
~/olieblind/start_web.sh

echo
echo Wait to see if websites start - 20s
sleep 5
echo Wait to see if websites start - 15s
sleep 5
echo Wait to see if websites start - 10s
sleep 5
echo Wait to see if websites start - 5s
sleep 5

echo
echo List of dotnet processes
ps -eaf | grep dotnet
