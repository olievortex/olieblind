#!/bin/sh
. ~/olieblind/source.sh
cd /opt/olieblind.cli
nohup dotnet olieblind.cli.dll satelliterequest >> /var/log/olieblind/olieblind.cli.log 2>&1 &
