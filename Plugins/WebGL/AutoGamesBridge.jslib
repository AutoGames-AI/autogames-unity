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
  },
  FetchUserAssetsFromJS: function (profileIdPtr) {
      var profileId = UTF8ToString(profileIdPtr);
      if (window.dispatchFetchUserAssets) {
          window.dispatchFetchUserAssets(profileId);
      } else {
          console.error("Method dispatchFetchUserAssets not found in index.html");
      }
  },

  FetchVotingsFromJS: function (tokenId) {
      if (window.dispatchFetchVotings) {
          window.dispatchFetchVotings(tokenId);
      } else {
          console.error("Method dispatchFetchVotings not found in index.html");
      }
  },
  VoteFromJS: function (voteId, optionId) {
      if (window.dispatchVote) {
          window.dispatchVote(voteId, optionId);
      } else {
          console.error("Method dispatchVote not found in index.html");
      }
  }
});