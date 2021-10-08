# The Homebrew API

## Description

thehomebrewapi allows for storing and retrieving of homebrew recipes. It has repositories for recipes, water profiles and brews. Currently there is no authentication or authorisation associated with the API. I've included docker files to facilitate the creation of docker images for home development.

## How to use

- Clone this repository
- Download and install [Docker](https://www.docker.com/) (requires signing up for a free account)
- Create a thehomebrewapi Docker image by running the following commands

      docker build -t homebrewapi-image .
      docker-compose up

  - The first command creates the basic api image

  - The second one places it behind a [NGINX](https://hub.docker.com/_/nginx) proxy)

- Check the api is up and running by navigating to `localhost:4004/api/recipes`
