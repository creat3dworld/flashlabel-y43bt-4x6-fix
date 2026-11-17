# Build Y43BT-4x6-Fix.exe (Windows, .NET 8 SDK required).
# Run from this folder. Output: .\publish\Y43BT-4x6-Fix.exe
dotnet publish Y43BT-4x6-Fix.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
