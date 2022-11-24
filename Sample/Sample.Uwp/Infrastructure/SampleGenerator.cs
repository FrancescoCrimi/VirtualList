using System;
using System.Collections.Generic;
using System.Text;
using CiccioSoft.VirtualList.Data.Domain;

namespace CiccioSoft.VirtualList.Sample.Uwp.Infrastructure
{
    public static class SampleGenerator
    {
        public static List<Model> Generate(int total = 10000)
        {
            var list = new List<Model>(total);
            for (var i = 1; i <= total; i++)
            {
                var str_build = new StringBuilder();
                var random = new Random();
                char letter;
                for (var l = 0; l < 7; l++)
                {
                    var flt = random.NextDouble();
                    var shift = Convert.ToInt32(Math.Floor(26 * flt));
                    letter = Convert.ToChar(shift + 65);
                    str_build.Append(letter);
                }
                var model = new Model((uint)i, str_build.ToString());
                list.Add(model);
            }
            return list;
        }
    }
}
