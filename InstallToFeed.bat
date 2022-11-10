@echo off
echo PRESS ANY KEY TO INSTALL Cadmus Libraries TO LOCAL NUGET FEED
echo Remember to generate the up-to-date package.
pause
c:\exe\nuget add .\Cadmus.Core\bin\Debug\Cadmus.Core.5.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index\bin\Debug\Cadmus.Index.5.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.MySql\bin\Debug\Cadmus.Index.MySql.5.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Sql\bin\Debug\Cadmus.Index.Sql.5.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Mongo\bin\Debug\Cadmus.Mongo.5.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Seed\bin\Debug\Cadmus.Seed.5.0.0.nupkg -source C:\Projects\_NuGet
pause
