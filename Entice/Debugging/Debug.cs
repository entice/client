using System.Windows.Forms;

namespace Entice.Debugging
{
        public static class Debug
        {
                public static void Error(string format, params object[] args)
                {
                        Networking.Networking.SignOut();

                        MessageBox.Show(string.Format(format, args) + "\n\nthe program behaviour is now UNDEFINED!\nFurthermore you were LOGGED OUT :P", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
        }
}