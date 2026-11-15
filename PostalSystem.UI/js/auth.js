const API_BASE_URL = 'http://localhost:5176/api'; 

document.getElementById('loginBtn').addEventListener('click', async () => {
    const name = document.getElementById('name').value;
    const phone = document.getElementById('phone').value;
    const messageDiv = document.getElementById('message');

    if (!name || !phone) {
        showMessage('Please fill in all fields!', 'error');
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
            showMessage('Login successful! Redirecting...', 'success');

            setTimeout(() => {
                window.location.href = 'dashboard.html';
            }, 1000);
        } else {
            // Use the error message from our ServiceResult/ErrorResponse
            const errorMsg = result.errorMessage || 'Invalid login credentials.';
            showMessage(errorMsg, 'error');
        }
    } catch (error) {
        showMessage('Could not connect to the server. Quack!', 'error');
    }
});

function showMessage(text, type) {
    const messageDiv = document.getElementById('message');
    messageDiv.textContent = text;
    messageDiv.className = type === 'error' ? 'error-text' : 'success-text';
}