document.addEventListener('DOMContentLoaded', async () => {
    initLanguage();
    await loadNavbar();

    const historyList = document.getElementById('trackingHistoryList');
    const pickGuidInput = document.getElementById('pickGuid');
    const pickBtn = document.getElementById('pickBtn');
    const messageDiv = document.getElementById('message');

    // Initial state: Show the "Please enter GUID" message
    historyList.innerHTML = `<tr><td colspan="5" style="text-align:center; opacity: 0.7;">${t('history_initial_msg')}</td></tr>`;

    // 1. Load given GUID's logs
    async function loadHistory(guid) {
        try {
            const events = await apiGet(`/packages/${guid}/history`);
            historyList.innerHTML = ''; // Clear current list

            if (!events || events.length === 0) {
                historyList.innerHTML = `<tr><td colspan="5" style="text-align:center;">${t('history_no_logs')}</td></tr>`;
                return;
            }

            events.forEach(eve => {
                const row = document.createElement('tr');

                // Format the date for the UI
                const date = new Date(eve.timeStamp).toLocaleString();

                row.innerHTML = `
                    <td>${eve.postmanName || t('na_text')}</td>
                    <td>${eve.warehouseName || t('na_text')}</td>
                    <td>${eve.cityName || t('na_text')}</td>
                    <td>${date}</td>
                    <td>${eve.details}</td>
                `;
                
                // When a row is clicked, highlight it
                row.onclick = () => {
                    // Remove 'selected' class from all rows
                    document.querySelectorAll('.package-table tr').forEach(r => r.classList.remove('selected'));

                    // Select this row
                    row.classList.add('selected');
                };

                historyList.appendChild(row);
            });
        } catch (err) {
            console.error("Quack! Failed to load logs:", err);
            historyList.innerHTML = `<tr><td colspan="5" style="color:var(--error); text-align:center;">${t('history_err_not_found')}</td></tr>`;
        }
    }

    // 2. Handle the Search Action
    pickBtn.onclick = async () => {
        const guid = pickGuidInput.value.trim();

        if (!guid) {
            messageDiv.innerHTML = `<span class="error-text">${t('err_required')}</span>`;
            return;
        }

        // Clear previous messages
        messageDiv.innerHTML = '';
        await loadHistory(guid);
        messageDiv.innerHTML = `<span class="success-text">${t('history_success')}</span>`;
    };
});