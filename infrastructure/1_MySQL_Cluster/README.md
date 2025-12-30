# MySQL Cluster
Following these steps will result in a usable MySQL instance hosted in Akamai Cloud.

1. Log into Akamai Cloud
2. Navigate to DATABASES -> Databases
3. Click "Create Database Cluster"
4. Fill out the page
    - Cluster Label: olieblind-db
    - Database Engine: MySQL v8
    - Region: US, Dallas, TX (us-central)
    - Shared CPU
    - Nanode 1 GB
    - Set Number of Nodes: 1 Node
    - Manage Access: Specific Access (recommended)
    - Allowed IP Addresses or Ranges: Put in your public IP address
    - VPC: leave blank
5. Click "Create Database Cluster"
6. You should now see a screen with Connection Details. Make note of the Username, Password, Host, and Port.
