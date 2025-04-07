public static class Translation
{
    public enum Language: byte
    {
        Korean,
        English
    }

    public static string GetTitle(Language language)
    {
        switch(language)
        {
            case Language.Korean:
                return "크라임 플랜";
            case Language.English:
                return "Crime Plan";
        }
        return null;
    }

    public static string GetIdentification(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "아이디";
            case Language.English:
                return "User Id";
        }
        return null;
    }

    public static string GetPassword(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "비밀번호";
            case Language.English:
                return "Password";
        }
        return null;
    }

    public static string GetSignUp(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "회원가입";
            case Language.English:
                return "Sign Up";
        }
        return null;
    }

    public static string GetSignIn(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "로그인";
            case Language.English:
                return "Sign In";
        }
        return null;
    }

    public static string GetVersion(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "버전";
            case Language.English:
                return "version";
        }
        return null;
    }
}