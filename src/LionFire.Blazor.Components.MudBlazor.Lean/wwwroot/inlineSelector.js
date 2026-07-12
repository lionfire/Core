// Sliding selection-highlight for InlineSelector.
//
// The component renders its options as MudButtons in a flex row. To make the
// highlight animate its POSITION and SIZE between selections (rather than the
// fill teleporting from one button to the next), an absolutely-positioned
// indicator element is parked over the selected button. CSS transitions on
// left/top/width/height/opacity do the 0.2s slide; this module just measures the
// selected button and writes those four numbers onto the indicator.
//
// Measurements use offsetLeft/Top/Width/Height, which are relative to the
// nearest positioned ancestor — the items container is given position:relative,
// so the indicator (also a child of that container) shares the same coordinate
// space. A ResizeObserver re-places the indicator when the row reflows (window
// resize, late-loading items, font swaps, wrapping).

const observers = new WeakMap();

export function init(container, indicator) {
    if (!container || !indicator) return;
    place(container, indicator, false);
    if (typeof ResizeObserver !== 'undefined' && !observers.has(container)) {
        const ro = new ResizeObserver(() => place(container, indicator, true));
        ro.observe(container);
        observers.set(container, ro);
    }
}

export function update(container, indicator) {
    place(container, indicator, true);
}

function place(container, indicator, animate) {
    if (!container || !indicator) return;
    const sel = container.querySelector('.inline-selector-item-selected');
    if (!sel) { indicator.style.opacity = '0'; return; }

    const apply = () => {
        indicator.style.left = sel.offsetLeft + 'px';
        indicator.style.top = sel.offsetTop + 'px';
        indicator.style.width = sel.offsetWidth + 'px';
        indicator.style.height = sel.offsetHeight + 'px';
        indicator.style.opacity = '1';
    };

    if (animate) {
        apply();
    } else {
        // First placement (and post-reflow): snap into position without sliding
        // in from the top-left corner. Briefly suspend the transition, then flush.
        const prev = indicator.style.transition;
        indicator.style.transition = 'none';
        apply();
        void indicator.offsetWidth; // force reflow so the snap is committed
        indicator.style.transition = prev;
    }
}

export function dispose(container) {
    const ro = observers.get(container);
    if (ro) { ro.disconnect(); observers.delete(container); }
}
