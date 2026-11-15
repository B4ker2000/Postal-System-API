// Helper to show feedback messages
function notify(text, type = 'success') {
    // A styled alert to keep things simple
    const color = type === 'success' ? 'var(--success)' : 'var(--error)';
    alert(`${type === 'success' ? '✅' : '❌'} ${text}`);
}

// Helper to fill the cities dropdown!
async function loadCities() {
    try {
        const cities = await apiRequest('GET', '/api/cities');
        const select = document.getElementById('regDestCitySelect');

        cities.forEach(city => {
            const option = document.createElement('option');
            option.value = city.cityId;         // The ID goes in the value
            option.textContent = city.cityName; // The Name goes in the text
            select.appendChild(option);
        });
    } catch (error) {
        console.error("Quack! Could not load cities:", error);
    }
}

// --- Authentication ---
document.getElementById('logoutBtn').addEventListener('click', () => {
    sessionStorage.removeItem('postmanId');
    window.location.href = 'index.html';
});

// --- Register Package ---
document.getElementById('regBtn').addEventListener('click', async () => {
    const data = {
        finalDestinationCityId: parseInt(document.getElementById('regDestCitySelect').value),
        destinationAddress: document.getElementById('regAddress').value,
        isFlaggedAsFragile: document.getElementById('regFragile').checked
    };

    if (!data.finalDestinationCityId || !data.destinationAddress) {
        return notify('Please fill in all registration fields!', 'error');
    }

    try {
        const result = await apiPost('/packages/register', data);
        notify(`Package registered! Tracking GUID: ${result.trackingGuid}`);
        // Clear inputs
        document.getElementById('regDestCitySelect').value = '';
        document.getElementById('regAddress').value = '';
        document.getElementById('regFragile').checked = false;
    } catch (err) {
        notify(err.message, 'error');
    }
});

// --- Pickup Package ---
document.getElementById('pickBtn').addEventListener('click', async () => {
    const guid = document.getElementById('pickGuid').value;

    if (!guid) 
        return notify('Please enter a Tracking GUID!', 'error');

    try {
        await apiPost('/packages/pickup', { trackingGuid: guid });
        notify('Package picked up successfully!');
        document.getElementById('pickGuid').value = '';
    } catch (err) {
        notify(err.message, 'error');
    }
});

// --- Transfer Package ---
document.getElementById('transferBtn').addEventListener('click', async () => {
    const guid = document.getElementById('transferGuid').value;

    if (!guid)
        return notify('Please enter a Tracking GUID!', 'error');
    
    try {
        await apiPost('/packages/transfer', { trackingGuid: guid });
        notify('Package transferred into warehouse!');
        document.getElementById('transferGuid').value = '';
    } catch (err) {
        notify(err.message, 'error');
    }
});

// --- Confirm Delivery ---
document.getElementById('deliverBtn').addEventListener('click', async () => {
    const data = {
        trackingGuid: document.getElementById('deliverGuid').value,
        deliveryOtp: document.getElementById('deliverOtp').value
    };

    if (!data.trackingGuid || !data.deliveryOtp)
        return notify('GUID and OTP are required!', 'error');

    try {
        await apiPost('/packages/confirm-delivery', data);
        notify('Package delivered successfully!');
        document.getElementById('deliverGuid').value = '';
        document.getElementById('deliverOtp').value = '';
    } catch (err) {
        notify(err.message, 'error');
    }
});

// --- Cancel Package ---
document.getElementById('cancelBtn').addEventListener('click', async () => {
    const guid = document.getElementById('cancelGuid').value;

    if (!guid)
        return notify('Please enter a Tracking GUID!', 'error');

    try {
        await apiPost('/packages/cancel', { trackingGuid: guid });
        notify('Package has been canceled.');
        document.getElementById('cancelGuid').value = '';
    } catch (err) {
        notify(err.message, 'error');
    }
});

// --- Tracking History ---
document.getElementById('histBtn').addEventListener('click', async () => {
    const guid = document.getElementById('histGuid').value;
    const listDiv = document.getElementById('histList');

    if (!guid)
        return notify('Please enter a Tracking GUID!', 'error');

    try {
        const history = await apiGet(`/packages/history/${guid}`);

        if (!history || history.length === 0) {
            listDiv.innerHTML = 'No events found for this package.';
            return;
        }

        listDiv.innerHTML = history.map(event => `
            <div class="history-item">
                <strong>${event.eventType}</strong> - ${event.timestamp}<br>
                Postman: ${event.postmanName || 'N/A'} | Warehouse: ${event.warehouseName || 'N/A'}<br>
                Details: ${event.details}
            </div>
        `).join('');
    } catch (err) {
        notify(err.message, 'error');
        listDiv.innerHTML = 'Error loading history.';
    }
});

// Load cities when the page loads
window.addEventListener('DOMContentLoaded', () => {
    loadCities();
});