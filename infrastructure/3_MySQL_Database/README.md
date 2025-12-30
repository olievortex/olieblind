# MySQL Database
Finally, we create the database, the tables, and a login for the website.

## Create the database

1. Using DBeaver, connect to and expand olieblind-db.
2. Right-Click on Database and select "Create New Database".
3. Database name: olieblind_dev
4. leave Charset and Collation as the default value. Click "OK"

![database](database.png)

## Create the website login
1. Right-Click on Users and select "Create New User"
2. User Name: olieblind_dev_user
3. Host: %
3. Put the password in the Password and Confirm fields.

![properties](properties.png)

4. Leave all DBA Privileges unchecked.
5. Click the "Schema Privileges" tab.
6. Select the "olieblind_dev" Catalog
7. Check All Table Privileges.
8. Uncheck the "Grant option" Table Privilege

![privileges](privileges.png)

9. Click "Save..."
10. When the SQL Preview appears, click "Execute".

## Create the tables
1. In DBeaver, from the menu, select "Open File".
2. Open the tables.sql file in this folder.
3. Execute the SQL script.
4. Refresh the datbase and expand the Tables to confirm they were created.

![tables](tables.png)