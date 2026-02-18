
class TabSwitchBridge {
  constructor() {
    this.nativePort = null;
    this.connectToNativeApp();
    this.setupListeners();
  }

  connectToNativeApp() {
    try {
      this.nativePort = browser.runtime.connectNative('com.tabswitch.host');
      
      this.nativePort.onMessage.addListener((message) => {
        this.handleNativeMessage(message);
      });

      this.nativePort.onDisconnect.addListener(() => {
        console.log('Native messaging disconnected:', browser.runtime.lastError);
        this.nativePort = null;

        setTimeout(() => this.connectToNativeApp(), 5000);
      });

      console.log('Connected to TabSwitch native host');
    } catch (error) {
      console.error('Failed to connect to native app:', error);
    }
  }

  setupListeners() {

    browser.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
      if (changeInfo.status === 'complete') {
        this.sendTabUpdate(tab);
      }
    });

    browser.tabs.onCreated.addListener((tab) => {
      this.sendTabUpdate(tab);
    });

    browser.tabs.onRemoved.addListener((tabId) => {
      this.sendTabRemoved(tabId);
    });

    browser.tabs.onActivated.addListener((activeInfo) => {
      browser.tabs.get(activeInfo.tabId).then((tab) => {
        this.sendTabActivated(tab);
      });
    });
  }

  async handleNativeMessage(message) {
    switch (message.action) {
      case 'getAllTabs':
        return await this.getAllTabs();
      
      case 'getHistory':
        return await this.getHistory(message.query, message.maxResults);
      
      case 'switchToTab':
        return await this.switchToTab(message.tabId);
      
      case 'closeTab':
        return await this.closeTab(message.tabId);
      
      case 'createTab':
        return await this.createTab(message.url);
      
      default:
        console.warn('Unknown message action:', message.action);
    }
  }

  async getAllTabs() {
    try {
      const tabs = await browser.tabs.query({});
      const tabData = tabs.map(tab => ({
        id: tab.id,
        title: tab.title,
        url: tab.url,
        favIconUrl: tab.favIconUrl,
        active: tab.active,
        windowId: tab.windowId,
        index: tab.index,
        pinned: tab.pinned
      }));

      this.sendToNative({
        action: 'tabsResponse',
        tabs: tabData
      });

      return tabData;
    } catch (error) {
      console.error('Error getting tabs:', error);
      this.sendToNative({
        action: 'error',
        message: error.message
      });
    }
  }

  async getHistory(query = '', maxResults = 100) {
    try {
      const historyItems = await browser.history.search({
        text: query,
        maxResults: maxResults,
        startTime: Date.now() - (30 * 24 * 60 * 60 * 1000)
      });

      const historyData = historyItems.map(item => ({
        id: item.id,
        title: item.title,
        url: item.url,
        lastVisitTime: item.lastVisitTime,
        visitCount: item.visitCount,
        typedCount: item.typedCount
      }));

      this.sendToNative({
        action: 'historyResponse',
        history: historyData
      });

      return historyData;
    } catch (error) {
      console.error('Error getting history:', error);
      this.sendToNative({
        action: 'error',
        message: error.message
      });
    }
  }

  async switchToTab(tabId) {
    try {
      const tab = await browser.tabs.get(tabId);
      await browser.windows.update(tab.windowId, { focused: true });
      await browser.tabs.update(tabId, { active: true });
      
      this.sendToNative({
        action: 'tabSwitched',
        tabId: tabId
      });
    } catch (error) {
      console.error('Error switching to tab:', error);
      this.sendToNative({
        action: 'error',
        message: error.message
      });
    }
  }

  async closeTab(tabId) {
    try {
      await browser.tabs.remove(tabId);
      this.sendToNative({
        action: 'tabClosed',
        tabId: tabId
      });
    } catch (error) {
      console.error('Error closing tab:', error);
      this.sendToNative({
        action: 'error',
        message: error.message
      });
    }
  }

  async createTab(url) {
    try {
      const tab = await browser.tabs.create({ url: url });
      this.sendToNative({
        action: 'tabCreated',
        tab: {
          id: tab.id,
          title: tab.title,
          url: tab.url,
          favIconUrl: tab.favIconUrl
        }
      });
    } catch (error) {
      console.error('Error creating tab:', error);
      this.sendToNative({
        action: 'error',
        message: error.message
      });
    }
  }

  sendTabUpdate(tab) {
    this.sendToNative({
      action: 'tabUpdated',
      tab: {
        id: tab.id,
        title: tab.title,
        url: tab.url,
        favIconUrl: tab.favIconUrl,
        active: tab.active,
        windowId: tab.windowId
      }
    });
  }

  sendTabRemoved(tabId) {
    this.sendToNative({
      action: 'tabRemoved',
      tabId: tabId
    });
  }

  sendTabActivated(tab) {
    this.sendToNative({
      action: 'tabActivated',
      tab: {
        id: tab.id,
        title: tab.title,
        url: tab.url,
        favIconUrl: tab.favIconUrl,
        windowId: tab.windowId
      }
    });
  }

  sendToNative(message) {
    if (this.nativePort) {
      try {
        this.nativePort.postMessage(message);
      } catch (error) {
        console.error('Error sending message to native app:', error);
      }
    }
  }
}

const tabSwitchBridge = new TabSwitchBridge();

