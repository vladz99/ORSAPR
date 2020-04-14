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

        [Test(Description = "���������� ���� ������� MaxValue")]
        public void Test_MaxValue_Get_CorrectValue()
        {
            var expected = 30;
            var actual = _parameter.MaxValue;
            Assert.AreEqual(expected, actual, 
                "������ MaxValue ����������� ���������� ��������");
        }

        [Test(Description = "���������� ���� ������� MaxValue")]
        public void Test_MaxValue_Set_CorrectValue()
        {
            var expected = 20;
            _parameter.MaxValue = expected;
            Assert.AreEqual(expected, _parameter.MaxValue, 
                "������ MaxValue ����������� ���������� ��������");
        }

        [Test(Description = "���������� ���� ������� MinValue")]
        public void Test_MinValue_Set_CorrectValue()
        {
            var expected = 10;
            Assert.AreEqual(expected, _parameter.MinValue, 
                "������ MinValue ����������� ���������� ��������");
        }

        [Test(Description = "���������� ���� ������� MinValue")]
        public void Test_MinValue_Get_CorrectValue()
        {
            var expected = 20;
            _parameter.MinValue = expected;
            var actual = _parameter.MinValue;
            Assert.AreEqual(expected, actual, 
                "������ MinValue ����������� ���������� ��������");
        }

        [Test(Description = "���������� ���� ������� Value")]
        public void Test_Value_Set_CorrectValue()
        {
            var expected = 20;
            _parameter.Value = expected;
            Assert.AreEqual(expected, _parameter.Value, 
                "������ Value ����������� ���������� ��������");
        }

        [Test(Description = "���������� ���� ������� Value")]
        public void Test_Value_Get_CorrectValue()
        {
            var expected = 10;
            var actual = _parameter.Value;
            Assert.AreEqual(expected, actual, 
                "������ Value ����������� ���������� ��������");
        }

        [Test(Description = "���� ������������ Parameter")]
        public void Test_Parameter_Designer()
        {
            string messege = "";
            var result = true;
            if (_parameter.MinValue != 10)
            {
                result = false;
                messege = "������ ��� �������� " +
                    "MinValue";
            }

            if (_parameter.MaxValue != 30)
            {
                result = false;
                messege = "������ ��� ��������" +
                    "MaxValue";
            }

            if (_parameter.Value != 10)
            {
                result = false;
                messege = "������ ��� �������� " +
                    "Value ";
            }
            Assert.IsTrue(result, messege);
        }

        [TestCase("-500", 
            "������ ��������� ���������� ����, ������������ �������� ������ �������������",
           TestName = "���������� �������� ������ ������������")]
        [TestCase("200", 
            "������ ��������� ���������� ����, ������������ �������� ������ �������������",
           TestName = "���������� �������� ������ �������������")]
        public void TestLastModTimeSet_ArgumentException(string wrongLastModTime, string messege)
        {
            Assert.Throws<ArgumentException>(() => 
            { _parameter.Value = double.Parse(wrongLastModTime); }, messege);
        }
    }
}