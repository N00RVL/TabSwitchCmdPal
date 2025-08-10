// Popup script for Firefox extension
document.addEventListener('DOMContentLoaded', async () => {
    const searchInput = document.getElementById('searchInput');
    const tabList = document.getElementById('tabList');
    
    let allTabs = [];
    let historyItems = [];
    
    // Load tabs and history
    await loadData();
    
    // Setup search functionality
    searchInput.addEventListener('input', (e) => {
        filterAndDisplayItems(e.target.value);
    });
    
    async function loadData() {
        try {
            // Get all tabs
            const tabs = await browser.tabs.query({});
            allTabs = tabs.map(tab => ({
                ...tab,
                type: 'tab',
                favicon: tab.favIconUrl || getDefaultFavicon(tab.url)
            }));
            
            // Get recent history
            const history = await browser.history.search({
                text: '',
                maxResults: 50,
                startTime: Date.now() - (7 * 24 * 60 * 60 * 1000) // Last 7 days
            });
            
            historyItems = history.map(item => ({
                ...item,
                type: 'history',
                favicon: getDefaultFavicon(item.url)
            }));
            
            // Display all items
            filterAndDisplayItems('');
            
        } catch (error) {
            console.error('Error loading data:', error);
            tabList.innerHTML = '<div class="empty-state">Error loading tabs and history</div>';
        }
    }
    
    function filterAndDisplayItems(searchText) {
        const filteredTabs = allTabs.filter(item => 
            item.title.toLowerCase().includes(searchText.toLowerCase()) ||
            item.url.toLowerCase().includes(searchText.toLowerCase())
        );
        
        const filteredHistory = historyItems.filter(item => 
            item.title.toLowerCase().includes(searchText.toLowerCase()) ||
            item.url.toLowerCase().includes(searchText.toLowerCase())
        );
        
        displayItems(filteredTabs, filteredHistory);
    }
    
    function displayItems(tabs, history) {
        let html = '';
        
        // Current tabs section
        if (tabs.length > 0) {
            html += '<div class="section-header">Open Tabs</div>';
            tabs.forEach(tab => {
                html += createItemHTML(tab);
            });
        }
        
        // History section
        if (history.length > 0) {
            html += '<div class="section-header">Recent History</div>';
            history.slice(0, 20).forEach(item => {
                html += createItemHTML(item);
            });
        }
        
        if (tabs.length === 0 && history.length === 0) {
            html = '<div class="empty-state">No matching tabs or history found</div>';
        }
        
        tabList.innerHTML = html;
        
        // Add click listeners
        document.querySelectorAll('.tab-item').forEach(item => {
            item.addEventListener('click', handleItemClick);
        });
    }
    
    function createItemHTML(item) {
        const isTab = item.type === 'tab';
        const title = escapeHtml(item.title || 'Untitled');
        const url = escapeHtml(item.url || '');
        const favicon = item.favicon || getDefaultFavicon(url);
        
        return `
            <div class="tab-item" data-url="${url}" data-tab-id="${item.id || ''}" data-type="${item.type}">
                <img src="${favicon}" class="tab-favicon" onerror="this.src='data:image/svg+xml,<svg xmlns=\\"http://www.w3.org/2000/svg\\" width=\\"16\\" height=\\"16\\"><rect width=\\"16\\" height=\\"16\\" fill=\\"%23ddd\\"/></svg>'">
                <div class="tab-info">
                    <div class="tab-title">${title}</div>
                    <div class="tab-url">${url}</div>
                </div>
            </div>
        `;
    }
    
    async function handleItemClick(event) {
        const item = event.currentTarget;
        const url = item.dataset.url;
        const tabId = item.dataset.tabId;
        const type = item.dataset.type;
        
        try {
            if (type === 'tab' && tabId) {
                // Switch to existing tab
                await browser.tabs.update(parseInt(tabId), { active: true });
                const tab = await browser.tabs.get(parseInt(tabId));
                await browser.windows.update(tab.windowId, { focused: true });
            } else {
                // Open URL in new tab
                await browser.tabs.create({ url: url, active: true });
            }
            
            // Send data to native host
            await sendToNativeHost({
                action: 'tabActivated',
                url: url,
                title: item.querySelector('.tab-title').textContent,
                timestamp: Date.now()
            });
            
            // Close popup
            window.close();
            
        } catch (error) {
            console.error('Error handling item click:', error);
        }
    }
    
    async function sendToNativeHost(data) {
        try {
            await browser.runtime.sendMessage({
                action: 'sendToNativeHost',
                data: data
            });
        } catch (error) {
            console.error('Error sending to native host:', error);
        }
    }
    
    function getDefaultFavicon(url) {
        try {
            const urlObj = new URL(url);
            return `${urlObj.protocol}//${urlObj.host}/favicon.ico`;
        } catch {
            return 'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"><rect width="16" height="16" fill="%23ddd"/></svg>';
        }
    }
    
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
});
