#!/bin/sh
mkdir -p /var/log/olieblind
sudo chown olievortex:olievortex /var/log/olieblind

mkdir -p /var/backup
chown olievortex:olievortex /var/backup
mkdir -p /var/backup/mysql
chown olievortex:olievortex /var/backup/mysql

mkdir -p /opt/olieblind.cli
chown olievortex:olievortex /opt/olieblind.cli
mkdir -p /opt/olieblind.api
chown olievortex:olievortex /opt/olieblind.api
mkdir -p /opt/olieblind.web
chown olievortex:olievortex /opt/olieblind.web
mkdir -p /opt/olieblind.purple
chown olievortex:olievortex /opt/olieblind.purple
