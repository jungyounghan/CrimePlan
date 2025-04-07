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
    
    public static string GetLoading(Language language)
    {
        switch(language)
        {
            case Language.Korean:
                return "�ε���";
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
                return "�ùٸ� ���̵� �Է����ּ���";
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
}