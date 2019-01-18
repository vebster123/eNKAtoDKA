using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automate
{
    public static class AutomateFunction
    {
        //удаляем пустые переходы
        public static eNKA deleteEps(eNKA enka)
        {
            //
            eNKA nka = new eNKA();
            nka.InputChars = enka.InputChars;
            //nka.States = enka.States;
            nka.firstStates = enka.firstStates;
            //nka.lastStates = enka.lastStates;

            //получаем состояния в которые можно перейти по пустому переходу
            List<string> stateFromEpsilon = new List<string>();
            foreach (Arg arg in enka.Function.Keys)
            {
                if (arg.Char == "0"[0])
                {
                    stateFromEpsilon.AddRange(enka.Function[arg]);
                }
            }
            //получаем состояния в которые можно перейти ТОЛЬКО по пустому переходу
            List<string> stateFromEpsilonOnly = new List<string>();
            foreach (string state in stateFromEpsilon)
            {
                bool flag = true;
                foreach (Arg arg in enka.Function.Keys)
                {
                    if (enka.Function[arg].Contains(state))
                    {
                        if (arg.Char != "0"[0]) flag = false;
                    }
                }
                if (flag) stateFromEpsilonOnly.Add(state);
            }
            //1)новое множество состояний, без состояний в которые можно попасть только по пустому переходу
            //удалили из состояний такие состояния, и объединили с начальными состояниями(в случае если такое было удалено)
            nka.States = enka.States.Except(stateFromEpsilonOnly).Union(enka.firstStates).ToList();
            //

            //2)переносим все функции без пустных символов где исходящее состояние входит в множество новых состояний
            //(используем пересечение с состояниями нка во входящих состояниях, чтобы не добавить удаленное состояние)

            foreach (Arg arg in enka.Function.Keys)
            {
                if (arg.Char != "0"[0] && nka.States.Contains(arg.State)) nka.Function.Add(arg, enka.Function[arg].Intersect(nka.States).ToList());
            }

            //для тройки состояний p, q, r ищем путь из p в q ненулевой длины состоящий из пустых переходов, и путь длины 1 из q в r по не пустому символу, это будет новая функция
            foreach (string stateP in nka.States)
            {
                List<string> temp = new List<string>();
                //лист состояний в которые можно попасть из p используя сколько угодно пустых переходов
                List<string> statesFromPbyEpsilon = StatesFromStateByEpsilon(stateP, temp, enka);

                foreach (string stateQ in enka.States)
                {
                    foreach (string stateR in nka.States)
                    {
                        foreach (Arg arg in enka.Function.Keys.Where(state => state.State == stateQ && state.Char != "0"[0]))
                        {
                            if (enka.Function[arg].Contains(stateR))
                            {
                                //если есть путь по пустым в состояние q
                                if (statesFromPbyEpsilon.Contains(stateQ))
                                {
                                    Arg arg2 = new Arg(stateP, arg.Char);
                                    if (nka.Function.ContainsKey(arg2))
                                    {
                                        if (!nka.Function[arg2].Contains(stateR)) nka.Function[arg2].Add(stateR);
                                    }
                                    else
                                    {
                                        List<string> st = new List<string>();
                                        st.Add(stateR);
                                        nka.Function.Add(arg2, st);
                                    }
                                }
                            }
                        }
                    }
                }
                temp.Clear();
            }


            //3)если из неконечного состояния по пустым переходам можно попасть в конечное, то делаем его конечным состоянием
            foreach (string state in nka.States)
            {
                List<string> temp = new List<string>();
                if (StatesFromStateByEpsilon(state, temp, enka).Intersect(enka.lastStates).ToList().Count > 0)
                    nka.lastStates.Add(state);
            }
            //оставшиеся заключительные состояния которые не удалились после пунка 1 так же переносятся
            nka.lastStates.AddRange(enka.lastStates.Intersect(nka.States));
            eNKA.print("noEps", nka);
            return nka;
        }

        //состояния в которые можно попасть из state используя сколько угодно пустых переходов
        public static List<string> StatesFromStateByEpsilon(string state, List<string> states, eNKA enka)
        {
            Arg arg = new Arg(state, "0"[0]);
            if (enka.Function.ContainsKey(arg))
            {
                states.AddRange(enka.Function[arg]);
                foreach (string st in enka.Function[arg])
                {
                    StatesFromStateByEpsilon(st, states, enka);
                }
            }
            return states;
        }

        //детерминизируем автомат
        public static DKA toDKA(eNKA nka)
        {
            //проверили если есть пустые переходы, удаляем их
            if (isENKA(nka))
            {
                nka = deleteEps(nka);
            }

            //новые символы
            List<char> InputCharsDKA = nka.InputChars;

            //новые состояния
            List<List<string>> StatesDKA = new List<List<string>>();
            StatesDKA.Add(nka.firstStates);

            //начальное состояние
            List<string> firstState = nka.firstStates;

            //функция переходов, ключ - множество состояний и символ перехода, значение множество состояний в которое можно попасть
            Dictionary<ArgDKA, List<string>> FunctionDKA = new Dictionary<ArgDKA, List<string>>();

            Queue<List<string>> quStates = new Queue<List<string>>();
            quStates.Enqueue(firstState);
            while (quStates.Count != 0)
            {
                List<string> tempState = quStates.Dequeue();
                foreach (char inpchar in nka.InputChars)
                {
                    List<string> tempStates = new List<string>();

                    foreach (string st in tempState)
                    {
                        try
                        {
                            foreach (string str in nka.Function[nka.Function.Keys.First(arg => arg.State == st && arg.Char == inpchar)])
                            {
                                if (!tempStates.Contains(str)) tempStates.Add(str);
                            }
                        }
                        catch(Exception) {  }
                    }

                    bool flag = true;
                    foreach (ArgDKA arg in FunctionDKA.Keys)
                    {
                        if (arg.State.SequenceEqual(tempState) && arg.Char == inpchar)
                        {
                            FunctionDKA[arg].Union(tempStates);
                            flag = false;
                        }
                    }
                    if (flag&&tempState.Count>0&&tempStates.Count>0) FunctionDKA.Add(new ArgDKA(tempState, inpchar), tempStates);
                    //if (flag) FunctionDKA.Add(new ArgDKA(tempState, inpchar), tempStates);
                    flag = true;
                    foreach (List<string> e in StatesDKA)
                    {
                        if (e.SequenceEqual(tempStates)) flag = false;
                    }
                    if (flag)
                    {
                        quStates.Enqueue(tempStates);
                        StatesDKA.Add(tempStates);
                    }
                }
            }

            DKA dka = new DKA();
            //переписали полученные символы
            dka.InputChars = nka.InputChars;
            //переписали начальное состояние
            foreach (string state in nka.firstStates)
            {
                dka.firstState += state;
            }
            //переписали символы
            dka.InputChars = InputCharsDKA;

            //переписали состояния
            foreach (List<string> states in StatesDKA)
            {
                string temp = string.Empty;
                foreach (string state in states)
                {
                    temp += state;
                }
                dka.States.Add(temp);
            }

            //переписали функции
            foreach (ArgDKA arg in FunctionDKA.Keys)
            {
                string statefrom = string.Empty;
                string stateto = string.Empty;
                foreach (string state in arg.State)
                {
                    statefrom += state;
                }
                foreach (string state1 in FunctionDKA[arg])
                {
                    stateto += state1;
                }
                dka.Function.Add(new Arg(statefrom, arg.Char), stateto);
            }

            //переписали заключительные состояния
            foreach (List<string> state in StatesDKA)
            {
                bool last = false;
                string lastState = string.Empty;
                foreach (string _state in state)
                {
                    if (nka.lastStates.Contains(_state)) last = true;
                }
                if (last)
                {
                    foreach (string _state in state)
                    {
                        lastState += _state;
                    }
                }
                dka.lastStates.Add(lastState);
            }
            return dka;
        }

        //проверка на пустые переходы
        public static bool isENKA(eNKA nka)
        {
            if (nka.Function.Keys.Where(arg => arg.Char == "0"[0]).Count() > 0) return true;
            else return false;
        }
    }
}
