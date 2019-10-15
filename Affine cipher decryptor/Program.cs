using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Affine_cipher_decryptor
{
    class Program
    {
        static string alphabet = "абвгдежзийклмнопрстуфхцчшщъыьэюя";
        static string periodicity = "оеаитнсрвлкмдпуяызьъбгчйхжюшцщэф";
        static string[] imp_combs = { "ъь", "ьъ", "уъ", "уь", "еъ", "еь", "ыъ", "ыь", "аь", "аъ", "оь", "оъ",
                                      "эь", "эъ", "яь", "яъ", "иь", "иъ", "юь", "юъ" };
        static void Main(string[] args)
        {

            List<string> Decrypted_string = new List<string>();
            Console.WriteLine(alphabet.Length);
            Console.Write("Введите текст: ");
            string input = Console.ReadLine();

            IEnumerable<char> chars = input.Distinct();
            Dictionary<char, int> count_of_chars = new Dictionary<char, int>(input.Distinct().Count());

            foreach (char c in chars) count_of_chars.Add(c, input.Where(i => i == c).Count());
            count_of_chars = count_of_chars.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            for (int i = 0; i < count_of_chars.Count; i++)
            {
                char c = count_of_chars.ElementAt(i).Key;
                Console.WriteLine(c + " - " + count_of_chars.ElementAt(i).Value + " - " + alphabet.IndexOf(c));
            }

            string s = "";
            for (int i1 = 0; i1 < 31; i1++)
            {
                for (int i2 = i1 + 1; i2 < 32; i2++)
                {
                    int a = 0;
                    int b = 0;
                    if (count_of_chars.ElementAt(0).Key != periodicity[i1]) a = Find_A(
                            alphabet.IndexOf(count_of_chars.ElementAt(0).Key), alphabet.IndexOf(periodicity[i1]),
                            alphabet.IndexOf(count_of_chars.ElementAt(1).Key), alphabet.IndexOf(periodicity[i2]), 32);
                    b = Find_B(alphabet.IndexOf(count_of_chars.ElementAt(0).Key), alphabet.IndexOf(periodicity[i1]), a, 32);
                    if (a != 0 && a != 32 && IsCoprime(a, 32))
                    { s = Decrypt(input, Inverse(a, 32), b); Decrypted_string.Add(s); }

                    if (count_of_chars.ElementAt(0).Key != periodicity[i1]) a = Find_A(
                            alphabet.IndexOf(count_of_chars.ElementAt(0).Key), alphabet.IndexOf(periodicity[i2]),
                            alphabet.IndexOf(count_of_chars.ElementAt(1).Key), alphabet.IndexOf(periodicity[i1]), 32);
                    b = Find_B(alphabet.IndexOf(count_of_chars.ElementAt(0).Key), alphabet.IndexOf(periodicity[i1]), a, 32);
                    if (a != 0 && a != 32 && IsCoprime(a, 32))
                    { s = Decrypt(input, Inverse(a, 32), b); Decrypted_string.Add(s); }
                }
            }
            Decrypted_string = Decrypted_string.Distinct().ToList();
            Console.WriteLine("До обработки: " + Decrypted_string.Count());
            List<string> result = new List<string>();
            var pattern = @"(.)\1{3}";

            foreach (string res in Decrypted_string)
            {
                int i = 0;
                if (res.First() == 'ы' || res.First() == 'ъ' || res.First() == 'ь' || res.First() == 'й' || Regex.IsMatch(res, pattern)) i++;
                foreach (string ic in imp_combs) if (res.Contains(ic)) i++;
                if (i == 0)
                {
                    result.Add(res);
                }
            }
            Console.WriteLine("После: " + result.Count());
            Console.ReadLine();
            Console.Clear();

            foreach (char c in periodicity)
            {
                var lst = result.FindAll(i => i[0] == c);
                if (lst.Count() == 0) continue;
                foreach (string str in lst) Console.WriteLine(str);
                Console.ReadLine();
            }
            Console.WriteLine("###############################################################################");
        }

        static bool IsCoprime(int a, int b)
        {
            if (a == 0 || b == 0) return false;
            else if (Gcd(a, b) == 1) return true;
            else return false;
        }

        static int Gcd(int a, int b)
        {
            if (a == b) return a;
            else if (a % b == 0) return b;
            else if (b % a == 0) return a;
            else
            {
                if (a > b) return Gcd(a - b, b);
                else return Gcd(b - a, a);
            }
        }

        static int Mod(int a, int mod)
        {
            if (a == 0) return 0;
            else if (a == 32) return 0;
            else if (a % mod < 0) return a % mod + mod;
            else return a % mod;
        }

        static int Inverse(int a, int mod)
        {
            for (int x = 1; x <= mod; x++) if ((a * x) % mod == 1) return x;
            return 0;
        }

        static int Find_A(int enc1, int pred1, int enc2, int pred2, int mod)
        {
            int z = Mod(enc1 - enc2, mod);
            int n = Mod(pred1 - pred2, mod);

            if (z % Gcd(n, 32) != 0) return 0;

            if (n < 1) n += mod;
            if (Gcd(z, Gcd(n, mod)) != 1)
            {
                int cd = Gcd(z, Gcd(n, mod));
                z /= cd;
                n /= cd;
                mod /= cd;
            }
            if (z % 2 != 0 || n % 2 != 0) while (z % n != 0) z += mod;

            int a = Mod(z / n, mod);

            return a;
        }

        static int Find_B(int enc, int pred, int a, int mod)
        {
            return (Mod(enc - a * pred, mod));
        }

        static string Decrypt(string s, int a_inv, int b)
        {
            char[] str = s.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                int c = alphabet.IndexOf(str[i]);
                str[i] = alphabet[Mod(a_inv * (c - b), 32)];
            }
            string st = new string(str);
            return st;
        }
    }
}
