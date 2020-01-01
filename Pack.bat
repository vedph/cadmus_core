@echo off
echo BUILD Cadmus packages
del .\Cadmus.Core\bin\Debug\*.nupkg
del .\Cadmus.Parts\bin\Debug\*.nupkg
del .\Cadmus.Archive.Parts\bin\Debug\*.nupkg
del .\Cadmus.Philology.Parts\bin\Debug\*.nupkg
del .\Cadmus.Mongo\bin\Debug\*.nupkg

cd .\Cadmus.Core
dotnet pack -c Debug --include-symbols
cd..
cd .\Cadmus.Parts
dotnet pack -c Debug --include-symbols
cd..
cd .\Cadmus.Archive.Parts
dotnet pack -c Debug --include-symbols
cd..
cd .\Cadmus.Lexicon.Parts
dotnet pack -c Debug --include-symbols
cd..
cd .\Cadmus.Philology.Parts
dotnet pack -c Debug --include-symbols
cd..
cd .\Cadmus.Mongo
dotnet pack -c Debug --include-symbols
cd..
cd .\Cadmus.Seed
dotnet pack -c Debug --include-symbols
cd..
cd .\Cadmus.Seed.Parts
dotnet pack -c Debug --include-symbols
pause
