using Android.OS;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace Skyfall
{
    public static class Firebase
    {
        public static FirebaseApp firebaseApp = null;
        public static string firebaseUsername = "unknown";
        public static string firebasePassword = "";

        public static void SetUp()
        {
            if (Build.Device.Equals("channel"))
            {
                firebaseUsername = "aeronmike@gmail.com";
                firebasePassword = "hygnus4312";
            }
            else if (Build.Device.Equals("potter"))
            {
                firebaseUsername = "guto.fagundes@gmail.com";
                firebasePassword = "hygnus0513";
            }

            Kernel.LogMessage("User: <i>" + Firebase.firebaseUsername + "</i>");

            firebaseApp = FirebaseApp.GetInstance("[DEFAULT]");
        }

        public static async void Login()
        {
            try
            {
                var user = await FirebaseAuth.GetInstance(firebaseApp).SignInWithEmailAndPasswordAsync(firebaseUsername, firebasePassword);
                var token = await user.User.GetIdTokenAsync(false);
                Kernel.LogMessage("Firebase Token: <i>OK</i>");
            }
            catch (Exception e)
            {
                Kernel.LogException(e);
                Kernel.LogMessage("Firebase Token: <i>Invalid</i>");
            }
        }
    }
}