#!/bin/sh
. ~olievortex/environments/sourceOlieBlind.sh
cd /opt/olieblind.web
nohup dotnet olieblind.web.dll --urls=http://localhost:7022 $1 >> /var/log/olieblind/olieblind.web.log 2>&1 &
