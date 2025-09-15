using System;
using System.Globalization;

namespace ClassicCalculatorSimple
{
    class Program
    {
        private static double lastAnswer = 0.0;
        private static double memory = 0.0;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Калькулятор ===");
            PrintHelp();

            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (line == null) break;

                line = line.Trim();
                if (line.Length == 0) continue;

                string low = line.ToLowerInvariant();
                if (low == ":q" || low == ":quit" || low == ":exit" || low == "q")
                    break;

                try
                {
                    if (TryHandleMemory(line))
                        continue;

                    double result = EvaluateSingleOperation(line);
                    lastAnswer = result;
                    Console.WriteLine("= " + result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Поддержка операций:");
            Console.WriteLine("Бинарные: a + b   a - b   a * b   a / b   a % b");
            Console.WriteLine("Унарные: sqrt x   sqr x   inv x");
            Console.WriteLine("Память: M+ [x]   M- [x]   MR   MC");
            Console.WriteLine("Переменные: ans последний результат,  mem значение в памяти");
            Console.WriteLine("Выход: q");
            Console.WriteLine();
        }


        private static bool TryHandleMemory(string line)
        {
            //   MR показать память и записать её в ans
            //   M+ [x] прибавить x к mem (если x опущен - прибавляем ans)
            //   M- [x] вычесть x из mem (если x опущен - вычитаем ans)
            //   MC очистить память
            string[] parts = SplitTokens(line);
            if (parts.Length == 0) return false;

            string cmd = parts[0].ToUpperInvariant();

            if (cmd == "MR")
            {
                Console.WriteLine("MR -> " + memory);
                lastAnswer = memory;
                Console.WriteLine("ans = " + lastAnswer);
                return true;
            }

            if (cmd == "MC")
            {
                memory = 0.0;
                Console.WriteLine("Память очищена (mem = 0)");
                return true;
            }

            if (cmd == "M+" || cmd == "M-")
            {
                double delta;
                if (parts.Length == 1)
                {
                    delta = lastAnswer;
                }
                else
                {
                    delta = ParseOperand(parts[1]);
                }

                if (cmd == "M-") delta = -delta;
                memory += delta;
                Console.WriteLine("mem = " + memory);
                return true;
            }

            return false;
        }


        private static double EvaluateSingleOperation(string line)
        {
            string[] parts = SplitTokens(line);

            if (parts.Length == 2)
            {
                string op = parts[0];
                double x = ParseOperand(parts[1]);
                return EvalUnary(op, x);
            }

            if (parts.Length == 3)
            {
                double a = ParseOperand(parts[0]);
                string op = parts[1];
                double b = ParseOperand(parts[2]);
                return EvalBinary(op, a, b);
            }

            throw new Exception("Допустима только одна операция.");
        }

        private static string[] SplitTokens(string s)
        {
            return s.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static double ParseOperand(string token)
        {
            string t = token.Trim();

            if (t == "ans") return lastAnswer;
            if (t == "mem") return memory;

            double val;
            bool ok = double.TryParse(
                t,
                NumberStyles.Float | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out val
            );

            if (ok) return val;

            throw new Exception("Не удалось распознать число или переменную: '" + token + "'");
        }

        private static double EvalBinary(string op, double a, double b)
        {
            if (op == "+") return a + b;
            if (op == "-") return a - b;
            if (op == "*") return a * b;

            if (op == "/")
            {
                if (b == 0.0) throw new Exception("Делить на ноль нельзя");
                return a / b;
            }

            if (op == "%")
            {
                if (b == 0.0) throw new Exception("Остаток от деления на ноль невозможен");
                return a % b;
            }

            throw new Exception("Неизвестный оператор: '" + op + "'");
        }

        private static double EvalUnary(string op, double x)
        {
            if (string.Equals(op, "sqrt", StringComparison.OrdinalIgnoreCase))
            {
                if (x < 0.0) throw new Exception("Квадратный корень из отрицательного числа невозможен");
                return Math.Sqrt(x);
            }

            if (string.Equals(op, "sqr", StringComparison.OrdinalIgnoreCase))
            {
                return x * x;
            }

            if (string.Equals(op, "inv", StringComparison.OrdinalIgnoreCase))
            {
                if (x == 0.0) throw new Exception("Делить на ноль нельзя");
                return 1.0 / x;
            }

            throw new Exception("Неизвестная операция: '" + op + "'");
        }
    }
}
