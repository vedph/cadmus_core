@echo off
echo PUSH PACKAGES TO NUGET
prompt
set nu=C:\Exe\nuget.exe
set src=-Source https://api.nuget.org/v3/index.json

%nu% push .\Cadmus.Core\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Cli.Core\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.Sql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.MsSql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Index.MySql\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Mongo\bin\Debug\*.nupkg %src% -SkipDuplicate
%nu% push .\Cadmus.Seed\bin\Debug\*.nupkg %src% -SkipDuplicate
echo COMPLETED
echo on
