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
  },

  GetAccessTokenJS: function () {
    var token = window.getAccessToken();

    if (!token) {
      return null;
    }

    var bufferSize = lengthBytesUTF8(token) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(token, buffer, bufferSize);
    
    return buffer;
  }
});