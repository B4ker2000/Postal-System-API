async function loadTransferPage() {
    const postmanId = sessionStorage.getItem('postmanId');
    // Call a new endpoint like /packages/my-packages to list what the postman currently has
    const packages = await apiRequest('/packages/my-packages', 'GET');
    renderPackageTable(packages);
}

async function handleTransfer() {
    const guid = document.getElementById('trackingGuid').value;
    // Assuming we use the postman's assigned warehouse for the transfer
    const warehouseId = sessionStorage.getItem('warehouseId'); 

    const response = await apiRequest('/packages/transfer', 'POST', { 
        trackingGuid: guid, 
        warehouseId: parseInt(warehouseId) 
    });
    
    if (response.isSuccess) {
        alert("Package transferred! Quack!");
        loadTransferPage();
    } else {
        alert("Error: " + response.message);
    }
}
