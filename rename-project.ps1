param(
    [Parameter(Mandatory = $true)]
    [string]$NewProjectName
)

# Configuration
$OldProjectName = "SolutionName"
$SolutionFile = "SolutionName.sln"
$RootDirectory = Get-Location

# Function to replace text in files
function Update-TextInFiles {
    param(
        [string]$Directory,
        [string]$SearchText,
        [string]$ReplaceText
    )

    Get-ChildItem -Path $Directory -Recurse -File | Where-Object {
        $_.FullName -notmatch '\\bin\\|\\obj\\|\.git\\'
    } | ForEach-Object {
        $content = Get-Content -Path $_.FullName -Raw
        if ($content -match [regex]::Escape($SearchText)) {
            $newContent = $content -replace [regex]::Escape($SearchText), $ReplaceText
            Set-Content -Path $_.FullName -Value $newContent
            Write-Host "Updated content in: $($_.FullName)"
        }
    }
}

# Function to rename files and directories
function Rename-ProjectFiles {
    param(
        [string]$Directory,
        [string]$SearchText,
        [string]$ReplaceText
    )

    # Rename directories first (deepest path first)
    Get-ChildItem -Path $Directory -Recurse -Directory | Sort-Object FullName -Descending | ForEach-Object {
        if ($_.Name -match $SearchText) {
            $newName = $_.Name -replace $SearchText, $ReplaceText
            Rename-Item -Path $_.FullName -NewName $newName
            Write-Host "Renamed directory: $($_.FullName) -> $newName"
        }
    }

    # Then rename files
    Get-ChildItem -Path $Directory -Recurse -File | ForEach-Object {
        if ($_.Name -match $SearchText) {
            $newName = $_.Name -replace $SearchText, $ReplaceText
            Rename-Item -Path $_.FullName -NewName $newName
            Write-Host "Renamed file: $($_.FullName) -> $newName"
        }
    }
}

# Function to update project references
function Update-ProjectReferences {
    param(
        [string]$Directory,
        [string]$SearchText,
        [string]$ReplaceText
    )

    Get-ChildItem -Path $Directory -Recurse -Filter "*.csproj" | ForEach-Object {
        $content = Get-Content $_.FullName -Raw
        if ($content -match [regex]::Escape($SearchText)) {
            $newContent = $content -replace [regex]::Escape($SearchText), $ReplaceText
            Set-Content -Path $_.FullName -Value $newContent
            Write-Host "Updated project reference in: $($_.FullName)"
        }
    }
}

# Function to update solution file
function Update-SolutionFile {
    param(
        [string]$SolutionPath,
        [string]$SearchText,
        [string]$ReplaceText
    )

    $content = Get-Content -Path $SolutionPath -Raw
    $newContent = $content -replace [regex]::Escape($SearchText), $ReplaceText
    Set-Content -Path $SolutionPath -Value $newContent
    Write-Host "Updated solution file: $SolutionPath"
}

# Main script
try {
    Write-Host "Renaming project '$OldProjectName' to '$NewProjectName'..."

    # 1. Update solution file
    $newSolutionFile = $SolutionFile -replace $OldProjectName, $NewProjectName
    Update-SolutionFile -SolutionPath $SolutionFile -SearchText $OldProjectName -ReplaceText $NewProjectName
    Rename-Item -Path $SolutionFile -NewName $newSolutionFile

    # 2. Rename files and directories
    Rename-ProjectFiles -Directory $RootDirectory -SearchText $OldProjectName -ReplaceText $NewProjectName

    # 3. Update project references
    Update-ProjectReferences -Directory $RootDirectory -SearchText $OldProjectName -ReplaceText $NewProjectName

    # 4. Replace text in code files (namespaces, etc.)
    Update-TextInFiles -Directory $RootDirectory -SearchText $OldProjectName -ReplaceText $NewProjectName

    # 5. Update assembly names
    Get-ChildItem -Path $RootDirectory -Filter "*.csproj" -Recurse | ForEach-Object {
        $content = Get-Content $_.FullName -Raw
        if ($content -match "<AssemblyName>$OldProjectName") {
            $newContent = $content -replace "<AssemblyName>$OldProjectName", "<AssemblyName>$NewProjectName"
            Set-Content -Path $_.FullName -Value $newContent
            Write-Host "Updated AssemblyName in: $($_.FullName)"
        }
    }

    # 6. Update appsettings
    Get-ChildItem -Path $RootDirectory -Recurse -Include "appsettings*.json" | ForEach-Object {
        $content = Get-Content $_.FullName -Raw
        if ($content -match [regex]::Escape($OldProjectName)) {
            $newContent = $content -replace [regex]::Escape($OldProjectName), $NewProjectName
            Set-Content -Path $_.FullName -Value $newContent
            Write-Host "Updated appsettings in: $($_.FullName)"
        }
    }

    # 7. Update Program.cs and Startup.cs
    foreach ($file in @("Program.cs", "Startup.cs")) {
        Get-ChildItem -Path $RootDirectory -Recurse -Filter $file | ForEach-Object {
            $content = Get-Content $_.FullName -Raw
            if ($content -match [regex]::Escape($OldProjectName)) {
                $newContent = $content -replace [regex]::Escape($OldProjectName), $NewProjectName
                Set-Content -Path $_.FullName -Value $newContent
                Write-Host "Updated $file in: $($_.FullName)"
            }
        }
    }

    Write-Host "`n✅ Project rename completed successfully!"
    Write-Host "👉 Review and rebuild the solution in your IDE."
    Write-Host "🛠  You may need to manually fix NuGet, launchSettings.json, or IDE caches."

} catch {
    Write-Error "❌ An error occurred: $_"
    exit 1
}




