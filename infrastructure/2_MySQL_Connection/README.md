# MySQL Connection
We will open DBeaver Community and configure a connection to our MySQL server. Coming up with this correct combination of settings was quite painful. There was no other way to make an IPv6 connection than plugging the cluster's IPv6 address into the Server Host field.

0. Make sure your IPv6 public address is in the Database Clusters Manage Access list:

![Database Cluster Networking](cluster_networking.png)

1. Open DBeaver Community.
2. Click on the "New Database Connection" icon. ![icon](dbeaver_create.png)
3. Select "MySQL" and click "Next".
4. Enter the IPv6 address, port, username, and password.

![DBeaver Server Settings](dbeaver_server.png)

5. Click the ![DBeaver Add SSL button](dbeaver_ssl_button.png) button and select "SSL".
6. Ensure the "Require SSL" is the only checked checkbox.

![DBeaver SSL Settings](dbeaver_ssl.png)

7. Click the "Test Connection..." button and resolve any errors.
8. Click the Finish button.
9. Rename the connection to "olieblind-db". Expand the connection to verify it looks like this:

![DBeaver Connections](dbeaver_connections.png)