# pim
Simple edit UI for your backend.

http://pim-pn.azurewebsites.net

![Screenshot](https://github.com/whyleee/pim/blob/master/screenshot.jpg)

## Features
- Basic create/update/delete flow
- Multiple external backends
- Authorization with HTTP headers
- Supported fields:
  - Checkbox
  - Datetime
  - Select many (checkbox list)
  - Select one (select list)
  - Text
  - Textarea
- Complex types:
  - Nested object
  - Table
- UI features:
  - Tabs
  - Validation
  - Unsaved changes indicator
  - Page leave alert on unsaved changes

## Boot Up
1. Start backend:
   - *IDE*: run `App` project (`Ctrl+F5` in Visual Studio)
   - *Command-line*: `dotnet run` in `App` directory
2. Start frontend in the command-line:
   ```
   cd App/ClientApp
   yarn
   yarn serve
   ```

## External backend (C#)
1. Install [Pim.Meta](https://www.nuget.org/packages/Pim.Meta/) NuGet package:
   ```powershell
   Install-Package Pim.Meta
   ```
2. Add controller to provide meta API:
   ```csharp
   [Route("api/[controller]")]
   public class MetaController : Controller
   {
       private readonly MetadataProvider _metadataProvider = new MetadataProvider();
 
       [HttpGet("{itemType}")]
       public ItemTypeInfo GetTypeInfo(string itemType)
       {
           return _metadataProvider.GetTypeInfo(typeof(Product));
       }
   }
   ```
3. Add backend to `App/ClientApp/src/config.json` with data URL pointing to REST CRUD API:
   ```json
   {
     "backends": [
       {
         "key": "products",
         "title": "Products",
         "meta": {
           "url": "https://external-backend.com/api/meta"
         },
         "data": {
           "url": "https://external-backend.com/api/products"
         }
       }
     ]
   }
   ```

## Config Reference
The list of available options in `App/ClientApp/src/config.json`:

#### backend.key (required)
URL-friendly identifier of the backend.

#### backend.title (required)
Human friendly name of the backend.

#### backend.authHeader
Name of the HTTP header to use for authorization to backend APIs. User will be prompted for an API key.

#### backend.meta.url (required)
URL to meta API, used to construct edit UI for the backend.

#### backend.data.url (required)
URL to REST CRUD data API (Create-Read-Update-Delete), used to work with data. The API must provide 5 HTTP endpoints:
- `GET %data.url%`: return all backend items as a list (IDs and names are required, other data is ignored)
- `GET %data.url%/{id}`: return full item data by ID
- `POST %data.url%`: create new item in the backend with posted data
- `PUT %data.url%/{id}`: replace item data by ID
- `DELETE %data.url%/{id}`: delete item by ID

#### backend.data.collectionItemsProperty
Name of the response data property containing result collection items from `GET %data.url%` API. By default entire response is treated as a collection.

## Deployment
The app is cloud ready and can be deployed in minutes to any cloud provider.

For example, to deploy the app to Azure: run `Publish...` action for `App` project in Visual Studio, select your Azure account and service and click Publish. The app will be live in several minutes.

## License

Copyright (C) 2018-present Pavlo Niezhentsev

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

C# metadata provider code in [Pim.Meta](/Pim.Meta) folder is licensed under the MIT license.
