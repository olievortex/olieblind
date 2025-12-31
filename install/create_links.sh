#!/bin/sh
mkdir -p ~/.local/share/fonts
cd ~/.local/share/fonts
ln --symbolic --force ~/source/repos/olieblind/olieblind.cli/Resources/Mount/SpicyRice-Regular.ttf

mkdir -p ~/olieblind
cd ~/olieblind
ln --symbolic --force ~/source/repos/olieblind/infrastructure/deploymentScripts/deployOlieBlind.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/startOlieBlind.Cli.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/startOlieBlind.Api.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/stopOlieBlind.Api.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/stopOlieBlind.Web.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/startOlieBlind.Web.sh
