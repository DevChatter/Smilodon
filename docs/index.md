
## Smilodon Docs

This project is just starting, so the docs are incomplete. 

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
