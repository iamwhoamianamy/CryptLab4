using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace CryptLab4
{
   class Program
   {
      static public string path = "../../../files/";
      static void Main(string[] args)
      {
         bool repeat = true;

         while (repeat)
         {
            Console.WriteLine("Выберете опцию:");
            Console.WriteLine("<1> Зашифровать файл");
            Console.WriteLine("<2> Дешифровать файл");
            Console.WriteLine("<3> Зашифровать только пиксели файла");
            Console.WriteLine("<0> Выйти(");

            string choice = Console.ReadLine();

            switch (choice)
            {
               case "1":
               {
                  Console.WriteLine("Введите название файла для шифрования:");
                  string inFileName = Console.ReadLine();
                  byte[] ToEncrypt;
                  try
                  {
                     ToEncrypt = File.ReadAllBytes(path + inFileName);
                  }
                  catch (Exception)
                  {
                     Console.WriteLine("Нет такого файла!");
                     break;
                  }

                  Console.WriteLine("Введите название файла для вывода:");
                  string outFileName = Console.ReadLine();

                  Console.WriteLine("Введите пароль для шифрования:");
                  byte[] Password = Encoding.UTF8.GetBytes(Console.ReadLine());

                  var hashAlg = GetHashAlgorithm();
                  File.WriteAllBytes(path + outFileName, Encrypt(ToEncrypt, Password, hashAlg, CipherMode.CBC));

                  Console.WriteLine("Шифрование прошло успешно!");
                  break;
               }
               case "2":
               {
                  Console.WriteLine("Введите название файла для дешифровки:");
                  string inFileName = Console.ReadLine();
                  byte[] ToDecrypt;
                  try
                  {
                     ToDecrypt = File.ReadAllBytes(path + inFileName);
                  }
                  catch (Exception)
                  {
                     Console.WriteLine("Нет такого файла!");
                     break;
                  }

                  Console.WriteLine("Введите название файла для вывода:");
                  string outFileName = Console.ReadLine();

                  Console.WriteLine("Введите пароль для дешифровки:");
                  byte[] Password = Encoding.UTF8.GetBytes(Console.ReadLine());

                  var hashAlg = GetHashAlgorithm();
                  try
                  {
                     File.WriteAllBytes(path + outFileName, Decrypt(ToDecrypt, Password, hashAlg, CipherMode.CBC));
                  }
                  catch(Exception)
                  {
                     Console.WriteLine("Неверный пароль или алгоритм генерации хеша!");
                     break;
                  }
                  Console.WriteLine("Дешифровка прошла успешно!");
                  break;
               }
               case "3":
               {
                  Console.WriteLine("Введите название файла для шифрования:");
                  string inFileName = Console.ReadLine();

                  Bitmap img;
                  try
                  {
                     img = new Bitmap(path + inFileName);
                  }
                  catch (Exception)
                  {
                     Console.WriteLine("Нет такого файла!");
                     break;
                  }

                  int w = img.Width;
                  int h = img.Height;

                  byte[] toEncrypt = new byte[h * w * 4];

                  for (int i = 0; i < h; i++)
                     for (int j = 0; j < w; j++)
                     {
                        Color pixel = img.GetPixel(j, i);
                        toEncrypt[i * w * 4 + j * 4 + 0] = pixel.A;
                        toEncrypt[i * w * 4 + j * 4 + 1] = pixel.R;
                        toEncrypt[i * w * 4 + j * 4 + 2] = pixel.G;
                        toEncrypt[i * w * 4 + j * 4 + 3] = pixel.B;
                     }

                  Console.WriteLine("Введите название файла для вывода:");
                  string outFileName = Console.ReadLine();

                  Console.WriteLine("Введите пароль для шифрования:");
                  byte[] Password = Encoding.UTF8.GetBytes(Console.ReadLine());

                  var hashAlg = GetHashAlgorithm();
                  byte[] encData = Encrypt(toEncrypt.ToArray(), Password, hashAlg, CipherMode.ECB);

                  Bitmap outImage = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                  for (int i = 0; i < h; i++)
                     for (int j = 0; j < w; j++)
                     {
                        byte A = encData[i * w * 4 + j * 4 + 0];
                        byte R = encData[i * w * 4 + j * 4 + 1];
                        byte G = encData[i * w * 4 + j * 4 + 2];
                        byte B = encData[i * w * 4 + j * 4 + 3];
                        outImage.SetPixel(j, i, Color.FromArgb(A, R, G, B));
                     }

                  outImage.Save(path + outFileName);

                  Console.WriteLine("Шифрование прошло успешно!");
                  break;
               }
               case "0":
               {
                  repeat = false;
                  break;
               }
               default:
               {
                  Console.WriteLine("Неверный выбор!");
                  break;
               }
            }
         }
      }

      public static HashAlgorithm GetHashAlgorithm()
      {
         bool repeat = true;
         HashAlgorithm res = HashAlgorithm.Create("SHA-256");

         while (repeat)
         {
            repeat = false;
            Console.WriteLine("Выберете алгоритм генерации хеша:");
            Console.WriteLine("<1> SHA-256");
            Console.WriteLine("<2> MD5");
            Console.WriteLine("<3> SHA1");

            string hashChoice = Console.ReadLine();

            switch (hashChoice)
            {
               case "1":
               {
                  res = HashAlgorithm.Create("SHA-256");
                  break;
               }
               case "2":
               {
                  res = HashAlgorithm.Create("MD5");
                  break;
               }
               case "3":
               {
                  res = HashAlgorithm.Create("SHA1");
                  break;
               }
               default:
               {
                  repeat = true;
                  Console.WriteLine("Неверный выбор!");
                  break;
               }
            }
         }
         return res;
      }

      public static byte[] Encrypt(byte[] dataToEncrypt, byte[] password, HashAlgorithm hashAlg, CipherMode cipherMode)
      {
         using (var aes = Aes.Create())
         {
            aes.Mode = cipherMode;
            aes.Key = hashAlg.ComputeHash(password);
            aes.IV = aes.Key.ToList().Take(16).ToArray();
            aes.Padding = PaddingMode.PKCS7;

            return PerformCrypt(aes.CreateEncryptor(), dataToEncrypt);
         }
      }

      public static byte[] Decrypt(byte[] dataToDecrypt, byte[] password, HashAlgorithm hashAlg, CipherMode cipherMode)
      {
         using (var aes = Aes.Create())
         {
            aes.Mode = cipherMode;
            aes.Key = hashAlg.ComputeHash(password);
            aes.IV = aes.Key.ToList().Take(16).ToArray();
            aes.Padding = PaddingMode.PKCS7;
            var decryptor = aes.CreateDecryptor();

            return PerformCrypt(aes.CreateDecryptor(), dataToDecrypt);
         }   
      }

      public static byte[] PerformCrypt(ICryptoTransform transform, byte[] data)
      {
         using (var msDecrypt = new MemoryStream())
         {
            using (var csEncrypt = new CryptoStream(msDecrypt, transform, CryptoStreamMode.Write))
            {
               csEncrypt.Write(data, 0, data.Length);
               csEncrypt.FlushFinalBlock();
               return msDecrypt.ToArray();
            }
         }
      }
   }
}
