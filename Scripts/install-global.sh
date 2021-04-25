#!/bin/sh

# uninstall
dotnet tool uninstall cli --global

# pack
dotnet pack

# install
dotnet tool install --global --add-source ./src/EagleRepair.Cli/nupkg EagleRepair.Cli
