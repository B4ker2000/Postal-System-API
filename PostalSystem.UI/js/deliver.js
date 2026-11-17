document.addEventListener('DOMContentLoaded', async () => {
    await loadNavbar();

    const availableList = document.getElementById('availablePackagesList');
    const pickGuidInput = document.getElementById('pickGuid');
    const pickOtpInput = document.getElementById('pickOtp');
    const pickBtn = document.getElementById('pickBtn');
    const messageDiv = document.getElementById('message');

    let selectedGuid = null;

    // 1. Load packages the current user has picked up 
    async function loadPickedUpPackages() {
        try {
            // We call the endpoint that uses the X-Postman-Id header
            const packages = await apiGet('/packages/picked-up');

            availableList.innerHTML = ''; // Clear current list

            if (packages.length === 0) {
                availableList.innerHTML = '<tr><td colspan="3" style="text-align:center;">You currently have no packages.</td></tr>';
                return;
            }

            packages.forEach(pkg => {
                const row = document.createElement('tr');

                // Format the date for the UI
                const date = new Date(pkg.timeStamp).toLocaleDateString();

                row.innerHTML = `
                    <td>${pkg.trackingGuid}</td>
                    <td>${pkg.destinationAddress}</td>
                    <td>${pkg.isFlaggedAsFragile ? '⚠️ Yes' : 'No'}</td>
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
            availableList.innerHTML = '<tr><td colspan="3" style="color:var(--error); text-align:center;">Failed to load inventory.</td></tr>';
        }
    }

    // 2. Handle the Transfer Action
    pickBtn.onclick = async () => {
        const payload = {
            TrackingGuid: pickGuidInput.value.trim(),
            Otp: pickOtpInput.value.trim()
        }

        if (!payload.TrackingGuid) {
            messageDiv.innerHTML = `<span class="error-text">Please select a package or enter a GUID and fill the OTP field!</span>`;
            return;
        }

        if (!payload.Otp) {
            messageDiv.innerHTML = `<span class="error-text">Please fill the OTP field!</span>`;
            return;
        }

        try {
            await apiPost('/packages/confirm-delivery', payload);

            messageDiv.innerHTML = `<span class="success-text">Package delivered successfully! Quack!</span>`;

            // Clear selection and inputs
            pickGuidInput.value = '';
            pickOtpInput.value = '';
            selectedGuid = null;
            document.querySelectorAll('.package-table tr').forEach(r => r.classList.remove('selected'));

            // Refresh the list since one package is now gone!
            await loadPickedUpPackages();
        } catch (err) {
            messageDiv.innerHTML = `<span class="error-text">${err.message || 'Something went wrong!'}</span>`;
        }
    };

    // Initial load
    await loadPickedUpPackages();
});