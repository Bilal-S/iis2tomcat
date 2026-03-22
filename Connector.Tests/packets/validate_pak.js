/**
 * Validate captured .pak files against AJPv13 wire format.
 * 
 * Wire format: [AB][payload_len:2B][type:1B][...data...]
 * For SendBodyChunk (type 0x03): [AB][payload_len][0x03][chunk_len:2B][body...][0x00]
 * 
 * Tomcat 11 behavior: Always appends exactly 1 byte 0x00 termination after body,
 * regardless of chunk_len. This differs from the AJP spec variable padding formula
 * (4 - chunkLen%4) % 4, but is what Tomcat 11 actually sends.
 * 
 * BonCode GetUserDataBytes() bug: Array.Copy(store, 3, out, 0, store.Length - 4)
 *   - Returns (payload_len - 4) bytes instead of chunk_len bytes
 *   - payload_len = chunk_len + 3 (type+chunklen) + 1 (0x00 termination) = chunk_len + 4
 *   - So bug output = chunk_len + 4 - 4 = chunk_len — CORRECT by coincidence!
 *   - The 0x00 termination byte makes the bug invisible for Tomcat 11.
 */

const fs = require('fs');
const path = require('path');

const capturedDir = process.argv[2]
    ? path.resolve(process.argv[2])
    : path.join(__dirname, 'captured');

const TYPE_NAMES = {
    0x02: 'ForwardRequest',
    0x03: 'SendBodyChunk',
    0x04: 'SendHeaders',
    0x05: 'EndResponse',
    0x06: 'GetBodyChunk',
    0x09: 'CPongReply'
};

function parsePak(filePath) {
    const buf = fs.readFileSync(filePath);
    const metaPath = filePath + '.meta';
    const meta = fs.existsSync(metaPath) ? fs.readFileSync(metaPath, 'utf8').trim() : '';

    if (buf.length < 4) return { error: 'Too short', file: path.basename(filePath) };

    const magic0 = buf[0];
    const magic1 = buf[1];
    const payloadLen = (buf[2] << 8) | buf[3];
    const type = buf.length > 4 ? buf[4] : null;

    const result = {
        file: path.basename(filePath),
        totalBytes: buf.length,
        magic: `${magic0.toString(16).toUpperCase()} ${magic1.toString(16).toUpperCase()}`,
        magicValid: (magic0 === 0x41 && magic1 === 0x42),
        payloadLen,
        payloadActual: buf.length - 4,
        payloadLenMatch: payloadLen === (buf.length - 4),
        type,
        typeName: TYPE_NAMES[type] || `Unknown(0x${(type||0).toString(16)})`,
        meta
    };

    if (result.magicValid && type === 0x03 && buf.length > 7) {
        const chunkLen = (buf[5] << 8) | buf[6];
        const termByte = buf[7 + chunkLen]; // byte after body
        const expectedPayloadLen = 3 + chunkLen + 1; // type(1) + chunklen(2) + body + 0x00
        const trailing = payloadLen - 3 - chunkLen;

        result.chunkLen = chunkLen;
        result.termByte = termByte;
        result.trailing = trailing;
        result.expectedPayloadLen = expectedPayloadLen;

        // Validate structure
        result.terminationValid = (termByte === 0x00 && trailing === 1);
        result.payloadLenCorrect = (payloadLen === expectedPayloadLen);

        // Bug simulation
        const bugOutputLen = payloadLen - 4;
        result.bugOutputLen = bugOutputLen;
        result.bugDelta = bugOutputLen - chunkLen;

        // With Tomcat 11's 0x00 termination, bug is always masked
        if (result.bugDelta === 0) {
            result.bugStatus = 'OK';
            result.bugDescription = `Output length correct (${bugOutputLen}B) — 0x00 termination masks the bug`;
        } else {
            result.bugStatus = 'BUG';
            result.bugDescription = `Output ${bugOutputLen}B vs expected ${chunkLen}B (delta=${result.bugDelta})`;
        }

        // Body data
        result.bodyData = buf.slice(7, 7 + chunkLen);
    }

    return result;
}

// --- Main ---

const pakFiles = fs.readdirSync(capturedDir)
    .filter(f => f.endsWith('.pak'))
    .sort()
    .map(f => path.join(capturedDir, f));

console.log('='.repeat(100));
console.log(' AJP13 .pak File Validation Report (Tomcat 11)');
console.log('='.repeat(100));
console.log(` Found ${pakFiles.length} .pak files\n`);

let issues = 0;
let bodyChunks = 0;
let chunksByConnection = {};

for (const f of pakFiles) {
    const p = parsePak(f);

    const connMatch = p.file.match(/T(\d+)_/);
    const connId = connMatch ? connMatch[1] : '??';
    if (!chunksByConnection[connId]) chunksByConnection[connId] = [];
    chunksByConnection[connId].push(p);

    if (!p.magicValid) {
        console.log(`  ${p.file}: [${p.magic}] Outbound packet — skip`);
        continue;
    }
    if (!p.payloadLenMatch) {
        console.log(`❌ ${p.file}: LENGTH MISMATCH header=${p.payloadLen} actual=${p.payloadActual}`);
        issues++;
        continue;
    }

    if (p.type === 0x03) {
        bodyChunks++;
        const termIcon = p.terminationValid ? '✅' : '❌';
        const bugIcon = p.bugStatus === 'OK' ? '✅' : '❌';
        console.log(`${termIcon} ${p.file}: chunk_len=${p.chunkLen} trail=${p.trailing} term=0x${(p.termByte||0).toString(16).padStart(2,'0')}`);
        if (!p.terminationValid) {
            console.log(`   ❌ Expected trail=1, term=0x00, got trail=${p.trailing}, term=0x${(p.termByte||0).toString(16).padStart(2,'0')}`);
            issues++;
        }
        if (p.bugStatus !== 'OK') {
            console.log(`   ${bugIcon} Bug: ${p.bugDescription}`);
            issues++;
        }
    } else {
        console.log(`  ${p.file}: ${p.typeName} (${p.totalBytes}B)`);
    }
}

// Per-connection summary
console.log('\n' + '='.repeat(100));
console.log(' Per-Connection Summary (SendBodyChunk sequences)');
console.log('='.repeat(100));

for (const [connId, packets] of Object.entries(chunksByConnection).sort()) {
    const chunks = packets.filter(p => p.type === 0x03);
    if (chunks.length === 0) continue;

    const totalBody = chunks.reduce((s, c) => s + c.chunkLen, 0);
    const allValid = chunks.every(c => c.terminationValid && c.bugStatus === 'OK');

    console.log(`\nConnection T${connId}: ${chunks.length} chunk(s) | total_body=${totalBody}B`);
    for (const c of chunks) {
        const icon = (c.terminationValid && c.bugStatus === 'OK') ? '✅' : '❌';
        console.log(`  ${icon} cl=${c.chunkLen} trail=${c.trailing} term=0x${(c.termByte||0).toString(16).padStart(2,'0')} bug_out=${c.bugOutputLen} [${c.bugStatus}]`);
    }
    if (!allValid) issues++;
}

// Final verdict
console.log('\n' + '='.repeat(100));
console.log(` SendBodyChunks: ${bodyChunks} total`);
if (issues > 0) {
    console.log(` ❌ FOUND ${issues} ISSUE(S)`);
} else {
    console.log(' ✅ All files valid — Tomcat 11 0x00 termination makes GetUserDataBytes bug invisible');
}
console.log('='.repeat(100));