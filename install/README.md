# OlieBlind Install
With the infrastructure in place, we can now install olieblind to the server. Log into the Linode instance and perform these steps.

## Create symbolic links
Creates ~/olieblind folder and adds links to scripts from within the repo. Also installs a font.

    # ~/source/repos/olieblind/infrastructure/linux/createLinks.sh

## Confirm Font Installation
Make sure the font "Spicy Rice" appears in the list. This font is used in the video and weather map production.

    # fc-list | grep Spicy

### Create install/log directories

    # sudo ~/source/repos/hop/olieblind/infrastructure/linux/createDirectories.sh

### Link to the mount
     # cd /var/www
     # sudo rmdir videos
     # sudo ln -s /mnt/olieblind-video/videos videos
     # chcon -R -t httpd_sys_content_t /mnt/olieblind-video/videos

### Create the environment source script

    # mkdir -p ~/environments
    # cp ~/source/repos/hop/olieblind/infrastructure/linux/sourceOlieBlind_template.sh ~/environments

Replace the placeholder values in the file and then rename it from **sourceOlieBlind_template.sh** to **sourceOlieBlind.sh**. There are instructions in the file on what values to use.

### MySQL
In mysql, grant the necessary permissions for mysqldump.

    # mysql --host=**** --port=**** --user=**** --password=****
    > use mysql
    > grant reload, process on *.* to 'olieblind_user'@'%';
    > grant reload, process on *.* to 'olieblind_dev_user'@'%';
    > flush privileges
    > exit

    # sudo mkdir -p /var/backup
    # sudo chown olievortex:olievortex /var/backup
    # mkdir -p /var/backup/mysql
    # mkdir -p ~/mysql
    # cp ~/source/repos/hop/olieblind/infrastructure/mysql/*.sh ~/mysql
    # chmod a+x ~/mysql/*.sh

Add the appropriate host, port, username, and password. Run both scripts and confirm the sql dump file was created.

### Install olieblind
    # ~/deploy/deployOlieBlind.sh
    # dotnet dev-certs https --trust

### Google CLI
Copy the contents of C:\Users\oliev\AppData\Roaming\gcloud\application_default_credentials.json to ~/environments/virtualstormchasing-de884bb5018e.json

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

