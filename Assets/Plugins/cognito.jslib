mergeInto(LibraryManager.library, {
  CognitoSignIn: function (
    userPoolId,
    clientId,
    username,
    password,
    gameObject,
    method
  ) {
    var poolData = {
      UserPoolId: Pointer_stringify(userPoolId),
      ClientId: Pointer_stringify(clientId),
    };
    var go = Pointer_stringify(gameObject);
    var m = Pointer_stringify(method);
    this.RefreshToken = "";
    this.userPool = new AmazonCognitoIdentity.CognitoUserPool(poolData);
    this.userData = {
      Username: Pointer_stringify(username),
      Pool: userPool,
    };
    var authenticationData = {
      Username: Pointer_stringify(username),
      Password: Pointer_stringify(password),
    };
    this.authenticationDetails =
      new AmazonCognitoIdentity.AuthenticationDetails(authenticationData);
    this.cognitoUser = new AmazonCognitoIdentity.CognitoUser(userData);
    cognitoUser.authenticateUser(authenticationDetails, {
      onSuccess: function (result) {
        RefreshToken = result.getRefreshToken();
        unityInstance.SendMessage(go, m, JSON.stringify(result));
      },
      onFailure: function (error) {
        console.log(error);
        unityInstance.SendMessage(go, m, "");
      },
    });
  },
  GetCognitoIdentityCredentials: function (
    region,
    userPoolId,
    idToken,
    identityPoolId,
    gameObject,
    method
  ) {
    AWS.config.region = Pointer_stringify(region);
    var go = Pointer_stringify(gameObject);
    var m = Pointer_stringify(method);
    this.Region = Pointer_stringify(region);
    this.UserPoolId = Pointer_stringify(userPoolId);
    this.IdentityPoolId = Pointer_stringify(identityPoolId);
    var loginMap = {};
    loginMap["cognito-idp." + Region + ".amazonaws.com/" + UserPoolId] =
      Pointer_stringify(idToken);
    AWS.config.credentials = new AWS.CognitoIdentityCredentials({
      IdentityPoolId: IdentityPoolId,
      Logins: loginMap,
    });
    AWS.config.credentials.clearCachedId();
    AWS.config.credentials.get(function (err) {
      if (err) {
        console.log(err.message);
        unityInstance.SendMessage(go, m, "");
      } else {
        var access = JSON.stringify(AWS.config.credentials.accessKeyId);
        var secret = JSON.stringify(AWS.config.credentials.secretAccessKey);
        var session = JSON.stringify(AWS.config.credentials.sessionToken);
        unityInstance.SendMessage(
          go,
          m,
          '{"AccessKeyId": ' +
            access +
            ', "SecretKey":' +
            secret +
            ',"SessionToken": ' +
            session +
            "}"
        );
      }
    });
  },
  RefreshCognitoToken: function (currentToken, gameobject, method) {
    var refreshToken = Pointer_stringify(currentToken);
    var go = Pointer_stringify(gameobject);
    var m = Pointer_stringify(method);
    var token = new AmazonCognitoIdentity.CognitoRefreshToken({
      RefreshToken: refreshToken,
    });
    cognitoUser.refreshSession(token, function (err, session) {
      if (err) {
        console.log(err.message);
        unityInstance.SendMessage(go, m, "");
      } else {
        unityInstance.SendMessage(go, m, JSON.stringify(session));
      }
    });
  },
});
