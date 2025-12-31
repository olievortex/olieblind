# olieblind
Generate weather content for the visually impaired. This website publishes narrated videos containing AI narraged weather products and visualizations from official weather sources.

# Development Environment
The following software is recommended for the development environment:

- [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
- [Visual Studio Code](https://code.visualstudio.com/)
- [uv](https://github.com/astral-sh/uv) (Python package manager)
- [DBeaver Community](https://dbeaver.io/) (Database management)
- [PuTTY](https://www.chiark.greenend.org.uk/~sgtatham/putty/) (SSH client)
- [WinSCP](https://winscp.net/eng/index.php) (Copy files between local computer and remote server)
- [Postman](https://www.postman.com/) (API testing)
- [Azure CLI 64-bit](https://learn.microsoft.com/en-us/cli/azure/?view=azure-cli-latest) (CLI for creating Azure resources)

# Installation
1. Follow the steps in the README.md file in the infrastructure subfolder.

# Projects
- olieblind.api: A REST API to support the website. (ASP.NET Core)
- olieblind.cli: A command line interface to run scheduled tasks. (.NET Console App)
- olieblind.data: Provides repositories and defines entities for the other projects. (.NET Library)
- olieblind.lib: Contains all the orchestration and business logic. (.NET Library)
- olieblind.purple: Python scripts for generating the weather maps
- olieblind.text: Unit tests (NUnit)
- olieblind.web: The website front-end to all this madness (ASP.NET Core)

# Infrastructure
For this demo, you will need an account for Azure Cloud, Akamai Cloud, and Google Cloud.

After two years of struggling with Azure Functions, Azure App Services, Azure Service Bus, Azure Cognative Services, Azure Blob Storage, Pipelines, and Azure Container Instances, I am throwing my hands into the air and moving on. Azure Cloud is one rabbit hole after another, sucking my time and will-to-live. There is always a gotcha. There is always a change to the infrastructure that breaks something that worked yesterday. There is no support for the little guy. And then there's the $$$cost$$$. I'm done with shiny objects. At the end of the day, I need to get work done.

Cloud will only be used in those areas where I saw true value.

### MySql Database (Akamai Cloud)
A single node database to support all the database needs for the project. The connections must be secure, and is protected by an IP white list.

### NVMe block storage service (Akamai Cloud)
I learned the hard way that Akamai Linode backups have several caveats that will prevent a successful restore. Therefore, all generated video/images are now stored on block storage. Now I can rebuild/restore a Linode without losing the genrated content.

### Linode virtual machine (Akamai Cloud)
A Linode 2 GB shared virtual machine running Fedora. It hosts the .NET applications, a Apache Web instances, and Python.

- Python 3.13, with uv package management

- .NET 10 SDK, with local builds.

### Text-to-Speech API (Google Cloud)
The scripts are brokend down into chuncks, submitted to the API, and then stitched together to narrate the videos. The workload falls comfortably within the free tier.

I do like the voices provided by Azure Cognative Services better, but Google has a much more generous free tier, and it isn't throttled to the point of uselessness like Azure.

### Application Insights (Azure Cloud)
All logging is stored in Application Insights, including the Python scripts. It also collects telemetry from the .NET web instances. The user tracking features are not implemented on the website.

Alerts for unhandled exceptions and uptime monitoring are implemented.

### Blob Storage (Azure Cloud)
Full database backup files are stored in a container on Azure Blob Storage. A lifecycle rule deletes backups older than 30 days. These backups allow me to easily spawn a new database instance for testing purposes. It also gives me piece of mind I can restore a database from scratch, since cloud offerings aren't always reliable. 

### DNS Zone (Microsoft Azure)
DNS resolution costs about 50 cents per month. The interface is professional, and the resource locks prevent me from accidently nuking the zone.

Akamia Cloud offers free DNS so long as you are hosting a Linode. I'm not a fan of vendor lock-in.

### Domain Registration (HostGator)
Good comprimise between cost and professionalism. Provides a domain privacy add-on and domain locks.
