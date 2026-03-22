/**
 * Generate spec-compliant AJP13 SendBodyChunk .pak files.
 * 
 * Lucee consistently writes trail=1 regardless of spec padding formula,
 * which masks the GetUserDataBytes bug. This generator creates packets
 * with CORRECT AJP spec padding (0-3 bytes based on chunk_len % 4)
 * to expose the bug.
 * 
 * AJPv13 spec: payload_len = 1 (type) + 2 (chunk_len) + chunk_len + (4 - chunk_len % 4) % 4
 * GetUserDataBytes bug: returns payload_len - 4 bytes instead of chunk_len bytes
 *   - padding=0: drops 1 byte of body
 *   - padding=1: masked (same as Lucee behavior)
 *   - padding=2: includes 1 garbage byte
 *   - padding=3: includes 2 garbage bytes
 */

const fs = require('fs');
const path = require('path');

const outDir = path.join(__dirname, 'captured', 'spec_compliant');
if (!fs.existsSync(outDir)) fs.mkdirSync(outDir, { recursive: true });

function specPadding(chunkLen) {
    return (4 - (chunkLen % 4)) % 4;
}

function sequentialByte(i) {
    return (i % 254) + 1; // 0x01..0xFE repeating
}

function buildSendBodyChunk(chunkLen, bodyGen) {
    const pad = specPadding(chunkLen);
    const payloadLen = 1 + 2 + chunkLen + pad; // type + chunk_len_field + body + padding
    const totalLen = 4 + payloadLen; // AB + length + payload
    
    const buf = Buffer.alloc(totalLen, 0);
    
    // Header
    buf[0] = 0x41; // 'A'
    buf[1] = 0x42; // 'B'
    buf[2] = (payloadLen >> 8) & 0xFF; // length high
    buf[3] = payloadLen & 0xFF;        // length low
    
    // Payload
    buf[4] = 0x03; // SendBodyChunk type
    buf[5] = (chunkLen >> 8) & 0xFF; // chunk_len high
    buf[6] = chunkLen & 0xFF;        // chunk_len low
    
    // Body data
    for (let i = 0; i < chunkLen; i++) {
        buf[7 + i] = bodyGen(i);
    }
    
    // Padding bytes already zeroed by Buffer.alloc
    
    return buf;
}

function buildMeta(chunkLen, pad, description) {
    const now = new Date();
    const ts = now.toISOString().replace('T', ' ').substring(0, 23);
    return [
        `Timestamp: ${ts}`,
        `ThreadID: 1`,
        `ConnectionID: 1`,
        `PacketType: 0x03`,
        `ByteCount: ${4 + 1 + 2 + chunkLen + pad}`,
        `ChunkLength: ${chunkLen}`,
        `SpecPadding: ${pad}`,
        `Description: ${description}`,
        `Note: Spec-compliant padding (NOT Lucee trail=1)`
    ].join('\n');
}

function writeChunk(name, chunkLen, bodyGen, description) {
    const pad = specPadding(chunkLen);
    const pak = buildSendBodyChunk(chunkLen, bodyGen);
    const meta = buildMeta(chunkLen, pad, description);
    
    const baseName = `spec_${name}_cl${chunkLen}_pad${pad}`;
    fs.writeFileSync(path.join(outDir, baseName + '.pak'), pak);
    fs.writeFileSync(path.join(outDir, baseName + '.pak.meta'), meta + '\n');
    
    return { name: baseName, chunkLen, pad, totalBytes: pak.length };
}

// --- Generate test cases ---

console.log('Generating spec-compliant AJP SendBodyChunk .pak files...\n');

const results = [];

// Case 1: 5000 bytes, single chunk, pad=0 → BUG DROPS 1 BYTE
results.push(writeChunk(
    'binary_5000_singlechunk',
    5000,
    sequentialByte,
    '5000B sequential, single chunk, pad=0 → drops last byte'
));

