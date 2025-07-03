# brcnet
Billion row challenge in dotnet

### Init
```
dotnet new console -n BrcNet
dotnet tool install --global dotnet-trace
export PATH="$PATH:/root/.dotnet/tools"
```

### Generate test files
```
dotnet publish ./GenerateFile/GenerateFile.csproj -c Release -p:DebugType=None
./GenerateFile/bin/Release/net10.0/publish/GenerateFile 10 10 .testfiles/output_10.txt
./GenerateFile/bin/Release/net10.0/publish/GenerateFile 10000 1000 .testfiles/output_10_000.txt
./GenerateFile/bin/Release/net10.0/publish/GenerateFile 1000000 10000 .testfiles/output_1_000_000.txt
./GenerateFile/bin/Release/net10.0/publish/GenerateFile 10000000 10000 .testfiles/output_10_000_000.txt
./GenerateFile/bin/Release/net10.0/publish/GenerateFile 1000000 20 .testfiles/output_1_000_000_b.txt
```

### Test run
```
dotnet publish ./BrcNet/BrcNet.csproj -c Release -p:DebugType=None

./BrcNet/bin/Release/net10.0/publish/BrcNet ./.testfiles/output_10_000_000.txt
```


### Perf 
```
dotnet build ./BrcNet/BrcNet.csproj -c Release

dotnet-trace collect --output mytrace.nettrace -- dotnet run -c Release --project ./BrcNet/BrcNet.csproj -- ./GenerateFile/.testfiles/output_1_000_000.txt

dotnet-trace report --trace-file trace.nettrace
```

drop file into:  
https://www.speedscope.app/ 

### Versions

0.1: parse line by line with StreamReader, use line.Split and double.TryParse when parsing lines, store temp as int until avg is needed. Results 10M = 2.1-2.4 seconds
0.2: using buffered FileStream and custom byte parsing using spans. Results 10M = 0.9 seconds


