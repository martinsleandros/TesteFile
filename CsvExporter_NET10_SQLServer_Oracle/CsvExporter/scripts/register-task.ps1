param([string]$ExePath="C:\Apps\CsvExporter\CsvExporter.Host.exe",[string]$TaskName="CSV-Daily-Generator")
$action=New-ScheduledTaskAction -Execute $ExePath -WorkingDirectory (Split-Path $ExePath)
$trigger=New-ScheduledTaskTrigger -Daily -At "02:00"
$settings=New-ScheduledTaskSettingsSet -MultipleInstances IgnoreNew -StartWhenAvailable
Write-Host "Register under a dedicated service account using approved credentials."
Register-ScheduledTask -TaskName $TaskName -Action $action -Trigger $trigger -Settings $settings
