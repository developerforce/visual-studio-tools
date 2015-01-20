$regex = New-Object System.Text.RegularExpressions.Regex('Warning')
$m = $regex.Match((Get-Content MSBuild.log))

