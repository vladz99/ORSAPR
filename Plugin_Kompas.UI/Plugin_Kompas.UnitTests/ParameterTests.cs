using NUnit.Framework;
using SleeveParameters;
using System;

namespace Plugin_Kompas.UnitTests
{
    public class ParameterTests
    {
        private Parameter _parameter;
        [SetUp]
        public void SetParameter()
        {
            _parameter = new Parameter(10, 30, 10, "nameParameter");
        }

        [Test(Description = "Позитивный тест геттера MaxValue")]
        public void Test_MaxValue_Get_CorrectValue()
        {
            var expected = 30;
            var actual = _parameter.MaxValue;
            Assert.AreEqual(expected, actual, 
                "Геттер MaxValue некорректно возвращает значение");
        }

        [Test(Description = "Позитивный тест сеттера MaxValue")]
        public void Test_MaxValue_Set_CorrectValue()
        {
            var expected = 20;
            _parameter.MaxValue = expected;
            Assert.AreEqual(expected, _parameter.MaxValue, 
                "Сеттер MaxValue некорректно записывает значение");
        }

        [Test(Description = "Позитивный тест сеттера MinValue")]
        public void Test_MinValue_Set_CorrectValue()
        {
            var expected = 10;
            Assert.AreEqual(expected, _parameter.MinValue, 
                "Сеттер MinValue некорректно записывает значение");
        }

        [Test(Description = "Позитивный тест геттера MinValue")]
        public void Test_MinValue_Get_CorrectValue()
        {
            var expected = 20;
            _parameter.MinValue = expected;
            var actual = _parameter.MinValue;
            Assert.AreEqual(expected, actual, 
                "Геттер MinValue некорректно возвращает значение");
        }

        [Test(Description = "Позитивный тест сеттера Value")]
        public void Test_Value_Set_CorrectValue()
        {
            var expected = 20;
            _parameter.Value = expected;
            Assert.AreEqual(expected, _parameter.Value, 
                "Сеттер Value некорректно записывает значение");
        }

        [Test(Description = "Позитивный тест геттера Value")]
        public void Test_Value_Get_CorrectValue()
        {
            var expected = 10;
            var actual = _parameter.Value;
            Assert.AreEqual(expected, actual, 
                "Геттер Value некорректно возвращает значение");
        }

        [Test(Description = "Тест конструктора Parameter")]
        public void Test_Parameter_Designer()
        {
            string messege = "";
            var result = true;
            if (_parameter.MinValue != 10)
            {
                result = false;
                messege = "Ошибка при создании " +
                    "MinValue";
            }

            if (_parameter.MaxValue != 30)
            {
                result = false;
                messege = "Ошибка при создании" +
                    "MaxValue";
            }

            if (_parameter.Value != 10)
            {
                result = false;
                messege = "Ошибка при создании " +
                    "Value ";
            }
            Assert.IsTrue(result, messege);
        }

        [TestCase("-500", 
            "Должно возникать исключение если, записываемое значение меньше минимиального",
           TestName = "Присвоение значения меньше минимального")]
        [TestCase("200", 
            "Должно возникать исключение если, записываемое значение больше максимального",
           TestName = "Присвоение значения больше максимального")]
        public void TestLastModTimeSet_ArgumentException(string wrongLastModTime, string messege)
        {
            Assert.Throws<ArgumentException>(() => 
            { _parameter.Value = double.Parse(wrongLastModTime); }, messege);
        }
    }
}