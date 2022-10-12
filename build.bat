dotnet publish ".\Code\WireGuardUIService\WireGuardUIService.csproj" -r win-x64 -p:PublishSingleFile=true --self-contained false --configuration Release

dotnet publish ".\Code\WireGuardGUI\WireGuardGUI.csproj" -r win-x64 -p:PublishSingleFile=true --self-contained false --configuration Release