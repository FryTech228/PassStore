using System;
using System.Text;

namespace PassStore
{
    static class RC4
    {
        public static string Encrypt(string str, string keyS, bool enc = true)
        {
            byte[] data = enc ? Encoding.UTF8.GetBytes(str) : Convert.FromBase64String(str);
            byte[] pwd = Encoding.UTF8.GetBytes(keyS);

            int a, i, j, k, tmp;
            int[] key, box;
            byte[] cipher;

            key = new int[256];
            box = new int[256];
            cipher = new byte[data.Length];

            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + key[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < data.Length; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }

            return enc ? Convert.ToBase64String(cipher) : Encoding.UTF8.GetString(cipher);
        }

        public static string Decrypt(string str, string keyS) => Encrypt(str, keyS, false);
    }
}
