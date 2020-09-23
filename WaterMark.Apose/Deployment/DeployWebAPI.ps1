Import-Module PSWriteColor
Import-Module BitsTransfer
clear
try { Build-MotelWebAPI } 
catch [System.Management.Automation.CommandNotFoundException]{
   write-host 'ERROR! The app file could not be found.' ;
} catch {
   write-host 'ERROR! Unknown error when executing the step. Error: ' + $_.Exception.Message ;
}

function Build-MotelWebAPI { 

$deploymentfolder=(Get-Date).ToString('dd-MMM-yyyy')
$deployfolder = "F:\Watermark\$deploymentfolder\Watermark"

Write-color 'WELCOME TO MOI BUILD AUTOMATION'  -color Green
$myscriptlocation = ScriptDirectory;
Write-color $myscriptlocation -color Green;
#Set-Location -Path ScriptDirectory
    Set-Location -Path $myscriptlocation;
    Set-Location -Path ..;
Write-color 'WEBAPI CLEAN STARTED' -color Green;
    dotnet clean
Write-color 'WEBAPI BUILD STARTED' -color Green;
    dotnet build;
Write-color 'WEBAPI PUBLISH STARTED' -color Green;
    dotnet publish --runtime ubuntu.18.04-x64 -c Debug
    
    Set-Location bin\Debug\netcoreapp3.1\ubuntu.18.04-x64
    Write-host $(get-location).Path;
    $pattern1 = "appset"
    $folders= Get-ChildItem -path $path -Recurse | Where-Object { if( $_.fullname -like "*$pattern1*") { write-host $_.fullname ;Remove-Item $_.fullname -Force }  }       
    
    Set-Location -Path ..;
    write-color '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*' -color yellow    
    Write-color 'Clean destination folder' -color Yellow;
    write-color '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*' -color yellow        
    Remove-Item $deployfolder -Recurse
    write-color '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*' -color green    
    Write-Host 'REMOVED '$deploymentfolder ' FOLDER FROM DESTINATION';
    write-color '~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*' -color green    
    New-Item -ItemType Directory -Force $deployfolder
    Write-Host $deploymentfolder' FOLDER CREATED IN DESTINATION';
    Write-Host  ''
    Write-Host  ''
    Write-color 'STARTING COPYING FROM SOURCE TO DESTINATION' -color green;
    Copy-Item -Path ubuntu.18.04-x64\publish  -Destination $deployfolder -Recurse
  # Start-BitsTransfer -Source . -Destination "F:\Motel\$deploymentfolder\MotelAPI" -Description "Deployment of Motel WebAPI" -DisplayName "MotelWebAPI"
  #  Copy-File deployfolder, deployfolder

    Write-color 'COPYING FILES FROM SOURCE TO DESTINATION COMPLETED' -color green;
    write-color '**********************************************************' -color green 
    write-color '**********************************************************' -color yellow
    write-color 'PUSBLISHING COMPLETE' -color green
    write-color '**********************************************************' -color yellow
    write-color '**********************************************************' -color green
   #get-childitem -path $(get-location).Path -include *.* | foreach ($_) {
   #if( $_.fullname -like "*$pattern1*") {
   #  Write-host $_.fullname
   #  #Remove-Item $_.fullname -Force -whatIf
   # }else {
   #   // Executes when the Boolean expression is false
   # }
   #
   #
   #}

}
function Copy-File {
    param( [string]$from, [string]$to)
    $ffile = [io.file]::OpenRead($from)
    $tofile = [io.file]::OpenWrite($to)
    Write-Progress -Activity "Copying file" -status "$from -> $to" -PercentComplete 0
    try {
        [byte[]]$buff = new-object byte[] 4096
        [int]$total = [int]$count = 0
        do {
            $count = $ffile.Read($buff, 0, $buff.Length)
            $tofile.Write($buff, 0, $count)
            $total += $count
            if ($total % 1mb -eq 0) {
                Write-Progress -Activity "Copying file" -status "$from -> $to" `
                   -PercentComplete ([int]($total/$ffile.Length* 100))
            }
        } while ($count -gt 0)
    }
    finally {
        $ffile.Dispose()
        $tofile.Dispose()
        Write-Progress -Activity "Copying file" -Status "Ready" -Completed
    }
}
function Get-ScriptDirectory {
    if ($psise) {
        Split-Path $psise.CurrentFile.FullPath
    }
    else {
        $global:PSScriptRoot
    }
}