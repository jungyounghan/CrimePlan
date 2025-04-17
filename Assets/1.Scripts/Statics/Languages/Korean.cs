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
                    letters[i] = "시작";
                    break;
                case Translation.Letter.Title:
                    letters[i] = "크라임 플랜";
                    break;
                case Translation.Letter.Version:
                    letters[i] = "버전";
                    break;
                case Translation.Letter.Setting:
                    letters[i] = "설정";
                    break;
                case Translation.Letter.Open:
                    letters[i] = "열기";
                    break;
                case Translation.Letter.Close:
                    letters[i] = "닫기";
                    break;
                case Translation.Letter.Confirm:
                    letters[i] = "확인";
                    break;
                case Translation.Letter.Yes:
                    letters[i] = "네";
                    break;
                case Translation.Letter.No:
                    letters[i] = "아니오";
                    break;
                case Translation.Letter.Success:
                    letters[i] = "성공";
                    break;
                case Translation.Letter.Failed:
                    letters[i] = "실패";
                    break;

                case Translation.Letter.Volume:
                    letters[i] = "음량";
                    break;
                case Translation.Letter.Master:
                    letters[i] = "마스터";
                    break;
                case Translation.Letter.Effect:
                    letters[i] = "효과";
                    break;
                case Translation.Letter.Background:
                    letters[i] = "배경";
                    break;
                case Translation.Letter.Language:
                    letters[i] = "언어";
                    break;
                case Translation.Letter.Day:
                    letters[i] = "{0}일";
                    break;
                case Translation.Letter.Evening:
                    letters[i] = "저녁";
                    break;
                case Translation.Letter.Morning:
                    letters[i] = "아침";
                    break;
                case Translation.Letter.Midday:
                    letters[i] = "점심";
                    break;
                case Translation.Letter.Survivor:
                    letters[i] = "생존자";
                    break;
                case Translation.Letter.Citizen:
                    letters[i] = "시민";
                    break;
                case Translation.Letter.Criminal:
                    letters[i] = "범인";
                    break;

                case Translation.Letter.Identification:
                    letters[i] = "아이디";
                    break;
                case Translation.Letter.Password:
                    letters[i] = "비밀번호";
                    break;
                case Translation.Letter.SignUp:
                    letters[i] = "회원가입";
                    break;
                case Translation.Letter.SignIn:
                    letters[i] = "로그인";
                    break;


                case Translation.Letter.RequestValid:
                    letters[i] = "올바른 {0}를 입력해주세요";
                    break;
                case Translation.Letter.Duplicated:
                    letters[i] = "중복된 {0}입니다";
                    break;
                case Translation.Letter.AlreadyConnected:
                    letters[i] = "이 계정은 현재 접속중입니다";
                    break;

                case Translation.Letter.Remaining:
                    letters[i] = "남은 {0}";
                    break;
                case Translation.Letter.DoYouWantTo:
                    letters[i] = "{0} 하시겠습니까?";
                    break;
                case Translation.Letter.TryConnection:
                    letters[i] = "접속 시도 중";
                    break;
                case Translation.Letter.LoseConnection:
                    letters[i] = "연결이 끊어졌습니다";
                    break;
                case Translation.Letter.Quit:
                    letters[i] = "종료";
                    break;
            }
        }
    }
}