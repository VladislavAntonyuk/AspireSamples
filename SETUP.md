## Build solution

1. Install .NET 9

1. Build
    ```pwsh
    dotnet restore --configfile NuGet.config
    dotnet build
    ```

## Update ReadMe

1. Install Python 3
1. Run `pip install MarkdownPP`
1. Optional. You may need to add env variables to path: `C:\Program Files\Python312\Scripts`. This path should contain `markdown-pp`.
1. Run PowerShell script `./md/run.ps1`