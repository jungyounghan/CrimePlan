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
                return "ũ���� �÷�";
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
                return "���̵�";
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
                return "��й�ȣ";
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
                return "ȸ������";
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
                return "�α���";
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
                return "����";
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
                return "�õ���";
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
                return "�ùٸ� ���̵� �Է����ּ���";
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
                return "�ùٸ� ��й�ȣ�� �Է����ּ���";
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
                return "����";
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
                return "������";
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
                return "ȿ����";
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
                return "�����";
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
                return "���";
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
                return "�ݱ�";
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
                return "���� �Ͻðڽ��ϱ�?";
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
                return "��";
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
                return "�ƴϿ�";
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
                return "ȸ�� ���� ����";
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
                return "�ߺ� ����";
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
                return "ȸ�� ���� �Ϸ�";
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
                return "�α��� ����";
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
                return "�߸��� �̸��� �����Դϴ�";
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
                return "�̹� �α��� ���� �����Դϴ�";
            case Language.English:
                return "This account is already logged in";
        }
        return null;
    }
}