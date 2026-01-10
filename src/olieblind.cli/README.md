The following evironment variables are required to run locally. Replace the dummy values and put these settings in your User Secrets file. 

{
  "OlieMySqlConnection": "server=[2600:3c00::2000:xxxx];port=20237;uid=olieblind_dev_user;pwd=xxxx;database=olieblind_dev;SslMode=Required;",
  "OlieMySqlBackupContainer": "https://xxxx.blob.core.windows.net/mysql-backups",
  "OlieMySqlBackupPath": "C:/workspace/backup/mysql",
  "OlieFfmpegPath": "/Users/oliev/OneDrive/Documents/olievortex/Libraries/windows/ffmpeg",
  "OlieFfmpegCodec": "libx264",
  "OlieFontPath": "/Users/oliev/OneDrive/Documents/fonts/Spicy_Rice/SpicyRice-Regular.ttf",
  "OlieModelForecastPath": "C:/workspace/images",
  "OliePurpleCmdPath": "C:/Users/oliev/source/repos/hop/olieblind/olieblind.purple/shell_win_dev.bat",
  "OlieVideoPath": "C:/workspace/video",
  "OlieBaseVideoUrl": "https://www.olievortex.com/videos",
  "OlieSpeechVoiceName": "en-US-Standard-C",
  "APPLICATIONINSIGHTS_CONNECTION_STRING": "xxxx"
  "OlieBlobBronzeContainerUri": "https://xxxx.blob.core.windows.net/bronze",
  "OlieOlieAwsServiceBus": "xxxx.servicebus.windows.net"
}