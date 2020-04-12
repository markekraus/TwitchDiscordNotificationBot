[CmdletBinding()]
[CmdletBinding()]
Param(
    [Parameter()]
    [string]
    $ProjectRoot = $BuildRoot,

    [Parameter()]
    [String]
    $DotnetSolutionFile = "TwitchDiscordNotificationBot.sln",

    [Parameter()]
    [String]
    $ExeName = "TwitchDiscordNotificationBot.exe",

    [Parameter()]
    [string]
    $OutputPath
)


Set-BuildHeader {
	param($Path)
    Write-Build Green ('=' * 80)
    Write-Build Green ('                Task {0}' -f $Path)
    Write-Build Green ('At {0}:{1}' -f $Task.InvocationInfo.ScriptName, $Task.InvocationInfo.ScriptLineNumber)
    if(($Synopsis = Get-BuildSynopsis $Task)) {
        Write-Build Green ('                {0}' -f $Synopsis)
    }
    Write-Build Green ('-' * 80)
	# task location in a script
    Write-Build Green ' '
}

# Define footers similar to default but change the color to DarkGray.
Set-BuildFooter {
	param($Path)
    Write-Build Green ' '
    Write-Build Green ('=' * 80)
    Write-Build DarkGray ('Done {0}, {1}' -f $Path, $Task.Elapsed)
    Write-Build Green ' '
    Write-Build Green ' '
}

Task Init {
    $Script:DotnetSolutionFile = Join-Path $ProjectRoot $DotnetSolutionFile

    if($OutputPath) {
        'OutputPath derived from user input'
    }
    if(-not $OutputPath -and $env:Build_ArtifactStagingDirectory) {
        'OutputPath derived from Build_ArtifactStagingDirectory'
        $Script:OutputPath = $env:Build_ArtifactStagingDirectory
    }
    if(-not $OutputPath) {
        'OutputPath derived from ProjectRoot'
        $Script:OutputPath = Join-Path $ProjectRoot 'bin'
    }

    ' '
    'ProjectRoot:                  {0}' -f $Script:ProjectRoot
    'DotnetSolutionFile:            {0}' -f $Script:DotnetSolutionFile
    'OutputPath:                   {0}' -f $Script:OutputPath

    ' '
    'Available Dotnet sdks'
    dotnet --list-sdks
}

Task Clean Init, {
    'Removing OutputPath contents'
    Get-ChildItem -Path $OutputPath -Force -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -Verbose
    dotnet clean $DotnetSolutionFile
}

Task Restore Init, {
    dotnet restore $DotnetSolutionFile
}

Task Build Init, {
    dotnet build $DotnetSolutionFile
}

Task Publish Init, {
    dotnet publish -o $OutputPath -r win-x64 -c Release --self-contained true /p:DebugType=None $DotnetSolutionFile
    if($LASTEXITCODE -ne 0)
    {
        Write-Error 'Failed to publish'
    }
    $Script:OutputExeFile =  Get-ChildItem $OutputPath -Filter "MarkEKraus.TwitchDiscordNotificationBot.exe"

    Remove-Item -Force (Join-Path $OutputPath $ExeName) -Verbose -ErrorAction SilentlyContinue

    $Script:NewExeFile = $OutputExeFile | Rename-Item -NewName $ExeName -Force -PassThru 

    ' '
    'OutputExeFile: {0}' -f $OutputExeFile.FullName
    'NewExeFile:    {0}' -f $NewExeFile.FullName

}
