# The Homebrew API

## Description

thehomebrewapi allows for storing and retrieving of homebrew recipes. It has repositories for recipes, water profiles and brews. Currently there is no authentication or authorisation associated with the API. I've included docker files to facilitate the creation of docker images for home development.

## How to use

- Clone this repository
- Download and install [Docker](https://www.docker.com/) (requires signing up for a free account)
- Create a thehomebrewapi Docker image by running the following commands from within the thehomebrewapi-V2 folder

      docker build -t homebrewapi-image -f Docker/Dockerfile .
      docker-compose -f .\Docker\docker-compose.yml up

  - The first command creates the basic api image

  - The second one places it behind a [NGINX](https://hub.docker.com/_/nginx) proxy

- Check the api is up and running by navigating to `localhost:4004/api/recipes`
- If you want to change the initial data, update the configuration in HomeBrewContext.cs, delete the InitialMigration file a then run the following command in Package Manager Console (the results can be viewed using the SQL Server Object Explorer)

      add-migration InitialMigration
      Update-Database
