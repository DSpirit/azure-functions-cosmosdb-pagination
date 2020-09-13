# azure-functions-cosmosdb-pagination
C# Azure Function App .NET Core 3.1 - CosmosDB Queries with Pagination

## Setup
- Add a local.settings.json
- Add your CosmosDB Connection String
- Run the function

## Usage
[GET] http://localhost:7071/api/{database}/{collection}?q={ "QueryText": "<query>", "Parameters": [ { "Name": "@foo", "Value": "bar"}] }&pageSize=<pageSize>

Example Request:
  
[GET] http://localhost:7071/api/storage/collection?q={ "QueryText": "SELECT * FROM c", "Parameters": [] }&pageSize=2

Example Response:
``` json
{
  "pagination": {
    "continuationToken": "[{\"token\":\"-RID:~PqxYAKxpJSkCAAAAAAAAAA==#RT:1#TRC:2#ISV:2#IEO:65551\",\"range\":{\"min\":\"\",\"max\":\"FF\"}}]",
    "urlEncodedContinuationToken": "%5B%7B%22token%22%3A%22-RID%3A~PqxYAKxpJSkCAAAAAAAAAA%3D%3D%23RT%3A1%23TRC%3A2%23ISV%3A2%23IEO%3A65551%22%2C%22range%22%3A%7B%22min%22%3A%22%22%2C%22max%22%3A%22FF%22%7D%7D%5D"
  },
  "data": [
    {
      "id": "test-1",
      "_rid": "PqxYAKxpJSkBAAAAAAAAAA==",
      "_self": "dbs/PqxYAA==/colls/PqxYAKxpJSk=/docs/PqxYAKxpJSkBAAAAAAAAAA==/",
      "_etag": "\"00000000-0000-0000-89e0-46af2ec801d6\"",
      "_attachments": "attachments/",
      "_ts": 1600009940
    },
    {
      "id": "test-2",
      "_rid": "PqxYAKxpJSkCAAAAAAAAAA==",
      "_self": "dbs/PqxYAA==/colls/PqxYAKxpJSk=/docs/PqxYAKxpJSkCAAAAAAAAAA==/",
      "_etag": "\"00000000-0000-0000-89e1-86a2378901d6\"",
      "_attachments": "attachments/",
      "_ts": 1600010477
    }
  ],
  "links": {
    "self": "http://localhost:7071/api/storage/collection?q=%7B%20%22QueryText%22:%20%22SELECT%20*%20FROM%20c%22,%20%22Parameters%22:%20[]%20%7D&pageSize=2",
    "next": "http://localhost:7071/api/storage/collection?q=%7B%20%22QueryText%22%3A%20%22SELECT%20%2A%20FROM%20c%22%2C%20%22Parameters%22%3A%20%5B%5D%20%7D&pageSize=2&continuation=%5B%7B%22token%22%3A%22-RID%3A~PqxYAKxpJSkCAAAAAAAAAA%3D%3D%23RT%3A1%23TRC%3A2%23ISV%3A2%23IEO%3A65551%22%2C%22range%22%3A%7B%22min%22%3A%22%22%2C%22max%22%3A%22FF%22%7D%7D%5D"
  }
}

```
