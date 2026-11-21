document.addEventListener('DOMContentLoaded', async () => {
    await loadNavbar();

    const historyList = document.getElementById('trackingHistoryList');
    const pickGuidInput = document.getElementById('pickGuid');
    const pickBtn = document.getElementById('pickBtn');
    const messageDiv = document.getElementById('message');

    // Initial state: Show the "Please enter GUID" message
    historyList.innerHTML = '<tr><td colspan="5" style="text-align:center; opacity: 0.7;">Enter a package GUID to see its logs</td></tr>';

    // 1. Load given GUID's logs
    async function loadHistory(guid) {
        try {
            const events = await apiGet(`/packages/${guid}/history`);

            historyList.innerHTML = ''; // Clear current list

            if (!events || events.length === 0) {
                historyList.innerHTML = '<tr><td colspan="5" style="text-align:center;">No logs found for this package.</td></tr>';
                return;
            }

            events.forEach(eve => {
                const row = document.createElement('tr');

                // Format the date for the UI
                const date = new Date(eve.timeStamp).toLocaleString();

                row.innerHTML = `
                    <td>${eve.postmanName || 'N/A'}</td>
                    <td>${eve.warehouseName || 'N/A'}</td>
                    <td>${eve.cityName || 'N/A'}</td>
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
            historyList.innerHTML = '<tr><td colspan="5" style="color:var(--error); text-align:center;">Package not found or error loading logs.</td></tr>';
        }
    }

    // 2. Handle the Search Action
    pickBtn.onclick = async () => {
        const guid = pickGuidInput.value.trim();

        if (!guid) {
            messageDiv.innerHTML = `<span class="error-text">Please enter a GUID!</span>`;
            return;
        }

        // Clear previous messages
        messageDiv.innerHTML = '';
        await loadHistory(guid);
        messageDiv.innerHTML = `<span class="success-text">Logs retrieved! Quack!</span>`;
    };
});