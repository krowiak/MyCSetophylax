using MyCSetophylax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PodpatrywaczDanych
{
    class Program
    {
        static void Main(string[] args)
        {
            var formatter = new BinaryFormatter();
            using (var strumien = File.OpenRead(
                @"E:\Dokumenty\visual studio 2017\Projects\MyCSetophylax\MyCSetophylax\bin\x64\Release\wyniki\2017-07-16--02-06-11\0-wyniki.a4c"))
            {
                WynikAlgorytmu podpatrywane = (WynikAlgorytmu)formatter.Deserialize(strumien);
            }
        }
    }
}
