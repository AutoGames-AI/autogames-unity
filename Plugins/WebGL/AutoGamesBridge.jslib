mergeInto(LibraryManager.library, {
  TriggerAutoGamesLogin: function() {
    if (typeof window.TriggerAutoGamesLogin === 'function') {
      window.TriggerAutoGamesLogin();
    } else {
      console && console.warn && console.warn('TriggerAutoGamesLogin not found');
    }
  },
  TriggerAutoGamesLogout: function() {
    if (typeof window.TriggerAutoGamesLogout === 'function') {
      window.TriggerAutoGamesLogout();
    } else if (window.AutoGamesSDK && window.AutoGamesSDK.logout) {
      window.AutoGamesSDK.logout();
    }
  }
});
