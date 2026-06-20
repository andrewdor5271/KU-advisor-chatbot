namespace MainApp
{
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
    }
}

