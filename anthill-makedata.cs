// ============================
// Подготовка тестовых данных (для публикации "Учёт в муравейнике")
// Для получения рабочей программы в Visual Studio надо создать консольный проект, заменить файл program.cs на anthill-makedata.cs и собрать проект
// ============================
// Разработчик: apelserg ; https://github.com/apelserg/
// Лицензия: WTFPL
// ============================

using System;
using System.Text;
using System.IO;

namespace anthill
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileAntList = "anthill-ant-list.csv"; // список муравьёв
            string fileAntType = "anthill-ant-type.csv"; // типы муравьёв
            string fileCellList = "anthill-cell-list.csv"; // список домов
            string fileHillType = "anthill-hill-type.csv"; // части (районы) муравейника
            string fileAntToCell = "anthill-ant-to-cell.csv"; // расселение муравёв по домам
            string fileLog = "anthill-makedata.log"; // журнал

            int cntAnts = 1000; // 1000 муравьёв - по дефолту
            int cntBuff = 1000000; // размер буфера для записи в файл

            Random random = new Random();
            StringBuilder sb = new StringBuilder();

            ANTHILL_RecreateFile(fileLog); // пересоздать/очистить журнал

            // Проверка параметров
            //
            if (args.Length == 1)
            {
                if (Int32.TryParse(args[0], out cntAnts))
                {
                    if (cntAnts < 1000 || cntAnts > 1000000000)
                    {
                        ANTHILL_WriteLog(fileLog, String.Format("Значение [{0}] не корректно. Должно быть от 1000 до 1000000000", cntAnts));
                        return;
                    }
                }
                else
                {
                    ANTHILL_WriteLog(fileLog, String.Format("Ошибка в параметре: {0}", args[0]));
                    return;
                }
            }
            if (args.Length > 1)
            {
                ANTHILL_WriteLog(fileLog, String.Format("Ошибка в количестве параметров: {0}", args.Length));
                return;
            }

            // Старт
            //
            ANTHILL_WriteLog(fileLog, "===========================");
            ANTHILL_WriteLog(fileLog, "== Начало подготовки тестовых данных");
            ANTHILL_WriteLog(fileLog, "===========================");
            ANTHILL_WriteLog(fileLog, String.Format("Задано: {0}", cntAnts));

            try
            {
                // Создать anthill-ant-type.csv
                //
                ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileAntType));

                ANTHILL_RecreateFile(fileAntType); // пересоздать/очистить файл

                sb.Clear();
                sb.Append("1\tЦарица").Append(Environment.NewLine);
                sb.Append("2\tЛичинка").Append(Environment.NewLine);
                sb.Append("3\tНянька").Append(Environment.NewLine);
                sb.Append("4\tРабочий").Append(Environment.NewLine);
                sb.Append("5\tСолдат").Append(Environment.NewLine);

                ANTHILL_WriteFile(fileAntType, sb.ToString());

                // Создать anthill-hill-type.csv
                //
                ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileHillType));

                ANTHILL_RecreateFile(fileHillType); // пересоздать/очистить файл

                sb.Clear();
                sb.Append("1\tЦентр").Append(Environment.NewLine);
                sb.Append("2\tВерх").Append(Environment.NewLine);
                sb.Append("3\tНиз").Append(Environment.NewLine);
                sb.Append("4\tСевер").Append(Environment.NewLine);
                sb.Append("5\tЮг").Append(Environment.NewLine);
                sb.Append("6\tВосток").Append(Environment.NewLine);
                sb.Append("7\tЗапад").Append(Environment.NewLine);

                ANTHILL_WriteFile(fileHillType, sb.ToString());

                // Создать anthill-ant-list.csv
                //
                ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileAntList));

                ANTHILL_RecreateFile(fileAntList); // пересоздать/очистить файл

                sb.Clear();
                sb.Append("1\t1").Append(Environment.NewLine); // царица - одна на весь муравейник
                for (int n = 2; n <= cntAnts; n++)
                {
                    sb.Append(n.ToString()).Append("\t").Append(random.Next(2, 6).ToString()).Append(Environment.NewLine);

                    // записать в файл
                    if (n % cntBuff == 0)
                    {
                        ANTHILL_WriteFile(fileAntList, sb.ToString());
                        sb.Clear();
                        ANTHILL_WriteLog(fileLog, String.Format("Создано: {0}", n));
                    }
                }

                // дописать в файл
                if (sb.Length > 0)
                    ANTHILL_WriteFile(fileAntList, sb.ToString());

                // Создать anthill-cell-list.csv
                //
                ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileCellList));

                ANTHILL_RecreateFile(fileCellList); // пересоздать/очистить файл

                sb.Clear();
                sb.Append("1\t1").Append(Environment.NewLine);
                for (int n = 2; n <= cntAnts; n++)
                {
                    sb.Append(n.ToString()).Append("\t").Append(random.Next(1, 8).ToString()).Append(Environment.NewLine);

                    // записать в файл
                    if (n % cntBuff == 0)
                    {
                        ANTHILL_WriteFile(fileCellList, sb.ToString());
                        sb.Clear();
                        ANTHILL_WriteLog(fileLog, String.Format("Создано: {0}", n));
                    }
                }

                // дописать в файл
                if (sb.Length > 0)
                    ANTHILL_WriteFile(fileCellList, sb.ToString());

                // Создать anthill-ant-to-cell.csv
                //
                ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileAntToCell));

                ANTHILL_RecreateFile(fileAntToCell); // пересоздать/очистить файл

                sb.Clear();
                for (int n = 1, c = cntAnts; n <= cntAnts; n++, c--)
                {
                    sb.Append(n.ToString()).Append("\t").Append(c.ToString()).Append(Environment.NewLine);

                    // записать в файл
                    if (n % cntBuff == 0)
                    {
                        ANTHILL_WriteFile(fileAntToCell, sb.ToString());
                        sb.Clear();
                        ANTHILL_WriteLog(fileLog, String.Format("Создано: {0}", n));
                    }
                }

                // дописать в файл
                if (sb.Length > 0)
                    ANTHILL_WriteFile(fileAntToCell, sb.ToString());
            }
            catch (Exception ex)
            {
                ANTHILL_WriteLog(fileLog, String.Format("Ошибка: [{0}]", ex.Message));
                return;
            }

            // Завершение
            //
            ANTHILL_WriteLog(fileLog, String.Format("Итого: {0}", cntAnts));
            ANTHILL_WriteLog(fileLog, "===========================");
            ANTHILL_WriteLog(fileLog, "== Подготовка тестовых данных завершена");
            ANTHILL_WriteLog(fileLog, "===========================");
        }
        //========
        // Переоткрыть файл
        //========
        private static void ANTHILL_RecreateFile(string fileFullName)
        {
            if (File.Exists(fileFullName))
                File.Delete(fileFullName);

            using(File.Create(fileFullName));
        }
        //========
        // Записать в файл
        //========
        private static void ANTHILL_WriteFile(string fileFullName, string strX)
        {
            strX = strX.TrimEnd(new char[] { '\n', '\r' }); 

            if (File.Exists(fileFullName))
                using (StreamWriter sw = File.AppendText(fileFullName))
                    sw.WriteLine(strX);
            else
                using (StreamWriter sw = File.CreateText(fileFullName))
                    sw.WriteLine(strX);
        }
        //========
        // Записать в журнал
        //========
        private static void ANTHILL_WriteLog(string fileLog, string strLog)
        {
            strLog = "[" + DateTime.Now.ToString(@"hh\:mm\:ss") + "] " + strLog;
            Console.WriteLine(strLog);
            ANTHILL_WriteFile(fileLog, strLog);
        }
    }
}
