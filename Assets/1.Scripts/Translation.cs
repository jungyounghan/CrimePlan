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
}