/**
 * Sanity check after npm install — catches incomplete node_modules on Windows
 * (slow/interrupted downloads often leave rxjs/@angular/common half-extracted).
 */
const fs = require('fs');
const path = require('path');

const root = path.join(__dirname, '..', 'node_modules');
const required = [
  '@angular/common/package.json',
  '@angular/common/index.d.ts',
  '@angular/common/fesm2020/common.mjs',
  '@angular/common/fesm2020/http.mjs',
  '@angular/core/fesm2020/core.mjs',
  'rxjs/package.json',
  'rxjs/dist/types/index.d.ts',
  'rxjs/dist/types/internal/Observable.d.ts',
  'rxjs/dist/types/internal/Subject.d.ts',
  'rxjs/dist/esm/index.js',
  'zone.js/package.json',
];

const missing = required.filter((rel) => !fs.existsSync(path.join(root, rel)));

if (missing.length) {
  console.error('\nERROR: node_modules is incomplete or corrupt. Missing:\n');
  missing.forEach((rel) => console.error('  -', rel));
  console.error('\nFix on Windows:');
  console.error('  repair-frontend-deps.bat');
  console.error('Or full reinstall:');
  console.error('  reinstall-frontend.bat\n');
  process.exit(1);
}

console.log('verify:deps OK — Angular, RxJS, and HTTP modules look complete.');
