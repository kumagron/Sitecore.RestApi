#Sitecore REST API
This project aims to provide a completely RESTful alternative to Sitecore's Item Web API. Apart from being completely RESTful it also a lot of flexibility in customizing the response output with little effort.

Read below to know more about what it can do to make your Sitecore client-side development more efficient.

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
If you want to create a default item profile and not add a profile param for every request just create an item profile named 'default'.

**FYI:** All these customizations are done in the content editor so you will see your changes right away the moment you execute a request.

##Querying##
The old Item Web API lets you query items by directly entering your full query in the URL - let's face it, it's an ugly thing to do. While the Sitecore REST API still lets you do the old querying way it also lets you create Query items where you can design your Sitecore XPath so that your request would be simplified to just using the 'name' of the query item:

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
##Additional Features

Getting a single item using the item's ID:
```
http://localhost/api/items/110d559f-dea5-42ea-9c1c-8a5df7e70ef9
```
Getting the item's children:
```
http://localhost/api/items/110d559f-dea5-42ea-9c1c-8a5df7e70ef9/children
```
Getting the item's ancestors:
```
http://localhost/api/items/110d559f-dea5-42ea-9c1c-8a5df7e70ef9/ancestors
```
Getting the item's descendants:
```
http://localhost/api/items/110d559f-dea5-42ea-9c1c-8a5df7e70ef9/descendants
```
