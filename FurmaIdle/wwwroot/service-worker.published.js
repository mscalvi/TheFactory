// service-worker.published.js (prod)
// Cache offline baseado no manifest gerado pelo Blazor: service-worker-assets.js
// O build gera esse arquivo automaticamente no publish.
self.importScripts('./service-worker-assets.js');

const cacheName = 'offline-cache-' + (self.assetsManifest?.version || 'v1');

// Filtra os assets (evita .br/.gz) e usa integridade do hash
const offlineAssets = (self.assetsManifest?.assets || [])
    .filter(a => !a.url.endsWith('.br') && !a.url.endsWith('.gz'))
    .map(a => new Request(a.url, { integrity: a.hash, cache: 'no-cache' }));

// Instalação: pré-cache de todos os assets
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(cacheName)
            .then(cache => cache.addAll(offlineAssets))
            .then(() => self.skipWaiting())
    );
});

// Ativação: limpa caches antigos
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys()
            .then(keys => Promise.all(
                keys.filter(k => k.startsWith('offline-cache-') && k !== cacheName)
                    .map(k => caches.delete(k))
            ))
            .then(() => self.clients.claim())
    );
});

// Estratégia de fetch:
// - Navegação (HTML): tenta rede; se cair, usa fallback do cache (SPA offline)
// - Demais requisições GET: cache-first, senão rede
self.addEventListener('fetch', event => {
    const req = event.request;
    if (req.method !== 'GET') return;

    const accept = req.headers.get('accept') || '';
    const isNavigation = accept.includes('text/html');

    if (isNavigation) {
        // Tenta rede; se offline, responde index.html do cache
        event.respondWith(
            fetch(req).catch(() => caches.match('index.html'))
        );
        return;
    }

    // Demais GETs: cache primeiro; se não houver, busca na rede
    event.respondWith(
        caches.match(req).then(resp => resp || fetch(req))
    );
});
