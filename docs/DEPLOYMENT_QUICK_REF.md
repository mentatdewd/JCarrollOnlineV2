# Quick Reference: GitHub Actions Deployment

## ?? Automatic Deployment Process

When you push to `master`, this happens automatically:

```
Your Code ? Build ? Test ? Deploy ? Live!
   ?         ?       ?       ?        ?
 Push    Compile  Verify  Upload   ?
```

**Deployment happens ONLY if:**
- ? Production build succeeds
- ? All unit tests pass (against that build)
- ? All verification checks pass

## ?? Common Tasks

### Push Code to Production
```bash
git add .
git commit -m "Your changes"
git push origin master
# Deployment starts automatically!
```

### Check Deployment Status
1. Go to: https://github.com/mentatdewd/JCarrollOnlineV2/actions
2. Click on the latest "Deploy to HostGator" workflow
3. Watch the three stages: Test ? Build ? Deploy

### Run Tests Only
```bash
# Tests run automatically on every PR
# Or manually trigger from Actions tab ? Unit Tests ? Run workflow
```

### View Test Results
1. Go to Actions tab
2. Click on any workflow run
3. Look for "Unit Test Results" in the summary

### Rollback After Bad Deployment
```bash
git revert HEAD
git push origin master
# This deploys the previous version
```

## ?? What Each Stage Does

| Stage | What It Does | Duration | Can Fail? |
|-------|-------------|----------|-----------|
| ?? **Test** | Runs all unit tests | ~2-5 min | Yes - stops pipeline |
| ?? **Build** | Compiles for production | ~3-5 min | Yes - stops deployment |
| ?? **Deploy** | Uploads to HostGator | ~5-10 min | Yes - keeps old version |

## ?? Troubleshooting

### "Deployment Skipped"
**Reason**: Tests or build failed  
**Fix**: Check the failed stage, fix the issue, push again

### "Tests Failed"
**Reason**: Unit test(s) failing  
**Fix**: Run tests locally, fix failing tests, push again
```bash
# Run tests locally in Visual Studio
Test ? Run All Tests
```

### "Build Failed"
**Reason**: Compilation error or missing files  
**Fix**: Build locally in Release mode, fix errors, push again
```bash
# Build in Release mode locally
Build ? Configuration Manager ? Release ? Build Solution
```

### "FTP Deploy Failed"
**Reason**: Network issue or FTP server problem  
**Fix**: Re-run the workflow from Actions tab (build artifact is cached)

## ?? Security Notes

- Secrets are injected during deployment (not stored in code)
- Web.config placeholders replaced automatically
- Build artifacts retained for 7 days only
- Production environment protected in GitHub settings

## ?? Monitoring

### Check if Deployment Worked:
1. Visit: https://jcarrollonline.com
2. Check GitHub Actions for green checkmarks
3. Review deployment summary in Actions

### Get Notified:
- Configure GitHub notifications in your settings
- Watch the repository to get email on failures
- Set up Slack integration (optional)

## ?? Key Concepts

**Artifact**: The compiled build output stored temporarily in GitHub. The deploy stage downloads this instead of rebuilding.

**Environment**: GitHub's protection for production deployments. Can require approvals or restrict who can deploy.

**Workflow**: The automated pipeline (Test ? Build ? Deploy)

**Job**: One stage in the workflow (e.g., the "Test" job)

**Step**: Individual actions within a job (e.g., "Checkout code")

## ?? Getting Help

1. Check the [Full Documentation](GITHUB_WORKFLOWS.md)
2. Review [Deployment Pipeline Details](DEPLOYMENT_PIPELINE.md)
3. Check workflow logs in Actions tab
4. Review the actual workflow files in `.github/workflows/`

## ? Pro Tips

?? **Tip #1**: Create feature branches and open PRs. Tests run automatically and you can review before merging.

?? **Tip #2**: If tests fail in CI but pass locally, check for environment differences (connection strings, file paths, etc.)

?? **Tip #3**: Build artifacts are saved. If deployment fails, you can re-run just the deploy stage without rebuilding.

?? **Tip #4**: Use draft PRs to run tests without requesting reviews.

?? **Tip #5**: The workflow logs show exactly which test failed and why.

---

**Last Updated**: When workflow files were created  
**Maintained By**: Development Team
