# Downloads external images referenced in .cshtml files into wwwroot/images and replaces src with local path
# Usage: run from repo root in PowerShell

$cshtmlFiles = Get-ChildItem -Path . -Include *.cshtml -Recurse | Where-Object { $_.FullName -notlike "*/bin/*" -and $_.FullName -notlike "*/obj/*" }
$imagesDir = Join-Path -Path "TechBricks" -ChildPath "wwwroot/images"
if (-not (Test-Path $imagesDir)) { New-Item -ItemType Directory -Path $imagesDir | Out-Null }

$regex = 'src\s*=\s*"(https?://[^"]+)"'

foreach ($file in $cshtmlFiles) {
    Write-Host "Processing $($file.FullName)"
    $content = Get-Content -Raw -Path $file.FullName
    $matches = [regex]::Matches($content, $regex)
    if ($matches.Count -eq 0) { continue }
    $updated = $false
    foreach ($m in $matches) {
        $url = $m.Groups[1].Value
        try {
            $uri = [uri]$url
        } catch {
            Write-Warning "Skipping invalid URL: $url"
            continue
        }
        $filename = [IO.Path]::GetFileName($uri.LocalPath)
        if ([string]::IsNullOrWhiteSpace($filename)) {
            $hash = [System.BitConverter]::ToString((New-Object Security.Cryptography.SHA1CryptoServiceProvider).ComputeHash([Text.Encoding]::UTF8.GetBytes($url))).Replace('-', '')
            $filename = "img_$hash.jpg"
        }
        $dest = Join-Path $imagesDir $filename
        if (-not (Test-Path $dest)) {
            Write-Host "Downloading $url -> $dest"
            try {
                Invoke-WebRequest -Uri $url -OutFile $dest -UseBasicParsing -ErrorAction Stop
            } catch {
                Write-Warning "Failed to download $url : $_"
                continue
            }
        } else {
            Write-Host "Already exists: $dest"
        }
        # Replace URL with local app path
        $local = "~/images/$filename"
        $content = $content -replace [regex]::Escape($m.Value), "src=\"$local\""
        $updated = $true
    }
    if ($updated) {
        Write-Host "Updating file $($file.FullName)"
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8
    }
}
Write-Host "Done. Review changes, commit and push the branch."
