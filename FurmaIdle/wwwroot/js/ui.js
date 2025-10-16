window.ui = window.ui || {};

window.ui.scrollToBottom = (el) => {
    if (!el) return;

    requestAnimationFrame(() => {
        el.scrollTop = el.scrollHeight;
    });
};

window.ui.scrollToBottomIfNearEnd = (el, threshold = 40) => {
    if (!el) return;
    const distance = el.scrollHeight - el.scrollTop - el.clientHeight;
    if (distance <= threshold) {
        requestAnimationFrame(() => { el.scrollTop = el.scrollHeight; });
    }
};
