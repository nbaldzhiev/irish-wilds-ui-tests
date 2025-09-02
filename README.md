# Irish Wilds UI Tests

A repository containing UI Tests, written in C# + Playwright, for the [Irish Wilds game](https://games.spinberry.com/irish_wilds_demo_site).

## Running tests

You can run the tests in two main ways: locally or via GitHub Actions (recommended) (you need to ask to be a repo collaborator).

When running the tests locally, be aware that numeric parsing in C# is culture-sensitive. Therefore, there can be issues with local executions and running via the GitHub Actions workflow is recommended. The implementation was done on VSCode with the .NET (`9.0.304`) toolkit on a macos.

### Locally

Clone the repository and run:

    $ cd irish-wilds-ui-tests
    $ dotnet build
    $ pwsh bin/Debug/netX/playwright.ps1 install
    $ HEADLESS=false DEVICE="Galaxy S24" dotnet test

Environment variable `DEVICE` can receive any of the valid [Playwright device descriptors](https://github.com/microsoft/playwright/blob/main/packages/playwright-core/src/server/deviceDescriptorsSource.json). Environment variable `HEADLESS` controls whether or not the browser is ran in headless mode (default is `true`).

### GitHub Actions (CI)

-   [run-tests.yml](https://github.com/nbaldzhiev/irish-wilds-ui-tests/blob/main/.github/workflows/run-tests.yml) - Runs all tests upon manual trigger, receiving the device name as workflow input parameter;

> **_NOTE:_** You need to be a repository collaborator in order to run the workflow.

> **_NOTE 2:_** There seems to be some limitation with Webkit on newer Ubuntu/Playwright versions and it fails due to missing dependencies (https://github.com/microsoft/playwright/issues/30368).

### Playwright traces

The tests generate Playwright traces that can be opened locally or in https://trace.playwright.dev/.

- When running the tests locally, the location of the traces is in `PlaywrightTests/bin/Debug/netX/playwright-traces/`.
- When running the tests via the GitHub Actions workflow, the traces are uploaded as workflow artifacts. You need to download the artifacts, unarchive and rename the files from `.tracezip` to `.zip` (which is due to an auto-unpack limitation of the `upload-artifact` action).
