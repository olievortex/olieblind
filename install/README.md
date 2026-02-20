# OlieBlind Install
Please first complete all the steps in the /infrastructure/README.md file before continuing here.

With the infrastructure in place, we can now install olieblind to the server. Log into the Linode instance and perform these steps.

## Clone projects
We clone the olieblind project into the correct folder. This project is also dependent upon the olievortex_purple project. We will clone the files into our source folder.

    # mkdir -p ~/source/repos
    # cd ~/source/repos
    # git clone https://github.com/olievortex/olieblind.git
    # git clone https://github.com/olievortex/olievortex_purple.git

## Create symbolic links
Create the olieblind folder. This will contain useful scripts. These scripts are linked to the scripts inside the repo. Also installs a font.

    # ~/source/repos/olieblind/install/create_links.sh

## Confirm Font Installation
Make sure the font "Spicy Rice" appears in the list. This font is used in the video and weather map production.

    # fc-list | grep Spicy

## Create install/log directories

    # sudo ~/source/repos/olieblind/install/create_directories.sh

## Link to the mount
If the mount is newly formatted, first add the videos folder.

     # cd /mnt/olieblind-video
     # sudo mkdir videos
     # sudo chown olievortex:olievortex videos

Next, link the website to the mount

     # cd /var/www
     # sudo ln -s /mnt/olieblind-video/videos videos
     # chcon -R -t httpd_sys_content_t /mnt/olieblind-video/videos

## Google Service Account Key
We now need to create a Google Service Account key so we can call the text-to-speech API. Log into the Google Cloud, select your project, and navigate to APIs & Services -> Credentials. Click on the Serice Account you want to use. To generate a new key:

- Click on the Keys tab
- Click on "Add key" -> "Create new key"
- Select Key type "JSON"
- Click "Create"
- Your browser should automatically download the json file.

Using WinSCP, make a connection to your Linode.

- Copy the json file to ~/olieblind

When you update the sourcing file in the next step, set the GOOGLE_APPLICATION_CREDENTIALS to the path of this file.

Secure the file and prevent alterations.

    # chmod go-r virtualstormchasing-*.json
    # chmod u-w virtualstormchasing-*.json

## Create and parameterize the environment sourcing script
The source_template.sh script is copied into the olieblind folder. This script is called by other scripts to load the proper environment variables.

    # cp ~/source/repos/olieblind/install/source_template.sh ~/olieblind
    # cat ~/olieblind/source_template.sh

Replace the placeholder values within the file and then rename it from **source_template.sh** to **source.sh**. There are instructions within the file on what to do.

    # mv source_template.sh source.sh

Secure the file to prevent inadvertant revelations or alterations.

    # chmod go-rx source.sh
    # chmod u-w source.sh

## MySQL
We will copy some MySQL scripts out of the repository and customize them.

    # cp ~/source/repos/olieblind/scripts/mysql/olieblind_backup.sh ~/olieblind

Replace the asterisks with the appropriate host, port, username, and password for both scripts. Don't wrap the IPv6 adress in brackets. Run both scripts and confirm the sql dump files were created in /var/backup/mysql.

Secure the files to prevent inadvertant revelations or alterations.

    # chmod go-rx olieblind_backup.sh
    # chmod u-w olieblind_backup.sh

## Install olieblind
    # ~/olieblind/deployOlieBlind.sh
    # dotnet dev-certs https --trust

## Validate Google Cloud
The cli has a simple test that lists text-to-speech voices. If this works, we know the credentials are configured correctly.

    # cd ~/olieblind/startOlieBlind.Cli.sh listvoices
    # cat /var/log/olieblind/olieblind.cli.log

Confirm that you see a bunch of voice names and genders in the log file.

## Initialize OlieBind.brown
Ensure we are able to create a Python environment for OlieBlind.brown.

    # cd /opt/olieblind.brown
    # uv python install 3.13
    # uv run main.py

