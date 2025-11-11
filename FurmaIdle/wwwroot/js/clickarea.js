window.ClickArea = {
    getPos: function (el, cx, cy) {
        if (typeof cx !== "number" || typeof cy !== "number") {
            const ev = cx || window.event;
            cx = (ev && (ev.clientX ?? ev.pageX)) || 0;
            cy = (ev && (ev.clientY ?? ev.pageY)) || 0;
        }

        const r = el.getBoundingClientRect();

        let x = cx - r.left;
        let y = cy - r.top;

        const sx = r.width / (el.offsetWidth || r.width || 1);
        const sy = r.height / (el.offsetHeight || r.height || 1);
        if (sx && sx !== 1) x /= sx;
        if (sy && sy !== 1) y /= sy;

        return { x, y };
    }
};