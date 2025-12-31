# Azure
We use a script to create all the Azure resources we need. The subscription_id provided in the script is just a random GUID. You must update the script to use your Azure subscription id.

You need to have the az cli installed.

    az login
    ./azure_script.sh

If you are running on Windows, you can run the script inside of the git bash prompt. This should be one of your terminal options inside Visual Studio Code.

## Why not use ARM templates
Ugg. Very fragile. Barely documented. The command line "just works".