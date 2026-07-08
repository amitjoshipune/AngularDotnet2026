/**
 * Quick sanity check after npm install — catches incomplete node_modules
 * (e.g. missing @angular/common/http on Windows when install was interrupted).
 */
const fs = require('fs');
const path = require('path');

const root = path.join(__dirname, '..', 'node_modules');
const required = [
  '@angular/common/fesm2020/http.mjs',
  '@angular/common/package.json',
  'rxjs/package.json',
  'rxjs/dist/types/index.d.ts',
  'zone.js/package.json',
];

const missing = required.filter((rel) => !fs.existsSync(path.join(root, rel)));

if (missing.length) {
  console.error('\nERROR: node_modules looks incomplete. Missing:\n');
  missing.forEach((rel) => console.error('  -', rel));
  console.error('\nRun reinstall-frontend.bat (Windows) or:');
  console.error('  rm -rf node_modules .angular && npm ci\n');
  process.exit(1);
}

console.log('verify:deps OK — Angular HTTP and RxJS files present.');
