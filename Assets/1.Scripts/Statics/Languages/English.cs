public static class English 
{
    public static void Set(ref string[] letters)
    {
        int length = letters != null ? letters.Length : 0;
        for (int i = 0; i < length; i++)
        {
            switch ((Translation.Letter)i)
            {
                case Translation.Letter.Start:
                    letters[i] = "Start";
                    break;
                case Translation.Letter.Ready:
                    letters[i] = "Ready";
                    break;
                case Translation.Letter.Title:
                    letters[i] = "Crime Plan";
                    break;
                case Translation.Letter.Version:
                    letters[i] = "Version";
                    break;
                case Translation.Letter.Setting:
                    letters[i] = "Setting";
                    break;
                case Translation.Letter.Open:
                    letters[i] = "Open";
                    break;
                case Translation.Letter.Close:
                    letters[i] = "Close";
                    break;
                case Translation.Letter.Confirm:
                    letters[i] = "Confirm";
                    break;
                case Translation.Letter.Yes:
                    letters[i] = "Yes";
                    break;
                case Translation.Letter.No:
                    letters[i] = "No";
                    break;
                case Translation.Letter.Success:
                    letters[i] = "Success";
                    break;
                case Translation.Letter.Failed:
                    letters[i] = "Failed";
                    break;
                case Translation.Letter.Select:
                    letters[i] = "Select";
                    break;
                case Translation.Letter.Agree:
                    letters[i] = "Agree";
                    break;
                case Translation.Letter.Disagree:
                    letters[i] = "Disagree";
                    break;
                case Translation.Letter.Volume:
                    letters[i] = "Volume";
                    break;
                case Translation.Letter.Master:
                    letters[i] = "Master";
                    break;
                case Translation.Letter.Effect:
                    letters[i] = "Effect";
                    break;
                case Translation.Letter.Background:
                    letters[i] = "Background";
                    break;
                case Translation.Letter.Language:
                    letters[i] = "Language";
                    break;
                case Translation.Letter.Time:
                    letters[i] = "Time";
                    break;
                case Translation.Letter.Evening:
                    letters[i] = "Evening";
                    break;
                case Translation.Letter.Morning:
                    letters[i] = "Morning";
                    break;
                case Translation.Letter.Midday:
                    letters[i] = "Midday";
                    break;
                case Translation.Letter.Mine:
                    letters[i] = "Mine";
                    break;
                case Translation.Letter.Survivor:
                    letters[i] = "Survivor";
                    break;
                case Translation.Letter.Alive:
                    letters[i] = "Alive";
                    break;
                case Translation.Letter.Dead:
                    letters[i] = "Dead";
                    break;
                case Translation.Letter.Unknown:
                    letters[i] = "Unknown";
                    break;
                case Translation.Letter.Citizen:
                    letters[i] = "Citizen";
                    break;
                case Translation.Letter.Criminal:
                    letters[i] = "Criminal";
                    break;
                case Translation.Letter.Victory:
                    letters[i] = "Victory";
                    break;
                case Translation.Letter.Identity:
                    letters[i] = "Identity";
                    break;
                case Translation.Letter.Identification:
                    letters[i] = "User Id";
                    break;
                case Translation.Letter.Password:
                    letters[i] = "Password";
                    break;
                case Translation.Letter.SignUp:
                    letters[i] = "Sign Up";
                    break;
                case Translation.Letter.SignIn:
                    letters[i] = "Sign In";
                    break;
                case Translation.Letter.Accessor:
                    letters[i] = "Accessor";
                    break;
                case Translation.Letter.Room:
                    letters[i] = "Room";
                    break;
                case Translation.Letter.Host:
                    letters[i] = "Host";
                    break;
                case Translation.Letter.Name:
                    letters[i] = "Name";
                    break;
                case Translation.Letter.Create:
                    letters[i] = "Create";
                    break;
                case Translation.Letter.Join:
                    letters[i] = "Join";
                    break;
                case Translation.Letter.Leave:
                    letters[i] = "Leave";
                    break;
                case Translation.Letter.Exit:
                    letters[i] = "Exit";
                    break;
                case Translation.Letter.Apply:
                    letters[i] = "Apply {0}";
                    break;
                case Translation.Letter.RequestValid:
                    letters[i] = "Please Enter a Valid {0}";
                    break;
                case Translation.Letter.Duplicated:
                    letters[i] = "The {0} is Duplicated";
                    break;
                case Translation.Letter.AlreadyConnected:
                    letters[i] = "This Account is Currently Logged in";
                    break;

                case Translation.Letter.Day:
                    letters[i] = "Day {0}";
                    break;
                case Translation.Letter.Increase:
                    letters[i] = "Increase {0}";
                    break;
                case Translation.Letter.Decrease:
                    letters[i] = "Decrease {0}";
                    break;
                case Translation.Letter.Remaining:
                    letters[i] = "Remaining {0}";
                    break;
                case Translation.Letter.DoYouWantTo:
                    letters[i] = "Do You Want to {0}?";
                    break;
                case Translation.Letter.TryConnection:
                    letters[i] = "Try to Connection";
                    break;
                case Translation.Letter.LoseConnection:
                    letters[i] = "The Connection has been Lost";
                    break;
                case Translation.Letter.Quit:
                    letters[i] = "Quit";
                    break;
            }
        }
    }
}