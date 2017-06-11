using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace license_tamper
{
    public partial class Form1 : Form
    {
        string userName ;
        string computerName ;
       //string userName = Environment.UserName;
       // string computerName = Environment.MachineName;
       string passwordxp = "Say_Something_I'm_Giving_Up_On_You!#@+";
        string passwordfinal;
        
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("something.license"))
            {
                MessageBox.Show("Please delete old license!");
                timer1.Stop();
            }
            else
            {
                computerName = textBox1.Text;
                userName = textBox2.Text;

                passwordfinal = passwordxp + userName + computerName;
                licenseCreator(passwordfinal);
                timer1.Start();
            }
         
        }

        public void licenseCreator(string password)
        {
            string path = "//something.txt";
            string fullpath = Application.StartupPath + path;
            string[] lines = { password };
            System.IO.File.WriteAllLines(fullpath, lines);

        }


        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        //Create lincese
        public void EncryptFile(string file, string password)
        {

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);


            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            File.WriteAllBytes(file, bytesEncrypted);
            System.IO.File.Move(file, "something.license");
            timer1.Stop();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (File.Exists("something.txt"))
            {
                var validExtensions = new[]
            {
                ".txt"
            };

                string[] files = Directory.GetFiles(Application.StartupPath);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]);
                    if (validExtensions.Contains(extension))
                    {
                        EncryptFile(files[i], passwordfinal);
                    }
                }

            }

            if (File.Exists("something.license"))
            {
                timer1.Stop();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string userNamex = Environment.UserName;
            string computerNamex = Environment.MachineName;
            string passwordxpx = "Say_Something_I'm_Giving_Up_On_You!#@+";
            string passwordfinalx;
            passwordfinalx = passwordxpx + userNamex + computerNamex;
            if (File.Exists("something.license"))
            {
                var validExtensions = new[]
            {
                ".license"
            };

                string[] files = Directory.GetFiles(Application.StartupPath);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]);
                    if (validExtensions.Contains(extension))
                    {
                        DecryptFile(files[i], passwordfinalx);
                    }
                }
            }
            if (!File.Exists("something.license"))
            {
                MessageBox.Show("license not found!");
            }


        }

        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {

                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();


                }
            }

            return decryptedBytes;
        }

        //Check license it work or not
        public void DecryptFile(string file, string password)
        {
            try
            {
                byte[] bytesToBeDecrypted = File.ReadAllBytes(file);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
                for (int i = 0; i < bytesDecrypted.Length; i++)
                {
                        
                }
                MessageBox.Show("good license!");
            }
            catch
            {
                MessageBox.Show("wrong license!");
            }


        }

    }
}
