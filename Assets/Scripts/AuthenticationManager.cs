using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Net;


//add by chen
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
////////////////////////////////////////////////////////////

public class AuthenticationManager : MonoBehaviour
{
   // the AWS region of where your services live
   // public static Amazon.RegionEndpoint Region = Amazon.RegionEndpoint.APSoutheast1;

   // In production, should probably keep these in a config file
   const string IdentityPool = "ap-southeast-1:31f5e3af-9fd1-4c5a-bc97-80959833ceea"; //insert your Cognito User Pool ID, found under General Settings
   const string AppClientID = "6rbfd74pbb2aic44f0lk8obak7"; //insert App client ID, found under App Client Settings
   const string userPoolId = "ap-southeast-1_kUVhK0aTG";
   const string region = "ap-southeast-1";

   // private AmazonCognitoIdentityProviderClient _provider;
   // private CognitoAWSCredentials _cognitoAWSCredentials;
   // private static string _userid = "";
   // private CognitoUser _user;

   public InputField emailFieldLogin;
   public InputField passwordFieldLogin;
   public UIInputManager _UiManager;

   //add by chen
   private static Credentials _cred;
   private static CognitoIdentityModel _token;
   private static JsUserInfo _userinfo;
   [DllImport("__Internal")]
   private static extern string CognitoSignIn(string userPoolId, string clientId, string username, string password, string gameObject, string method);
   [DllImport("__Internal")]
   private static extern string GetCognitoIdentityCredentials(string region, string userPoolId, string idToken, string identityPoolId, string gameObject, string method);
    ////////////////////////////////////////////////////////////

   // Limitation note: so this GlobalSignOutAsync signs out the user from ALL devices, and not just the game.
   // So if you had other sessions for your website or app, those would also be killed.  
   // Currently, I don't think there is native support for granular session invalidation without some work arounds.
   public void SignOut()
   {
      // await _user.GlobalSignOutAsync();

      // Important! Make sure to remove the local stored tokens 
      // UserSessionCache userSessionCache = new UserSessionCache("", "", "", "");
      // SaveDataManager.SaveJsonData(userSessionCache);

      Debug.Log("user logged out.");
   }

   // access to the user's authenticated credentials to be used to call other AWS APIs
   // public CognitoAWSCredentials GetCredentials()
   // {
   //    return _cognitoAWSCredentials;
   // }

   // access to the user's access token to be used wherever needed - may not need this at all.
   // public string GetAccessToken()
   // {
   //    UserSessionCache userSessionCache = new UserSessionCache();
   //    SaveDataManager.LoadJsonData(userSessionCache);
   //    return userSessionCache.getAccessToken();
   // }
   
   // void Awake()
   // {
   //    Debug.Log("AuthenticationManager: Awake");
   //    _provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Region);
   // }

    //add by chen
    public void JsSiginIn()
    {
        Debug.Log("CognitoHelper::siginIn");
        _cred = new Credentials();
        _token = new CognitoIdentityModel();

        Debug.Log("userName.text: "+emailFieldLogin.text);

        if (string.IsNullOrEmpty(emailFieldLogin.text))
        {
            Debug.Log("Please enter a valid email");          
        }
        else if (string.IsNullOrEmpty(passwordFieldLogin.text))
        {
            Debug.Log("Please enter a valid password");
        }
        else
        {
            CognitoSignIn(userPoolId, AppClientID, emailFieldLogin.text, passwordFieldLogin.text, "OptionsContainer", "JsGetToken");
        }
    }
    public void JsGetToken(string data)
    {
        Debug.Log("CognitoHelper::getToken");
        if (!string.IsNullOrEmpty(data))
        {
            try
            {
                Debug.Log("JsGetToken converting token"+data);
                _token = JsonConvert.DeserializeObject<CognitoIdentityModel>(data);
                Debug.Log("JsGetToken token: "+_token.idToken.jwtToken);
                if (!string.IsNullOrEmpty(_token.idToken.jwtToken) && !string.IsNullOrEmpty(_token.accessToken.jwtToken))
                {
                    Debug.Log("Auth token generated");
                    GetCognitoIdentityCredentials(region, userPoolId, _token.idToken.jwtToken, IdentityPool, "OptionsContainer", "JsLogin");
                    Debug.Log("Login Successfull!");
                    Debug.Log("token: "+_token);
                }else{
                  Debug.LogError("Unauthorized Access");
                }
            }
            catch (Exception e)
            {
                Debug.Log("JsGetToken error");
                Debug.LogError(e);
            }
        }
        else
        {
            Debug.LogError("Empty data received");
        }
    }
    public void JsLogin(string data)
    {
        Debug.Log("CognitoHelper::login");
        if (!string.IsNullOrEmpty(data))
        {
            try
            {
                Credentials awsCred = JsonConvert.DeserializeObject<Credentials>(data);
                if (!string.IsNullOrEmpty(awsCred.AccessKeyId) && !string.IsNullOrEmpty(awsCred.SecretKey))
                {
                    _cred = awsCred;
                    Debug.Log(_cred.AccessKeyId);
                    Debug.Log(_cred.SecretKey);
                    Debug.Log(_cred.SessionToken);
                    Debug.Log("Login Completed!");
                    _UiManager.DisplayComponentsFromAuthStatus(true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        else
        {
            Debug.LogError("JsLogin Empty data received");
        }
    }
    ////////////////////////////////////////////////////////////
}
