#Sitecore REST API
This project aims to provide a completely RESTful alternative to Sitecore's Item Web API. Apart from being completely RESTful it also provides a lot of flexibility in customizing the response output with little effort.

Read below to know more about what it can do to accelerate your client-side development.

##Item Profiling
Item profiling will let you do the following things you wish you could do with the Item Web API

1. Removal of unwanted item properties
2. Addition of custom item properties
3. Formatting of item property values using external processors
3. Removal of unwanted field properties
4. Formatting of field values using external processors
5. Camel-casing of property names

An item profile can be attached to the request as a query param with the key 'profile'.
```
http://localhost/api/items/?query=/sitecore/content/*?profile=veryslimprofile
```
If you want to create a default item profile and not add a profile param for every request just create one and name it 'default'.

**FYI:** All these customizations are done in the content editor so you will see your changes right away the moment you execute a request.

##Querying##
The old Item Web API lets you query items by directly entering your full query in the URL - let's face it, it's an ugly thing to do. While the Sitecore REST API still lets you do the old querying way it also lets you create query items where you can design your Sitecore XPath so that your request would be simplified to just using the 'name' of the query item:

```
http://localhost/api/items/items-that-i-like-to-query/
```

Did I mention that you can also supply query params in the URL?

```
http://localhost/api/items/items-with-specific-templatekey/?templatekey=sometemplatekey
```

And of course your query should look like this:
```
fast:/sitecore/content//*[@@templatekey={templatekey}]
```
###Additional Querying Features


Getting a single item using the item's ID:
```
http://localhost/api/items/[GUID]
```
Getting the item's children:
```
http://localhost/api/items/[GUID]/children
```
Getting the item's ancestors:
```
http://localhost/api/items/[GUID]/ancestors
```
Getting the item's descendants:
```
http://localhost/api/items/[GUID]/descendants
```
##Content Types
By default the REST API will return JSON but you can also force it to return an xml by setting the header content-type to either application/xml or text/xml. 

##Upcoming Features
* Token based authentication
* Item property formatting
* Caching and eTagging
* Lucene search

##How to install
Although this project is already fully functional an installer is still on its way and to compensate for that I've included the serialized templates and items that you can already use.

After deserialization build the project and copy all the DLLs to your site's bin folder, and Sitecore.RestApi.config to your site's Include folder.

You can also create a Local.targets file alongside the project file and it will be imported automatically to your build targets.
```
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <ItemGroup>
    <BinFiles Include=".\bin\*.dll"/>
    <ConfigFiles Include=".\App_Config\Include\*.config"/>
    </ItemGroup>
  <PropertyGroup>
    <LocalSiteRoot>[Full path to your Website folder]</LocalSiteRoot>
  </PropertyGroup>
  <Target Name="AfterBuild">
    <Copy SourceFiles="@(BinFiles)" DestinationFolder="$(LocalSiteRoot)\bin" ContinueOnError="false" />
    <Copy SourceFiles="@(ConfigFiles)" DestinationFolder="$(LocalSiteRoot)\App_Config\Include" ContinueOnError="false" /> </Target>
</Project>
```
Be aware that I've used the latest Sitecore build for this project (7.2 build 140526) and I haven't tested it on earlier versions.
