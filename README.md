# Order API for Mango application

# Running Docker via any terminal

## Building the image:-
docker build `   --secret id=nugetconfig,src="$env:NUGET_CONFIG_PATH" `   -t mango-orderapi-local:dev `   .

## Building the container:- 
docker run --name mango-orderapi --env-file .env -p 5201:8080 mango-orderapi-local:dev
