@echo off
echo BUILD Cadmus packages
del .\Cadmus.Core\bin\Debug\*.*nupkg
del .\Cadmus.Cli.Core\bin\Debug\*.*nupkg
del .\Cadmus.Index\bin\Debug\*.*nupkg
del .\Cadmus.Index.Sql\bin\Debug\*.*nupkg
del .\Cadmus.Index.MsSql\bin\Debug\*.*nupkg
del .\Cadmus.Index.MySql\bin\Debug\*.*nupkg
del .\Cadmus.Seed\bin\Debug\*.*nupkg
del .\Cadmus.Mongo\bin\Debug\*.*nupkg

cd .\Cadmus.Core
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Cli.Core
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index.Sql
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index.MsSql
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index.MySql
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Mongo
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Seed
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
pause
