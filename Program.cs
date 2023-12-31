using System;

namespace Logger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Settings Log
            Log.Level = Log.EnumLevel.DEBUG; //Минимальный уровень отображения логов
            Log.WriteMode = Log.EnumWriteMode.CONSOLE_AND_FILE; //Режим записи логов
            Log.SeparateLogging = true; //Раздельное ведение журнала логов
            Log.NewFileForNewStartProgram = true; //Необходимость генерации новых имен файлов
                                                  //для каждого нового запуска программы

            //Write Log
            Log.Debug("Debug Hello");
            Log.Info("Info Hello");
            Log.Warn("Warn Hello");
            Log.Error("Error Hello");

            Console.ReadKey();
        }
    }
}
