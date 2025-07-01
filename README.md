# brcnet
Billion row challenge in dotnet

### Init
dotnet new console -n BrcNet

### Generate test files
cd GenerateFile
dotnet publish -c Release -p:DebugType=None
./bin/Release/net10.0/publish/GenerateFile 10 10 .testfiles/output_10.txt
./bin/Release/net10.0/publish/GenerateFile 10000 1000 .testfiles/output_10_000.txt
./bin/Release/net10.0/publish/GenerateFile 1000000 10000 .testfiles/output_1_000_000.txt

### Test run
dotnet publish ./BrcNet/BrcNet.csproj -c Release -p:DebugType=None
./BrcNet/bin/Release/net10.0/publish/BrcNet ./GenerateFile/.testfiles/output_1_000_000.txt

