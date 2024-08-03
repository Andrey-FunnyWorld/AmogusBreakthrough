mergeInto(LibraryManager.library, {
  SaveExtern: function(data) {
    const dataString = UTF8ToString(data);
    if (player && player.getMode() !== 'lite') {
      SaveDataYandex(player, dataString);
    } else {
      SaveDataLocal(dataString);
    }
  },
  AskToLoginExtern: function() {
    let clientMethod = "ReceiveAuthRequestResultString";
    ysdk.auth.openAuthDialog().then(() => {
      initPlayer().then(_player => {
        sendStartupData(); // see index
        myGameInstance.SendMessage(HtmlBridge, clientMethod, "true");
      }).catch(err => {
        console.log("AskToLoginExtern: Player init ERROR ", err);
        myGameInstance.SendMessage(HtmlBridge, clientMethod, "false");
      });
    }).catch(() => {
      myGameInstance.SendMessage(HtmlBridge, clientMethod, "false");
      // player is not authorized
    });
  },
  ShowRewardedExtern: function() {
    ysdk.adv.showRewardedVideo({
      callbacks: {
          onOpen: () => {
            console.log('Video ad open.');
          },
          onRewarded: () => {
            myGameInstance.SendMessage(HtmlBridge, "ReceiveRewardedResultString", "true");
          },
          onClose: () => {
            myGameInstance.SendMessage(HtmlBridge, "RewardedClosed");
          }, 
          onError: (e) => {
            myGameInstance.SendMessage(HtmlBridge, "RewardedClosed");
          }
      }
    })
  },
  ShowInterstitialExtern: function() {
    ysdk.adv.showFullscreenAdv({
      callbacks: {
          onClose: function(wasShown) {
            myGameInstance.SendMessage(HtmlBridge, "InterstitialClosed");
          },
          onError: function(error) {
            myGameInstance.SendMessage(HtmlBridge, "InterstitialClosed");
          }
      }
    })
  },
  PingYandexReadyExtern: function() {
    console.log("GameReady API");
    ysdk.features.LoadingAPI?.ready();
  },
  ReportMetricExtern: function(id) {
    //ym(TOKEN PLACEHOLDER,'reachGoal', UTF8ToString(id));
  },
  RateGameExtern: function() {
    ysdk.feedback.canReview()
    .then(({ value, reason }) => {
        if (value) {
            ysdk.feedback.requestReview()
                .then(({ feedbackSent }) => {
                    console.log("Review result: ", feedbackSent);
                })
        } else {
            console.log(reason)
        }
    })
  }
});