#!/bin/sh
olieblind_path=~/source/repos/olieblind
oliepurple_path=~/source/repos/olievortex_purple

echo olieblind- pull latest
cd ${olieblind_path}
git pull

echo olievortex_purple- pull latest
cd ${oliepurple_path}
git pull
