public static class Korean
{
    public static void Set(ref string[] letters)
    {
        int length = letters != null ? letters.Length : 0;
        for (int i = 0; i < length; i++)
        {
            switch ((Translation.Letter)i)
            {
                case Translation.Letter.Start:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Title:
                    letters[i] = "ũ���� �÷�";
                    break;
                case Translation.Letter.Version:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Setting:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Open:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Close:
                    letters[i] = "�ݱ�";
                    break;
                case Translation.Letter.Confirm:
                    letters[i] = "Ȯ��";
                    break;
                case Translation.Letter.Yes:
                    letters[i] = "��";
                    break;
                case Translation.Letter.No:
                    letters[i] = "�ƴϿ�";
                    break;
                case Translation.Letter.Success:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Failed:
                    letters[i] = "����";
                    break;

                case Translation.Letter.Volume:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Master:
                    letters[i] = "������";
                    break;
                case Translation.Letter.Effect:
                    letters[i] = "ȿ��";
                    break;
                case Translation.Letter.Background:
                    letters[i] = "���";
                    break;
                case Translation.Letter.Language:
                    letters[i] = "���";
                    break;
                case Translation.Letter.Day:
                    letters[i] = "{0}��";
                    break;
                case Translation.Letter.Evening:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Morning:
                    letters[i] = "��ħ";
                    break;
                case Translation.Letter.Midday:
                    letters[i] = "����";
                    break;
                case Translation.Letter.Survivor:
                    letters[i] = "������";
                    break;
                case Translation.Letter.Citizen:
                    letters[i] = "�ù�";
                    break;
                case Translation.Letter.Criminal:
                    letters[i] = "����";
                    break;

                case Translation.Letter.Identification:
                    letters[i] = "���̵�";
                    break;
                case Translation.Letter.Password:
                    letters[i] = "��й�ȣ";
                    break;
                case Translation.Letter.SignUp:
                    letters[i] = "ȸ������";
                    break;
                case Translation.Letter.SignIn:
                    letters[i] = "�α���";
                    break;


                case Translation.Letter.RequestValid:
                    letters[i] = "�ùٸ� {0}�� �Է����ּ���";
                    break;
                case Translation.Letter.Duplicated:
                    letters[i] = "�ߺ��� {0}�Դϴ�";
                    break;
                case Translation.Letter.AlreadyConnected:
                    letters[i] = "�� ������ ���� �������Դϴ�";
                    break;

                case Translation.Letter.Remaining:
                    letters[i] = "���� {0}";
                    break;
                case Translation.Letter.DoYouWantTo:
                    letters[i] = "{0} �Ͻðڽ��ϱ�?";
                    break;
                case Translation.Letter.TryConnection:
                    letters[i] = "���� �õ� ��";
                    break;
                case Translation.Letter.LoseConnection:
                    letters[i] = "������ ���������ϴ�";
                    break;
                case Translation.Letter.Quit:
                    letters[i] = "����";
                    break;
            }
        }
    }
}