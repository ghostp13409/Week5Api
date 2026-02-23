// Node.js console client for Week5Api
// Requires Node 18+ for fetch support

const BASE = "http://localhost:5235"; // adjust port if necessary
const API_KEY = "MY_SECRET_KEY_123";

async function call(path, options = {}) {
  options.headers = options.headers || {};
  options.headers["X-Api-Key"] = API_KEY;
  options.headers["Content-Type"] = "application/json";
  const res = await fetch(BASE + path, options);
  const text = await res.text();
  let body;
  try {
    body = JSON.parse(text);
  } catch {
    body = text;
  }
  console.log(path, res.status, body);
  return { status: res.status, body };
}

async function testValidation() {
  console.log("--- validation tests ---");
  await call("/api/item", {
    method: "POST",
    body: JSON.stringify({ name: "", quantity: 1 }),
  });
  await call("/api/item", {
    method: "POST",
    body: JSON.stringify({ name: "Foo", quantity: -5 }),
  });
  await call("/api/item", {
    method: "POST",
    body: JSON.stringify({ name: "Bar", quantity: 10 }),
  });
}

async function testSecurity() {
  console.log("--- security tests ---");
  // without key
  const res1 = await fetch(BASE + "/usage");
  console.log("/usage without key", res1.status, await res1.text());
  // wrong key
  const res2 = await fetch(BASE + "/usage", {
    headers: { "X-Api-Key": "WRONG" },
  });
  console.log("/usage wrong key", res2.status, await res2.text());
  // correct key
  await call("/usage");
}

async function testState() {
  console.log("--- state tests ---");
  for (let i = 0; i < 3; i++) {
    await call("/usage");
  }
  // another key
  const otherKey = "OTHER";
  for (let i = 0; i < 2; i++) {
    const res = await fetch(BASE + "/usage", {
      headers: { "X-Api-Key": otherKey },
    });
    console.log("/usage other", res.status, await res.text());
  }
}

async function testTransfer() {
  console.log("--- transfer tests ---");
  await call("/transfer", {
    method: "POST",
    body: JSON.stringify({ fromId: 1, toId: 2, amount: 30 }),
  });
  await call("/transfer", {
    method: "POST",
    body: JSON.stringify({ fromId: 1, toId: 2, amount: 1000 }),
  });
}

async function testError() {
  console.log("--- error test ---");
  await call("/error");
}

async function main() {
  await testValidation();
  await testSecurity();
  await testState();
  await testTransfer();
  await testError();
}

main().catch(console.error);
