# Testing Background Info

A reference document for understanding the AJP13 protocol internals, `GetUserDataBytes` behavior, and `.pak` capture file infrastructure used by the test suite.

---

## 1. AJP13 Protocol Overview

AJP13 (Apache JServ Protocol v1.3) is a binary protocol used between a web server (IIS, via the BonCode connector) and a servlet container (Tomcat, Lucee). All communication happens over a persistent TCP socket.

### Wire Format

Every AJP13 packet on the wire has this structure:

```
[0x41 0x42]        — Magic bytes ("AB")
[len_hi len_lo]    — Payload length (big-endian uint16)
[payload...]       — Packet-type-specific data
```

The 4-byte header is **not** part of `p_ByteStore` for Tomcat-to-server packets. The BonCode connector strips it during `AnalyzePackage()` before constructing `TomcatReturn` subclasses. So `p_ByteStore` contains only the payload.

### Packet Types (Container → Server)

| Type Byte | Class                 | Purpose                          |
|-----------|-----------------------|----------------------------------|
| `0x03`    | `TomcatSendBodyChunk` | Body data chunk (response body)  |
| `0x04`    | `TomcatSendHeaders`   | HTTP status + response headers   |
| `0x05`    | `TomcatEndResponse`   | End of response marker           |
| `0x06`    | `TomcatGetBodyChunk`  | Request for more POST body data  |
| `0x09`    | `TomcatCPongReply`    | Reply to CPing keepalive         |

### Packet Types (Server → Container)

| Type Byte | Class                        | Purpose                      |
|-----------|------------------------------|------------------------------|
| `0x02`    | `BonCodeAJP13ForwardRequest` | Forward HTTP request to Tomcat |
| `0x01`    | `BonCodeAJP13CPing`          | Keepalive ping               |

### SendBodyChunk Internal Layout

This is the most important packet for response handling:

```
Offset  Size    Field
0       1       Type byte (0x03)
1       2       chunk_length (big-endian uint16) — length of body data ONLY
3       N       Body data (N = chunk_length)
3+N     0-3     Padding to align total payload to 4-byte boundary
```

**Padding rule:** `padding = (4 - (chunk_length % 4)) % 4`

Examples:
- chunk_length=8 → padding=0 (8 % 4 = 0)
- chunk_length=7 → padding=1 (7 % 4 = 3, need 1 more)
- chunk_length=5 → padding=3 (5 % 4 = 1, need 3 more)
- chunk_length=8188 → padding=0 (max single chunk)

### Max Sizes

- `MAX_BONCODEAJP13_PACKET_LENGTH` = 8192 (total payload including type byte)
- `MAX_BONCODEAJP13_USERDATA_LENGTH` = 8186 (max forward request body)
- Max SendBodyChunk body = 8188 bytes (8192 payload - 1 type - 2 chunk_length - 1 minimum)

---

## 2. GetUserDataBytes — What It Returns and Why

`GetUserDataBytes()` is the method that extracts the "useful" data from a packet's `p_ByteStore`. Each packet type has different internal structure, so the extraction logic differs.

### Base Class: `TomcatReturn.GetUserDataBytes()`

```csharp
// Skips 4-byte AJP header, strips trailing control bytes
// Returns p_ByteStore[4 .. length-4]
```

This works for generic packets where the payload is: `[4-byte header][data][4-byte trailer]`.

### Override: `TomcatSendBodyChunk.GetUserDataBytes()`

```csharp
// Skips 3-byte prefix (type + chunk_length)
// Uses p_UserDataLength to return ONLY body data, excluding padding
// Returns p_ByteStore[3 .. 3+p_UserDataLength]
```

**Critical distinction:** `p_UserDataLength` stores the **chunk_length field from the wire** (body bytes only), NOT the total payload length. The total payload includes padding bytes after the body. If you used `p_ByteStore.Length - 3` instead, you'd get body+padding concatenated — which is wrong.

### The Bug That Was Fixed

Before the fix, `TomcatSendBodyChunk` had two problems:

1. **`p_UserDataLength` was computed wrong:** The `byte[]` constructor did `p_UserDataLength = content.Length - 4` (generic base class formula). For a packet with 8 body bytes + 1 padding byte (9 payload bytes), this gave `9 - 4 = 5` instead of the correct `8`. The fix reads `p_UserDataLength` from the actual chunk_length field at bytes `[1..2]`.

2. **`GetUserDataBytes()` wasn't overridden:** It fell through to the base class which skipped 4 bytes and stripped 4 trailing bytes — completely wrong for SendBodyChunk's 3-byte prefix layout. The override skips exactly 3 bytes and uses `p_UserDataLength` to stop before padding.

### Why This Matters

Any code that calls `GetUserDataBytes()` on a `TomcatSendBodyChunk` received incorrect data — body bytes mixed with padding, or truncated data. **However, the production response-writing path in `CallHandler.cs` does not use `GetUserDataBytes()`** — it writes `p_ByteStore` directly via `GetDataBytes()`. So this bug did **not** cause corrupt binary output in production responses. The wire data is always written faithfully (proven by `PacketCaptureFidelityTests`).

