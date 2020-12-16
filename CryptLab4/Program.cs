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
            Console.WriteLine("<4> Дешифровать только пиксели файла");
            Console.WriteLine("<0> Выйти(");

            string choice = Console.ReadLine();

            switch (choice)
            {
               case "1":
               {
                  Console.WriteLine("Введите название файла для шифрования:");
                  string inFileName = Console.ReadLine();
                  byte[] toEncrypt;
                  try
                  {
                     toEncrypt = File.ReadAllBytes(path + inFileName);
                  }
                  catch (Exception)
                  {
                     Console.WriteLine("Нет такого файла!");
                     break;
                  }

                  Console.WriteLine("Введите название файла для вывода:");
                  string outFileName = Console.ReadLine();

                  Console.WriteLine("Введите пароль для шифрования:");
                  byte[] password = Encoding.UTF8.GetBytes(Console.ReadLine());

                  var hashAlg = GetHashAlgorithm();
                  File.WriteAllBytes(path + outFileName, Encrypt(toEncrypt, password, hashAlg, CipherMode.CBC));

                  Console.WriteLine("Шифрование прошло успешно!");
                  break;
               }
               case "2":
               {
                  Console.WriteLine("Введите название файла для дешифровки:");
                  string inFileName = Console.ReadLine();
                  byte[] toDecrypt;
                  try
                  {
                     toDecrypt = File.ReadAllBytes(path + inFileName);
                  }
                  catch (Exception)
                  {
                     Console.WriteLine("Нет такого файла!");
                     break;
                  }

                  Console.WriteLine("Введите название файла для вывода:");
                  string outFileName = Console.ReadLine();

                  Console.WriteLine("Введите пароль для дешифровки:");
                  byte[] password = Encoding.UTF8.GetBytes(Console.ReadLine());

                  var hashAlg = GetHashAlgorithm();
                  try
                  {
                     File.WriteAllBytes(path + outFileName, Decrypt(toDecrypt, password, hashAlg, CipherMode.CBC));
                  }
                  catch (Exception)
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
                        toEncrypt[(i * w + j) * 4 + 0] = pixel.A;
                        toEncrypt[(i * w + j) * 4 + 1] = pixel.R;
                        toEncrypt[(i * w + j) * 4 + 2] = pixel.G;
                        toEncrypt[(i * w + j) * 4 + 3] = pixel.B;
                     }

                  Console.WriteLine("Введите название файла для вывода:");
                  string outFileName = Console.ReadLine();

                  Console.WriteLine("Введите пароль для шифрования:");
                  byte[] password = Encoding.UTF8.GetBytes(Console.ReadLine());

                  var hashAlg = GetHashAlgorithm();
                  byte[] encData = Encrypt(toEncrypt.ToArray(), password, hashAlg, CipherMode.ECB);

                  for (int i = 0; i < h; i++)
                     for (int j = 0; j < w; j++)
                     {
                        byte A = encData[(i * w + j) * 4 + 0];
                        byte R = encData[(i * w + j) * 4 + 1];
                        byte G = encData[(i * w + j) * 4 + 2];
                        byte B = encData[(i * w + j) * 4 + 3];
                        img.SetPixel(j, i, Color.FromArgb(A, R, G, B));
                     }

                  img.Save(path + outFileName, System.Drawing.Imaging.ImageFormat.Png);

                  Console.WriteLine("Шифрование прошло успешно!");
                  break;
               }
               case "4":
               {
                  Console.WriteLine("Введите название файла для дешифровки:");
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

                  byte[] toDecrypt = new byte[h * w * 4];

                  for (int i = 0; i < h; i++)
                     for (int j = 0; j < w; j++)
                     {
                        Color pixel = img.GetPixel(j, i);
                        toDecrypt[(i * w + j) * 4 + 0] = pixel.A;
                        toDecrypt[(i * w + j) * 4 + 1] = pixel.R;
                        toDecrypt[(i * w + j) * 4 + 2] = pixel.G;
                        toDecrypt[(i * w + j) * 4 + 3] = pixel.B;
                     }

                  Console.WriteLine("Введите название файла для вывода:");
                  string outFileName = Console.ReadLine();

                  Console.WriteLine("Введите пароль для дешифровки:");
                  byte[] password = Encoding.UTF8.GetBytes(Console.ReadLine());

                  var hashAlg = GetHashAlgorithm();
                  byte[] decData = Decrypt(toDecrypt.ToArray(), password, hashAlg, CipherMode.ECB);

                  for (int i = 0; i < h; i++)
                     for (int j = 0; j < w; j++)
                     {
                        byte A = decData[(i * w + j) * 4 + 0];
                        byte R = decData[(i * w + j) * 4 + 1];
                        byte G = decData[(i * w + j) * 4 + 2];
                        byte B = decData[(i * w + j) * 4 + 3];
                        img.SetPixel(j, i, Color.FromArgb(A, R, G, B));
                     }

                  img.Save(path + outFileName, System.Drawing.Imaging.ImageFormat.Png);

                  Console.WriteLine("Дешифровка прошла успешно!");
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
            aes.Padding = PaddingMode.Zeros;

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
            aes.Padding = PaddingMode.Zeros;
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
