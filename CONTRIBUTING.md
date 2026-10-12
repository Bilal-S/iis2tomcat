# Contributing to the BonCode AJP13 Connector

Thank you for considering a contribution! This document covers how to report issues, set up a build environment, follow the project's code and testing conventions, and submit pull requests.

Using an AI coding agent? Also read [AGENTS.md](AGENTS.md) - it defines the constraints agents must follow, including the rule that agents never perform git operations.

---

## Reporting Issues

Please open an issue on the [GitHub Issues page](https://github.com/Bilal-S/iis2tomcat/issues) before investing time in a large change. Bug fixes and small features can usually go straight to a PR.

**Bug reports should include:**

- Connector version (from the installer or the DLL file version)
- Windows version and IIS version
- Servlet container / CFML engine and its version (Tomcat, Lucee, Railo, Adobe ColdFusion, ...)
- Steps to reproduce
- A relevant excerpt from the BonCode log (enable logging and note the log level used)

**Feature requests** should describe the use case, not only the proposed solution.

---

## Development Environment

- **OS:** Windows - this is an IIS module; builds and unit tests run on Windows
- **IDE:** Visual Studio 2022 (Community is fine) with the **.NET Framework 4.8 targeting pack**
- **Tooling:** MSBuild and NuGet, both installed with Visual Studio
- **Optional:** local IIS plus a servlet container (e.g., Tomcat or Lucee) for manual integration testing

After cloning, restore packages once:

```powershell
nuget restore ConsoleTCPIP.sln
```

> This repository uses old-style (packages.config) projects and **commits the `packages/` folder**. Do not add, update, or remove packages casually.

### Note on Debug builds

The Debug configuration of `BonCodeAJP13` and `BonCodeIIS` writes output to `C:\Sites\AJP13\BIN\` (a maintainer-local IIS site). Either create that directory or adjust the `OutputPath` locally - never commit output-path changes. **Release** builds output to the standard `bin\Release\` folders and are what CI uses.

---

## Building

```powershell
# Build everything (the same invocation CI uses)
msbuild ConsoleTCPIP.sln /p:Configuration=Release /p:Platform="Any CPU" /m
```

See [`Running Local Tests.md`](Running%20Local%20Tests.md) for a full walkthrough including typical Visual Studio tool paths.

---

## Running Tests

The test project is `Connector.Tests` (xUnit 2.9.x). After a successful build:

```powershell
# Full suite
dotnet test Connector.Tests --no-restore --verbosity normal

# A single test class
dotnet test Connector.Tests --filter "FullyQualifiedName~LoggerTests" --no-restore --verbosity normal
```

Tests can also be run from Visual Studio's Test Explorer or with `vstest.console.exe` (see [`Running Local Tests.md`](Running%20Local%20Tests.md)).

CI (`.github/workflows/build.yml`) additionally runs a full integration test on every PR: it installs IIS and Lucee on a Windows runner, deploys the freshly built DLLs, and exercises requests through the connector. Your PR must pass both the unit tests and this integration job.

---

## Code Style

This is a long-lived .NET Framework codebase with established conventions. **Match the surrounding code** - the rules below exist to keep diffs small and reviewable.

- **Target framework:** .NET Framework 4.8, **C# 7.3 syntax maximum** (the default for old-style projects). Avoid C# 8+ features: switch expressions, ranges, nullable reference types, `using` declarations, default interface members, target-typed `new`.
- **License header:** every new source file begins with the Apache License 2.0 header - copy it from an existing file such as `BonCodeAJP13/BonCodeAJP13Packet.cs`.
- **Fields:** private/protected fields use the `p_` prefix (`p_PacketType`, `p_ByteStore`).
- **Regions:** group class members into `#region` blocks (`data Members`, `Properties`, `Constructors`, ...).
- **Documentation:** public members get XML `///` doc comments.
- **Comments are terse by convention.** Keep the *why*, drop the narrative - production details (traffic volumes, deployment specifics) belong in the issue, not the source. Reference the issue number so the full story stays one click away.
- **No drive-by refactoring or "modernization."** Keep diffs minimal and focused on the change.
- **New `.cs` files must be registered** in the project's `.csproj` with a `<Compile Include="..." />` entry - old-style projects do not pick up files automatically.

---

## Testing Style

Tests live in `Connector.Tests`, mirroring the projects under test (`Connector.Tests/BonCodeAJP13/`, `Connector.Tests/BonCodeIIS/`).

- **Framework:** xUnit 2.9.x - `[Fact]` for single cases, `[Theory]` for parameterized ones.
- **Layout:** AAA with explicit `// Arrange`, `// Act`, `// Assert` comments.
- **Naming:** `MethodUnderTest_Scenario_ExpectedResult`, e.g. `Constructor_WithValidBuffer_SetsUserDataLength`.
- **Shared static state:** tests that touch the static `BonCodeAJP13Settings` (`LOG_LEVEL`, `LOG_DIR`, ...) must be marked `[Collection("Sequential")]` (defined in `SequentialCollection.cs`); otherwise they race under parallel execution.
- **Wire-accurate bytes:** when constructing AJP13 packets in tests, build real wire layouts - magic bytes `0x41 0x42`, big-endian lengths, correct padding. See `ServerConnectionTests.cs`.
- **Packet replay:** captured traffic (`.pak` files under `Connector.Tests/packets/`) can be replayed through the packet classes - see `GetUserDataBytesPakReplayTests.cs` and [`Connector.Tests/Capture README.md`](Connector.Tests/Capture%20README.md).
- **Protected members:** expose them via a small testable subclass inside the test file (see `TestablePacket` in `PacketTests.cs`) rather than widening production visibility.

For depth, read [`Connector.Tests/Testing_Background_Info.md`](Connector.Tests/Testing_Background_Info.md) - it explains the AJP13 wire format, the packet classes, and the `.pak` capture infrastructure.

---

## Code Coverage Goals

Coverage tooling (coverlet/OpenCover, CI reporting) is planned future work. Until then, coverage is enforced qualitatively:

- **Every behavior change ships with tests** that would fail without the change.
- **Every bug fix ships with a regression test** that reproduces the reported bug, with the issue number noted in a comment.
- Prefer extending the nearest existing test file over creating overlapping new ones.

---

## Submitting Pull Requests

1. Branch from `master` and target `master`.
2. Keep PRs small and focused; split unrelated changes into separate PRs.
3. Run the full unit suite locally; CI (build + the IIS/Lucee integration job) must pass.
4. Describe *what* changed and *why*, linking related issues.
5. Contributions are accepted under the project's Apache License 2.0 terms.

---

## AI-Assisted Contributions

Contributions made with coding agents (Claude Code, Copilot, Codex, Cursor, ...) are welcome. Agents must follow [`AGENTS.md`](AGENTS.md). In particular, **agents do not perform git operations** - staging, committing, pushing, pulling, and opening PRs are done by the human contributor, who remains responsible for the submission.