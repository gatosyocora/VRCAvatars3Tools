Param(
    [String]$src_file_path,
    [String]$dst_file_path
)
$text = Get-Content -Path $src_file_path -Encoding UTF8

for ($i = 0; $i -lt $text.Length; $i++) {
  if (($text[$i].Length -ne 0) -And ($text[$i+1].Length -ne 0)) {
    $text[$i] += '  '
  }
}

Set-Content -Path $dst_file_path -Encoding UTF8 -Value $text