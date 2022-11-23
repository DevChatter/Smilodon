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

## Project Components

The Codename "Smilodon" Project contains a few projects.

### Web API Project

The WebApp project exposes the REST API, which includes the [ActivityPub](https://activitypub.rocks/) Endpoint and a [WebFinger](https://webfinger.net/) Endpoint. It's built using AspNetCore on .NET 7. Having this separate from the Web Streaming allows them to scale separately.

### Web Streaming Project

The WebStreaming project handles the real-time updates and other long-running connections using web sockets. Having this separate from the Web API allows them to scale separately.

### Background Processing Project

TBD - Most likely a .NET 7 process of some kind.

### Database

TBD - Most likely a Postgres database compatible with the schema used by Mastodon, so that switching a server to/from Mastodon is easy.

## Primary Contributors

### Brendan Enrick (@Brendoneus)

[![Brendoneus Twitter Follow](https://img.shields.io/twitter/follow/brendoneus?style=social)](https://twitter.com/brendoneus)
[![Brendoneus Mastodon Follow](https://img.shields.io/mastodon/follow/109288133487144928?domain=https%3A%2F%2Four.devchatter.com&style=social)](https://our.devchatter.com/@brendoneus)
[![YouTube Channel Subscribers](https://img.shields.io/youtube/channel/subscribers/UCA8TsqMrOdFBv66iIuU6efA?style=social)](https://www.youtube.com/c/devchatter)
[![DevChatter Twitch](https://img.shields.io/badge/Twitch-DevChatter-9146FF)](https://www.twitch.tv/DevChatter)

**Note: we'll add people here once we have more contributors!**
