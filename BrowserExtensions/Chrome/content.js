// Content script for TabSwitch - monitors page changes
(function() {
  'use strict';

  // Track page metadata for better tab identification
  let pageInfo = {
    title: document.title,
    url: window.location.href,
    favicon: getFaviconUrl(),
    description: getPageDescription(),
    keywords: getPageKeywords()
  };

  // Send initial page info
  sendPageInfo();

  // Monitor title changes
  const titleObserver = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      if (mutation.type === 'childList' && document.title !== pageInfo.title) {
        pageInfo.title = document.title;
        sendPageInfo();
      }
    });
  });

  titleObserver.observe(document.querySelector('title') || document.head, {
    childList: true,
    subtree: true
  });

  // Monitor URL changes (for SPAs)
  let currentUrl = window.location.href;
  setInterval(() => {
    if (window.location.href !== currentUrl) {
      currentUrl = window.location.href;
      pageInfo.url = currentUrl;
      pageInfo.title = document.title;
      sendPageInfo();
    }
  }, 1000);

  function getFaviconUrl() {
    const favicon = document.querySelector('link[rel*="icon"]');
    return favicon ? favicon.href : null;
  }

  function getPageDescription() {
    const metaDesc = document.querySelector('meta[name="description"]');
    return metaDesc ? metaDesc.content : '';
  }

  function getPageKeywords() {
    const metaKeywords = document.querySelector('meta[name="keywords"]');
    return metaKeywords ? metaKeywords.content : '';
  }

  function sendPageInfo() {
    chrome.runtime.sendMessage({
      action: 'pageInfoUpdate',
      pageInfo: pageInfo
    });
  }

  // Listen for messages from background script
  chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.action === 'getPageInfo') {
      sendResponse(pageInfo);
    }
  });
})();
