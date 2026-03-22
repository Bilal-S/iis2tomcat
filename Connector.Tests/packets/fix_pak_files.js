/**
 * Organize captured .pak files:
 *   - Inbound (0x41 0x42): keep in TomcatCapture/, rename to {connId}_{seq}_{type}.pak
 *   - Outbound (0x12 0x34): move to TomcatCapture/Outbound/, rename to OB_{type}_{connId}_{seq}.pak
 *   - .meta files move alongside their .pak
 */

const fs = require('fs');
const path = require('path');

const captureDir = path.join(__dirname, 'captured', 'TomcatCapture');
const outboundDir = path.join(captureDir, 'Outbound');

const TYPE_NAMES = {
    0x02: 'ForwardRequest',
    0x03: 'SendBodyChunk',
    0x04: 'SendHeaders',
    0x05: 'EndResponse',
    0x06: 'GetBodyChunk',
    0x09: 'CPongReply'
};

if (!fs.existsSync(outboundDir)) fs.mkdirSync(outboundDir, { recursive: true });

const pakFiles = fs.readdirSync(captureDir)
    .filter(f => f.endsWith('.pak'))
    .sort();

let inboundCount = 0, outboundCount = 0, errorCount = 0;
const connSeq = {}; // track per-connection sequence numbers

for (const f of pakFiles) {
    const filePath = path.join(captureDir, f);
    const metaPath = filePath + '.meta';
    const buf = fs.readFileSync(filePath);

    if (buf.length < 5) {
        console.log(`⚠ SKIP (too short): ${f}`);
        errorCount++;
        continue;
    }

    const isInbound = buf[0] === 0x41 && buf[1] === 0x42;
    const isOutbound = buf[0] === 0x12 && buf[1] === 0x34;

    // Extract connection ID from filename
    const connMatch = f.match(/_T(\d+)_/);
    const connId = connMatch ? connMatch[1] : '00';
    if (!connSeq[connId]) connSeq[connId] = { in: 0, out: 0 };

    const packetType = buf[4];
    const typeName = TYPE_NAMES[packetType] || `Type${packetType.toString(16)}`;

    if (isOutbound) {
        connSeq[connId].out++;
        const seq = String(connSeq[connId].out).padStart(2, '0');
        const newName = `OB_${typeName}_T${connId}_${seq}.pak`;
        const newPath = path.join(outboundDir, newName);
        fs.renameSync(filePath, newPath);
        if (fs.existsSync(metaPath)) {
            fs.renameSync(metaPath, newPath + '.meta');
        }
        outboundCount++;
    } else if (isInbound) {
        connSeq[connId].in++;
        const seq = String(connSeq[connId].in).padStart(2, '0');
        const newName = `T${connId}_${seq}_${typeName}.pak`;
        const newPath = path.join(captureDir, newName);
        fs.renameSync(filePath, newPath);
        if (fs.existsSync(metaPath)) {
            fs.renameSync(metaPath, newPath + '.meta');
        }
        inboundCount++;
    } else {
        console.log(`⚠ SKIP (unknown magic 0x${buf[0].toString(16)} 0x${buf[1].toString(16)}): ${f}`);
        errorCount++;
    }
}

// Also move outbound 8192-byte response files
const responseFiles = fs.readdirSync(captureDir)
    .filter(f => f.endsWith('.response'));

console.log('='.repeat(60));
console.log(' PAK File Organization Complete');
console.log('='.repeat(60));
console.log(` Inbound:  ${inboundCount} files → ${captureDir}/`);
console.log(` Outbound: ${outboundCount} files → ${outboundDir}/`);
console.log(` Errors:   ${errorCount}`);
console.log(` Response files in place: ${responseFiles.length}`);
console.log('='.repeat(60));

// List inbound files grouped by connection
console.log('\nInbound files by connection:');
const inboundFiles = fs.readdirSync(captureDir)
    .filter(f => f.endsWith('.pak'))
    .sort();
const byConn = {};
for (const f of inboundFiles) {
    const c = f.match(/T(\d+)_/);
    const id = c ? c[1] : '??';
    if (!byConn[id]) byConn[id] = [];
    byConn[id].push(f);
}
for (const [id, files] of Object.entries(byConn).sort((a,b) => a[0].localeCompare(b[0]))) {
    console.log(`  T${id}: ${files.join(', ')}`);
}