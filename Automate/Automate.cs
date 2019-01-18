using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automate
{

    public struct Arg
    {
        public string State;
        public char Char;

        public Arg(string state, char c)
        {
            State = state;
            Char = c;
        }
    }
    public struct ArgDKA
    {
        public List<string> State;
        public char Char;

        public ArgDKA(List<string> state, char c)
        {
            State = state;
            Char = c;
        }
    }

    public class eNKA
    {
        //множество состояний
        public List<string> States = new List<string>();

        //множество входных символов
        public List<char> InputChars = new List<char>();

        //функции переходов
        public Dictionary<Arg, List<string>> Function = new Dictionary<Arg, List<string>>();

        //начальное состояние
        public List<string> firstStates = new List<string>();

        //множество заключительных(допускающих) состояний
        public List<string> lastStates = new List<string>();

        public eNKA() { }

        public eNKA(string name)
        {
            //прочитать из файла
            List<string> line = File.ReadLines($"../../" + name + ".txt").ToList();
            //считали входные символы
            List<string> inpchar = line[0].Split(null as string[], StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            foreach (string str in inpchar)
            {
                InputChars.Add(str[0]);
            }
            //считали состояния
            States = line[1].Split(null as string[], StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            firstStates = line[2].Split(null as string[], StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            lastStates = line[3].Split(null as string[], StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            //считываем функции перехода
            for (int i = 4; i<line.Count;i++)
            {
                List<string> func = line[i].Split(null as string[], StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                Function.Add(new Arg(func[0], func[1][0]), func.Skip(2).ToList<string>());
            }
        }

        public static void print(string name, eNKA nka)
        {
            using (FileStream fs = new FileStream($"../../" + name + ".txt", FileMode.Create, FileAccess.Write))
            {
                TextWriter tw = new StreamWriter(fs);

                string temp = string.Empty;
                foreach (char ch in nka.InputChars)
                {
                    temp += ch + " ";
                }
                tw.WriteLine(temp);
                temp = string.Empty;

                foreach (string state in nka.States)
                {
                    temp += state + " ";
                }
                tw.WriteLine(temp);
                temp = string.Empty;

                foreach (string state in nka.firstStates)
                {
                    temp += state + " ";
                }
                tw.WriteLine(temp);
                temp = string.Empty;

                foreach (string state in nka.lastStates)
                {
                    temp += state + " ";
                }
                tw.WriteLine(temp);
                temp = string.Empty;

                foreach (Arg arg in nka.Function.Keys)
                {
                    temp = arg.State + " " + arg.Char + " ";
                    foreach (string str in nka.Function[arg])
                    {
                        temp += str + " ";
                    }
                    tw.WriteLine(temp);
                    temp = string.Empty;
                }
                tw.Flush();
                fs.Flush();
            }
        }

    }

    public class DKA
    {
        //множество состояний
        public List<string> States = new List<string>();

        //множество входных символов
        public List<char> InputChars = new List<char>();

        //функции переходов
        public Dictionary<Arg, string> Function = new Dictionary<Arg, string>();

        //начальное состояние
        public string firstState = string.Empty;

        //множество заключительных(допускающих) состояний
        public List<string> lastStates = new List<string>();

        public DKA()
        {

        }


        //запись в файл
        public void PrintToFile(string name)
        {
            using (FileStream fs = new FileStream($"../../"+name+".txt", FileMode.Create, FileAccess.Write))
            {
                TextWriter tw = new StreamWriter(fs);

                string temp = string.Empty;
                foreach(char ch in InputChars)
                {
                    temp += ch + " ";
                }
                tw.WriteLine(temp);
                temp = string.Empty;

                foreach(string state in States)
                {
                    temp += state + " ";
                }
                tw.WriteLine(temp);
                temp = string.Empty;

                tw.WriteLine(firstState);

                foreach (string state in lastStates)
                {
                    temp += state + " ";
                }
                tw.WriteLine(temp);
                temp = string.Empty;

                foreach(Arg arg in Function.Keys)
                {
                    temp = arg.State + " " + arg.Char + " " + Function[arg];
                    tw.WriteLine(temp);
                    temp = string.Empty;
                }
                tw.Flush();
                fs.Flush();
            }
        }


    }
}
