using NUnit.Framework;
using SleeveParameters;
using System.Collections.Generic;

namespace Plugin_Kompas.UnitTests
{
    public class ModelParametersTests
    {
        private ModelParameters _modelParameters;
        [SetUp]
        public void SetParameter()
        {
            _modelParameters = new ModelParameters();
        }

        [Test(Description = "Позитивный тест метода Parameter")]
        public void Test_Parameter()
        {
            var result = true;
            var message = "";
            if(_modelParameters.Parameter(ParametersName.CentralRingDiameter1).MinValue != 50)
            {
                result = false;
                message = "Ошибка при создании минимального значения внешнего диаметра " +
                    "центрального кольца";
            }
            if (_modelParameters.Parameter(ParametersName.CentralRingDiameter1).MaxValue != 60)
            {
                result = false;
                message = "Ошибка при создании максимального значения внешнего диаметра " +
                   "центрального кольца";
            }
            if (_modelParameters.Parameter(ParametersName.CentralRingDiameter1).Value != 50)
            {
                result = false;
                message = "Ошибка при создании текущего значения внешнего диаметра " +
                   "центрального кольца";
            }
            Assert.IsTrue(result, message);
        }

        [Test(Description = "Позитивный тест метода CalculationOuterRingDiameter1")]
        public void Test_CalculationOuterRingD1()
        {
            var expected = 200;
            _modelParameters.Parameter(ParametersName.MiddleRingDiameter2).Value = 200;
            _modelParameters.CalculationOuterRingDiameter1();
            var actual = _modelParameters.Parameter(ParametersName.OuterRingDiameter1).Value;
            Assert.AreEqual(expected, actual, "Метод CalculationOuterRingD1 работает некорректно");
        }

        [Test(Description = "Позитивный тест метода CalculationMiddleRingDiameter2")]
        public void Test_CalculationMiddleRingD2()
        {
            var expected = 200;
            _modelParameters.Parameter(ParametersName.OuterRingDiameter1).Value = 200;
            _modelParameters.CalculationMiddleRingDiameter2();
            var actual = _modelParameters.Parameter(ParametersName.OuterRingDiameter1).Value;
            Assert.AreEqual(expected, actual, "Метод CalculationMiddleRingD2 работает некорректно");
        }

        [Test(Description = "Позитивный тест метода CalculationCalculationJumperLenght")]
        public void Test_CalculationJumperLenght()
        {
            var expected = 45;
            _modelParameters.Parameter(ParametersName.MiddleRingDiameter1).Value = 180;
            _modelParameters.Parameter(ParametersName.CentralRingDiameter2).Value = 90;
            _modelParameters.CalculationJumperLenght();
            var actual = _modelParameters.Parameter(ParametersName.JumperLenght).Value;
            Assert.AreEqual(expected, actual, "Метод CalculationCalculationJumperL работает некорректно");
        }

        [TestCase("10",
           TestName = "Присвоение значения меньше максимального")]
        [TestCase("35",
           TestName = "Присвоение значения больше максимального")]
        [Test(Description = "Позитивный тест метода CalculationHeightSleeve")]
        public void Test_CalculationHeightSleeve(double newHeight)
        {
            _modelParameters.Parameter(ParametersName.CentralRingHeight).Value = newHeight;
            _modelParameters.Parameter(ParametersName.MiddleRingHeight).Value = newHeight;
            _modelParameters.Parameter(ParametersName.OuterRingHeight).Value = newHeight;
            var message = "";
            var result = true;
            var height = 20;
            _modelParameters.Parameter(ParametersName.SleeveHeight).Value = height;
            if (newHeight > height)
            {
                newHeight = height; 
            }
            _modelParameters.CalculationHeightSleeve();
            if (_modelParameters.Parameter(ParametersName.OuterRingHeight).MaxValue != height)
            {
                result = false;
                message = "Ошибка при присвоении максимального значения " +
                    "высоты большого кольца";
            }
            if(_modelParameters.Parameter(ParametersName.MiddleRingHeight).MaxValue != height)
            {
                result = false;
                message = "Ошибка при присвоении максимального значения " +
                    "высоты среднего кольца";
            }
            if(_modelParameters.Parameter(ParametersName.CentralRingHeight).MaxValue != height)
            {
                result = false;
                message = "Ошибка при присвоении максимального значения " +
                    "высоты маленького кольца";
            }

            if (_modelParameters.Parameter(ParametersName.OuterRingHeight).Value != newHeight)
            {
                result = false;
                message = "Ошибка при присвоении значения " +
                    "высоты большого кольца";
            }
            if (_modelParameters.Parameter(ParametersName.MiddleRingHeight).Value != newHeight)
            {
                result = false;
                message = "Ошибка при присвоении значения " +
                    "высоты среднего кольца";
            }
            if (_modelParameters.Parameter(ParametersName.CentralRingHeight).Value != newHeight)
            {
                result = false;
                message = "Ошибка при присвоении значения " +
                    "высоты маленького кольца";
            }
            Assert.IsTrue(result, message);
        }

        [TestCase(45, 180, 90,
           TestName = "Длина перемычек меньше расстояния")]
        [TestCase(55, 190, 80,
           TestName = "Длина перемычек больше расстояния")]
        [Test(Description = "Позитивный тест метода CalculationDMidelAndCenter")]
        public void Test_CalculationDMidelAndCenter(double lenght, double diameter1, double diameter2)
        {
            var message = "";
            var result = true;
            _modelParameters.Parameter(ParametersName.JumperLenght).Value = lenght;
            _modelParameters.CalculationDiameterMidelAndCenter();
            if (_modelParameters.Parameter(ParametersName.MiddleRingDiameter1).Value != diameter1)
            {
                result = false;
                message = "Ошибка при расчете внутреннего диаметра " +
                    "среднего кольца";
            }
            if (_modelParameters.Parameter(ParametersName.CentralRingDiameter2).Value != diameter2)
            {
                result = false;
                message = "Ошибка при расчете внешнего диаметра " +
                    "маленького кольца" + _modelParameters.Parameter(ParametersName.CentralRingDiameter2).Value;
            }
            Assert.IsTrue(result, message);
        }


        [Test(Description = "Позитивный тест метода перечисления ToString")]
        public void Test_ToString()
        {
            var expected = "CentralRingDiameter1";
            var actual = ParametersName.CentralRingDiameter1.ToString();
            Assert.AreEqual(expected, actual, "Метод перечисления ToString работает некорректно");
        }

        [Test(Description = "Позитивный тест конструктора ModelParameters")]
        public void Test_ModelParameters()
        {
            var modelParameters = new ModelParameters();
            var result = true;
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
                if(value.name == ParametersName.MiddleRingDiameter1
                    || value.name == ParametersName.JumperLenght
                    || value.name == ParametersName.SleeveHeight)
                {
                    if (modelParameters.Parameter(value.name).MaxValue != value.max ||
                    modelParameters.Parameter(value.name).MinValue != value.min ||
                    modelParameters.Parameter(value.name).Value != value.max)
                    {
                        result = false;
                    }
                }
                else
                {
                    if (modelParameters.Parameter(value.name).MaxValue != value.max ||
                    modelParameters.Parameter(value.name).MinValue != value.min ||
                    modelParameters.Parameter(value.name).Value != value.min)
                    {
                        result = false;
                    }
                }
            }
            Assert.IsTrue(result, "Конструктор ModelParameters не создает корректный экземпляр класса");
        }
    }
}
