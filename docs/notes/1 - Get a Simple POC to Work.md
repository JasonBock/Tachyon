TODO:

* DONE - Need to process the `InvocationExpressionSyntax` to ensure it's not `partial`, we get the symbol, and we create a model for what we need
* DONE - Do the filtering on our model instances
* DONE - Register the output to create the invocations
* Always emit `TachyonContext`, so change `MethodInvocationBuilder.Build()` to use the `Length` of the array to gen the rest of the logging code, but it always emits `TachyonContext`.
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

Future Work:
* Process the additional file to get a `EquatableArray<string>` of filtered methods
    * Maybe use the additional file to provide extra filtering for `InterceptorsNamespaces`