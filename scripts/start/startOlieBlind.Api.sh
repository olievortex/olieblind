#!/bin/sh
. ~olievortex/environments/sourceOlieBlind.sh
cd /opt/olieblind.api
nohup dotnet olieblind.api.dll --urls=http://localhost:7021 $1 >> /var/log/olieblind/olieblind.api.log 2>&1 &
