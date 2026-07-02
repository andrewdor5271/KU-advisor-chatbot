namespace MainApp
{
    public enum UserType
    {
        None,
        AnonUser,
        IdentityUser
    }

    public enum SenderType
    {
        User,
        Bot
    }

    public static class GlobalStrings
    {
        public static string NonEmptyStringConstraint(String VarName)
        {
            // to be changed with the db - SQL Syntax varies
            return $"LEN(LTRIM(RTRIM({VarName}))) > 0";
        }
    }

    public static class Consts
    {
        public const int MESSAGE_BATCH_SIZE = 32;

        public const string ANON_ID_COOKIE_NAME = "anon_id";
        public const int ANON_ID_COOKIE_EXPIRATION_PERIOD = 30;

        public const string AUTH_CONTEXT_USER_TYPE_KEY = "user_type";
        public const string AUTH_CONTEXT_IDENTITY_USER_KEY = "identity_user";
        public const string AUTH_CONTEXT_ANON_USER_KEY = "anon_user";

        public const string DUMMY_CONVO_DEFAULT_TITLE = "Your conversation";
        public const string CREATE_CONVO_BUTTON_TEXT = "New conversation";
    }
}

