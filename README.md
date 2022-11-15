# Codename "Smilodon"

ActivityPub implementation in dotnet (.net).

<p>
  <a href="https://github.com/DevChatter/Smilodon/graphs/contributors" alt="Contributors">
  <img src="https://img.shields.io/github/contributors/DevChatter/Smilodon" /></a>

  <a href="https://github.com/DevChatter/Smilodon/stargazers" alt="Stars">
  <img src="https://img.shields.io/github/stars/DevChatter/Smilodon" /></a>

  <a href="https://github.com/DevChatter/Smilodon/issues" alt="Issues">
  <img src="https://img.shields.io/github/issues/DevChatter/Smilodon" /></a>

  <a href="https://github.com/DevChatter/Smilodon/blob/main/LICENSE" alt="License">
  <img src="https://img.shields.io/github/license/DevChatter/Smilodon" /></a>
</p>

## Current Build status

[![PR - Build and Test](https://github.com/DevChatter/Smilodon/actions/workflows/pr-build.yml/badge.svg)](https://github.com/DevChatter/Smilodon/actions/workflows/pr-build.yml)

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

## Primary Contributors

### Brendan Enrick (@Brendoneus)

[![Brendoneus Twitter Follow](https://img.shields.io/twitter/follow/brendoneus?style=social)](https://twitter.com/brendoneus)
[![Brendoneus Mastodon Follow](https://img.shields.io/mastodon/follow/109288133487144928?domain=https%3A%2F%2Four.devchatter.com&style=social)](https://our.devchatter.com/@brendoneus)
[![YouTube Channel Subscribers](https://img.shields.io/youtube/channel/subscribers/UCA8TsqMrOdFBv66iIuU6efA?style=social)](https://www.youtube.com/c/devchatter)
[![DevChatter Twitch](https://img.shields.io/badge/Twitch-DevChatter-9146FF)](https://www.twitch.tv/DevChatter)