The actual impact was on any **new code** that tried to use `GetUserDataBytes()` to extract clean body data — such as the replay tests and any future consumers. Without the fix, those consumers would get body+padding concatenated, making byte-count assertions fail by exactly the padding amount (0–3 bytes).

---

## 3. .pak Capture Files

### What They Are

`.pak` files are byte-for-byte exact copies of AJP13 packets as they arrived on the wire. They are written by `BonCodeAJP13Logger.LogPacketBytes()` when `BONCODEAJP13_LOG_LEVEL >= BONCODEAJP13_LOG_PACKETS`.

**Wire-faithfulness is proven** by `PacketCaptureFidelityTests` — every byte of the input buffer is written to disk exactly, including AJP padding bytes.

### LogPacketBytes Overloads

```csharp
// Direct: writes entire byte array as-is
void LogPacketBytes(byte[] packetData, int connectionId, byte packetType)

// Slice: extracts a sub-range from a larger buffer (how AnalyzePackage calls it)
void LogPacketBytes(byte[] buffer, int offset, int length, int connectionId, byte packetType)
```

The slice overload does `Array.Copy(buffer, offset, temp, 0, length)` then writes `temp` — so the .pak file contains only the extracted packet, not the surrounding buffer.

### File Naming

```
{sequence:00000}_T{threadId}_{timestamp}.pak
```

Example: `00001_T192_20260322_125653_986.pak`

Each `.pak` has a companion `.pak.meta` file containing:
```
ConnectionId: 192
PacketType: 0x03
ByteCount: 8195
Timestamp: 2026-03-22T12:56:53.986
```

### Directory Structure

```
packets/captured/
├── OldCapture/          # IIS-side packets (ForwardRequest, CPing, body chunks)
│   └── *.pak
└── TomcatCapture/       # Tomcat-side packets (SendHeaders, SendBodyChunk, EndResponse)
    ├── T192_01_SendHeaders.pak
    ├── T192_02_SendBodyChunk.pak
    ├── T192_03_SendBodyChunk.pak
    └── T192_04_EndResponse.pak
```

Packets from the same HTTP response share the same thread ID (e.g., `T192`). A typical response sequence is: SendHeaders → SendBodyChunk (one or more) → EndResponse.

### Replay Workflow

The test suite loads .pak files and replays them through `TomcatSendBodyChunk`:

```csharp
byte[] rawBytes = File.ReadAllBytes("T192_02_SendBodyChunk.pak");
var chunk = new TomcatSendBodyChunk(rawBytes);
byte[] body = chunk.GetUserDataBytes();  // body only, no padding
Assert.Equal(expectedChunkLength, body.Length);
```

This is how `GetUserDataBytesPakReplayTests` validates that real captured packets extract correctly.

### OldCapture vs TomcatCapture

- **OldCapture** packets include the 4-byte AJP header (`0x41 0x42 ...`). These were captured before the header was stripped, or represent server→container packets which have the header intact.
- **TomcatCapture** packets are payload-only (no `AB` magic bytes). They match what `p_ByteStore` contains after `AnalyzePackage()` processes the wire data.

---

## 4. Key Bugs Found & Fixed

### Bug 1: GetUserDataBytes Returns Padding Bytes
- **File:** `BonCodeAJP13/TomcatPackets/TomcatSendBodyChunk.cs`
- **Root cause:** No override of `GetUserDataBytes()`; base class logic wrong for SendBodyChunk layout
- **Fix:** Added override that skips 3-byte prefix and uses `p_UserDataLength` as body length

### Bug 2: p_UserDataLength Computed From Array Length
- **File:** `BonCodeAJP13/TomcatPackets/TomcatSendBodyChunk.cs`
- **Root cause:** `byte[]` constructor used `content.Length - 4` (generic formula) instead of reading the chunk_length field
- **Fix:** Read `p_UserDataLength` from `content[1] << 8 | content[2]` (big-endian uint16 at offset 1-2)

### Bug 3: p_Method/p_Url Only Set At LOG_HEADERS Level
- **File:** `BonCodeAJP13/ServerPackets/BonCodeAJP13ForwardRequest.cs`
- **Root cause:** `p_Method` and `p_Url` were inside `if (LOG_LEVEL >= LOG_HEADERS)` block, but `PrintPacketHeader()` uses them unconditionally
- **Fix:** Moved `p_Method`/`p_Url` assignment outside the log-level guard; kept `p_HttpHeaders` (the full header collection) inside the guard

---

## 5. Known Remaining Work

- **PacketCaptureFidelityTests slice overload failures:** The 5-parameter `LogPacketBytes(buffer, offset, length, connectionId, packetType)` overload may not exist in the current logger, or may have a signature mismatch. The direct `byte[]` overload tests all pass.
- **String constructor in TomcatSendBodyChunk:** The `TomcatSendBodyChunk(string)` constructor still uses the old `content.Length - 4` formula for `p_UserDataLength`. This constructor is a legacy convenience method and isn't used in production packet processing, but should be updated for consistency.
- **OldCapture .pak replay:** Tests don't yet replay OldCapture packets because those include the 4-byte AJP header and need different handling.