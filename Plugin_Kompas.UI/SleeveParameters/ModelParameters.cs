using System.Collections.Generic;

namespace SleeveParameters
{
    /// <summary>
    /// Класс хранить словарь параметров модели 
    /// и реализует методы перерасчета значений 
    /// параметров
    /// </summary>
    public class ModelParameters
    {
        /// <summary>
        /// Поле хранит словарь параметров модели
        /// </summary>
        private Dictionary<ParametersName, Parameter> _parameters = 
            new Dictionary<ParametersName, Parameter>();

        /// <summary>
        /// Поле хранит тип резьбы 
        /// маленького кольца
        /// </summary>
        private ThreadType _threadType;

        /// <summary>
        /// Возвращает текущий
        /// тип резьбы 
        /// маленького кольца
        /// </summary>
        /// <returns></returns>
        public ThreadType ThreadTypeValue()
        {
            return _threadType;
        }

        /// <summary>
        /// Устанавливает текущий 
        /// вид резьбы
        /// маленького кольца
        /// </summary>
        public void InstallThreadType(ThreadType type)
        {
            _threadType = type;
        }

        /// <summary>
        /// Возвращает параметер 
        /// в соответствии с заданным именем
        /// </summary>
        /// <param name="name">Название параметра</param>
        /// <returns>Параметер</returns>
        public Parameter Parameter(ParametersName name)
        {
            return _parameters[name];
        }

        /// <summary>
        /// Расчет длины перемычек
        /// </summary>
        public void CalculationJumperLenght()
        {
            Parameter(ParametersName.JumperLenght).Value =
                (Parameter(ParametersName.MiddleRingDiameter1).Value
                - Parameter(ParametersName.CentralRingDiameter2).Value) / 2;
        }

        /// <summary>
        /// Расчет диаметра большого кольца
        /// </summary>
        public void CalculationOuterRingDiameter1()
        {
            Parameter(ParametersName.OuterRingDiameter1).Value =
                Parameter(ParametersName.MiddleRingDiameter2).Value;
        }

        //Расчет диаметра среднего кольца
        public void CalculationMiddleRingDiameter2()
        {
            Parameter(ParametersName.MiddleRingDiameter2).Value =
                Parameter(ParametersName.OuterRingDiameter1).Value;
        }

        /// <summary>
        /// Расчет высот 
        /// </summary>
        public void CalculationHeightSleeve() 
        {
            var height = Parameter(ParametersName.SleeveHeight).Value;
            Parameter(ParametersName.OuterRingHeight).MaxValue = height;
            Parameter(ParametersName.MiddleRingHeight).MaxValue = height;
            Parameter(ParametersName.CentralRingHeight).MaxValue = height;
            if (Parameter(ParametersName.OuterRingHeight).Value > height)
            {
                Parameter(ParametersName.OuterRingHeight).Value = height;
            }

            if (Parameter(ParametersName.MiddleRingHeight).Value > height)
            {
                Parameter(ParametersName.MiddleRingHeight).Value = height;
            }

            if (Parameter(ParametersName.CentralRingHeight).Value > height)
            {
                Parameter(ParametersName.CentralRingHeight).Value = height;
            }
        }

        /// <summary>
        /// Расчет диаметров большого и маленького
        /// колец
        /// </summary>
        public void CalculationDiameterMidelAndCenter()
        {
            var Diameter1 = 
                Parameter(ParametersName.MiddleRingDiameter1).Value / 2;
            var maxDiameter1 = 
                Parameter(ParametersName.MiddleRingDiameter1).MaxValue / 2;
            var minDiameter1 = 
                Parameter(ParametersName.MiddleRingDiameter1).MinValue / 2;
            var Diameter2 = 
                Parameter(ParametersName.CentralRingDiameter2).Value / 2;
            var minDiameter2 = 
                Parameter(ParametersName.CentralRingDiameter2).MinValue / 2;
            var maxDiameter2 = 
                Parameter(ParametersName.CentralRingDiameter2).MaxValue / 2;
            var Lenght = Parameter(ParametersName.JumperLenght).Value;
            if(Lenght < (maxDiameter1 - minDiameter2 - (maxDiameter1 - minDiameter1)))
            {
                Diameter1 = minDiameter1;
                Diameter2 = minDiameter2 + 
                    ((maxDiameter1 - minDiameter2) - (maxDiameter1 - minDiameter1) - Lenght);
            }
            else
            {
                Diameter1 = maxDiameter1 - ((maxDiameter1 - minDiameter2) - Lenght);
                Diameter2 = minDiameter2;
            }
            Parameter(ParametersName.MiddleRingDiameter1).Value = Diameter1 * 2;
            Parameter(ParametersName.CentralRingDiameter2).Value = Diameter2 * 2;
        }

        /// <summary>
        /// Конструктор класса ModelParameters
        /// </summary>
        public ModelParameters()
        {
            _parameters = new Dictionary<ParametersName, Parameter>();
            var values = new List<(double min, double max, ParametersName name)>
            {
                (50, 60, ParametersName.CentralRingDiameter1),
                (80, 90, ParametersName.CentralRingDiameter2),
                (10, 35, ParametersName.CentralRingHeight),
                (6, 10, ParametersName.JumperDiameter),
                (45, 55, ParametersName.JumperLenght),
                (3, 6, ParametersName.JumperNumber),
                (180, 190, ParametersName.MiddleRingDiameter1),
                (195, 220, ParametersName.MiddleRingDiameter2),
                (10, 35, ParametersName.MiddleRingHeight),
                (195, 220, ParametersName.OuterRingDiameter1),
                (230, 260, ParametersName.OuterRingDiameter2),
                (10, 35, ParametersName.OuterRingHeight),
                (10, 35, ParametersName.SleeveHeight)
            };
            foreach (var value in values)
            {
                Parameter parameter = null;
                if (value.name == ParametersName.MiddleRingDiameter1
                    || value.name == ParametersName.JumperLenght
                    || value.name == ParametersName.SleeveHeight)
                {
                    parameter = 
                        new Parameter(value.min, value.max, value.max, value.name.ToString());
                }
                else
                {
                    parameter = 
                        new Parameter(value.min, value.max, value.min, value.name.ToString());
                }
                _parameters.Add(value.name,parameter);
                _threadType = ThreadType.NoneThread;
            }
        }   
    }
}
