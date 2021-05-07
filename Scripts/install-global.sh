#!/bin/sh

# uninstall
dotnet tool uninstall EagleRepair.Cli --global

# pack
dotnet pack

# install
dotnet tool install --global --add-source ./src/EagleRepair.Cli/nupkg EagleRepair.Cli
