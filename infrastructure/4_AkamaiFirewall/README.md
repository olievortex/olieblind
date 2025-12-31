# Akamai Firewall
Akamai Cloud provides a free firewall, so let's use it

1. Log into the Akamai Cloud
2. Navigate to Networking - Firewalls
3. Click "Create Firewall"
4. Label: olieblind-firewall
5. Accept the defaults and click "Create Firewall".
6. Now we see the firewalls list. Click in to the olieblind-firewall.
7. Add the following inbound rules
    - Preset: SSH, Add Rule
    - Preset: HTTP, Add Rule
    - Preset: HTTPS, Add Rule

The firewall configuration should look like this:

![firewall configuration](firewall.png)