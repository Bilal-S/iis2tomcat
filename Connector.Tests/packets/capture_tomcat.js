/**
 * Tomcat-Only Packet Capture Script
 * 
 * Hits ALL JSP test pages through http://localhost/ (IIS → BonCode → Tomcat AJP)
 * to generate raw .pak files via BonCode's log level 5 packet capture.
 * 
 * This captures PURE Tomcat 11 AJP packets — no Lucee engine involved.
 * 
 * Usage: node capture_tomcat.js
 * 
 * Prerequisites:
 *   - BonCode LogLevel set to 5 in app.config/web.config
 *   - IIS running with BonCode connector
 *   - Tomcat 11 running with JSP pages deployed to webapps/ROOT/
 */

const http = require('http');
const fs = require('fs');
const path = require('path');
const querystring = require('querystring');

const BASE_URL = 'http://localhost';
const DELAY_MS = 500;
const capturedDir = path.join(__dirname, 'captured', 'TomcatCapture');

if (!fs.existsSync(capturedDir)) {
    fs.mkdirSync(capturedDir, { recursive: true });
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function makeRequest(urlPath, method, body) {
    return new Promise((resolve, reject) => {
        const url = new URL(urlPath, BASE_URL);
        const opts = {
            hostname: url.hostname,
            port: url.port || 80,
            path: url.pathname + url.search,
            method: method || 'GET'
        };
        if (body) {
            opts.headers = { 'Content-Type': 'application/x-www-form-urlencoded', 'Content-Length': Buffer.byteLength(body) };
        }
        const req = http.request(opts, (res) => {
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
        if (body) req.write(body);
        req.end();
    });
}

function saveResponse(name, body) {
    const filePath = path.join(capturedDir, name + '.response');
    fs.writeFileSync(filePath, body);
    return filePath;
}

function validateBytePattern(body, startByte, pattern) {
    // pattern: 'sequential' = (i % 254) + 1, '0x00_0xFF' = i & 0xFF
    let corruptionCount = 0;
    for (let i = 0; i < body.length; i++) {
        let expected;
        if (pattern === 'sequential') {
            expected = (i % 254) + 1;
        } else if (pattern === '0x00_0xFF') {
            expected = i & 0xFF;
        } else {
            return { corruptionCount: 0, message: 'Unknown pattern' };
        }
        if (body[i] !== expected) {
            corruptionCount++;
            if (corruptionCount <= 5) {
                console.log(`  ✗ Byte ${i}: expected 0x${expected.toString(16).padStart(2,'0')}, got 0x${body[i].toString(16).padStart(2,'0')}`);
            }
        }
    }
    return { corruptionCount, message: corruptionCount === 0 ? 'All bytes match' : `${corruptionCount} corrupted bytes` };
}

function checkPass(result) {
    return result.corruptionCount === 0;
}

async function main() {
    console.log('===========================================');
    console.log(' Tomcat 11 AJP Packet Capture — Full Suite');
    console.log(' Target: ' + BASE_URL);
    console.log(' Output: ' + capturedDir);
    console.log('===========================================');
    console.log('\nMake sure LogLevel is set to 5 in BonCode config!');
    console.log('JSP must be deployed to Tomcat webapps/ROOT/\n');

    let passCount = 0;
    let failCount = 0;
    let skipCount = 0;

    try {
        // =====================================================
        // GET Tests
        // =====================================================

        // Test 1: Simple text (12 bytes, padding=0)
        console.log('--- Test 1: GET simple text (12B, padding=0) ---');
        let r = await makeRequest('/test_get_simple.jsp');
        let fp = saveResponse('test_get_simple', r.body);
        let expected = Buffer.from('Hello World!', 'utf-8');
        let ok = r.body.length === 12 && r.body.equals(expected);
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 12`);
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : `Body: "${r.body.toString('utf-8')}"`}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 2: Binary PNG (67 bytes, padding=1)
        console.log('\n--- Test 2: GET binary PNG (67B, padding=1) ---');
        r = await makeRequest('/test_get_binary.jsp');
        fp = saveResponse('test_get_binary', r.body);
        let pngExpected = Buffer.from('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==', 'base64');
        ok = r.body.length === 67 && r.body.equals(pngExpected);
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 67`);
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : (r.body.length === 66 ? '1 byte short — GetUserDataBytes truncation!' : `Got ${r.body.length} bytes`)}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 3: Binary odd (3 bytes, padding=1)
        console.log('\n--- Test 3: GET binary odd (3B, padding=1) ---');
        r = await makeRequest('/test_get_binary_odd.jsp');
        fp = saveResponse('test_get_binary_odd', r.body);
        ok = r.body.length === 3 && r.body[0] === 0xFF && r.body[1] === 0xFE && r.body[2] === 0xFD;
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 3`);
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : `Bytes: ${[...r.body].map(b => '0x'+b.toString(16).padStart(2,'0')).join(' ')}`}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 4: Binary single chunk (5000 bytes, padding=0)
        console.log('\n--- Test 4: GET binary single chunk (5000B, padding=0) ---');
        r = await makeRequest('/test_get_binary_singlechunk.jsp');
        fp = saveResponse('test_get_binary_singlechunk', r.body);
        let v = validateBytePattern(r.body, 0, 'sequential');
        ok = r.body.length === 5000 && checkPass(v);
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 5000`);
        if (r.body.length !== 5000) {
            console.log(`  ⚠ Size mismatch: ${r.body.length} vs 5000 (${5000 - r.body.length} bytes ${r.body.length < 5000 ? 'short' : 'extra'})`);
        }
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${checkPass(v) ? '' : v.message}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 5: Binary 9000 bytes (multi-chunk, padding varies)
        console.log('\n--- Test 5: GET binary 9000B (multi-chunk) ---');
        r = await makeRequest('/test_get_binary_9000.jsp');
        fp = saveResponse('test_get_binary_9000', r.body);
        v = validateBytePattern(r.body, 0, 'sequential');
        ok = r.body.length === 9000 && checkPass(v);
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 9000`);
        if (r.body.length !== 9000) {
            console.log(`  ⚠ Size mismatch: ${r.body.length} vs 9000 (${r.body.length - 9000} bytes ${r.body.length > 9000 ? 'extra (padding leaked!)' : 'short'})`);
        }
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${checkPass(v) ? '' : v.message}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 6: Binary large PNG (20,510 bytes, 3 chunks)
        console.log('\n--- Test 6: GET binary large PNG (20,510B, 3 chunks) ---');
        r = await makeRequest('/test_get_binary_large.jsp');
        fp = saveResponse('test_get_binary_large', r.body);
        // Compare against original file
        let originalPngPath = path.join(__dirname, 'jsp', 'playstore-icon.png');
        let originalPng = fs.existsSync(originalPngPath) ? fs.readFileSync(originalPngPath) : null;
        if (originalPng) {
            ok = r.body.length === originalPng.length && r.body.equals(originalPng);
            console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: ${originalPng.length}`);
            console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : (r.body.length !== originalPng.length ? `Size diff: ${r.body.length - originalPng.length} bytes` : 'Content mismatch (corrupted PNG)')}`);
        } else {
            console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length} (no original to compare)`);
            ok = r.body.length === 20510;
            console.log(`  ${ok ? '✓ PASS (size only)' : '✗ FAIL'} Expected 20510 bytes`);
        }
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 7: Text odd (5 bytes, padding=3)
        console.log('\n--- Test 7: GET text odd (5B "ABCDE", padding=3) ---');
        r = await makeRequest('/test_get_text_odd.jsp');
        fp = saveResponse('test_get_text_odd', r.body);
        ok = r.body.length === 5 && r.body.toString('utf-8') === 'ABCDE';
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 5`);
        if (r.body.length > 5) {
            console.log(`  ⚠ ${r.body.length - 5} extra bytes (padding leaked into output!)`);
            console.log(`  Extra bytes: ${[...r.body.slice(5)].map(b => '0x'+b.toString(16).padStart(2,'0')).join(' ')}`);
        }
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : `"${r.body.toString('utf-8')}"`}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 8: Text size 4 (4 bytes, padding=0)
        console.log('\n--- Test 8: GET text size4 (4B "ABCD", padding=0) ---');
        r = await makeRequest('/test_get_text_size4.jsp');
        fp = saveResponse('test_get_text_size4', r.body);
        ok = r.body.length === 4 && r.body.toString('utf-8') === 'ABCD';
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 4`);
        if (r.body.length === 3) {
            console.log(`  ⚠ 1 byte short — GetUserDataBytes dropped last byte!`);
        }
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : `"${r.body.toString('utf-8')}" (${r.body.length} bytes)`}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 9: Large response (16,384 bytes "X", 2 chunks)
        console.log('\n--- Test 9: GET large response (16384B "X", 2 chunks) ---');
        r = await makeRequest('/test_get_large_response.jsp');
        fp = saveResponse('test_get_large_response', r.body);
        ok = r.body.length === 16384 && r.body.every(b => b === 0x58);
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 16384`);
        if (r.body.length !== 16384) {
            console.log(`  ⚠ Size mismatch: ${r.body.length} vs 16384`);
        }
        let nonX = r.body.filter(b => b !== 0x58).length;
        if (nonX > 0) {
            console.log(`  ⚠ ${nonX} non-X bytes found (padding bytes leaked!)`);
        }
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 10: Headers multi
        console.log('\n--- Test 10: GET headers multi ---');
        r = await makeRequest('/test_headers_multi.jsp');
        fp = saveResponse('test_headers_multi', r.body);
        let setCookies = (r.headers['set-cookie'] || []);
        if (typeof setCookies === 'string') setCookies = [setCookies];
        let hasCustom = r.headers['x-custom-header'] === 'test-value-12345';
        let hasRequestId = !!r.headers['x-request-id'];
        let bodyOk = r.body.toString('utf-8') === 'HEADERS_OK';
        ok = setCookies.length >= 2 && hasCustom && hasRequestId && bodyOk;
        console.log(`  Status: ${r.statusCode}, Body: "${r.body.toString('utf-8')}"`);
        console.log(`  Set-Cookie count: ${setCookies.length} (expected >= 2)`);
        console.log(`  X-Custom-Header: ${r.headers['x-custom-header'] || 'MISSING'}`);
        console.log(`  X-Request-Id: ${r.headers['x-request-id'] || 'MISSING'}`);
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 11: Status codes
        console.log('\n--- Test 11: Status codes ---');
        for (let code of [200, 301, 404, 500]) {
            r = await makeRequest(`/test_status_codes.jsp?code=${code}`);
            let expectedBody = `STATUS_${code}`;
            // 301 may redirect, body might not arrive
            let bodyMatch = r.statusCode === 301 || r.body.toString('utf-8').trim() === expectedBody;
            ok = r.statusCode === code && bodyMatch;
            console.log(`  code=${code}: status=${r.statusCode}, body="${r.body.toString('utf-8').trim()}" ${ok ? '✓' : '✗'}`);
            ok ? passCount++ : failCount++;
            await sleep(DELAY_MS);
        }

        // Test 16: Sized packets 1-15 (padding boundary sweep)
        console.log('\n--- Test 16: Sized packets 1-15 (padding boundary sweep) ---');
        // Bug behavior prediction for each size:
        //   size % 4 == 0: padding=0 → Length-4 drops 1 byte (size-1 returned)
        //   size % 4 == 1: padding=3 → Length-4 gains 2 garbage bytes (size+2 returned)
        //   size % 4 == 2: padding=2 → Length-4 gains 1 garbage byte (size+1 returned)
        //   size % 4 == 3: padding=1 → Length-4 correct by accident (size returned)
        let sizedPass = 0, sizedFail = 0;
        for (let size = 1; size <= 15; size++) {
            r = await makeRequest(`/test_get_sized.jsp?size=${size}`);
            fp = saveResponse(`test_get_sized_${size}`, r.body);
            let padding = (4 - (size % 4)) % 4;
            let buggySize = (size + 3) - 4; // = size + padding - 4
            let bugType = '';
            if (padding === 0) bugType = 'TRUNCATE';
            else if (padding === 1) bugType = 'OK(accident)';
            else if (padding === 2) bugType = '+1GARBAGE';
            else if (padding === 3) bugType = '+2GARBAGE';

            ok = r.body.length === size;
            // Also validate byte pattern for first `size` bytes
            if (ok) {
                for (let i = 0; i < size; i++) {
                    if (r.body[i] !== ((i % 254) + 1)) { ok = false; break; }
                }
            }
            let icon = ok ? '✓' : '✗';
            let detail = ok ? 'PASS' : `${r.body.length}B (pad=${padding}, ${bugType})`;
            console.log(`  size=${String(size).padStart(2)}: ${r.body.length}B expected=${String(size).padStart(2)} pad=${padding} ${bugType.padEnd(12)} ${icon} ${detail}`);
            ok ? sizedPass++ : sizedFail++;
            await sleep(DELAY_MS);
        }
        console.log(`  Sized sweep: ${sizedPass} PASS, ${sizedFail} FAIL out of 15`);
        passCount += sizedPass;
        failCount += sizedFail;

        // =====================================================
        // POST Tests
        // =====================================================

        // Test 12: POST small
        console.log('\n--- Test 12: POST small form data ---');
        let postBody = querystring.stringify({ field1: 'hello', field2: 'world' });
        r = await makeRequest('/test_post_small.jsp', 'POST', postBody);
        fp = saveResponse('test_post_small', r.body);
        ok = r.body.toString('utf-8') === 'field1=hello,field2=world';
        console.log(`  Status: ${r.statusCode}, Body: "${r.body.toString('utf-8')}"`);
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 13: POST chunked
        console.log('\n--- Test 13: POST chunked transfer ---');
        // Send 15000 bytes chunked
        let chunkData = 'X'.repeat(15000);
        r = await makeRequest('/test_post_chunked.jsp', 'POST', chunkData);
        fp = saveResponse('test_post_chunked', r.body);
        let chunkResult = r.body.toString('utf-8');
        ok = chunkResult === 'CONTENT_LENGTH=15000';
        console.log(`  Status: ${r.statusCode}, Body: "${chunkResult}"`);
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : `Expected CONTENT_LENGTH=15000`}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 14: POST large (~20KB)
        console.log('\n--- Test 14: POST large form data (~20KB) ---');
        let largeData = querystring.stringify({ data: 'A'.repeat(20000) });
        r = await makeRequest('/test_post_large.jsp', 'POST', largeData);
        fp = saveResponse('test_post_large', r.body);
        let largeResult = r.body.toString('utf-8');
        ok = largeResult === 'RECEIVED_LENGTH=20000';
        console.log(`  Status: ${r.statusCode}, Body: "${largeResult}"`);
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : `Expected RECEIVED_LENGTH=20000`}`);
        ok ? passCount++ : failCount++;
        await sleep(DELAY_MS);

        // Test 15: POST binary (base64 PNG round-trip)
        console.log('\n--- Test 15: POST binary (base64 PNG round-trip) ---');
        let pngBase64 = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';
        let postBinaryBody = querystring.stringify({ imagedata: pngBase64 });
        r = await makeRequest('/test_post_binary.jsp', 'POST', postBinaryBody);
        fp = saveResponse('test_post_binary', r.body);
        ok = r.body.length === 67 && r.body.equals(pngExpected);
        console.log(`  Status: ${r.statusCode}, Bytes: ${r.body.length}, Expected: 67`);
        console.log(`  ${ok ? '✓ PASS' : '✗ FAIL'} ${ok ? '' : (r.body.length === 66 ? '1 byte short on binary response!' : `Got ${r.body.length} bytes`)}`);
        ok ? passCount++ : failCount++;

        // =====================================================
        // Summary
        // =====================================================
        console.log('\n===========================================');
        console.log(` Results: ${passCount} PASS, ${failCount} FAIL, ${skipCount} SKIP`);
        console.log('===========================================');
        console.log('\nCheck C:\\Sites\\AJP13\\BIN\\ for .pak files.');
        console.log('Validate: node validate_pak.js C:\\Sites\\AJP13\\BIN\\');
        console.log('===========================================\n');

        process.exit(failCount > 0 ? 1 : 0);

    } catch (err) {
        console.error('\nFATAL ERROR:', err.message);
        if (err.code === 'ECONNREFUSED') {
            console.error('Cannot connect to localhost. Is IIS running?');
        }
        process.exit(1);
    }
}

main();