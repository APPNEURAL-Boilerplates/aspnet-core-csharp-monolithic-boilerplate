.PHONY: restore dev test build run docker clean

restore:
	dotnet restore DotnetMonolith.sln

dev:
	dotnet watch --project src/DotnetMonolith.Api run

test:
	dotnet test DotnetMonolith.sln

build:
	dotnet build DotnetMonolith.sln --configuration Release

run:
	dotnet run --project src/DotnetMonolith.Api

docker:
	docker compose up --build

clean:
	dotnet clean DotnetMonolith.sln
