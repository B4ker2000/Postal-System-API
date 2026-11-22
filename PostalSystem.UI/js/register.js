document.addEventListener('DOMContentLoaded', async () => {
    // Inject the language first and then the navbar! 
    initLanguage()
    await loadNavbar();

    const citySelect = document.getElementById('regDestCitySelect');
    const addressInput = document.getElementById('regAddress');
    const fragileInput = document.getElementById('regFragile');
    const regBtn = document.getElementById('regBtn');
    const messageDiv = document.getElementById('message');

    // Helper to get current language string from dictionary
    const t = (key) => {
        const lang = sessionStorage.getItem('preferredLang') || 'en';
        return translations[lang][key] || key;
    };

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
        messageDiv.innerHTML = `<span class="error-text">${t('reg_err_cities')}</span>`;
    }

    // 2. Handle Registration
    regBtn.onclick = async () => {
        const payload = {
            finalDestinationCityId: parseInt(citySelect.value),
            destinationAddress: addressInput.value.trim(),
            isFlaggedAsFragile: fragileInput.checked
        };

        if (!payload.finalDestinationCityId || !payload.destinationAddress) {
            messageDiv.innerHTML = `<span class="error-text">${t('reg_err_required')}</span>`;
            return;
        }

        try {
            const response = await apiPost('/packages/register', payload);

            // Success! Show the tracking GUID and offer to view history
            messageDiv.innerHTML = `
            <span class="success-text">
                ${t('reg_success_title')}<br>
                ${t('reg_tracking_id')} <strong>${response.trackingGuid}</strong><br>
                ${t('reg_otp')} <strong>${response.deliveryOTP}</strong>
                </span>
                <button onclick="window.location.href='history.html?guid=${response.trackingGuid}'"
                        style="background-color: var(--secondary-color); margin-top: 10px;">
                    ${t('reg_view_history')}
                </button>`;
                
            // Reset form
            addressInput.value = '';
            fragileInput.checked = false;
        } catch (err) {
            messageDiv.innerHTML = `<span class="error-text">${err.message || t('reg_err_generic')}</span>`;
        }
    };
});