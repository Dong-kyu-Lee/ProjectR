# .claude/skills (원본) 내용을 .agents/skills (Codex용) 로 그대로 미러링한다.
# skill은 항상 .claude/skills 에서만 수정하고, 수정 후 이 스크립트를 실행해 함께 커밋한다.
# 사용법 (저장소 루트에서): powershell -ExecutionPolicy Bypass -File .agents/sync-skills.ps1

$root = Split-Path -Parent $PSScriptRoot
$src  = Join-Path $root '.claude\skills'
$dst  = Join-Path $root '.agents\skills'

robocopy $src $dst /MIR /NFL /NDL /NJH /NJS /NP | Out-Null
if ($LASTEXITCODE -ge 8) {
    Write-Error "skill 동기화 실패 (robocopy exit $LASTEXITCODE)"
    exit 1
}
Write-Host "동기화 완료: $src -> $dst"
exit 0
