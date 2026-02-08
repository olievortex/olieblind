#!/bin/sh
. ~/olieblind/source.sh
cd /opt/olieblind.cli
dotnet olieblind.cli.dll $1 $2 $3 >> /var/log/olieblind/olieblind.cli.log 2>&1
