#!/bin/sh
mkdir -p /var/www/videos
chown olievortex:olievortex /var/www/videos

mkdir -p /var/log/olieblind
sudo chown olievortex:olievortex /var/log/olieblind

mkdir -p /opt/olieblind.cli
chown olievortex:olievortex /opt/olieblind.cli
mkdir -p /opt/olieblind.api
chown olievortex:olievortex /opt/olieblind.api
mkdir -p /opt/olieblind.web
chown olievortex:olievortex /opt/olieblind.web
mkdir -p /opt/olieblind.purple
chown olievortex:olievortex /opt/olieblind.purple
