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
    
    public static string GetLoading(Language language)
    {
        switch(language)
        {
            case Language.Korean:
                return "로딩중";
            case Language.English:
                return "Loading";
        }
        return null;
    }

    public static string GetRequestIdentification(Language language)
    {
        switch(language)
        {
            case Language.Korean:
                return "올바른 아이디를 입력해주세요";
            case Language.English:
                return "Please enter a valid ID";
        }
        return null;
    }

    public static string GetVolume(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "음량";
            case Language.English:
                return "Volume";
        }
        return null;
    }

    public static string GetMasterVolume(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "마스터";
            case Language.English:
                return "Master";
        }
        return null;
    }

    public static string GetEffectVolume(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "효과음";
            case Language.English:
                return "Effect";
        }
        return null;
    }

    public static string GetBackgroundVolume(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "배경음";
            case Language.English:
                return "Background";
        }
        return null;
    }
}