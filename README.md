# CommunityHub
Library focused digital community hub

The Administration software repo is located at https://github.com/Xenetics/CommunityHubAdminPanel

## Azure
For this project you will need to create a storage account on Azure. Below is a link to a tutorial for doing this that is kept up to date.  
[Azure Tutorial](https://docs.microsoft.com/en-us/azure/storage/common/storage-create-storage-account)  

## Fields to Modify  

### AzureStorageConsole.cs
<b>StorageAccount</b> : Azure storage key can be found in the azure portal after you create storage account	AdminPanel. Its labeled Account1 in this file  
<b>StorageKey</b> : Azure storage key can be found in the azure portal after you create storage account	AdminPanel. Its labeeled Secret1 in this file  
You can also have secondary and tertiary accounts for multiple purposes or backups.  
### PinUtilities.cs
<b>POIContainer</b> : Azure Blob Container for map points of interest  
### QRCode.cs
<b>QRCodeContainer</b> : Azure Blob Container for QR Codes  
### Product.cs
<b>ProductsContainer</b> : Azure Blob Container for Products  
### Events.cs
<b>EventsContainer</b> : Azure Table for calandar events  
### QuizGame.cs
<b>TriviaContainer</b> : Azure Blob Container for Trivia Questions  
### UserUtilities.cs
<b>containerName</b> : Azure Blob Container for Users   
### RestHelper.cs
<b>m_url</b> : Library Sierra server URL	AdminPanel  
<b>m_authSecret</b> : Library Sierra general API Key	AdminPanel  
