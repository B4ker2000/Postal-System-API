const API_BASE_URL = 'http://localhost:5176/api';

async function apiRequest(endpoint, options = {}) {
    const postmanId = sessionStorage.getItem('postmanId');

    // If we need a postmanId but don't have one, send them back to login
    if (!postmanId && endpoint !== '/auth/login') {
        window.location.href = 'index.html';
        return;
    }

    const defaultHeaders = {
        'Content-Type': 'application/json',
    };

    if (postmanId) {
        defaultHeaders['X-Postman-Id'] = postmanId;
    }

    const config = {
        ...options,
        headers: {
            ...defaultHeaders,
            ...options.headers,
        },
    };

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
        const data = await response.json();

        if (!response.ok) {
            // Throw an error containing the backend's error message
            throw new Error(data.errorMessage || 'Something went wrong. Quack!');
        }

        return data;
    } catch (error) {
        console.error('API Error:', error.message);
        throw error;
    }
}

// Helper functions for common methods to make our code even cleaner
async function apiGet(endpoint) {
    return apiRequest(endpoint, { method: 'GET' });
}

async function apiPost(endpoint, body) {
    return apiRequest(endpoint, { 
        method: 'POST', 
        body: JSON.stringify(body),
    });
}