using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

/*Выполнил : Струков А.В
* Для : ООО «Кросс-Информ»*/

namespace Task1
{

    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cancel = new CancellationTokenSource();
            CancellationToken ct = cancel.Token;// токен
     
            string str = Console.ReadLine();

            Console.WriteLine(MostFrequentTriplet(str,ct));

            string s = Console.ReadLine();
            if (s == "Y")
               cancel.Cancel();
 
            Console.Read();
        }

        private static string MostFrequentTriplet(string str, CancellationToken ct)
        {
            List<string> trip = new List<string>();
            string triplet = "error",sim = "";
            int tripletcount = 0,max = 0;
            var list = GetWords(str);
            foreach (var word in list )
            {
                Task task1 = new Task(() => // первый поток
                {
                    if (word.Count() > 2)
                    {
                        for (int i = 0; i < word.Count() - 2; i++)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                Console.WriteLine("Операция прервана");
                                return;
                            }
                            else
                            {
                              trip.Add(word[i].ToString() + word[i+1].ToString() + word[i+2].ToString());
                              trip.Sort();
                            }

                        }
                    }
                });
               var task2 = task1.ContinueWith(_ => // второй поток
                {

                    foreach (var word1 in trip)
                    {
                        var result = Counter(trip, word1);
                        if(result > tripletcount)                 
                        {
                            triplet = word1;
                            tripletcount = result;
                        }
                        if(result == tripletcount && triplet != word1)
                        {
                          sim = word1; max = result;
                        }
                    }

                });    
                task1.Start();//запуск потока
                Thread.Sleep(100);
            }
            if(max == tripletcount && triplet != sim)
            {
                Console.Write("{0},{1},", sim, max);
            }
            return string.Format("{0},{1}", triplet, tripletcount); 
        }

        static List<string> GetWords(string str)//разделение слов
        {
            char[] separator = new char[7];
            string[] result;
 
            separator[0] = ';';
            separator[1] = ' ';
            separator[2] = '.';
            separator[3] = ',';
            separator[4] = ':';
            separator[5] = '(';
            separator[6] = ')';
 
            result = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
 
            return new List<string>(result.ToList()); 
        }

        static int Counter(List<string> a, string b)//подсчет
        {
            int count = 0;
 
            foreach (var i in a)
                if (i == b) count++;
            return count;
        }
    }
}
