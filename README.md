# pim
Simple edit UI for your backend. http://pim-pn.azurewebsites.net

![Screenshot](https://raw.githubusercontent.com/whyleee/pim/master/screenshot-1.jpg)

## Features
- Basic create/update/delete flow
- Multiple external backends
- Multiple collections with filters per backend
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
2. Create backend definition model:
   ```csharp
   public class Backend
   {
       [Collection]
       [CollectionQueryFilter("category")]
       [CollectionQueryFilter("market")]
       public IEnumerable<Product> Products { get; set; }
   
       [Collection(Path = "/{productId}/" + nameof(Variants))]
       [CollectionRefFilter("productId", nameof(Products), Required = true)]
       public IEnumerable<Variant> Variants { get; set; }
   }
   ```
3. Add controller to provide meta API:
   ```csharp
   [Route("api/[controller]")]
   public class MetaController : Controller
   {
       private readonly MetadataProvider _metadataProvider = new MetadataProvider();
 
       [HttpGet]
       public BackendInfo GetMeta()
       {
           return _metadataProvider.GetBackendInfo(typeof(Backend));
       }
   }
   ```
4. Add backend to `App/ClientApp/src/config.json` with data URL pointing to REST CRUD API:
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
           "baseUrl": "https://external-backend.com/api"
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

#### backend.data.baseUrl (required)
Base URL to data API, used to work with backend data. Every collection in the backend must provide 5 HTTP API endpoints:
- `GET %data.url%`: return all backend items as a list (IDs and names are required, other data is ignored)
- `GET %data.url%/{id}`: return full item data by ID
- `POST %data.url%`: create new item in the backend with posted data
- `PUT %data.url%/{id}`: replace item data by ID
- `DELETE %data.url%/{id}`: delete item by ID

> `POST`, `PUT` and `DELETE` APIs are not required if collection is marked as readonly.

## C# Meta Attributes
The list of available C# attributes and options for metadata:

#### CollectionAttribute.Path
Path to collection REST CRUD API (Create-Read-Update-Delete) used to work with data, appended to `%data.baseUrl%` from config. Default path is C# property name. 

#### CollectionAttribute.Readonly
Marks collection as readonly. Disables all editing functionality, only list view is available.

#### CollectionAttribute.Constant
Marks collection as constant. Disables existing item editing, but allows viewing and creating items.

#### CollectionAttribute.ItemsProperty
Name of the response data property containing result collection items from `GET %data.baseUrl%%collection.path%` API. By default entire response is treated as a collection.

#### CollectionQueryFilterAttribute
Adds a string filter to collection items. Displayed as a text input in the collection list view.

#### CollectionRefFilterAttribute
Adds a referenced collection filter to collection items. Displayed as a select list in the collection list view.

## Deployment
The app is cloud ready and can be deployed in minutes to any cloud provider.

For example, to deploy the app to Azure: run `Publish...` action for `App` project in Visual Studio, select your Azure account and service and click Publish. The app will be live in several minutes.

## License

Copyright (C) 2018-present Pavlo Niezhentsev

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

C# metadata provider code in [Pim.Meta](/Pim.Meta) folder is licensed under the MIT license.
