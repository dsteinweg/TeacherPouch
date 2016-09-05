using System;

namespace TeacherPouch.Utilities
{
    public static class Guard
    {
        public static void AgainstNull<TArgument>(string parameterName, TArgument argument) where TArgument : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName, String.Format("{0} is null.", parameterName));
            }
        }

        public static void AgainstNullOrWhiteSpace(string parameterName, string parameterValue)
        {
            if (String.IsNullOrWhiteSpace(parameterValue))
            {
                throw new ArgumentNullException(parameterName, String.Format("{0} is null or white space.", parameterName));
            }
        }
    }
}
