#!/bin/sh
#### Replace __cosmos_endpoint__ with the appropriate CosmosDB endpoint from the Azure Portal.
#### It should be in the format https://server.documents.azure.com:443/
export OlieMySqlConnection='server=[::1];port=3306;uid=olieblind_user;pwd=********;database=olieblind'
# export OlieCosmosEndpoint=__cosmos_endpoint__
# export OlieCosmosDatabase=olieblind
#### Replace __language_endpoint__ with the appropriate language endpoint from the Azure Portal.
#### It should be in the format https://server.cognitiveservices.azure.com/
# export OlieLanguageEndpoint=__language_endpoint__
# export OlieSpeechRegion=centralus
#### Replace __speech_resource_id__ with the identifier of the speech service. May need to obtain from the Azure CLI.
#### It should be in the format /subscriptions/guid/resourceGroups/resourceGroup/providers/Microsoft.CognitiveServices/accounts/accountName
# export OlieSpeechResourceId=__speech_resource_id__
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
export OliePurpleCmdPath=/opt/olieblind.purple/shell_fedora.sh
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
export APPLICATIONINSIGHTS_CONNECTION_STRING='__applicationinsights_connection_string__'
export GOOGLE_APPLICATION_CREDENTIALS=/home/olievortex/environments/virtualstormchasing-de884bb5018e.json
export OlieMySqlBackupContainer=https://stolieblinda3dd.blob.core.windows.net/mysql-backups
export OlieMySqlBackupPath=C:/workspace/backup/mysql
export OlieBlobBronzeContainerUri=https://stolieblinda3dd.blob.core.windows.net/bronze
