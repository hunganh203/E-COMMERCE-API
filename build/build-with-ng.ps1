# COMMON PATHS

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = Join-Path $buildFolder "outputs"
$webHostFolder = Join-Path $slnFolder "src/Dobby.Web.Host"
$ngFolder = Join-Path $buildFolder "../../reactjs"

## CLEAR ######################################################################

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## RESTORE NUGET PACKAGES #####################################################

Set-Location $slnFolder
dotnet restore

## PUBLISH WEB HOST PROJECT ###################################################

Set-Location $webHostFolder
dotnet publish --output (Join-Path $outputFolder "Host") --configuration Release

## PUBLISH WEB PUBLIC PROJECT ###################################################

# Change Public configuration
$publicConfigPath = Join-Path $outputFolder "Host/appsettings.Staging.json"
(Get-Content $publicConfigPath) -replace "9903", "9902" | Set-Content $publicConfigPath

 
## CREATE DOCKER IMAGES #######################################################

# Host
Set-Location (Join-Path $outputFolder "Host")

docker rmi dobby/host -f
docker build -t dobby/host .


## DOCKER COMPOSE FILES #######################################################

Copy-Item (Join-Path $slnFolder "docker/ng/*.*") $outputFolder

## FINALIZE ###################################################################

Set-Location $outputFolder