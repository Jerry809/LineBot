using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeanChat
{
    public class Hint
    {
        public int A { get; set; }
        public int B { get; set; }

        public Hint(int a, int b)
        {
            A = a;
            B = b;
        }
    }

    public class GuessNum
    {
        public string GenNum()
        {
            string result = string.Empty;
            var dic = new SortedDictionary<int, int>();
            var random = new Random();
            for (int i = 0; i < 10; i++)
                dic.Add(random.Next(), i);

            int cnt = 0;
            foreach (var item in dic.Keys)
            {
                result += dic[item].ToString();
                cnt++;
                if (cnt >= 4)
                    break;
            }
            return result;
        }

        public Hint Compare(string answer, string trial, out string msg)
        {
            msg = string.Empty;
            List<int> dupCheck = new List<int>();
            int a = 0, b = 0;
            for (int i = 0; i < trial.Length; i++)
            {
                if (dupCheck.Contains(trial[i])) msg="數字不可以重覆";
                dupCheck.Add(trial[i]);
                if (answer[i] == trial[i]) a++;
                else if (answer.Contains(trial[i])) b++;
            }

            return new Hint(a, b);
        }
    }
}