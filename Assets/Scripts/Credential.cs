public class JsUserInfo
{
    public string username { get; set; }
    public string password { get; set; }
}

public class IdToken
{
    public string jwtToken { get; set; }
}
public class RefreshToken
{
    public string token { get; set; }
}
public class AccessToken
{
    public string jwtToken { get; set; }
}
public class CognitoIdentityModel
{
    public IdToken idToken { get; set; }
    public RefreshToken refreshToken { get; set; }
    public AccessToken accessToken { get; set; }
    public int clockDrift { get; set; }
}
public class Credentials
{
    public string AccessKeyId { get; set; }
    public string SecretKey { get; set; }
    public string SessionToken { get; set; }
}