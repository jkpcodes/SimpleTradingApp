Requirements:
1. Setup C# .NET in local machine
  1.1. Install .NET v9
  1.2. Install 'dotnet-ef' tool for Entity Framework by running code below in command line:
    dotnet tool install --global dotnet-ef
2. Download/pull repository from Github (<insert link here>)
3. Setup user secrets for SimpleTradingApp.Api cspoj
  3.1. Using powershell/command prompt navigate to SimpleTradingApp.Api folder
  3.2. Set user secrets using the following commands: (Note: the values for Step 3.2 and Step 4.3 must be consistent)
    dotnet user-secrets set "POSTGRES_USER" "<user>"
    dotnet user-secrets set "POSTGRES_PASSWORD" "<password>"
    dotnet user-secrets set "POSTGRES_PORT" "<Unused local port>"
4. Setup local postgresql via Docker:
  4.1. Download Docker
  4.2. Download latest postgresql image via "docker pull postgresql:latest" command
  4.3. Run postgresql container via command:
      docker run
        --name <Container Name>
        -p <Unused local port>:5432
        -e POSTGRES_USER=<user>
        -e POSTGRES_PASSWORD=<password>
        -d postgres:latest