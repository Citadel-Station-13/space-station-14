Write-Output "Moony's upstream merge workflow tool."
Write-Output "This tool can be stopped at any time, i.e. to finish a merge or resolve conflicts. Simply rerun the tool after having resolved the merge with normal git cli."
Write-Output "Pay attention to any output from git!"
$target = Read-Host "Enter the branch you're syncing toward (typically upstream/master or similar)"
$refs = git log --reverse --format=format:%H HEAD.. $target

$cherryPickOption = New-Object System.Management.Automation.Host.ChoiceDescription "&Cherry-pick","Uses git cherry pick to integrate the commit into the current branch. BE VERY CAREFUL WITH THIS."
$mergeOption = New-Object System.Management.Automation.Host.ChoiceDescription "&Merge","Uses git merge to integrate the commit and any of it's children into the current branch."
$skipOption = New-Object System.Management.Automation.Host.ChoiceDescription "&Skip","Skips introducing this commit."

$mergeOptions = [System.Management.Automation.Host.ChoiceDescription[]]($skipOption, $mergeOption, $cherryPickOption)

foreach ($unmerged in $refs) {
    $summary = git show --format=format:%s $unmerged

    if ($summary -ieq "automatic changelog update") {
        Write-Output ("Deliberately skipping changelog bot commit {0}." -f $unmerged)
        continue
    }

    git show --format=full --summary $unmerged

    $response = $host.UI.PromptForChoice("Commit action?", "", $mergeOptions, 0)

    Switch ($response) {
        2 {
            Write-Output "== GIT =="
            git cherry-pick $unmerged
            Write-Output "== DONE =="
        }
        1 {
            Write-Output "== GIT =="
            git merge $unmerged
            Write-Output "== DONE =="
        }
        0 { Write-Output ("Skipping " -f $unmerged) }
    }
}
