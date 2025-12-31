#!/bin/sh
mkdir -p ~/.local/share/fonts
cd ~/.local/share/fonts
ln --symbolic --force ~/source/repos/olieblind/olieblind.cli/Resources/Mount/SpicyRice-Regular.ttf

mkdir -p ~/deploy
cd ~/deploy
ln --symbolic --force ~/source/repos/olieblind/infrastructure/deploymentScripts/deployOlieBlind.sh

mkdir -p ~/start
cd ~/start
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/startOlieBlind.Cli.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/startOlieBlind.Api.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/stopOlieBlind.Api.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/stopOlieBlind.Web.sh
ln --symbolic --force ~/source/repos/olieblind/infrastructure/startScripts/startOlieBlind.Web.sh
