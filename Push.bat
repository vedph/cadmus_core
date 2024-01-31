@echo off
echo PUSH PACKAGES TO NUGET
prompt
set nu=C:\Exe\nuget.exe
set src=-Source https://api.nuget.org/v3/index.json

%nu% push .\Cadmus.Core\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.Ef\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.Ef.MySql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.Ef.PgSql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.Sql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.MsSql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.MySql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.PgSql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Mongo\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Seed\bin\Debug\*.nupkg %src% -SkipDuplicate

%nu% push .\Cadmus.Graph\bin\Debug\*.nupkg %src%
%nu% push .\Cadmus.Graph.Ef\bin\Debug\*.nupkg %src%
%nu% push .\Cadmus.Graph.Ef.MySql\bin\Debug\*.nupkg %src%
%nu% push .\Cadmus.Graph.Ef.PgSql\bin\Debug\*.nupkg %src%
%nu% push .\Cadmus.Graph.Extras\bin\Debug\*.nupkg %src%

echo COMPLETED
echo on
