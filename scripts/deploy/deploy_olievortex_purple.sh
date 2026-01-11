#!/bin/sh
basePath=~/source/repos/olievortex_purple/src
pubPath=/opt/olievortex_purple
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
if [ ! -x ${sourceFile} ]; then
    echo "The sourcing file ${sourceFile} doesn't exist"
    exit 1
fi

echo git pull
cd ${basePath}
git pull

echo deploy
rm -rf ${pubPath}/__pycache__
cp -r . ${pubPath}
