@echo off
echo PRESS ANY KEY TO INSTALL Cadmus Libraries TO LOCAL NUGET FEED
echo Remember to generate the up-to-date package.
c:\exe\nuget add .\Cadmus.Core\bin\Debug\Cadmus.Core.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index\bin\Debug\Cadmus.Index.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Ef\bin\Debug\Cadmus.Index.Ef.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Ef.MySql\bin\Debug\Cadmus.Index.Ef.MySql.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Ef.PgSql\bin\Debug\Cadmus.Index.Ef.PgSql.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.MySql\bin\Debug\Cadmus.Index.MySql.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.PgSql\bin\Debug\Cadmus.Index.PgSql.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Index.Sql\bin\Debug\Cadmus.Index.Sql.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Mongo\bin\Debug\Cadmus.Mongo.6.1.17.nupkg -source C:\Projects\_NuGet
c:\exe\nuget add .\Cadmus.Seed\bin\Debug\Cadmus.Seed.6.1.17.nupkg -source C:\Projects\_NuGet
pause
