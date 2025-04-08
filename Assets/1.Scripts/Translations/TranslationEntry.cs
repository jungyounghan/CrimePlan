public static partial class Translation
{
    public enum Language : byte
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
    
    public static string GetTrying(Language language)
    {
        switch(language)
        {
            case Language.Korean:
                return "시도중";
            case Language.English:
                return "Trying";
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

    public static string GetRequestPassword(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "올바른 비밀번호를 입력해주세요";
            case Language.English:
                return "Please enter a valid Password";
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

    public static string GetLanguage(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "언어";
            case Language.English:
                return "Language";
        }
        return null;
    }

    public static string GetClose(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "닫기";
            case Language.English:
                return "Close";
        }
        return null;
    }

    public static string GetQuit(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "종료 하시겠습니까?";
            case Language.English:
                return "Do you want to Quit?";
        }
        return null;
    }

    public static string GetYes(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "네";
            case Language.English:
                return "Yes";
        }
        return null;
    }

    public static string GetNo(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "아니오";
            case Language.English:
                return "No";
        }
        return null;
    }

    public static string GetSignUpFailure(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "회원 가입 실패";
            case Language.English:
                return "Failed to sign up";
        }
        return null;
    }

    public static string GetSignUpDuplicate(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "중복 가입";
            case Language.English:
                return "The ID is duplicated";
        }
        return null;
    }

    public static string GetSignUpSuccess(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "회원 가입 완료";
            case Language.English:
                return "The membership is complete";
        }
        return null;
    }

    public static string GetSignInFailure(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "로그인 실패";
            case Language.English:
                return "Login failed";
        }
        return null;
    }

    public static string GetSignInInvalidEmail(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "잘못된 이메일 형식입니다";
            case Language.English:
                return "Invalid email format";
        }
        return null;
    }

    public static string GetSignInAlready(Language language)
    {
        switch (language)
        {
            case Language.Korean:
                return "이미 로그인 중인 계정입니다";
            case Language.English:
                return "This account is already logged in";
        }
        return null;
    }
}