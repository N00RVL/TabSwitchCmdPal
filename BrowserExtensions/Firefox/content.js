

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

function getFavicon() {
  const favicon = document.querySelector('link[rel="icon"]') || 
                 document.querySelector('link[rel="shortcut icon"]') ||
                 document.querySelector('link[rel="apple-touch-icon"]');
  
  if (favicon) {
    return favicon.href;
  }
  

  return `${window.location.protocol}
}

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

