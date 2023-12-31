using System;
using System.IO;
using System.Runtime.CompilerServices; // Для вызова [CallerFilePath], [CallerMemberName], [CallerLineNumber]

namespace Logger
{
    public static class Log
    {
        //Стандартное расположение журнала логов: Logger\bin\Debug


        /** Перечисление допустимых уровней логирования */
        public enum EnumLevel { TRACE, DEBUG, INFO, WARN, ERROR }

        /** Перечисление допустимых режимов записи логов 
         *{@value} CONSOLE - вывод ТОЛЬКО на консоль
         *{@value} FILE - вывод ТОЛЬКО в файл
         *{@value} CONSOLE_AND_FILE - вывод на консоль и файл
         */
        public enum EnumWriteMode { CONSOLE, FILE, CONSOLE_AND_FILE }

        //Fields with default settings
        /** Хранение имени файла. По умолчанию: название проекта 
         * Если SeprarateLogging == true, то к имени файла добавляется уровень лога:
         * (fileName == "Name" && SeprarateLogging == true) => Name_Trace.log
         */
        private static string _fileName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name; 
        
        /** Хранение расширения файла без возможности изменения*/
        private static readonly string FILE_EXTENSION = ".log";

        /** Хранит текущий уровень логирования. По умолчанию: TRACE*/
        private static EnumLevel _level = EnumLevel.TRACE;

        /** Хранение текущего режима записи логов. По умолчанию: CONSOLE */
        private static EnumWriteMode _writeMode = EnumWriteMode.CONSOLE;

        /** Хранение режима ведения журнала логов. По умолчанию: false 
         *{@value} true - для каждого уровня лога свой файл
         *{@value} false - все логи в одном файле
         */
        private static bool _separateLogging = false;

        /** Содержит сведения о необходимости генерации новых имен файлов при каждом новом запуске программы */
        private static bool _newFileForNewStartProgram = false;

        /** Хранит текущую дату в случае необходимости генерации новых 
         * файлов при каждом новом запуске программы
         */
        private static readonly string _dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

     //Properies
        /** Установление|Получение текущего уровня логирования*/
        public static EnumLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }

        /** Установление|Получение текущего режима записи логов*/
        public static EnumWriteMode WriteMode
        {
            get { return _writeMode; }
            set { _writeMode = value; }
        }

        /** Установление|Получение текущего имени файла ведения логов*/
        public static string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /** Установление|Получение режима раздельного ведения журнала логов согласно их уровню*/
        public static bool SeparateLogging {
            get { return _separateLogging; }
            set { _separateLogging = value; }
        }

        /** Установление|Получение сведений о необходимости генерации новых имен файлов для каждого нового запуска программы*/
        public static bool NewFileForNewStartProgram
        {
            get { return _newFileForNewStartProgram; }
            set { _newFileForNewStartProgram= value; }
        }

     //Methods
     // Процедуры инициации вызова записи логов.Созданы для удобства пользователя:
     // Log.WriteData(Log.EnumLevel.Trace, "Message") -> Log.Trace("Message")

        /**
            * Процедура инициации вызова записи логов
            * @param message - сообщение для вывода в лог
            * @param filePath - путь до файла программы, откуда совершен вызов. | Автоматическое заполнение.
            * @param member - Функция, откуда совершен вызов. | Автоматическое заполнение.
            * @param line - Строка программы, откуда совершен вызов. | Автоматическое заполнение.
            */
        public static void Trace(string message, [CallerFilePath] string filePath = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0)
        {
            WriteData(EnumLevel.TRACE, ref message, ref filePath, ref member, ref line);
        }

        public static void Debug(string message, [CallerFilePath] string filePath = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0)
        {
            WriteData(EnumLevel.DEBUG, ref message, ref filePath, ref member, ref line);
        }

        public static void Info(string message, [CallerFilePath] string filePath = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0)
        {
            WriteData(EnumLevel.INFO, ref message, ref filePath, ref member, ref line);
        }
        public static void Warn(string message, [CallerFilePath] string filePath = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0)
        {
            WriteData(EnumLevel.WARN, ref message, ref filePath, ref member, ref line);
        }
        public static void Error(string message, [CallerFilePath] string filePath = "",
                        [CallerMemberName] string member = "",
                        [CallerLineNumber] int line = 0)
        {
            WriteData(EnumLevel.ERROR, ref message, ref filePath, ref member, ref line);
        }


        /**
         * Процедура записи логов
         * @param levelMessage - уровень лога сообщения
         * @param message - ссылка на сообщение для вывода в лог
         * @param filePath - ссылка на путь до файла программы, откуда совершен вызов
         * @param member - ссылка на название функции, откуда совершен вызов
         * @param line - ссылка на номер строки программы, откуда совершен вызов
         */
        private static void WriteData(EnumLevel levelMesage, ref string message,
            ref string filePath, ref string member, ref int line)
        {
            //Вывод логов согласно минимальному уровню (Level)
            if (levelMesage < _level)
                return;

            //Формирование текста лога
            string text = $"[{DateTime.Now}] | {levelMesage} | {filePath}\\{member}:{line} -> {message}\n";

            //Формирование имени файла 
            string fileName = FileName;
            if (SeparateLogging) //в зависимости от режима ведения журнала
                fileName += "_" + levelMesage.ToString();
            if (NewFileForNewStartProgram) //в зависимости от необходимости генерации новых имен файла для каждого нового запуска
                fileName += "_" + _dateTimeNow;
            fileName += FILE_EXTENSION;

            //Запись лога согласно настроенному режиму записи (WriteMode)
            switch (_writeMode)
            {
                case EnumWriteMode.CONSOLE:
                    Console.Write(text); //Вывод на консоль
                    break;
                case EnumWriteMode.FILE:
                    File.AppendAllText(fileName, text); //Запись лога в файл
                    break;
                case EnumWriteMode.CONSOLE_AND_FILE:
                    Console.Write(text);
                    File.AppendAllText(fileName, text); //Запись лога в файл
                    break;
                default:
                    Console.Write("Неопределенный режим записи. " + text); //Вывод на консоль
                    break;
            }
        }
    }
}
