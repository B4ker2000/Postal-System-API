document.addEventListener('DOMContentLoaded', async () => {
    // Inject the nav bar first! 
    await loadNavbar();

    const citySelect = document.getElementById('regDestCitySelect');
    const addressInput = document.getElementById('regAddress');
    const fragileInput = document.getElementById('regFragile');
    const regBtn = document.getElementById('regBtn');
    const messageDiv = document.getElementById('message');

    // 1. Load Cities for the dropdown
    try {
        const cities = await apiGet('/cities');
        cities.forEach(city => {
            const option = document.createElement('option');
            option.value = city.cityId;
            option.textContent = city.cityName;
            citySelect.appendChild(option);
        });
    } catch (err) {
        messageDiv.innerHTML = `<span class="error-text">Quack! Failed to load cities.</span>`;
    }

    // 2. Handle Registration
    regBtn.onclick = async () => {
        const payload = {
            finalDestinationCityId: parseInt(citySelect.value),
            destinationAddress: addressInput.value.trim(),
            isFlaggedAsFragile: fragileInput.checked
        };

        if (!payload.finalDestinationCityId || !payload.destinationAddress) {
            messageDiv.innerHTML = `<span class="error-text">Please fill in all required fields!</span>`;
            return;
        }

        try {
            const response = await apiPost('/packages/register', payload);

            // Success! Show the tracking GUID and offer to view history
            messageDiv.innerHTML = `
            <span class="success-text">
                Package Registered!<br>
                Tracking ID: <strong>${response.trackingGuid}</strong><br>
                OTP: <strong>${response.deliveryOTP}</strong>
                </span>
                <button onclick="window.location.href='history.html?guid=${response.trackingGuid}'"
                        style="background-color: var(--secondary-color); margin-top: 10px;">
                    View Tracking History
                </button>`;
                
            // Reset form
            addressInput.value = '';
            fragileInput.checked = false;
        } catch (err) {
            messageDiv.innerHTML = `<span class="error-text">${err.message || 'Something went wrong!'}</span>`;
        }
    };
});