document.addEventListener('DOMContentLoaded', async () => {
    // Load the language and then the shared navigation bar. Quack!
    initLanguage();
    await loadNavbar();
});