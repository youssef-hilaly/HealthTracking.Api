using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.Configration.Messages
{
    public static class ErrorsMessages
    {
        public static class Generic
        {
            public static string TypeInternalServerError = "Internal Server Error";
            public static string TypeBadRequest = "Bad Request";

            public static string SomethingWentWrong = "Something went wrong try again later";
            public const string InvalidPayload = "Invalid Payload";
        }

        public static class profile
        {
            public static string UserNotFound = "User Not Found";
        }

        public static class User
        {
            public static string EmailInUse = "The Email is aleady in use";
        }

    }
}
