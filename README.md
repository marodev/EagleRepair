<p align="center">
  <a href="https://github.com/marodev/EagleRepair"><img src="https://marodev.github.io/EagleRepair/img/eaglerepair-logo.png" alt="EagleRepair" width="400" ></a>
</p>

<p align="center">
    <em>EagleRepair is a cross-platform command line tool for automatically fixing static analysis warnings in C# programs.</em>
</p>

<p align="center">
    <a href="https://github.com/marodev/EagleRepair/actions/workflows/ci.yml">
      <img src="https://github.com/marodev/EagleRepair/actions/workflows/ci.yml/badge.svg" />
    </a>
    <a href="https://codecov.io/gh/marodev/EagleRepair">
      <img src="https://codecov.io/gh/marodev/EagleRepair/branch/main/graph/badge.svg?token=DPOM31AXDV"/>
    </a>
    <a href="https://www.nuget.org/packages/EagleRepair.Cli">
     <img src="https://img.shields.io/nuget/v/EagleRepair.Cli" />
    </a>
    <a href="https://github.com/marodev/EagleRepair/actions/workflows/codeql-analysis.yml">
       <img src="https://github.com/marodev/EagleRepair/actions/workflows/codeql-analysis.yml/badge.svg" />
    </a>
    <a href="https://github.com/marodev/EagleRepair/blob/main/.github/dependabot.yml">
       <img src="https://img.shields.io/badge/Debandabot-enabled-informational.svg" />
    </a>
    <a href="https://github.com/marodev/EagleRepair/blob/main/LICENSE">
      <img src="https://img.shields.io/badge/license-MIT-informational.svg" />
    </a>
</p>

---

**Documentation**: <a href="https://marodev.github.io/EagleRepair" target="_blank">https://marodev.github.io/EagleRepair</a>

**Source-Code**: <a href="https://github.com/marodev/EagleRepair" target="_blank">https://github.com/marodev/EagleRepair</a>

---

## Install

### Prerequisites
Install [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or later

### Command line tool
EagleRepair is [available on NuGet](https://www.nuget.org/packages/EagleRepair.Cli) and can be installed as a global tool:
```
dotnet tool install --global EagleRepair.Cli
```

![Install Gif](docs/img/eagle-repair-install-video.gif) 

## Getting Started
- first build your C# project
- run EagleRepair

and specify the target rules (e.g, rule R5):
```
eaglerepair -r R5 -p .
```
where "." looks for a solution file (.sln) in the current folder (default if not provided)

## Contributing

PR's are welcome!
Start here: [CONTRIBUTING.md](CONTRIBUTING.md)

