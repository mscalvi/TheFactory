window.ClickArea = {
    getPos: function (el, ev) {
        // el: HTMLElement (root); ev: PointerEvent/MouseEvent
        const rect = el.getBoundingClientRect();
        return {
            x: ev.clientX - rect.left,
            y: ev.clientY - rect.top
        };
    }
};
