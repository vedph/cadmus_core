@echo off
echo PRESS ANY KEY TO INSTALL Cadmus Libraries TO LOCAL NUGET FEED
echo Remember to generate the up-to-date package.

c:\exe\nuget add .\Cadmus.Core\bin\Debug\Cadmus.Core.8.0.7.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index\bin\Debug\Cadmus.Index.8.0.7.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Ef\bin\Debug\Cadmus.Index.Ef.8.0.7.nupkg -source C:\Projects\_NuGet
rem c:\exe\nuget add .\Cadmus.Index.Ef.MySql\bin\Debug\Cadmus.Index.Ef.MySql.9.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Ef.PgSql\bin\Debug\Cadmus.Index.Ef.PgSql.9.0.0.nupkg -source C:\Projects\_NuGet
rem c:\exe\nuget add .\Cadmus.Index.MySql\bin\Debug\Cadmus.Index.MySql.9.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.PgSql\bin\Debug\Cadmus.Index.PgSql.9.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Sql\bin\Debug\Cadmus.Index.Sql.9.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Mongo\bin\Debug\Cadmus.Mongo.8.0.7.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Seed\bin\Debug\Cadmus.Seed.8.0.7.nupkg -source C:\Projects\_NuGet

c:\exe\nuget add .\Cadmus.Graph\bin\Debug\Cadmus.Graph.8.0.7.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Graph.Ef\bin\Debug\Cadmus.Graph.Ef.8.0.7.nupkg -source C:\Projects\_NuGet
rem c:\exe\nuget add .\Cadmus.Graph.Ef.MySql\bin\Debug\Cadmus.Graph.Ef.MySql.9.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Graph.Ef.PgSql\bin\Debug\Cadmus.Graph.Ef.PgSql.9.0.0.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Graph.Extras\bin\Debug\Cadmus.Graph.Extras.8.0.7.nupkg -source C:\Projects\_NuGet

pause
