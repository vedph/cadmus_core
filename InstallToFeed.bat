@echo off
echo PRESS ANY KEY TO INSTALL Cadmus Libraries TO LOCAL NUGET FEED
echo Remember to generate the up-to-date package.
pause
c:\exe\nuget add .\Cadmus.Archive.Parts\bin\Debug\Cadmus.Archive.Parts.2.2.45.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Core\bin\Debug\Cadmus.Core.2.2.43.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index\bin\Debug\Cadmus.Index.1.0.19.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Sql\bin\Debug\Cadmus.Index.Sql.1.0.43.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Lexicon.Parts\bin\Debug\Cadmus.Lexicon.Parts.2.2.46.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Mongo\bin\Debug\Cadmus.Mongo.2.2.55.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Parts\bin\Debug\Cadmus.Parts.2.2.52.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Philology.Parts\bin\Debug\Cadmus.Philology.Parts.2.2.58.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Seed\bin\Debug\Cadmus.Seed.1.0.47.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Seed.Parts\bin\Debug\Cadmus.Seed.Parts.1.0.55.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Seed.Philology.Parts\bin\Debug\Cadmus.Seed.Philology.Parts.1.0.55.nupkg -source C:\Projects\_NuGet
pause
