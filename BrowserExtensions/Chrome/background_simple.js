
let nativePort = null;

function connectToNativeHost() {
  try {
    nativePort = chrome.runtime.connectNative('com.tabswitch.nativehost');
    
    nativePort.onMessage.addListener((message) => {
      console.log('Received from native host:', message);
    });
    
    nativePort.onDisconnect.addListener(() => {
      console.log('Native host disconnected');
      nativePort = null;

      setTimeout(connectToNativeHost, 5000);
    });
    
    console.log('Connected to native host');
  } catch (error) {
    console.error('Failed to connect to native host:', error);
  }
}

async function sendTabsToNativeHost() {
  try {
    const tabs = await chrome.tabs.query({});
    
    const tabData = tabs
      .filter(tab => !tab.incognito)
      .map(tab => ({
        id: tab.id.toString(),
        title: tab.title || 'Untitled',
        url: tab.url || '',
        favicon: tab.favIconUrl || '',
        active: tab.active,
        browser: 'Chrome',
        timestamp: Date.now(),
        type: 'tab'
      }));

    if (nativePort) {
      nativePort.postMessage({
        action: 'updateTabData',
        data: {
          tabs: tabData,
          browser: 'Chrome'
        }
      });
    }
  } catch (error) {
    console.error('Error sending tabs to native host:', error);
  }
}

chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
  if (changeInfo.status === 'complete') {
    sendTabsToNativeHost();
  }
});

chrome.tabs.onCreated.addListener(() => {
  sendTabsToNativeHost();
});

chrome.tabs.onRemoved.addListener(() => {
  sendTabsToNativeHost();
});

chrome.windows.onFocusChanged.addListener(() => {
  sendTabsToNativeHost();
});

chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message.action === 'sendToNativeHost' && nativePort) {
    nativePort.postMessage(message.data);
    sendResponse({ success: true });
  } else {
    sendResponse({ success: false, error: 'No native host connection' });
  }
});

connectToNativeHost();
sendTabsToNativeHost();

setInterval(sendTabsToNativeHost, 30000);