// Case 2: 9000 bytes split into chunks with varying padding
// Chunk 1: 8184 bytes (8184 % 4 = 0, pad = 0) → BUG DROPS 1 BYTE
results.push(writeChunk(
    'binary_9000_chunk1',
    8184,
    i => sequentialByte(i),
    '9000B chunk 1/2: 8184B, pad=0 → drops last byte'
));

// Chunk 2: 816 bytes (816 % 4 = 0, pad = 0) → BUG DROPS 1 BYTE  
results.push(writeChunk(
    'binary_9000_chunk2',
    816,
    i => sequentialByte(8184 + i),
    '9000B chunk 2/2: 816B, pad=0 → drops last byte'
));

// Case 3: 20510-byte PNG split into 3 chunks
// Chunk 1: 8188 bytes (8188 % 4 = 0, pad = 0) → DROPS 1
results.push(writeChunk(
    'png_20510_chunk1',
    8188,
    i => { /* placeholder, real PNG would need actual bytes */ return sequentialByte(i); },
    '20510B PNG chunk 1/3: 8188B, pad=0 → drops last byte'
));

// Chunk 2: 8188 bytes (8188 % 4 = 0, pad = 0) → DROPS 1
results.push(writeChunk(
    'png_20510_chunk2',
    8188,
    i => sequentialByte(8188 + i),
    '20510B PNG chunk 2/3: 8188B, pad=0 → drops last byte'
));

// Chunk 3: 4134 bytes (4134 % 4 = 2, pad = 2) → INCLUDES 1 GARBAGE BYTE
results.push(writeChunk(
    'png_20510_chunk3',
    4134,
    i => sequentialByte(16376 + i),
    '20510B PNG chunk 3/3: 4134B, pad=2 → includes 1 garbage byte'
));

// Case 4: Small sizes covering all padding values
// pad=0: chunk_len % 4 = 0
results.push(writeChunk('pad0_4bytes', 4, sequentialByte, '4B, pad=0 → drops last byte'));
results.push(writeChunk('pad0_8bytes', 8, sequentialByte, '8B, pad=0 → drops last byte'));

// pad=1: chunk_len % 4 = 3
results.push(writeChunk('pad1_3bytes', 3, sequentialByte, '3B, pad=1 → MASKED (same as Lucee)'));
results.push(writeChunk('pad1_7bytes', 7, sequentialByte, '7B, pad=1 → MASKED'));

// pad=2: chunk_len % 4 = 2
results.push(writeChunk('pad2_2bytes', 2, sequentialByte, '2B, pad=2 → includes 1 garbage byte'));
results.push(writeChunk('pad2_6bytes', 6, sequentialByte, '6B, pad=2 → includes 1 garbage byte'));

// pad=3: chunk_len % 4 = 1
results.push(writeChunk('pad3_1byte', 1, sequentialByte, '1B, pad=3 → includes 2 garbage bytes'));
results.push(writeChunk('pad3_5bytes', 5, sequentialByte, '5B, pad=3 → includes 2 garbage bytes'));

// Case 5: Large single chunk with pad=0 (worst case)
results.push(writeChunk('pad0_8188bytes', 8188, sequentialByte, '8188B, pad=0 → drops last byte'));

// Print summary
console.log('Generated files in: ' + outDir + '\n');
for (const r of results) {
    const bugDelta = r.pad - 1; // payloadLen - 4 - chunkLen = (1+2+cl+pad) - 4 - cl = pad - 1
    let impact;
    if (bugDelta === 0) impact = 'MASKED';
    else if (bugDelta < 0) impact = `DROPS ${-bugDelta} BYTE(S)`;
    else impact = `INCLUDES ${bugDelta} GARBAGE BYTE(S)`;
    
    console.log(`  ${r.name}.pak: cl=${r.chunkLen} pad=${r.pad} total=${r.totalBytes}B → ${impact}`);
}

console.log('\nKey insight: Lucee always writes trail=1, making pad-1=0 (MASKED).');
console.log('Spec-compliant padding varies, exposing TRUNCATE and GARBAGE bugs.\n');