After installing a bunch of packages. you should see "Hello from olieblind.brown!"

## Initialize olievortex_purple
The settings for this project are stored in the .env file. There is a .env_template provided.

Do not put brackets around an IPv6 MySQL host

    # cd /opt/olievortex_purple
    # cp .env_template .env
    # vi .env

Update the appropriate settings and save the file. Now install the packages.

    # uv python install 3.13
    # uv run main.py

Confirm that it prints a friendly hello world message.

Note: When creating a satellite map for the first time, a temporary RAM boost (8GB) is needed to accomodate build the projection mapping cache.


## Validate video process
Run the SPC Day One process, confirm a video was created, and check the log for errors.

    # ~/olieblind/startOlieBlind.Cli.sh spcdayonevideo
    # cat /var/log/olieblind/olieblind.cli.log
    # cd /var/www/videos
    # ls -R | grep mp4

## Validate map process
Run the SPC Day One process, confirm the maps were created, and check the log for errors.

    # ~/olieblind/startOlieBlind.Cli.sh dayonemaps
    # cat /var/log/olieblind/olieblind.cli.log
    # cd /var/www/videos
    # ls -R | grep png

### Validate Olieblind.Api
The deploy script should have started an instance of Olieblind.Api. Send a request to the local server and confirm it returns a valid JSON string without any errors.

    # curl http://localhost:7021/api/video/page

### Validate Olieblind\.Web
The deploy script should have started an instance of Olieblind\.Web. Send a request to the local server and confirm it returns a valid HTML response without any errors.

    # curl http://localhost:7022/

### Crontab
This sets up all the scheduled jobs. Paste the lines from the olievortex.cron file into the crontab editor and save.

    # cat ~/source/repos/olieblind/install/olievortex.cron
    # crontab -e

Verify the schedule took effect. The results of this command should match the olievortex.cron file you copied from.

    # crontab -l

### SELinux Settings
Out of the box Apache isn't allowed to relay network traffic. Nor can it follow a symbolic link. SELinux needs to be configured to allow this.

    # sudo semanage port --add --proto tcp --type http_port_t 7021
    # sudo semanage port --add --proto tcp --type http_port_t 7022
    # sudo setsebool -P httpd_can_network_relay 1
    # #sudo chcon -h -t httpd_sys_content_t /var/www/videos (not sure if this is needed)
    # sudo chcon -Rv --type=httpd_sys_content_t /mnt/olieblind-video/videos
    # sudo semanage fcontext -a -t httpd_sys_content_t "/mnt/olieblind-video/videos(/.*)?"
    # sudo restorecon -Rv /mnt/olieblind-video/videos


### Apache SSL Site
Configure the SSL site to proxy requests to the dotnet applications, and to the videos folder. Copy the relevant lines.

    # cat ~/source/repos/olieblind/infrastructure/5_AkamaiLinode/000-olieblind-2-default-le-ssl.conf
    # sudo vi /etc/httpd/conf.d/000-olieblind-2-default-le-ssl.conf
    # sudo systemctl restart httpd

### Validate startup
Confirm that Apache and the dotnet applications automatically start at boot.

    # sudo reboot

### Validate Video URL
Confirm you can navigate to a video with your browser: https://olieblind-2.olievortex.com/videos/25/08/Day1Outlook2508181235.gif. The image or video should display.

Replace the path in the link with the path to video file you created from the "Validate Olieblind.Cli" step.

### Validate API URL
Confirm you can navigate to a video with your browser: https://olieblind-2.olievortex.com/api/video/page.

Confirm the result contains a valid JSON response without errors.

### Validate Web URL
Confirm you can navigate to a video with your browser: https://olieblind-2.olievortex.com/.

Confirm the result contains a valid web page without errors.

### Update DNS
Add the oliebline-2 IP Addresses to the @.olievortex.com and www.olievortex.com DNS.
