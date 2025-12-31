# OlieBlind Install
With the infrastructure in place, we can now install olieblind to the server. Log into the Linode instance and perform these steps.

## Create symbolic links
Creates ~/olieblind folder and adds links to scripts from within the repo. Also installs a font.

    # ~/source/repos/olieblind/install/create_links.sh

## Confirm Font Installation
Make sure the font "Spicy Rice" appears in the list. This font is used in the video and weather map production.

    # fc-list | grep Spicy

### Create install/log directories

    # sudo ~/source/repos/olieblind/install/create_directories.sh

### Link to the mount
If the mount is newly formatted, first add the videos folder.

     # cd /mnt/olieblind-video
     # sudo mkdir videos
     # sudo chown olievortex:olievortex videos

Next, link the website to the mount

     # cd /var/www
     # sudo ln -s /mnt/olieblind-video/videos videos
     # chcon -R -t httpd_sys_content_t /mnt/olieblind-video/videos

### Google Service Account Key
We now need to create a Google Service Account key so we can call the text-to-speech API. Log into the Google Cloud, select your project, and navigate to APIs & Services -> Credentials. Click on the Serice Account you want to use. To generate a new key:

- Click on the Keys tab
- Click on "Add key" -> "Create new key"
- Select Key type "JSON"
- Click "Create"
- Your browser should automatically download the json file.

Using WinSCP, make a connection to your Linode.

- Copy the json file to ~/olieblind

When you update the sourcing file in the next step, set the GOOGLE_APPLICATION_CREDENTIALS to the path of this file.

### Create and parameterize the environment sourcing script
The sourceOlieBlind_template.sh script is copied into the olieblind folder. This script is called by other scripts to load the proper environment variables.

    # cp ~/source/repos/olieblind/install/sourceOlieBlind_template.sh ~/olieblind
    # cat ~/olieblind/sourceOlieBlind_template.sh

Replace the placeholder values within the file and then rename it from **sourceOlieBlind_template.sh** to **sourceOlieBlind.sh**. There are instructions within the file on what to do.

    # mv sourceOlieBlind_template.sh sourceOlieBlind.sh

### MySQL
We will copy some MySQL scripts out of the repository and customize them.

    # cp ~/source/repos/olieblind/scripts/mysql/*.sh ~/olieblind

Replace the asterisks with the appropriate host, port, username, and password for both scripts. Don't wrap the IPv6 adress in brackets. Run both scripts and confirm the sql dump files were created in /var/backup/mysql.

### Install olieblind
    # ~/olieblind/deployOlieBlind.sh
    # dotnet dev-certs https --trust

### Validate Olieblind.Cli
Run the SPC Day One process, confirm a video was created, and check the log for errors.

    # ~/start/startOlieBlind.Cli.sh spcdayonevideo
    # cat /var/log/olieblind/olieblind.cli.log
    # ll /var/www/videos -R

### Validate Olieblind.Api
The deploy script should have started an instance of Olieblind.Api. Send a request to the local server and confirm it returns a valid JSON string without any errors.

    # curl http://localhost:7021/api/video/page

### Validate Olieblind\.Web
The deploy script should have started an instance of Olieblind\.Web. Send a request to the local server and confirm it returns a valid HTML response without any errors.

    # curl http://localhost:7022/

### Crontab
This sets up all the scheduled jobs. Paste the lines from the olievortex.cron file into the crontab editor and save.

    # cat ~/source/repos/hop/olieblind/infrastructure/linux/olievortex.cron
    # crontab -e

Verify the schedule took effect. The results of this command should match the olievortex.cron file you copied from.

    # crontab -l

### Apache SSL Site
Configure the SSL site to proxy requests to the dotnet applications, and to the videos folder. Copy the relevant lines.

    # cat ~/source/repos/hop/olieblind/infrastructure/linux/000-olieblind-2-default-le-ssl.conf
    # sudo vi /etc/httpd/conf.d/000-olieblind-2-default-le-ssl.conf
    # sudo systemctl restart httpd

### SELinux Settings
Out of the box Apache isn't allowed to relay network traffic. SELinux needs to be configured to allow this.

    # sudo semanage port --add --proto tcp --type http_port_t 7021
    # sudo semanage port --add --proto tcp --type http_port_t 7022
    # sudo setsebool -P httpd_can_network_relay 1

### Initialize Python

    # cd /opt/olieblind.purple
    # uv python install 3.13
    # uv run main.py

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
Add the oliebline-2 IP Addresses to the olievortex.com and www.olievortex.com DNS.

=====================================================================

### Conda environment
    # conda create -n olieblind_purple python=3.13
    # conda activate olieblind_purple
    # conda install xarray
    # pip install eccodes
    # pip install cfgrib
    # conda install -c conda-forge metpy
    # conda install -c conda-forge azure-monitor-opentelemetry

