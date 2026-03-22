/**
 * Packet Capture Script
 * 
 * Hits all CFM test pages through http://localhost/ to generate raw .pak files
 * via BonCode's log level 5 packet capture.
 * 
 * Usage: node capture.js
 * 
 * Prerequisites:
 *   - BonCode LogLevel set to 5 in app.config/web.config
 *   - IIS running with BonCode connector
 *   - Lucee/Tomcat running with CFM pages deployed
 */

const http = require('http');
const fs = require('fs');
const path = require('path');

const BASE_URL = 'http://localhost';
const DELAY_MS = 500;
const capturedDir = path.join(__dirname, 'captured');

// Ensure captured directory exists
if (!fs.existsSync(capturedDir)) {
    fs.mkdirSync(capturedDir, { recursive: true });
}

// --- Helpers ---

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function makeRequest(options, body) {
    return new Promise((resolve, reject) => {
        const req = http.request(options, (res) => {
            const chunks = [];
            res.on('data', chunk => chunks.push(chunk));
            res.on('end', () => {
                const buf = Buffer.concat(chunks);
                resolve({
                    statusCode: res.statusCode,
                    headers: res.headers,
                    body: buf
                });
            });
        });
        req.on('error', reject);
        if (body) {
            req.write(body);
        }
        req.end();
    });
}

function getRequest(urlPath) {
    const url = new URL(urlPath, BASE_URL);
    return makeRequest({
        hostname: url.hostname,
        port: url.port || 80,
        path: url.pathname + url.search,
        method: 'GET'
    });
}

function postRequest(urlPath, contentType, body) {
    const url = new URL(urlPath, BASE_URL);
    return makeRequest({
        hostname: url.hostname,
        port: url.port || 80,
        path: url.pathname + url.search,
        method: 'POST',
        headers: {
            'Content-Type': contentType,
            'Content-Length': Buffer.byteLength(body)
        }
    }, body);
}

function postChunkedRequest(urlPath, contentType, bodyChunks) {
    return new Promise((resolve, reject) => {
        const url = new URL(urlPath, BASE_URL);
        const req = http.request({
            hostname: url.hostname,
            port: url.port || 80,
            path: url.pathname + url.search,
            method: 'POST',
            headers: {
                'Content-Type': contentType,
                'Transfer-Encoding': 'chunked'
            }
        }, (res) => {
            const chunks = [];
            res.on('data', chunk => chunks.push(chunk));
            res.on('end', () => {
                resolve({
                    statusCode: res.statusCode,
                    headers: res.headers,
                    body: Buffer.concat(chunks)
                });
            });
        });
        req.on('error', reject);

        // Write each chunk with chunked encoding format
        for (const chunk of bodyChunks) {
            const chunkData = Buffer.isBuffer(chunk) ? chunk : Buffer.from(chunk);
            req.write(chunkData.toString('hex').length / 2 === chunkData.length
                ? chunkData
                : chunkData);
        }
        req.end();
    });
}

function saveResponseFile(name, body) {
    const filePath = path.join(capturedDir, name + '.response');
    fs.writeFileSync(filePath, body);
    return filePath;
}

function logResult(label, result, saveAs) {
    const size = result.body ? result.body.length : 0;
    let msg = `  ${label}: status=${result.statusCode}, bytes=${size}`;
    if (saveAs) {
        const fp = saveResponseFile(saveAs, result.body);
        msg += ` -> saved ${fp}`;
    }
    console.log(msg);
}

// --- Test Scenarios ---

async function runGetTests() {
    console.log('\n=== GET Tests ===\n');

    // 1. Simple GET - 12 bytes HTML (padding=0)
    let r = await getRequest('/test_get_simple.cfm');
    logResult('GET  simple (12B, pad=0)', r);

    await sleep(DELAY_MS);

    // 2. Text size 4 - exactly 4 bytes (padding=0) - worst case for Bug 1
    r = await getRequest('/test_get_text_size4.cfm');
    logResult('GET  text_size4 (4B, pad=0)', r);

    await sleep(DELAY_MS);

    // 3. Text odd - 5 bytes (padding=3) - includes padding bytes
    r = await getRequest('/test_get_text_odd.cfm');
    logResult('GET  text_odd (5B, pad=3)', r);

    await sleep(DELAY_MS);

    // 4. Binary PNG - 67 bytes (padding=1) - drops last byte
    r = await getRequest('/test_get_binary.cfm');
    logResult('GET  binary_png (67B, pad=1)', r, 'test_get_binary_expected.png');

    await sleep(DELAY_MS);

    // 5. Binary odd - 3 bytes (padding=1) - works by accident
    r = await getRequest('/test_get_binary_odd.cfm');
    logResult('GET  binary_odd (3B, pad=1)', r);

    await sleep(DELAY_MS);

    // 5b. Binary single chunk - 5000 bytes (fits in one SendBodyChunk)
    r = await getRequest('/test_get_binary_singlechunk.cfm');
    logResult('GET  binary_singlechunk (5000B, single chunk)', r, 'test_get_binary_singlechunk.response');

    await sleep(DELAY_MS);

    // 5c. Binary 9000 bytes - multi-chunk, loop-generated sequential pattern
    r = await getRequest('/test_get_binary_9000.cfm');
    logResult('GET  binary_9000 (9000B, multi-chunk)', r, 'test_get_binary_9000.response');

    await sleep(DELAY_MS);

    // 5d. Binary large - 20,510-byte real PNG (3 chunks)
    r = await getRequest('/test_get_binary_large.cfm');
    logResult('GET  binary_large (20510B PNG, 3 chunks)', r, 'test_get_binary_large_playstore.response.png');

    await sleep(DELAY_MS);

    // 6. Large response - 16KB multi-chunk
    r = await getRequest('/test_get_large_response.cfm');
    logResult('GET  large_response (16KB)', r);

    await sleep(DELAY_MS);

    // 7. Multi headers
    r = await getRequest('/test_headers_multi.cfm');
    logResult('GET  headers_multi', r);

    await sleep(DELAY_MS);
}

