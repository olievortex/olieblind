#!/bin/sh
basePath=~/source/repos/olieblind/src/olieblind.brown
pubPath=/opt/olieblind.brown
sourceFile=~/olieblind/sourceOlieBlind.sh
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

echo olieblind.brown - deploy
cd ${basePath}
rm -rf ${pubPath}/__pycache__
cp -r * ${pubPath}
