# Smilodon

## Running Development Build Locally

### Running the WebApp (API) locally

To run the WebApp, you can navigate to the `\WebApp` folder from your preferred commmand line and run the following command:

``` bat
dotnet run watch
```

If you want the combined experience of both sites running together, the AspNetCore WebApp project is set up to proxy all non-handled requests to the "http://localhost:1336", which is the address of the Client app if you run the client site locally.

### Running the ClientApp (Front-End) locally

The client application can be run from the command line using the following command while in the `\WebApp\ClientApp` folder:

``` bat
npm run dev
```
