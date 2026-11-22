document.addEventListener('DOMContentLoaded', async () => {
    initLanguage();
    await loadNavbar();

    const availableList = document.getElementById('availablePackagesList');
    const pickGuidInput = document.getElementById('pickGuid');
    const pickBtn = document.getElementById('pickBtn');
    const messageDiv = document.getElementById('message');

    let selectedGuid = null;

    // Helper to get current language string from dictionary
    const t = (key) => {
        const lang = sessionStorage.getItem('preferredLang') || 'en';
        return translations[lang][key] || key;
    };

    // 1. Load available packages for the current postman
    async function loadInventory() {
        try {
            // We call the endpoint that uses the X-Postman-Id header
            const packages = await apiGet('/packages/available');
            availableList.innerHTML = ''; // Clear current list

            if (packages.length === 0) {
                availableList.innerHTML = `<tr><td colspan="3" style="text-align:center;">${t('pickup_no_pkgs')}</td></tr>`;
                return;
            }

            packages.forEach(pkg => {
                const row = document.createElement('tr');

                // Format the date for the UI (Unused for now)
                const date = new Date(pkg.timeStamp).toLocaleDateString();

                row.innerHTML = `
                    <td>${pkg.trackingGuid}</td>
                    <td>${pkg.destinationAddress}</td>
                    <td class="fragile-cell" data-fragile="${pkg.isFlaggedAsFragile}">${pkg.isFlaggedAsFragile ? t('fragile_yes') : t('fragile_no')}</td>
                `;
                
                // When a row is clicked, highlight it and fill the input field
                row.onclick = () => {
                    // Remove 'selected' class from all rows
                    document.querySelectorAll('.package-table tr').forEach(r => r.classList.remove('selected'));

                    // Select this row
                    row.classList.add('selected');

                    // Fill the manual input and update selectedGuid
                    selectedGuid = pkg.trackingGuid;
                    pickGuidInput.value = pkg.trackingGuid;
                };

                availableList.appendChild(row);
            });
        } catch (err) {
            console.error("Quack! Failed to load inventory:", err);
            availableList.innerHTML = `<tr><td colspan="3" style="color:var(--error); text-align:center;">${t('err_inventory')}</td></tr>`;
        }
    }

    // 2. Handle the Pickup Action
    pickBtn.onclick = async () => {
        const guid = pickGuidInput.value.trim();

        if (!guid) {
            messageDiv.innerHTML = `<span class="error-text">${t('err_required')}</span>`;
            return;
        }

        try {
            await apiPost('/packages/pickup', { trackingGuid: guid });
            messageDiv.innerHTML = `<span class="success-text">${t('pickup_success')}</span>`;

            // Clear selection and input
            pickGuidInput.value = '';
            selectedGuid = null;
            document.querySelectorAll('.package-table tr').forEach(r => r.classList.remove('selected'));

            // Refresh the list since one package is now gone!
            await loadInventory();
        } catch (err) {
            messageDiv.innerHTML = `<span class="error-text">${err.message || 'Something went wrong!'}</span>`;
        }
    };

    // Initial load
    await loadInventory();
});