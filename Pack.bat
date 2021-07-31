@echo off
echo BUILD Cadmus packages
del .\Cadmus.Core\bin\Debug\*.*nupkg
del .\Cadmus.Index\bin\Debug\*.*nupkg
del .\Cadmus.Index.Sql\bin\Debug\*.nupkg
del .\Cadmus.Lexicon.Parts\bin\Debug\*.*nupkg
del .\Cadmus.Parts\bin\Debug\*.*nupkg
del .\Cadmus.Archive.Parts\bin\Debug\*.*nupkg
del .\Cadmus.Philology.Parts\bin\Debug\*.*nupkg
del .\Cadmus.Seed\bin\Debug\*.*nupkg
del .\Cadmus.Seed.Parts\bin\Debug\*.*nupkg
del .\Cadmus.Seed.Philology.Parts\bin\Debug\*.*nupkg
del .\Cadmus.Mongo\bin\Debug\*.*nupkg

cd .\Cadmus.Core
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index.Sql
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Parts
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Archive.Parts
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Lexicon.Parts
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Philology.Parts
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Mongo
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Seed
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Seed.Parts
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Seed.Philology.Parts
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
pause
