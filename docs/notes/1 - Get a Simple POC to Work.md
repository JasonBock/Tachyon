TODO:

* DONE - Need to process the `InvocationExpressionSyntax` to ensure it's not `partial`, we get the symbol, and we create a model for what we need
* DONE - Do the filtering on our model instances
* DONE - Register the output to create the invocations
* Write tests
    * Static
        * GenerateWithNoParametersAndVoidReturnAsync
        * GenerateWithNoParametersAndReturnAsync
        * DONE - GenerateWithParametersAndVoidReturnAsync
        * GenerateWithParametersAndReturnAsync
    * Instance
        * GenerateWithNoParametersAndVoidReturnAsync
        * GenerateWithNoParametersAndReturnAsync
        * GenerateWithParametersAndVoidReturnAsync
        * GenerateWithParametersAndReturnAsync
    * GenerateWithMultipleNamespaces
* Write integration tests
    * Create a project that contains a static `Logger` instance. Or, I generate a type that has that public static property on it. That may make things

Future Work:
* Process the additional file to get a `EquatableArray<string>` of filtered methods
    * Maybe use the additional file to provide extra filtering for `InterceptorsNamespaces`
