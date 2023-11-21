@echo off
echo BUILD Cadmus packages
del .\Cadmus.Core\bin\Debug\*.*nupkg
del .\Cadmus.Index\bin\Debug\*.*nupkg
del .\Cadmus.Index.Ef\bin\Debug\*.*nupkg
del .\Cadmus.Index.Ef.MySql\bin\Debug\*.*nupkg
del .\Cadmus.Index.Ef.PgSql\bin\Debug\*.*nupkg
del .\Cadmus.Index.Sql\bin\Debug\*.*nupkg
del .\Cadmus.Index.MsSql\bin\Debug\*.*nupkg
del .\Cadmus.Index.MySql\bin\Debug\*.*nupkg
del .\Cadmus.Index.PgSql\bin\Debug\*.*nupkg
del .\Cadmus.Seed\bin\Debug\*.*nupkg
del .\Cadmus.Mongo\bin\Debug\*.*nupkg

cd .\Cadmus.Core
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index.Ef
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index.Ef.MySql
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Index.Ef.PgSql
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
cd .\Cadmus.Index.PgSql
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Mongo
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
cd .\Cadmus.Seed
dotnet pack -c Debug -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
cd..
pause
