#!/bin/sh
mkdir -p ~/.local/share/fonts
cd ~/.local/share/fonts
ln --symbolic --force ~/source/repos/olieblind/src/olieblind.cli/Resources/Mount/SpicyRice-Regular.ttf

mkdir -p ~/olieblind
cd ~/olieblind
chmod go-rx .
ln --symbolic --force ~/source/repos/olieblind/scripts/deploy/deploy.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/start_api.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/start_cli.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/start_satelliterequest.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/start_web.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/stop_api.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/stop_satelliterequest.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/stop_web.sh
