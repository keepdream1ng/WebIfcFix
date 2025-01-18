function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
function copyTextToClipboard(text) {
    navigator.clipboard.writeText(text).then(function() {
        console.log('Text copied to clipboard');
    }).catch(function(error) {
        console.error('Could not copy text: ', error);
    });
}

function prefersDarkMode() {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
}

function enableDarkMode() {
    document.documentElement.setAttribute('data-bs-theme', 'dark');
}
function disableDarkMode() {
    document.documentElement.removeAttribute('data-bs-theme');
}

if (prefersDarkMode) {
    enableDarkMode();
}
