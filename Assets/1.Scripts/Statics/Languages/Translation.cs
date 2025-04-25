public static class Translation
{
    public enum Language : byte
    {
        Korean,
        English
    }

    public enum Letter: byte
    {
        Start,
        Ready,
        Title,
        Version,
        Setting,
        Open,
        Close,
        Confirm,
        Yes,
        No,
        Success,
        Failed,
        Select,
        Agree,
        Disagree,
        Volume,
        Master,
        Effect,
        Background,
        Language,
        Time,
        Evening,
        Morning,
        Midday,
        Mine,
        Survivor,
        Alive,
        Dead,
        Unknown,
        Citizen,
        Criminal,
        Victory,
        Identity,
        Identification,
        Password,
        SignUp,
        SignIn,
        Accessor,
        Room,
        Host,
        Name,
        Create,
        Join,
        Leave,
        Exit,
        Apply,
        RequestValid,
        Duplicated,
        AlreadyConnected,

        Day,
        Increase,
        Decrease,
        Remaining,
        DoYouWantTo,
        TryConnection,
        LoseConnection,
        Quit,
        End
    }

    private static string[] letters = new string[(int)Letter.End];

    public static void Set(Language language)
    {
        switch(language)
        {
            case Language.Korean:
                Korean.Set(ref letters);
                break;
            case Language.English:
                English.Set(ref letters);
                break;
        }
    }

    public static string Get(Letter letter)
    {
        if (letter >= Letter.Start && letter < Letter.End)
        {
            return letters[(int)letter];
        }
        return null;
    }
}