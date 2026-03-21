# Packet Capture Test Files

## Overview

This directory contains test assets for capturing and validating raw AJP13 packet bytes.

- **`cfm/`** — Lucee/CFML test pages that generate specific response scenarios when accessed through the BonCode AJP connector
- **`captured/`** — (create manually) Destination for `.pak` files captured at log level 5

## Capturing Packets

### Automated (recommended)

Use the provided Node.js script to hit every test page automatically:

```bash
node Connector.Tests/packets/capture.js
```

**Prerequisites:**
- BonCode `LogLevel` set to `5` in your AJP configuration (app.config or web.config)
- IIS running with the BonCode AJP connector
- Lucee/Tomcat running with the CFM test pages deployed to the web root

**What the script does:**
- Sends 16 sequential requests (7 GET, 4 status code variations, 5 POST) to `http://localhost/`
- Waits 500ms between requests to prevent packet interleaving
- Saves binary response files (PNG) to `captured/` for offline comparison
- Prints status code, byte count, and response body for each request

**Output:** `.pak` and `.pak.meta` files appear in BonCode's configured log directory. Move selected files to `captured/` for test validation.

### Manual

1. Set `LogLevel` to `5` in your BonCode AJP configuration
2. Make requests through IIS → BonCode → Lucee to each `.cfm` test page
3. Raw packet files (`.pak` + `.pak.meta`) are written to the configured log directory
4. Move selected `.pak` files here for test validation

## File Format

### `.pak` files

Raw wire-level bytes of a single AJP13 packet:
```
[magic: 0x12 0x34] [length: 2B big-endian] [type: 1B] [data: NB] [padding: 0-3B]
```

### `.pak.meta` files

Human-readable metadata sidecar:
```
Timestamp: 2026-03-21 13:45:00.123
ThreadID: 8
ConnectionID: 42
PacketType: 0x03
ByteCount: 11
```

### Packet Type Reference

| Type | Hex | Description |
|------|-----|-------------|
| SendBodyChunk | 0x03 | Response body data |
| SendHeaders | 0x04 | Response headers + status |
| EndResponse | 0x05 | End of response |
| GetBodyChunk | 0x06 | Request more POST body data |
| CPongReply | 0x09 | Reply to CPing |
| ForwardRequest | 0x02 | Outgoing request to Tomcat |

## Test Scenarios

| File | Scenario | Bug Exposed |
|------|----------|-------------|
| `test_get_simple.cfm` | GET, 12-byte HTML (padding=0) | Bug 1: drops last byte |
| `test_get_text_size4.cfm` | GET, 4-byte text (padding=0) | Bug 1: drops last byte |
| `test_get_text_odd.cfm` | GET, 5-byte text (padding=3) | Bug 1: includes padding bytes |
| `test_get_binary.cfm` | GET, 67-byte PNG (padding=1) | Bug 1: drops last byte |
| `test_get_binary_odd.cfm` | GET, 3-byte binary (padding=1) | Works by accident |
| `test_get_large_response.cfm` | GET, 16KB multi-chunk | Multi-chunk assembly |
| `test_post_small.cfm` | POST, small form data | Basic POST flow |
| `test_post_large.cfm` | POST, ~20KB data | Bug 2: integer division |
| `test_post_binary.cfm` | POST + binary response | Bug 1 on response |
| `test_post_chunked.cfm` | Chunked transfer encoding | Chunk handling |
| `test_headers_multi.cfm` | Multiple Set-Cookie headers | Header delimiter parsing |
| `test_status_codes.cfm` | Various HTTP status codes | GetStatus parsing |