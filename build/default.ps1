#  PARAMETERS
param(
	[Parameter( Position = 0, Mandatory = 0, HelpMessage = "Relative Path to Source" )]
	[string] $source_path, 
	[Parameter( Position = 1, Mandatory = 0, HelpMessage = "Relative Path to Tools" )]
	[string] $tools_path = ".\..\tools",
	[Parameter( Position = 2, Mandatory = 0, HelpMessage = "Relative Path to nupkg folder" )]
	[string] $nupkg_path,
	[Parameter( Position = 3, Mandatory = 0, HelpMessage = "Should the script pack dlls or source code" )]
	[bool] $pack_source_code_only = $false,
	[Parameter( Position = 4, Mandatory = 0, HelpMessage = "Print debug info" )]
	[bool] $show_debug = $false
)
#  /PARAMETERS


#  CONFIG
if ($show_debug) {
	$DebugPreference = "Continue"
}
#  /CONFIG



#  DEPENDENCIES
Include _key.ps1
Include utils.ps1
Include nunit.ps1
Include nuget.ps1
Include versioning.ps1
#  /DEPENDENCIES



properties {
	Write-Header "Loading Properties"
    Write-Progress "Loading solution properties"
    $conf = @{}
    $conf.source_dir = Set-Source-Path($source_path)
    $conf.configuration = "Release"
    $conf.version = "1.0.0.0"

    #SOLUTION
    $conf.solution_file = $(Get-ChildItem "$($conf.source_dir)\*.sln" | Select-Object -First 1).FullName
    $conf.solution_name = [System.IO.Path]::GetFileNameWithoutExtension($conf.solution_file)
    $conf.solution_shared_assembly = "$($conf.source_dir)\SharedAssemblyInfo.cs"
    $conf.solution_project_assembly_pattern = "$($conf.source_dir)\*\Properties\AssemblyInfo.cs"

    #PROJECT
    $conf.project_dir = "$($conf.source_dir)\$($conf.solution_name)"
    $conf.project_release_dir = "$($conf.project_dir)\bin\$($conf.configuration)"
    $conf.project_release_dll = "$($conf.project_release_dir)\$($conf.solution_name).dll"

    #TEST
    $conf.tests_dir = "$($conf.source_dir)\$($conf.solution_name).Tests"
    $conf.tests_bin_dir = "$($conf.tests_dir)\bin\$($conf.configuration)"

    #NUGET
    $conf.nuget_nupkg_dir = if(![string]::IsNullOrEmpty($nupkg_path)) { $nupkg_path } else { $conf.project_release_dir }

	Write-Success
    Write-HashTable-Debug $conf "Configuration variables"
  
    Write-Progress "Loading tools properties"
    $tools = @{}
    $tools.dir = $tools_path
    $tools.transform = "$($tools.dir)\TransformXml.proj"
    Assert (Test-Path($tools.transform)) "Could not find TransformXml.proj"

    $tools.nuget = "$($tools.dir)\nuget\nuget.exe"
    Assert (Test-Path($tools.nuget)) "Could not find nuget exe"

	Write-Success
    Write-HashTable-Debug $tools "Tools variables"
}



#  TASK NAME FORMAT TASK
FormatTaskName {
   param($taskName)
   Write-Header "Executing Task: $taskName"
}
#  /TASK NAME FORMAT TASK



#  GENERAL TASKS
Task Default -depends Publish
Task Build -depends Clean, Rebuild, Test, Update-Version-From-Dll
Task Package -depends Build, Pack
Task Publish -depends Package, Push
Task PublishFiles -depends Update-Version-From-Nuspec, Pack, Push
#  /GENERAL TASKS



#  VERSION RELATED TASKS
Task Update-Version-From-Dll {
    $conf.version = Get-Dll-Version($conf.project_release_dll)
    $conf.version = Get-Incremented-Version-Numbers $conf.version "build"
    $assembly_files = @(Get-ChildItem $conf.solution_project_assembly_pattern | select FullName | % { "$($_.FullName)" })
    if (![string]::IsNullOrEmpty($conf.solution_shared_assembly)) {
        $resolvedPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($conf.solution_shared_assembly)
        if (Test-Path $resolvedPath) {
            $assembly_files += "$($resolvedPath)"
        }
    }
    Set-Assembly-Version $assembly_files $conf.version
}

Task Update-Version-From-Nuspec {
    $nuspec_file = Get-ChildItem "$(Resolve-Path $conf.source_dir)\" -Recurse -include *.nuspec | Select-Object -First 1
    $conf.version = Get-Nuspec-Version $nuspec_file.FullName $conf.version
    $conf.version = Get-Incremented-Version-Numbers $conf.version "build"
    Update-Nuspec-Version $nuspec_file.FullName $conf.version
}
#  /VERSION RELATED TASKS



#  CLEAN-BUILD-TEST RELATED TASKS
Task Clean {
    Clean-Directory $conf.project_release_dir
}

Task Rebuild {
    Write-Progress "Building $($conf.solution_file)`n`n"
    Exec { msbuild $($conf.solution_file) /t:Rebuild /p:Configuration=$($conf.configuration) /p:OutDir=$($conf.release_dir) /v:minimal /nologo } 
}

Task Test {
	$path = "$($conf.tests_bin_dir)\*.Tests.dll"
	if (Test-Path $path) {
		$test_results_dir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("$($conf.source_dir)\..\TestResults")
		if (!(Test-Path $test_results_dir)) {
			New-Item $test_results_dir -type directory | Out-Null
		}
		Invoke-TestRunner (Get-ChildItem $path) (Resolve-Path $($conf.tests_bin_dir)) $test_results_dir
	} else {
		Write-Host "    " -NoNewline
		Write-Warning "No tests found."
	}
}
#  /CLEAN-BUILD-TEST RELATED TASKS



#  NUGET RELATED TASKS
Task Pack  {
    $nuspecs = @(Get-ChildItem "$(Resolve-Path $conf.source_dir)\" -Recurse -include *.nuspec,*.nutrans)
    Write-HashTable-Debug $nuspecs "Found nuspecs files"
    foreach ($nuspec in $nuspecs) {
        Create-Nupkg $nuspec $conf.nuget_nupkg_dir $conf.configuration $pack_source_code_only $conf.version  
    }
}

Task Push {
    #$nupkgs = @(Get-ChildItem -path "$(Resolve-Path $conf.source_dir)\*\bin" -recurse -attribute Directory "Release" | foreach { Get-ChildItem $_ -File -Recurse -include "*.nupkg" })
    $nupkgs = @(Get-ChildItem "$(Resolve-Path $conf.nuget_nupkg_dir)\" -recurse -include *.nupkg)
    Write-HashTable-Debug $nupkgs "Found nupkg files"
    
    foreach($nupkg in $nupkgs) { 
        Push-Nupkg $nupkg.FullName
    }
}
#  /NUGET RELATED TASKS
