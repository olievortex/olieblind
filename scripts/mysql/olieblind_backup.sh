#!/bin/sh
timestamp=$(date +%Y%m%d_%H%M%S)
mysqldump --host=**** --port=**** --single-transaction --no-tablespaces --user=olieblind_user --password=******** --set-gtid-purged=off olieblind > /var/backup/mysql/${timestamp}_olieblind.sql
