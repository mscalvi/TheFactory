// service-worker.js (dev)
// No desenvolvimento, o SW não interfere no tráfego. Isso evita cache chato.
self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', () => self.clients.claim());
self.addEventListener('fetch', () => { /* no-op */ });
