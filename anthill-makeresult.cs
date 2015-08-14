// ============================
// Подготовка файлов результата (для публикации "Учёт в муравейнике")
// Для получения рабочей программы в Visual Studio надо создать консольный проект, заменить файл program.cs на anthill-makeresult.cs и собрать проект
// Примечание: в свойствах проекта, в разделе BUILD, снять галочку [Prefer 32-bit]
// Запускать программу следует в той-же директории, где находятся файлы тестовых данных
//   anthill-ant-list.csv (список муравьёв)
//   anthill-ant-type.csv (типы муравьёв)
//   anthill-cell-list.csv (список домов)
//   anthill-hill-type.csv (части (районы) муравейника)
//   anthill-ant-to-cell.csv (расселение муравёв по ячейкам)
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
            string fileAntToCell = "anthill-ant-to-cell.csv"; // расселение муравёв по ячейкам
            string fileLog = "anthill-makeresult.log"; // журнал

            int cntAnts = 0; // количество муравьёв (от 1000 до 1000000000)
            int cntCells = 0; // количество домов (по количеству муравьёв - от 1000 до 1000000000)
            int cntAntToCells = 0; // по количеству муравьёв (от 1000 до 1000000000)
            int cntAntTypes = 0; // количество типов муравьёв (до 10, хотя в текущей реализации 5)
            int cntHillTypes = 0; // количество частей муравейника (до 10, хотя в текущей реализации 7)
            int cntBuff = 10000; // размер буфера для записи в файл (уменьшен до 10000, так как одновременно пишется несколько файлов)
            int cntLog = 1000000; // через сколько циклов показывать инфо в консоли
            int totalMax = 1000000000; // максимальное обрабатываемое количество (при нехватке памяти может быть уменьшено - соответственно должно быть уменьшено количество входных данных)
            int filtrAntType = 5; // дефолтный фильтр - солдат

            // 10 + 1 (по количеству частей муравейника - до 10 (в текущей реализации 7)
            // единица добавлена для естественной нумерации: номер строки = номеру индекса
            //
            int[] sbCntBuff = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            StringBuilder[] sb = { /* 0 */ new StringBuilder(), /* 1 */ new StringBuilder(), /* 2 */ new StringBuilder(), /* 3 */ new StringBuilder(), /* 4 */ new StringBuilder(), /* 5 */ new StringBuilder(), /* 6 */ new StringBuilder(), /* 7 */ new StringBuilder(), /* 8 */ new StringBuilder(), /* 9 */ new StringBuilder(), /* 10 */ new StringBuilder() };

            string[] antType = new string[10 + 1]; // массив типов муравьёв
            string[] hillType = new string[10 + 1]; // массив типов частей муравейника
            byte[] antList; // массив муравьёв
            byte[] cellList; // массив ячеек

            // пересоздать/очистить журнал
            //
            ANTHILL_RecreateFile(fileLog); 

            // Выделение памяти
            //
            try
            {
                antList = new byte[totalMax + 1]; // массив муравьёв
                cellList = new byte[totalMax + 1]; // массив домов
            }
            catch (Exception ex)
            {
                ANTHILL_WriteLog(fileLog, String.Format("Ошибка выделения памяти [{0}]", ex.Message));
                return;
            }

            // Проверка параметров
            //
            if (args.Length == 1)
            {
                if (Int32.TryParse(args[0], out filtrAntType))
                {
                    if (filtrAntType < 2 || filtrAntType > 5)
                    {
                        ANTHILL_WriteLog(fileLog, String.Format("Значение [{0}] не корректно. Должно быть от 2 до 5", filtrAntType));
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
            ANTHILL_WriteLog(fileLog, "== Начало обработки тестовых данных (формирование файлов результата)");
            ANTHILL_WriteLog(fileLog, "===========================");

            // считать файл anthill-ant-type.csv в массив
            //
            ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileAntType));

            try
            {
                using (StreamReader sReader = File.OpenText(fileAntType))
                {
                    while (!sReader.EndOfStream)
                    {
                        cntAntTypes++;
                        string[] strList = sReader.ReadLine().Split(new char[] { '\t' });
                        int idx = int.Parse(strList[0]);
                        antType[idx] = strList[1];
                    }
                }
            }
            catch (Exception ex)
            {
                ANTHILL_WriteLog(fileLog, String.Format("Ошибка в файле: {0}, строка {1} [{2}]", fileAntType, cntAntTypes, ex.Message));
                return;
            }

            // считать файл anthill-hill-type.csv в массив
            //
            ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileHillType));
            
            try
            {
                using (StreamReader sReader = File.OpenText(fileHillType))
                {
                    while (!sReader.EndOfStream)
                    {
                        cntHillTypes++;
                        string[] strList = sReader.ReadLine().Split(new char[] { '\t' });
                        int idx = int.Parse(strList[0]);
                        hillType[idx] = strList[1];
                    }
                }
            }
            catch (Exception ex)
            {
                ANTHILL_WriteLog(fileLog, String.Format("Ошибка в файле: {0}, строка {1} [{2}]", fileHillType, cntHillTypes, ex.Message));
                return;
            }

            // считать файл anthill-ant-list.csv в массив
            //
            ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileAntList));

            try
            {
                using (StreamReader sReader = File.OpenText(fileAntList))
                {
                    while (!sReader.EndOfStream)
                    {
                        cntAnts++;
                        string[] strList = sReader.ReadLine().Split(new char[] { '\t' });
                        int idx = int.Parse(strList[0]);
                        antList[idx] = byte.Parse(strList[1]);

                        if (cntAnts % cntLog == 0)
                            ANTHILL_WriteLog(fileLog, String.Format("Считано: {0}", cntAnts));
                    }
                }
            }
            catch (Exception ex)
            {
                ANTHILL_WriteLog(fileLog, String.Format("Ошибка в файле: {0}, строка {1} [{2}]", fileAntList, cntAnts, ex.Message));
                return;
            }

            // считать файл anthill-cell-list.csv в массив
            //
            ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileCellList));

            try
            {
                using (StreamReader sReader = File.OpenText(fileCellList))
                {
                    while (!sReader.EndOfStream)
                    {
                        cntCells++;
                        string[] strList = sReader.ReadLine().Split(new char[] { '\t' });
                        int idx = int.Parse(strList[0]);
                        cellList[idx] = byte.Parse(strList[1]);

                        if (cntCells % cntLog == 0)
                            ANTHILL_WriteLog(fileLog, String.Format("Считано: {0}", cntCells));
                    }
                }
            }
            catch (Exception ex)
            {
                ANTHILL_WriteLog(fileLog, String.Format("Ошибка в файле: {0}, строка {1} [{2}]", fileCellList, cntCells, ex.Message));
                return;
            }

            // считать файл anthill-ant-to-cell.csv и создать файлы результата
            //
            ANTHILL_WriteLog(fileLog, String.Format("Файл: {0}", fileAntToCell));

            try
            {
                // очистить/пересоздать файлы результата
                //
                foreach (string s in hillType)
                    if (!String.IsNullOrEmpty(s))
                        ANTHILL_RecreateFile(ANTHILL_GetFileName(s, antType[filtrAntType]));

                int currAnt, currCell;
                int currAntType, currHillType;
                string currAntTypeName, currHillTypeName;

                using (StreamReader sReader = File.OpenText(fileAntToCell))
                {
                    while (!sReader.EndOfStream)
                    {
                        // считать строку
                        //
                        cntAntToCells++;
                        string[] strList = sReader.ReadLine().Split(new char[] { '\t' });

                        currAnt = int.Parse(strList[0]);
                        currCell = int.Parse(strList[1]);
                        currAntType = antList[currAnt];
                        currHillType = cellList[currCell];
                        currAntTypeName = antType[currAntType];
                        currHillTypeName = hillType[currHillType];

                        if (cntAntToCells % cntLog == 0)
                            ANTHILL_WriteLog(fileLog, String.Format("Считано: {0}", cntAntToCells));

                        // записать результат
                        //
                        if (currAntType == filtrAntType)
                        {
                            sbCntBuff[currHillType]++;
                            sb[currHillType].Append(currAnt.ToString()).Append("\t").Append(currCell.ToString()).Append(Environment.NewLine);

                            if (sbCntBuff[currHillType] % cntBuff == 0)
                            {
                                string strFileName = ANTHILL_GetFileName(currHillTypeName, antType[filtrAntType]);

                                ANTHILL_WriteFile(strFileName, sb[currHillType].ToString());
                                sb[currHillType].Clear();

                                ANTHILL_WriteLog(fileLog, String.Format("Запись в файл: {0}, строк {1}", strFileName, sbCntBuff[currHillType]));
                            }
                        }

                    }

                    // дописать файлы
                    //
                    for (int n = 1; n < hillType.Length; n++)
                        if (!String.IsNullOrEmpty(hillType[n]))
                            if (sb[n].Length > 0)
                                ANTHILL_WriteFile(ANTHILL_GetFileName(hillType[n], antType[filtrAntType]), sb[n].ToString());
                }
            }
            catch (Exception ex)
            {
                ANTHILL_WriteLog(fileLog, String.Format("Ошибка в файле: {0}, строка {1} [{2}]", fileAntToCell, cntAntToCells, ex.Message));
                return;
            }

            // Завершение
            //
            ANTHILL_WriteLog(fileLog, String.Format("Обработано: {0}", cntAnts));
            ANTHILL_WriteLog(fileLog, "===========================");
            ANTHILL_WriteLog(fileLog, "== Тестовые данные обработаны (файлы с результатами готовы)");
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
        //========
        // Новое имя файла
        //========
        private static string ANTHILL_GetFileName(string strHillType, string strAntType)
        {
            return "anthill-" + strHillType + "-" + strAntType + ".csv";
        }
    }
}
