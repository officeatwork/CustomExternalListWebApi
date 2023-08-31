# Description

This is a sample implementation for a service that fulfills the officeatwork Designer & Wizard Custom External List contract.

# Setup

## Prerequisite

1. Create an Azure App Registration in your Tenant
2. Under **Expose an API**
   1. Create an **Application ID URI**. It should be of the form `api://SOME-GUID`
   2. Add the `access_as_user` scope
   3. Under **Authorized client applications**, add the officeatwork Designer and Wizard client application and allow it to access the previously created scope:
      - `0c67871c-ffbc-4b37-bd61-afce12b299f9`
   4. If you plan to use the officeatwork Verifier, also add the officeatwork Verifier client application:
      - `7c87bc6a-481c-474a-b99d-4323f60ad764`
3. Under **API permissions**
   1. Click **Grant admin consent for `your tenant name`** to allow all users in your tenant to get tokens for this App.

## Configure Web API

1. Open the project in Visual Studio Code
2. Open appsettings.json and fill in these variables:
   - Domain - your domain
   - TenantId - your tenant id
   - ClientId - the Client ID of the App Registration created earlier

## Run the Web API

1. Press F5 to run the Web API
   - This will run the ".NET Core Launch (web)" debug target
   - The API is hosted at this URI:
     - https://localhost:5001/api/contacts
2. Verify that the server is working
   - Navigate to the API route in a browser
   - You should see a JSON structure in the browser
   - The HTTPS certificate should be valid
     - If the HTTPS certificate is not valid, try running this command to trust the self-signed certificate:
       - `dotnet dev-certs https --trust`
     - If you continue to have problems, please consult the dotnet documentation:
       - https://docs.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide
3. You should now be able to use the API in the Custom External List Input Field in the officeatwork Designer by using this URL:
   - https://localhost:5001/api/contacts
