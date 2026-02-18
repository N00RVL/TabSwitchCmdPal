
document.addEventListener('DOMContentLoaded', function() {
    const statusElement = document.getElementById('status');
    const tabCountElement = document.getElementById('tabCount');
    const openCmdPaletteBtn = document.getElementById('openCmdPalette');
    const refreshTabsBtn = document.getElementById('refreshTabs');
    const testConnectionBtn = document.getElementById('testConnection');

    updateStatus();
    updateTabCount();

    openCmdPaletteBtn.addEventListener('click', openCommandPalette);
    refreshTabsBtn.addEventListener('click', refreshTabs);
    testConnectionBtn.addEventListener('click', testConnection);

    async function updateStatus() {
        try {

            const response = await chrome.runtime.sendMessage({action: 'ping'});
            
            if (response && response.status === 'ok') {
                statusElement.className = 'status connected';
                statusElement.textContent = '✓ Connected to TabSwitch';
                enableButtons();
            } else {
                statusElement.className = 'status disconnected';
                statusElement.textContent = '✗ Connection failed';
                disableButtons();
            }
        } catch (error) {
            statusElement.className = 'status disconnected';
            statusElement.textContent = '✗ Extension not running';
            disableButtons();
        }
    }

    async function updateTabCount() {
        try {
            const tabs = await chrome.tabs.query({});
            const activeTab = tabs.find(tab => tab.active);
            
            tabCountElement.textContent = `${tabs.length} tabs open`;
            
            if (activeTab) {
                tabCountElement.textContent += ` • Current: ${truncateText(activeTab.title, 30)}`;
            }
        } catch (error) {
            tabCountElement.textContent = 'Unable to get tab information';
        }
    }

    function openCommandPalette() {

        alert('Press Win+R and type "TabSwitch" to open in Command Palette');
    }

    async function refreshTabs() {
        refreshTabsBtn.disabled = true;
        refreshTabsBtn.textContent = 'Refreshing...';
        
        try {

            await chrome.runtime.sendMessage({action: 'refreshTabs'});
            await updateTabCount();
            
            setTimeout(() => {
                refreshTabsBtn.disabled = false;
                refreshTabsBtn.textContent = 'Refresh Tabs';
            }, 1000);
        } catch (error) {
            refreshTabsBtn.disabled = false;
            refreshTabsBtn.textContent = 'Refresh Tabs';
            console.error('Error refreshing tabs:', error);
        }
    }

    async function testConnection() {
        testConnectionBtn.disabled = true;
        testConnectionBtn.textContent = 'Testing...';
        
        try {
            const response = await chrome.runtime.sendMessage({action: 'testNativeConnection'});
            
            if (response && response.connected) {
                statusElement.className = 'status connected';
                statusElement.textContent = '✓ Native connection working';
            } else {
                statusElement.className = 'status disconnected';
                statusElement.textContent = '✗ Native app not found';
            }
        } catch (error) {
            statusElement.className = 'status disconnected';
            statusElement.textContent = '✗ Connection test failed';
        }
        
        setTimeout(() => {
            testConnectionBtn.disabled = false;
            testConnectionBtn.textContent = 'Test Connection';
        }, 2000);
    }

    function enableButtons() {
        openCmdPaletteBtn.disabled = false;
        refreshTabsBtn.disabled = false;
        testConnectionBtn.disabled = false;
    }

    function disableButtons() {
        openCmdPaletteBtn.disabled = true;
        refreshTabsBtn.disabled = true;
        testConnectionBtn.disabled = true;
    }

    function truncateText(text, maxLength) {
        if (text.length <= maxLength) return text;
        return text.substring(0, maxLength - 3) + '...';
    }

    setInterval(updateStatus, 5000);
});

