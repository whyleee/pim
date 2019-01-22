# pim
Simple edit UI for your backend. http://pim-pn.azurewebsites.net

![Screenshot](https://raw.githubusercontent.com/whyleee/pim/master/screenshot-1.jpg)

## Features
- Basic create/update/delete flow
- Multiple external backends
- Multiple collections with filters per backend
- Relations between collections (across all backends)
- Authorization with HTTP headers
- Supported collection filters:
  - Reference (items from other collections)
  - Query (text parameter)
- Supported fields:
  - Checkbox
  - Datetime
  - Reference (select list with autocomplete)
  - Reference list (multiselect with autocomplete)
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

### CollectionAttribute
Specifies collection options.

#### CollectionAttribute.Path
Path to collection REST CRUD API (Create-Read-Update-Delete) used to work with data, appended to `%data.baseUrl%` from config. Default path is C# property name.

#### CollectionAttribute.UpdateMethod
HTTP method to use for updating collection items (`PUT` or `PATCH`).

#### CollectionAttribute.Readonly
Marks collection as readonly. Disables all editing functionality, only list view is available.

#### CollectionAttribute.Constant
Marks collection as constant. Disables existing item editing, but allows viewing and creating items.

#### CollectionAttribute.ItemsProperty
Name of the response data property containing result collection items from `GET %data.baseUrl%%collection.path%` API. By default entire response is treated as a collection.

#### CollectionAttribute.KeyDelimiter
Delimiter to use in URLs for items with compound key (for example `/` for `items/key1/key2` or `-` for `items/key1-key2` URLs).

### Collection Filters
Add filters for collection list view to reduce results.

#### CollectionQueryFilterAttribute
Adds a string filter to collection items. Displayed as a text input in the collection list view.

#### CollectionRefFilterAttribute
Adds a referenced collection filter to collection items. Displayed as a select list with autocomplete in the collection list view.

#### CollectionFilterAttribute.Key
Filter parameter name in API URL. Selected filter value will be set into `{filterKey}` placeholder in `CollectionAttribute.Path` or added as a query string parameter otherwise.

#### CollectionFilterAttribute.Name
Friendly filter name displayed in UI.

#### CollectionFilterAttribute.Description
Short filter description or hint displayed in UI.

#### CollectionFilterAttribute.Required
Specifies that filter must be set before collection list view displays results. Selects first item automatically in ref filters.

#### CollectionRefFilterAttribute.RefCollectionKey
Key of the referenced collection, to get its items as filter options.

#### CollectionRefFilterAttribute.BackendKey
Key of the backend where the referenced collection exposed. Empty means current backend.

#### CollectionRefFilterAttribute.Multiple
Allows multiple selection from the referenced collection. Sends selected values comma-separated to API URL.

#### CollectionRefFilterValueAttribute
Adds a value passed to referenced collection API when loading filter or reference field options.

#### CollectionRefFilterValueAttribute.RefKey
Key of the collection filter to attach the value to. When used with ref fields (`CollectionRefAttribute`): this must be a key of the referenced collection.

#### CollectionRefFilterValueAttribute.Key
Parameter name in API URL to set the value to. Sets value to `{paramKey}` placeholder in the referenced `CollectionAttribute.Path` or added as a query string parameter otherwise.

#### CollectionRefFilterValueAttribute.Value
Parameter value passed to referenced collection API URL. Allows placeholders like `{paramKey}` that will be resolved from other selected filters.

### Model Attributes

#### GetModelAttribute (class)
References C# model type for items returned by GET methods, to expose additional fields in metadata. Useful when POST C# model type used as collection model in metadata (because it has validation attributes), but this model lacks some required fields (like item ID for example).

#### CollectionRefAttribute
Sets field type to collection reference. Displayed as select list with autocomplete from referenced collection items. Allows multiselection automatically if C# property type is enumerable (excluding strings).

#### CollectionRefAttribute.Key
Key of the referenced collection, to get its items as select list options.

#### CollectionRefAttribute.BackendKey
Key of the backend where the referenced collection exposed. Empty means current backend.

#### ReadFromAttribute
Tells to read data from other field. Useful when POST field is different from GET. Tested with constant fields only (allow initial value, but readonly afterwards).

#### TitleAttribute
Marks field as the one that returns the item title. Similar to standard C# `KeyAttribute` that marks item IDs.

## Deployment
The app is cloud ready and can be deployed in minutes to any cloud provider.

For example, to deploy the app to Azure: run `Publish...` action for `App` project in Visual Studio, select your Azure account and service and click Publish. The app will be live in several minutes.

## License

Copyright (C) 2019-present Pavlo Niezhentsev

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

C# metadata provider code in [Pim.Meta](/Pim.Meta) folder is licensed under the MIT license.
