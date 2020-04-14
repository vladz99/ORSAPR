using System;
using Kompas6API5;
using SleeveParameters;
using System.Runtime.InteropServices;

namespace BuilderSleeve
{
    /// <summary>
    /// Класс подключает экземпляры построителя модели к САПР Компас 3D и инициализирует их
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// Поле хранит экземпляр построителя 3D модели
        /// </summary>
        private BuilderModel _builderModel;

        /// <summary>
        /// Подключение к экземпляру компас 3D
        /// </summary>
        /// <returns>Экземпляр Kompas</returns>
        private KompasObject OpenKompas3D()
        {
            KompasObject kompas = null;
            try
            {
                kompas = (KompasObject)Marshal.GetActiveObject("KOMPAS.Application.5");
                kompas.Visible = true;
            }
            catch
            {
                Type t = Type.GetTypeFromProgID("KOMPAS.Application.5");
                kompas = (KompasObject)Activator.CreateInstance(t);
                kompas.Visible = true;
            }
            finally
            {
                kompas.ActivateControllerAPI();
            }
            return kompas;
        }
        
        /// <summary>
        /// Конструктор класса Manager вызывает метод для инициализации экземпляра построителя 3D модели 
        /// </summary>
        /// <param name="parameters">Параметры модели</param>
        public Manager(ModelParameters parameters)
        {
            InirializeModel(parameters);
        }

        /// <summary>
        /// Метод создает экземпляр класса построителя модели
        /// </summary>
        /// <param name="parameters">Параметры модели</param>
        private void InirializeModel(ModelParameters parameters)
        {
            _builderModel = 
                new BuilderModel(parameters,OpenKompas3D());
        }
    }
}
