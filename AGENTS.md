# AGENTS.md

Instructions for AI coding agents working in this repository. Humans contributing with an agent should also read [CONTRIBUTING.md](CONTRIBUTING.md); this file focuses on the constraints agents most often get wrong.

## Project Overview

BonCode AJP13 Connector - a fully managed IIS module that bridges IIS to AJP13-compatible servlet containers (Tomcat, JBoss, Jetty, Lucee, Railo, Adobe ColdFusion, ...).

| Path | Role |
|---|---|
| `BonCodeAJP13/` | Core AJP13 protocol library: packets, server connection, logging, settings. Most work happens here. |
| `BonCodeAJP13/ServerPackets/` | Packets sent **to** the container (ForwardRequest, CPing) |
| `BonCodeAJP13/TomcatPackets/` | Packets received **from** the container (SendHeaders, SendBodyChunk, EndResponse, GetBodyChunk, CPongReply) |
| `BonCodeIIS/` | IIS integration: managed request handler and web management pages |
| `ConsoleTCPIP/` | Standalone TCP test console |
| `Connector.Tests/` | xUnit test suite plus `.pak` packet-capture fixtures |
| `.github/workflows/build.yml` | CI: Release build, then a real IIS + Lucee integration test |

## Hard Constraints

1. **.NET Framework 4.8 - not .NET / .NET Core.** Do not migrate projects to SDK-style or use `dotnet build` for the solution.
2. **C# 7.3 syntax maximum** (projects do not set `LangVersion`). Forbidden: switch expressions, ranges, nullable reference types, `using` declarations, default interface members, target-typed `new`.
3. **Old-style `.csproj` files:** every new `.cs` file must be registered with a `<Compile Include="..." />` entry in the correct project file or it will not compile.
4. **Windows-only.** Building and testing require MSBuild / Visual Studio tooling on Windows.
5. **Vendored packages:** NuGet packages are committed under `packages/`. Never add, update, or delete packages or edit `packages.config` unless the task explicitly requires it.
6. **Do not modify:**
   - `packages/` (vendored NuGet packages)
   - `.dev/` (maintainer working notes)
   - `Connector.Tests/packets/captured/` (test fixture data)
   - `BonCodeAJP13/ReadMe Notes.txt` (maintainer release notes)
   - `ConsoleTCPIP.vsmdi` and `*.testsettings` (legacy solution items)
7. **Match the style of the file you are editing:** `p_`-prefixed fields, `#region` grouping, XML doc comments on public members, Apache 2.0 header at the top of new source files.
8. **Terse comments.** Explain *why*, skip the story; production details belong in the issue tracker. Reference issue numbers (`// see #115`) instead of narrating.
9. **Minimal diffs.** No drive-by refactors, renames, formatting sweeps, or "modernization." Unrelated cleanups belong in a separate PR - opened by the user, not you.
10. **No console output in production code.** The connector runs inside IIS worker processes; use `BonCodeAJP13Logger`, never `Console.WriteLine`.

## Git Policy: Hands Off

**Never perform state-changing git operations.** Do not run:

- `git add`, `git commit`, `git stash`
- `git push`, `git pull`, `git fetch`
- `git merge`, `git rebase`, `git reset`, `git revert`
- `git checkout` / `git switch` (changing branches or working-tree state), `git clean`

Read-only inspection is fine and encouraged when gathering context: `git status`, `git diff`, `git log`, `git show`, `git blame`.

**Your deliverable is verified working-tree changes - nothing more.** When finished, stop and hand back to the user. The user reviews the diff, commits, pushes, and opens the PR.

## Build and Test Commands

From the repository root (PowerShell on Windows):

```powershell
# One-time restore
nuget restore ConsoleTCPIP.sln

# Build (same invocation as CI)
msbuild ConsoleTCPIP.sln /p:Configuration=Release /p:Platform="Any CPU" /m

# Full unit test suite
dotnet test Connector.Tests --no-restore --verbosity normal

# Single class or test
dotnet test Connector.Tests --filter "FullyQualifiedName~LoggerTests" --no-restore --verbosity normal
```

Notes:

- The **Debug** configuration of `BonCodeAJP13`/`BonCodeIIS` outputs to `C:\Sites\AJP13\BIN\`, a maintainer-local path. Build **Release** unless the task specifically needs Debug; never "fix" these paths.
- Full CI verification includes a real IIS + Lucee integration run (`.github/workflows/build.yml`). You cannot reproduce that locally unless you set up IIS yourself; rely on CI after the user pushes.

## Testing Rules

- Every behavior change needs tests; every bug fix needs a regression test that fails without the fix. Note the issue number in a test comment.
- xUnit 2.9.x, AAA layout with `// Arrange / // Act / // Assert` comments.
- Naming: `MethodUnderTest_Scenario_ExpectedResult`.
- Tests touching the static `BonCodeAJP13Settings` (`LOG_LEVEL`, `LOG_DIR`, ...) **must** use `[Collection("Sequential")]` (see `SequentialCollection.cs`) or they race under parallel execution.
- Build wire-accurate AJP13 byte layouts in tests: magic bytes `0x41 0x42`, big-endian lengths, correct SendBodyChunk padding. Copy the patterns in `ServerConnectionTests.cs`.
- To test protected members, define a small testable subclass inside the test file (`TestablePacket` in `PacketTests.cs`); do not widen production visibility just for tests.
- To test private methods, use a focused reflection helper (see the `Analyze` helper in `ServerConnectionTests.cs`).
- New test files go in `Connector.Tests/<ProjectName>/` and **must be registered in `Connector.Tests/Connector.Tests.csproj`**.

## AJP13 Quick Facts

- Wire format: `[0x41 0x42] [len_hi len_lo] [payload...]` - payload length is a big-endian `uint16`; the 4-byte header is not part of the payload.
- Size limits: max packet payload 8192; max ForwardRequest user data 8186; max SendBodyChunk body 8188 (see `BonCodeAJP13Consts` / `BonCodeAJP13Settings`).
- SendBodyChunk payload: `[0x03][chunk_len: u16 BE][body...][padding]`, where `padding = (4 - (chunk_length % 4)) % 4`.
- Packet type bytes - container→server: `0x04` SendHeaders, `0x03` SendBodyChunk, `0x05` EndResponse, `0x06` GetBodyChunk, `0x09` CPongReply; server→container: `0x02` ForwardRequest, `0x01` CPing.
- Before changing any packet class, read [`Connector.Tests/Testing_Background_Info.md`](Connector.Tests/Testing_Background_Info.md).

## Definition of Done

1. Release build succeeds: `msbuild ConsoleTCPIP.sln /p:Configuration=Release /p:Platform="Any CPU" /m`.
2. Full unit suite passes: `dotnet test Connector.Tests --no-restore --verbosity normal`.
3. New/changed behavior is covered by tests following the conventions above.
4. No unrelated files modified; no git operations performed.
5. Comments are terse and reference issue numbers where relevant.
6. Report a summary of changes and test results, then stop - the user handles git and the PR.

## Further Reading

- [`CONTRIBUTING.md`](CONTRIBUTING.md) - human-oriented contribution workflow and conventions
- [`Connector.Tests/Testing_Background_Info.md`](Connector.Tests/Testing_Background_Info.md) - AJP13 internals, `.pak` capture format, known-bug history
- [`Connector.Tests/Capture README.md`](Connector.Tests/Capture%20README.md) - how to capture new `.pak` fixtures
- [`Running Local Tests.md`](Running%20Local%20Tests.md) - local build/test walkthrough