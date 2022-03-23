# NOI community API - Consumer documentation

The following API endpoints are of interest for the NOI App:

- **[Accounts](#accounts)**
  Lists all accounts/organisations that are member of the NOI community.
  ![](https://user-images.githubusercontent.com/7909989/158361210-dfcf557c-5ede-49ef-8807-f9bee3de43c5.png)

- **[Contacts](#contacts)**
  Lists all the contacts that are member of the NOI community.
  ![](https://user-images.githubusercontent.com/7909989/158358399-fcdf74f8-991d-4894-a856-faecd6f7f5ff.png)

## Gettings started

In order to consume data from the community API a **valid authentification token** has to be provided as bearer token.
For documentation on how to authenticate againts the [NOI authentication server](https://github.com/noi-techpark/authentication-server) and produce a valid bearer please consult the documentation of the NOI authentication server.

The NOI community API uses the OData protocol in order to filter and select certain parts of the data.
Please refer to the official OData documentation which can be found [here](https://www.odata.org/documentation/).

> There exists some utility functionality which is not part of the default OData protocol (it is allowed to extend the standard) in order to interface with Dynamics 365 which defines some custom data types. Those functionality isn't explained in by the OData documentation, but can be found by searching for the documentation on the internet. The special utility functionality used for the two alluded endpoints gets explained as they arise.

## Accounts

The following URL queries for the accounts which are member of the NOI community:

https://api.community.noi.testingmachine.eu/accounts?%24top=10&%24filter=Microsoft.Dynamics.CRM.ContainValues(PropertyName%3D%40p1,PropertyValues%3D%40p2)%20and%20statuscode%20eq%201&%40p1='crb14_accountcat_placepresscommunity'&%40p2=%5B'952210000'%5D&%24select=name,noi_nameit,websiteurl,emailaddress1,address1_line1,address1_city,address1_postalcode

This are the parts of the URL which form the query:

1. `https://api.community.noi.testingmachine.eu`
   This is the base URL to the NOI community api.
1. `/accounts`
   This is the endpoint which returns all accounts.
1. Query arguments:
    1. `$top=10`
    Returns the $top 10 accounts.
    1. `$filter=Microsoft.Dynamics.CRM.ContainValues(PropertyName=@p1,PropertyValues=@p2) and statuscode eq 1`
    This is the $filter argument, which returns only the active members of the NOI community.
    This part isn't complete, because it refers to the two parameters `@p1` and `@p2`.
    1. `@p1='crb14_accountcat_placepresscommunity'`
        The field to query against. This is a special [Multi-Select Picklist](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/multi-select-picklist) field special to Dynamics 365 which allows to specify multiple values on a single field.
    1. `@p2=['952210000']`
    The value to query against. This can be a list of multiple ids. In our case we are only interested in members of the community API which has the special id 952210000.
    1. `$select=name,noi_nameit,websiteurl,emailaddress1,address1_line1,address1_city,address1_postalcode`
    The $select argument allows to return only the fields which are of interest for the consumer.

## Contacts

The following URL queries for the contacts which are member of the NOI community:

https://api.community.noi.testingmachine.eu/contacts?%24filter=Microsoft.Dynamics.CRM.ContainValues(PropertyName%3D%40p1,PropertyValues%3D%40p2)%20and%20statuscode%20eq%201&%40p1='noi_contactcat_placepresscommunity'&%40p2=%5B'181640000'%5D&%24top=100&%24select=emailaddress1,firstname,lastname

This are the parts of the URL which form the query:

1. `https://api.community.noi.testingmachine.eu`
   This is the base URL to the NOI community api.
1. `/contacts`
   This is the endpoint which returns all contacts.
1. Query arguments:
    1. `$top=100`
    Returns the $top 100 accounts.
    1. `$filter=Microsoft.Dynamics.CRM.ContainValues(PropertyName=@p1,PropertyValues=@p2) and statuscode eq 1`
    This is the $filter argument, which returns only the active members of the NOI community.
    This part isn't complete, because it refers to the two parameters `@p1` and `@p2`.
    1. `@p1='noi_contactcat_placepresscommunity'`
        The field to query against. This is a special [Multi-Select Picklist](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/multi-select-picklist) field special to Dynamics 365 which allows to specify multiple values on a single field.
    1. `@p2=['181640000']`
    The value to query against. This can be a list of multiple ids. In our case we are only interested in members of the community API which has the special id 181640000.
    1. `$select=emailaddress1,firstname,lastname`
    The $select argument allows to return only the fields which are of interest for the consumer.
