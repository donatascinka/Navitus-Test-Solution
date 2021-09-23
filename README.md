# Navitus-Test-Solution
Completed solution for given task

REST API solution for given task.
Using any application for API testing like PostMan

# How to use it?
Server point: **http://localhost:5000/**



## **Add customer into database**

HTTP METHOD: POST

Endpoint: **http://localhost:5000/**

RAW BODY PAYLOAD (JSON)

"Name" - required

Request format:
```
{
"Name": "Name",
"Age": 20,
"Comment": "Comment"
}
```


## **Get all customer details**

HTTP METHOD: GET

Endpoint: **http://localhost:5000/GetAll**

No request body.



## **Get specific customer**

Possible to find specific customer by using any combination, "Id" would pull one specific customer and if not using "Id" then any combination of "Name", "Age","Comment" would filter all entries from database and returns a list.

HTTP METHOD: GET

Endpoint: **http://localhost:5000/**

RAW BODY PAYLOAD (JSON)

Request format:

```
{
"Id": 1
"Name": "Name",
"Age": 20,
"Comment": "Comment"
}
```

## **Edit customer**


HTTP METHOD: POST

Endpoint: **http://localhost:5000/Edit**

RAW BODY PAYLOAD (JSON)

Request format:

"Id" - required, everthing else can change

```
{
"Id": 1
"Name": "Name",
"Age": 20,
"Comment": "Comment"
}
```

## **Delete customer**

HTTP METHOD: DELETE

Endpoint: **http://localhost:5000/{Id}**

Replace {Id} with specific customer Id

No request body.


## **Receive customer history details**

HTTP METHOD: GET

Endpoint: **http://localhost:5000/History/{Id}**

Replace {Id} with specific customer Id

No request body.