async function runStatusTests() {
    console.log('\n=== Status Code Tests ===\n');

    const codes = [200, 301, 404, 500];
    for (const code of codes) {
        const r = await getRequest(`/test_status_codes.cfm?code=${code}`);
        logResult(`GET  status_${code}`, r);
        await sleep(DELAY_MS);
    }
}

async function runPostTests() {
    console.log('\n=== POST Tests ===\n');

    // 1. Small POST
    const smallBody = 'field1=hello&field2=world';
    let r = await postRequest('/test_post_small.cfm', 'application/x-www-form-urlencoded', smallBody);
    logResult('POST small', r);
    console.log(`    Response: ${r.body.toString().trim()}`);

    await sleep(DELAY_MS);

    // 2. Large POST - ~20KB to trigger multi-packet and expose Bug 2
    // Use 8187 bytes of 'A' to hit the exact boundary where integer division fails
    // (8187 / 8186 = 1 via int division, but should be 2 packets)
    const largeData = 'data=' + 'A'.repeat(8187);
    r = await postRequest('/test_post_large.cfm', 'application/x-www-form-urlencoded', largeData);
    logResult('POST large (8187B body, exposes Bug 2)', r);
    console.log(`    Response: ${r.body.toString().trim()}`);

    await sleep(DELAY_MS);

    // 3. POST binary - send base64 PNG, get PNG back
    const pngBase64 = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';
    const binaryBody = 'imagedata=' + encodeURIComponent(pngBase64);
    r = await postRequest('/test_post_binary.cfm', 'application/x-www-form-urlencoded', binaryBody);
    logResult('POST binary (PNG round-trip)', r, 'test_post_binary_response.png');

    await sleep(DELAY_MS);

    // 4. Chunked POST - send data in chunks
    const chunk1 = 'field1=chunked_test&data=';
    const chunk2 = 'x'.repeat(5000);
    r = await postChunkedRequest('/test_post_chunked.cfm', 'application/x-www-form-urlencoded', [chunk1, chunk2]);
    logResult('POST chunked', r);
    console.log(`    Response: ${r.body.toString().trim()}`);

    await sleep(DELAY_MS);

    // 5. Another large POST with different size to test boundary (8193 bytes)
    const largeData2 = 'data=' + 'B'.repeat(8193);
    r = await postRequest('/test_post_large.cfm', 'application/x-www-form-urlencoded', largeData2);
    logResult('POST large (8193B body, boundary test)', r);
    console.log(`    Response: ${r.body.toString().trim()}`);

    await sleep(DELAY_MS);
}

// --- Main ---

async function main() {
    console.log('===========================================');
    console.log(' BonCode AJP13 Packet Capture Script');
    console.log(' Target: ' + BASE_URL);
    console.log(' Output: ' + capturedDir);
    console.log('===========================================');
    console.log('\nMake sure LogLevel is set to 5 in BonCode config!');
    console.log('Response files saved to captured/ for comparison.\n');

    try {
        await runGetTests();
        await runStatusTests();
        await runPostTests();

        console.log('\n===========================================');
        console.log(' Done! Check your BonCode log directory for');
        console.log(' .pak and .pak.meta files.');
        console.log(' Move selected files to captured/ for tests.');
        console.log('===========================================\n');
    } catch (err) {
        console.error('\nFATAL ERROR:', err.message);
        if (err.code === 'ECONNREFUSED') {
            console.error('Cannot connect to localhost. Is IIS running?');
        }
        process.exit(1);
    }
}

main();