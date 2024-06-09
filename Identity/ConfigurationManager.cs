using System.Text;

namespace AppIdentity;

internal static class ConfigurationManager
{
    public static string AesKey { get; set; } = "LIiVHXrhLvIlPn71rSfYlgbV8Rwke0Q6";
    public static byte[] JwtKey { get; set; } = Encoding.ASCII.GetBytes("eyJhbGciOiJIUzI1NiJ9.eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsImV4cCI6MTY4MTU5MzkxNCwiaWF0IjoxNjgxNTkzOTE0fQ.Nx2qlF9NG_3VHv47ELbLSb79eT9rtIXtb7Qy28TPl5g");
}