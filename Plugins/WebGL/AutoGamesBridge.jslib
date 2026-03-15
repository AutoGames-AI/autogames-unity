mergeInto(LibraryManager.library, {
  TriggerAutoGamesLogin: function() {
    if (typeof window.TriggerAutoGamesLogin === 'function') {
      window.TriggerAutoGamesLogin();
    }
  },

  TriggerAutoGamesLogout: function() {
    if (typeof window.TriggerAutoGamesLogout === 'function') {
      window.TriggerAutoGamesLogout();
    }
  },

  RequestAccessTokenJS: function () {
    if (typeof window.dispatchGetAccessToken === 'function') {
        window.dispatchGetAccessToken();
    } else {
        console.error("dispatchGetAccessToken not found in index.html");
    }
  }
});