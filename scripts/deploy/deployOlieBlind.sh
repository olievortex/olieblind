#!/bin/sh
cd $(dirname $([ -L $0 ] && readlink -f $0 || echo $0))
set -e

./deployOlieBlind.Brown.sh
./deploy_olievortex_purple.sh
./deployOlieBlind.Cli.sh
./deployOlieBlind.Api.sh
./deployOlieBlind.Web.sh
