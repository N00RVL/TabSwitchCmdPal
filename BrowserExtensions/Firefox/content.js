// Content script for Firefox extension
// This script runs in the context of web pages to collect tab information

// Listen for messages from background script
browser.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message.action === 'getPageInfo') {
    const pageInfo = {
      title: document.title,
      url: window.location.href,
      favicon: getFavicon(),
      timestamp: Date.now()
    };
    sendResponse(pageInfo);
  }
});

// Get the favicon URL for the current page
function getFavicon() {
  const favicon = document.querySelector('link[rel="icon"]') || 
                 document.querySelector('link[rel="shortcut icon"]') ||
                 document.querySelector('link[rel="apple-touch-icon"]');
  
  if (favicon) {
    return favicon.href;
  }
  
  // Fallback to default favicon location
  return `${window.location.protocol}//${window.location.host}/favicon.ico`;
}

// Notify background script when page loads
document.addEventListener('DOMContentLoaded', () => {
  browser.runtime.sendMessage({
    action: 'pageLoaded',
    pageInfo: {
      title: document.title,
      url: window.location.href,
      favicon: getFavicon(),
      timestamp: Date.now()
    }
  });
});
