#!/bin/sh
mkdir -p ~/.local/share/fonts
cd ~/.local/share/fonts
ln --symbolic --force ~/source/repos/olieblind/src/olieblind.cli/Resources/Mount/SpicyRice-Regular.ttf

mkdir -p ~/olieblind
cd ~/olieblind
ln --symbolic --force ~/source/repos/olieblind/scripts/deploy/deployOlieBlind.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/startOlieBlind.Cli.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/startOlieBlind.Api.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/stopOlieBlind.Api.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/stopOlieBlind.Web.sh
ln --symbolic --force ~/source/repos/olieblind/scripts/start/startOlieBlind.Web.sh
