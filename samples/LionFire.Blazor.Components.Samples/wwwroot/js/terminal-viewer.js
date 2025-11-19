window.terminalViewer = {
    rightClickHandlers: new Map(),
    capturedSelections: new Map(),

    initialize: function(terminalContainer, dotNetRef, enableRightClickCopy) {
        // Initialize terminal viewer
        console.log('Terminal viewer initialized', { enableRightClickCopy });

        // Setup right-click to copy if enabled
        this.updateRightClickCopy(terminalContainer, dotNetRef, enableRightClickCopy);
    },

    updateRightClickCopy: function(terminalContainer, dotNetRef, enableRightClickCopy) {
        console.log('Updating right-click copy', { enableRightClickCopy });

        // Remove existing handlers if present
        const existingHandlers = this.rightClickHandlers.get(terminalContainer);
        if (existingHandlers) {
            terminalContainer.removeEventListener('mousedown', existingHandlers.mousedown);
            terminalContainer.removeEventListener('contextmenu', existingHandlers.contextmenu);
            this.rightClickHandlers.delete(terminalContainer);
            this.capturedSelections.delete(terminalContainer);
        }

        // Add new handlers if enabled
        if (enableRightClickCopy && terminalContainer && dotNetRef) {
            const self = this;

            // Capture selection on mousedown for right-clicks
            const mousedownHandler = function(e) {
                // Right mouse button is button 2
                if (e.button === 2) {
                    const selection = window.getSelection();
                    const selectedText = selection ? selection.toString() : '';
                    self.capturedSelections.set(terminalContainer, selectedText);
                    console.log('Captured selection on mousedown:', selectedText.length, 'chars');
                }
            };

            // Handle contextmenu event
            const contextmenuHandler = async function(e) {
                e.preventDefault(); // Always prevent context menu when feature is enabled

                const capturedText = self.capturedSelections.get(terminalContainer);
                console.log('Contextmenu - captured text:', capturedText ? capturedText.length : 0, 'chars');

                if (capturedText && capturedText.length > 0) {
                    try {
                        await dotNetRef.invokeMethodAsync('OnRightClickCopyWithText', capturedText);
                        // Clear selection after successful copy
                        const selection = window.getSelection();
                        if (selection) {
                            selection.removeAllRanges();
                        }
                    } catch (err) {
                        console.error('Right-click copy failed:', err);
                    }
                }

                // Clear captured selection
                self.capturedSelections.delete(terminalContainer);
            };

            terminalContainer.addEventListener('mousedown', mousedownHandler);
            terminalContainer.addEventListener('contextmenu', contextmenuHandler);

            this.rightClickHandlers.set(terminalContainer, {
                mousedown: mousedownHandler,
                contextmenu: contextmenuHandler
            });
        }
    },

    cleanup: function(terminalContainer) {
        const handlers = this.rightClickHandlers.get(terminalContainer);
        if (terminalContainer && handlers) {
            terminalContainer.removeEventListener('mousedown', handlers.mousedown);
            terminalContainer.removeEventListener('contextmenu', handlers.contextmenu);
            this.rightClickHandlers.delete(terminalContainer);
            this.capturedSelections.delete(terminalContainer);
        }
    },
    
    scrollToBottom: function(terminalContent) {
        if (terminalContent) {
            terminalContent.scrollTop = terminalContent.scrollHeight;
        }
    },
    
    getScrollInfo: function(terminalContent) {
        if (!terminalContent) {
            return { scrollTop: 0, scrollHeight: 0, clientHeight: 0 };
        }
        
        return {
            scrollTop: terminalContent.scrollTop,
            scrollHeight: terminalContent.scrollHeight,
            clientHeight: terminalContent.clientHeight
        };
    },
    
    getSelectedText: function() {
        const selection = window.getSelection();
        return selection.toString();
    },
    
    copyToClipboard: function(text) {
        if (navigator.clipboard && navigator.clipboard.writeText) {
            return navigator.clipboard.writeText(text);
        } else {
            // Fallback for older browsers
            const textArea = document.createElement('textarea');
            textArea.value = text;
            textArea.style.position = 'fixed';
            textArea.style.left = '-999999px';
            textArea.style.top = '-999999px';
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();
            
            try {
                document.execCommand('copy');
                document.body.removeChild(textArea);
                return Promise.resolve();
            } catch (err) {
                document.body.removeChild(textArea);
                return Promise.reject(err);
            }
        }
    },
    
    enableFullscreen: function(terminalContainer) {
        if (terminalContainer) {
            terminalContainer.classList.add('fullscreen');
        }
    },
    
    disableFullscreen: function(terminalContainer) {
        if (terminalContainer) {
            terminalContainer.classList.remove('fullscreen');
        }
    }
};