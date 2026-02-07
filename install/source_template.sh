#!/bin/sh
#### Remove the brackets from the IPv6 address on Linux or it won't resolve
#### It must be wrapped in single quotes
export OlieMySqlConnection='server=[::1];port=3306;uid=olieblind_user;pwd=********;database=olieblind;;SslMode=Required;'
export OlieSpeechVoiceName=en-US-Standard-C
export OlieFfmpegPath=/usr/bin/ffmpeg
export OlieFfmpegCodec=libopenh264
export OlieFontPath=/home/olievortex/.local/share/fonts/SpicyRice-Regular.ttf
export OlieVideoPath=/var/www/videos
export OlieBaseVideoUrl=https://www.olievortex.com/videos
export OlieBlueCors=http://localhost:5164,https://www.olievortex.com,https://olievortex.com
export OlieBlueUrl=https://www.olievortex.com
export OlieCookieConsentVersion=1
export OlieModelForecastPath=/var/www/videos
export OlieBrownCmdPath=/opt/olieblind.brown/shell_fedora.sh
export OliePurpleCmdPath=/opt/olievortex_purple/shell_fedora.sh
#### The following settings are to create tokens from the ar-olieblind Application Registration.
#### Open the ar-olieblind Application Registration in the Azure portal to get the following values.
#### Replace __tenent_id__ with the "Directory (tenent) Id" from the portal
#### Replace __application_id__ with the "Application (client) Id" from the portal
#### To create a secret, navigate to Manage -> Certificates & secrets -> Client secrets
####   Click "New Client Secret". The value replaces __secret__ below.
####   Once you leave this page, you can never see the secret again.
export AZURE_TENANT_ID=__tenent_id__
export AZURE_CLIENT_ID=__application_id__
export AZURE_CLIENT_SECRET=__secret__
#### From the Azure portal, copy the connection string from the appropriate application insights instance.
#### The connection string must be wrapped in single quotes
export APPLICATIONINSIGHTS_CONNECTION_STRING='__applicationinsights_connection_string__'
export GOOGLE_APPLICATION_CREDENTIALS=/home/olievortex/olieblind/xxxx.json
export OlieMySqlBackupContainer=https://xxxx.blob.core.windows.net/mysql-backups
export OlieMySqlBackupPath=C:/workspace/backup/mysql
export OlieBlobBronzeContainerUri=https://xxxx.blob.core.windows.net/bronze
export OlieAwsServiceBus=xxxx.servicebus.windows.net
export OlieSatelliteRequestQueueName=satellite_adhoc
export Authentication__Schemes__OpenIdConnect__ClientSecret=xxxx