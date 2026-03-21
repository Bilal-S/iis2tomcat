# BonCode AJP13 Connector for IIS

**Connect IIS to Apache Tomcat in under a minute — no ISAPI, no hassle.**

[![Download Latest](https://img.shields.io/github/v/release/Bilal-S/iis2tomcat?label=latest%20release&color=0078d4)](https://github.com/Bilal-S/iis2tomcat/releases/latest)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512bd4)](https://docs.microsoft.com/en-us/dotnet/framework/)
[![Windows](https://img.shields.io/badge/platform-Windows%207%20–%20Server%202025-0078d4)]()
[![License](https://img.shields.io/badge/license-see%20source-green)]()

---

## What Is It?

BonCode is a lightweight, fully managed IIS native module that bridges IIS to any AJP13-compatible servlet container — **Tomcat, JBoss, Jetty**, and CFML engines like **Lucee, Railo, OpenBD, and Adobe ColdFusion**.

```
  Browser ──► IIS ──► BonCode AJP13 Module ──► Tomcat / JBoss / Jetty / CFML Engine
```

No ISAPI DLLs. No virtual directory gymnastics. No IIS6-era workarounds. Just a clean, modern IIS extensibility module that gets out of your way.

---

## Quick Start

1. **Download** the latest release from [GitHub Releases](https://github.com/Bilal-S/iis2tomcat/releases/latest)
2. **Run** `Connector_Setup.exe` — the installer configures everything automatically
3. **Done.** Your IIS site is now routing to Tomcat.

For scripted deployments, the connector supports silent command-line installation — ideal for CI/CD pipelines and automated provisioning.

---

## Why BonCode?

| | **BonCode Connector** | **Legacy ISAPI Connector** |
|---|---|---|
| Architecture | Modern IIS Native Module (managed code) | ISAPI filter (IIS6 era) |
| Install | One-click installer or CLI | Manual DLL registration & config |
| IIS Impact | Zero interference with non-Java requests | Can block or slow unrelated requests |
| Bitness | Single build, any process (32/64-bit) | Separate DLLs required |
| Configuration | IIS UI + inheritable config hierarchy | Manual XML editing |
| Virtual Directories | Not required | Required |

---

## Features

### Performance
- Partial stream flushing for faster time-to-first-byte
- Tomcat thread awareness — won't overload or drop connections
- Reduced traffic and processing on both IIS and Tomcat sides
- Connect to multiple Tomcat instances from a single IIS site

### Compatibility
- Works on **Windows 7 through Windows 11** and **Windows Server 2003 through 2025**
- Supports **IIS 7.0+** (all modern versions)
- Single build handles both **32-bit and 64-bit** worker processes
- Compatible with Adobe ColdFusion 10–2021 AJP dialects
- Compatible with Lucee, Railo, and OpenBD CFML engines

### Security
- Built-in simple-security for web administration pages (Lucee, Tomcat, Railo, OpenBD, ColdFusion)
- Client fingerprint mechanism for safer sessions
- Full SSL passthrough to the servlet container
- IPv6 support

### Request Fidelity
- Complete HTTP header forwarding (including previously unavailable headers)
- Accurate SSL data transfer to the servlet container
- Improved load balancer header translation for correct client IP detection
- Alternate Path-Info header transmission via AJP
- All request headers transferred faithfully

### Operations
- Configure directly in the **IIS Manager UI**
- Configuration inherits to sub-paths and virtual sites
- Easy install and uninstall
- No virtual directories or virtual mappings needed

---

## Supported Backends

| Backend | Supported |
|---|:---:|
| Apache Tomcat | ✅ |
| JBoss / WildFly | ✅ |
| Jetty | ✅ |
| Lucee | ✅ |
| Railo | ✅ |
| OpenBD | ✅ |
| Adobe ColdFusion (10–2021) | ✅ |
| Web Methods | ✅ |

*Any AJP13-compatible server will work.*

---

## Proxy & URL Rewrite Benefits

If you're replacing a reverse proxy or URL rewrite setup, BonCode gives you:

- Fully integrated SSL passthrough — no SSL termination proxy needed
- Your servlets receive **correct** HTTP headers, URLs, and client IPs
- No double-hop overhead — direct AJP13 binary protocol
- Run alongside other ISAPI connectors (e.g., Shibboleth) without conflict

---

## Documentation

- 📖 **Full manual** — included in the [release ZIP](https://github.com/Bilal-S/iis2tomcat/releases) as `BonCode_Tomcat_Connector_Manual.pdf`
- 🌐 [Online documentation](http://boncode.net/connector/webdocs/) — includes manual install instructions (automated installer recommended)

### Common Setup Guides

- **[Upgrading from Railo/Lucee Connector](http://www.boncode.net/boncode-connector/upgrading-railo-or-lucee-connector)**
- **[Using with Adobe ColdFusion](http://www.boncode.net/boncode-connector/using-boncode-with-adobe-coldfusion)**

---

## Version History

See the [Releases page](https://github.com/Bilal-S/iis2tomcat/releases) for the full changelog. Detailed release notes are also available in `BonCodeAJP13/ReadMe Notes.txt` in the source tree.

---

## Feedback & Issues

Found a bug or have a feature request? Please open an issue on the [GitHub Issues page](https://github.com/Bilal-S/iis2tomcat/issues).

---

## Building from Source

This solution targets **.NET Framework 4.8** and requires Visual Studio with the .NET Framework 4.8 targeting pack.

```bash
# Build the solution
msbuild ConsoleTCPIP.sln /p:Configuration=Release

# Run tests
msbuild ConsoleTCPIP.sln /p:Configuration=Release
dotnet test Connector.Tests/
```

---

## Project Structure

```
├── BonCodeAJP13/          # Core AJP13 protocol library
│   ├── ServerPackets/     # Packets sent to Tomcat
│   ├── TomcatPackets/     # Packets received from Tomcat
│   └── Config/            # Configuration provider
├── BonCodeIIS/            # IIS native module (managed handler)
├── ConsoleTCPIP/          # Standalone TCP test console
├── Connector.Tests/       # Unit tests
└── ConsoleTCPIP.sln       # Solution file
```

---

*BonCode AJP13 Connector — Copyright 2011-2026 © Bilal Soylu*