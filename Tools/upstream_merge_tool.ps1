Write-Output "Moony's upstream merge workflow tool."
Write-Output "This tool can be stopped at any time, i.e. to finish a merge or resolve conflicts. Simply rerun the tool after having resolved the merge with normal git cli."
Write-Output "Pay attention to any output from git! DO NOT RUN THIS ON A WORKING TREE WITH UNCOMMITTED FILES OF ANY KIND."
$target = Read-Host "Enter the branch you're syncing toward (typically upstream/master or similar)"
$refs = git log --reverse --format=format:%H HEAD.. $target

$cherryPickOption = New-Object System.Management.Automation.Host.ChoiceDescription "&Cherry-pick","Uses git cherry pick to integrate the commit into the current branch. BE VERY CAREFUL WITH THIS."
$mergeOption = New-Object System.Management.Automation.Host.ChoiceDescription "&Merge","Uses git merge to integrate the commit and any of it's children into the current branch."
$skipOption = New-Object System.Management.Automation.Host.ChoiceDescription "&Skip","Skips introducing this commit."

$mergeOptions = [System.Management.Automation.Host.ChoiceDescription[]]($skipOption, $mergeOption, $cherryPickOption)

$nonlinears = @()

foreach ($unmerged in $refs) {
    # Finding non-linear commits i.e. merges..
    $parents = (git log --format=format:%P -n 1 $unmerged) -split '\s+'
    if ($parents.Length -eq 1) {
        continue
    }

    # And indexing them, as we're going to need to skip them later to pick the actual merge.
    $nonlinears = $nonlinears + $parents[1..($parents.Length-1)]
}

foreach ($unmerged in $refs) {
    if ($nonlinears -contains $unmerged) {
        Write-Output ("Skipping over {0}, which we'll merge later (non-linear history encountered)." -f $unmerged)
        continue
    }

    $summary = git show --format=format:%s $unmerged

    if ($summary -ieq "automatic changelog update") {
        Write-Output ("Deliberately skipping changelog bot commit {0}." -f $unmerged)
        Write-Output "== GIT =="
        $blanktree = git write-tree # We construct a new tree object.
        # With the current HEAD and the to-be-included commit as parents, adding it to the repo.
        $newhead = git commit-tree $blanktree -p HEAD -p $unmerged -m ("squash! Mergebot skipped commit {0}." -f $unmerged)
        git checkout $newhead -q
        Write-Output "== DONE =="
        continue
    }

    git show --format=full --summary $unmerged

    $parents = (git log --format=format:%P -n 1 $unmerged) -split '\s+'
    Write-Output $parents

    if ($parents.Length -ne 1) {
        $mergedin = $parents[1..($parents.Length-1)]
        if ($summary -match ".*Merge tool skipping '[a-f0-9]{40}'$") {
            Write-Output "Which has children (note: THIS IS A SKIP COMMIT, ANY OPTION BESIDES SKIP WILL FLATTEN IT):"
        } else {
            Write-Output "Which has children (note: Merging again will create a tower of merges, but fully preserves history):"
        }
        
        foreach ($tomerge in $mergedin) {
            git show --format=full --summary $mergedin
        }
    }

    $response = $host.UI.PromptForChoice("Commit action?", "", $mergeOptions, 0)

    Switch ($response) {
        2 {
            Write-Output "== GIT =="
            git cherry-pick -m 1 --allow-empty $unmerged
            Write-Output "== DONE =="
        }
        1 {
            Write-Output "== GIT =="
            git merge --no-ff -m ("squash! Merge tool integrating '{0}'" -f $unmerged) $unmerged
            Write-Output "== DONE =="
        }
        0 {
            Write-Output ("Skipping {0}" -f $unmerged)
            Write-Output "== GIT =="
            $blanktree = git write-tree # We construct a new tree object.
            # With the current HEAD and the to-be-included commit as parents, adding it to the repo.
            $newhead = git commit-tree $blanktree -p HEAD -p $unmerged -m ("squash! Mergebot skipped commit {0}." -f $unmerged)
            git checkout $newhead -q
            Write-Output "== DONE =="
        }
    }
}