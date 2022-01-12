// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

function SetDarkMode(darkMode) {
    if (typeof darkMode === 'undefined') {
        darkMode = window.localStorage.getItem('darkMode') === 'true'
        console.log('SetDarkMode was ', darkMode)
    }

    console.log('SetDarkMode', darkMode)
    if (darkMode) {
        document.body.classList.add('dark')
    } else {
        document.body.classList.remove('dark')
    }
    window.localStorage.setItem('darkMode', darkMode.toString())
}
