# Connector GUI / Connector API

> A lightweight Windows .NET project providing a GUI and connector components for integrating with the CV_Analayser platform.

## Project Overview

This repository contains the Connector GUI and supporting API code used to connect local systems to the CV_Analayser service. It includes a WinForms GUI (`ConnectorGui.sln`) and related projects, configuration files, and OpenAPI specifications located in the `OpenAPIs/` folder.

## Key Features

- WinForms-based configuration and management UI
- Connector components to integrate with CV_Analayser
- OpenAPI specifications for API endpoints (see `OpenAPIs/`)
- Simple build and run instructions for Windows developers

## Prerequisites

- Windows 10 or later
- Visual Studio 2017/2019/2022 (recommended) or MSBuild
- .NET Framework (project uses packages.config — ensure a compatible .NET Framework is installed) or the .NET SDK if you plan to migrate to SDK-style projects

## Getting Started

1. Clone the repository:

   git clone https://github.com/your-org/ConnectorAPI.git
   cd ConnectorAPI

2. Open the solution in Visual Studio:

   - Double-click `ConnectorGui.sln` and build the solution (Build → Build Solution).

3. Or build from the command line (MSBuild):

   msbuild ConnectorGui.sln /p:Configuration=Debug

   If you have the .NET SDK and the project is compatible, you can also try:

   dotnet build ConnectorGui.sln

4. Run the executable found under `bin/Debug/` (or the Debug output folder configured for your build).

## Configuration

- Application configuration files: `app.config`, `App1.config`
- Project settings: see [Properties/Settings.settings](Properties/Settings.settings)
- Secrets and environment-specific settings should be stored securely and not committed to the repo.

## OpenAPI / API Specs

API definitions and OpenAPI specs are available in the `OpenAPIs/` directory. Use these to generate client code or to understand the contract exposed by any HTTP endpoints.

## Usage Examples

- Launch the GUI and navigate to the connector configuration panel to set endpoint URLs, API keys, and test connections.
- When running from command line, ensure required config files are present in the working directory or copied to the output folder.

## Contributing

Contributions are welcome. Please follow these steps:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature`.
3. Make changes and add tests where appropriate.
4. Submit a pull request describing your changes.

Guidelines:

- Keep changes focused and well-documented.
- Update this README or add documentation for any user-facing changes.

## Tests

No automated tests are included in this repository by default. If you add tests, please include instructions here for running them.

## TODOs

- Add a `LICENSE` file to state the project license.
- Add CI configuration (GitHub Actions, Azure Pipelines, etc.) to build and test PRs.

## License

This project does not include a license file. Add a LICENSE (for example, MIT or Apache-2.0) to clarify terms for contributors and consumers.

## Contact

For questions or support, contact the project maintainer or open an issue in the repository.

## Acknowledgements

Thanks to the contributors and the CV_Analayser project team for guidance and API specifications.
