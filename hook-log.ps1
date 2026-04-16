param($payload)

if ($payload) {
    Add-Content -Path 'c:\Users\davor\Documents\GitHub\asp_labvj\logs\agent_log.txt' -Value $payload
}
elseif ($input) {
    $input | Out-String | Add-Content -Path 'c:\Users\davor\Documents\GitHub\asp_labvj\logs\agent_log.txt'
}