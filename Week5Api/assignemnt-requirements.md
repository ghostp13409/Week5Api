WEEK 6 LAB: Exception Handling, Security, State, and Transactions in Services

Environment: Local only (no cloud deployment required)
Stack:

    .NET 9 or .NET 10 Web API

    Node.js console client (or C# client if you prefer to swap)

Learning outcomes:
Create and consume services with exception handling.
Create and consume secure services.
Create and consume services that maintain state.
Create and consume services that support transactions.
Starting Point (5 minutes)

You may either:

    Use your Week 5 API project as a starting point, or

    Create a new .NET 9 or .NET 10 Web API (minimal API or controller based)

If you start fresh, create a simple resource, for example:

    "Orders"

    "Tasks"

    "Items"

    "Bookings"

You must have at least:

    A GET endpoint that returns a list

    A POST endpoint that accepts a request body

Part 1 - Exception Handling in the API

1. Add basic validation

In one of your endpoints (for example POST /orders):

    Validate input data

    If invalid, do not throw raw exceptions

    Return a structured error response instead

Example rules:

    Quantity must be greater than 0

    Name must not be empty

Example JSON error format to use throughout the lab:

{
"error": "InvalidParameter",
"message": "Quantity must be greater than zero"
}

You must:

    Check input and return HTTP 400 when validation fails.

    Include the "error" and "message" properties in the JSON body.

2. Add global exception handling

Add a global exception handler using middleware:

    Wrap the pipeline with a try catch

    Log the exception (Console.WriteLine is enough for this lab)

    Return a generic error response with HTTP 500

Example JSON body:

{
"error": "ServerError",
"message": "An unexpected error occurred."
}

Task:
Trigger an intentional exception (for example, divide by zero) in one endpoint and confirm that the client receives the generic ServerError JSON and HTTP 500.
Part 2 - Securing the Service with a Simple API Key (20 minutes)

In this lab you will use a very simple "API key" approach for security.

1. Define an API key

Pick a constant for the lab, for example:

MY_SECRET_KEY_123

Do not hardcode this in production, but it is acceptable here. 2. Add simple authentication middleware

In Program.cs:

    Before your endpoints, add middleware that checks a header such as "X-Api-Key"

    If missing or incorrect, return HTTP 401 Unauthorized with JSON:

{
"error": "Unauthorized",
"message": "Missing or invalid API key."
}

    If correct, allow the request to continue

Apply this middleware only to certain routes if you like, or to all. 3. Update the client to send the key

In your Node.js client (or C# client if you prefer):

    Include the header "X-Api-Key" with the correct key

    Show that when the key is missing or wrong, the server returns 401

    Show that with the correct key, calls succeed

Task:
Capture a screenshot of:

    A failed call with wrong or missing key

    A successful call with the correct key

Part 3 - Stateful Service Behavior (20 minutes)

You will now make your service maintain simple state in memory.

1. Add a state container

In Program.cs or a separate service:

    Create an in memory dictionary keyed by "clientId" or "apiKey"

For example:

    Key: API key or a client id from a query parameter

    Value: a counter representing how many requests this client has made

2. Endpoint to track state

Create an endpoint, for example:

GET /usage

Behavior:

    Read the client identifier, for example from the "X-Api-Key" or a query string

    Increment that client's count in the dictionary

    Return JSON with something like:

{
"clientId": "MY_SECRET_KEY_123",
"callCount": 5
}

3. Consume the state from the client

Update your Node.js client to:

    Call /usage multiple times in a loop (for example 3 calls)

    Print the response each time

    You should see the callCount increasing for the same client

Task:
Verify that:

    Using the same API key or client id increases the count

    Changing to another client id or key gives a separate counter

Part 4 - Simple Transaction Like Behavior (20 minutes)

For this lab you will simulate a transaction using EF Core or an in memory collection.
Option A: Using EF Core (if your project already uses a database)

    Create a simple model, for example "Account" with a Balance.

    Create an endpoint to "Transfer" between two accounts.

    Use a transaction to ensure both balances are updated together.

Basic pattern:

    Begin transaction

    Subtract from one account

    Add to the other account

    If any step fails, rollback and return an error

Return:

{
"status": "Success"
}

or

{
"status": "Failed",
"error": "InsufficientFunds"
}

Option B: In memory transactional simulation (if you do not want a database)

    Store accounts in an in memory list or dictionary.

    Implement a "transfer" endpoint that:

        Checks source account has enough balance

        Deducts from source

        Adds to destination

        If any validation fails, revert changes and return an error

The key idea is: both updates happen, or none happen.
Client consumption

In your Node.js client:

    Call the transfer endpoint

    Print the success or failure JSON

    Test at least one failure case, such as insufficient funds

Task:
Provide sample outputs for:

    A successful transfer

    A failed transfer due to validation (for example negative balance)
