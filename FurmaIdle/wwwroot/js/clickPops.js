window.ClickPops = (function () {

    function spawn(rootEl, text, options) {
        // rootEl vem do ElementReference do Blazor
        const root = rootEl;
        if (!root) return;

        const span = document.createElement('span');
        span.className = 'click-pop';

        const inner = document.createElement('span');
        inner.className = 'click-pop-inner';
        inner.textContent = text;

        if (options && options.icon) {
            const img = document.createElement('img');
            img.src = options.icon;
            img.className = 'click-pop-icon';
            img.alt = '';
            inner.appendChild(img);
        }

        span.appendChild(inner);

        const dir = Math.random() < 0.5 ? -1 : 1;
        const dx = dir * (20 + Math.random() * 30);
        const dy = - (64 + Math.random() * 40);

        span.style.setProperty('--dx', dx + 'px');
        span.style.setProperty('--dy', dy + 'px');

        const durMs = options?.durationMs ?? 800;
        span.style.setProperty('--dur', durMs + 'ms');

        root.appendChild(span);

        span.addEventListener('animationend', () => {
            span.remove();
        });
    }

    return { spawn };
})();
