const API_BASE_URL = 'http://localhost:5176/api'; 

// 1. Initialize everything when the DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    initLanguage();
    languageToggle();
});

function languageToggle() {
    const langToggle = document.getElementById('langToggle');
    if (!langToggle) 
        return;
    
    // Set initial button text based on current language
    const initialLang = sessionStorage.getItem('preferredLang') || 'en';
    langToggle.textContent = initialLang === 'en' ? 'FA' : 'EN';
    
    langToggle.addEventListener('click', () => {
        const currentLang = sessionStorage.getItem('preferredLang') || 'en';
        const newLang = currentLang === 'en' ? 'fa' : 'en';

        updateLanguage(newLang);

        // Update button text to show the opposite language
        langToggle.textContent = newLang === 'en' ? 'FA' : 'EN';
        console.log("Quack! Language toggled.");
    });
}

// 2. Handle the Login logic
document.getElementById('loginBtn').addEventListener('click', async () => {
    const name = document.getElementById('name').value;
    const phone = document.getElementById('phone').value;

    if (!name || !phone) {
        showMessage(t('err_empty_fields'), 'error');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, phone })
        });

        const result = await response.json();

        if (response.ok) {
            // Store the PostmanId for future requests
            sessionStorage.setItem('postmanId', result.postmanId);
            showMessage(t('login_success'), 'success');

            setTimeout(() => {
                window.location.href = 'dashboard.html';
            }, 1000);
        } else {
            // Use the error message from our ServiceResult/ErrorResponse
            const errorMsg = result.errorMessage || t('err_invalid_credentials');
            showMessage(errorMsg, 'error');
        }
    } catch (error) {
        showMessage(t('err_server_connection'), 'error');
    }
});

function showMessage(text, type) {
    const messageDiv = document.getElementById('message');
    messageDiv.textContent = text;
    messageDiv.className = type === 'error' ? 'error-text' : 'success-text';
}