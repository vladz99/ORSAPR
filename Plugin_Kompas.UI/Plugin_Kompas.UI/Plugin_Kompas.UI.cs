using System.Collections.Generic;
using System.Windows.Forms;
using SleeveParameters;
using System;
using System.Drawing;
using BuilderSleeve;

namespace Plugin_Kompas.UI
{
    public partial class Plugin_Form : Form
    {
        /// <summary>
        /// Поле хранит все параметры модели
        /// </summary>
        private ModelParameters _modelParameters = 
            new ModelParameters();

        /// <summary>
        /// Хранит словарь соответствий TextBox и параметров моделей
        /// </summary>
        private Dictionary<TextBox, ParametersName> _formElements =
            new Dictionary<TextBox, ParametersName>();

        public Plugin_Form()
        {
            //Инициализация формы
            InitializeComponent();
            //Создание списка элементов TextBox существующих на форме
            var elements = new List<(TextBox textBox, ParametersName parameter)>
                  {
                     (CentralD1TextBox, ParametersName.CentralRingDiameter1),
                     (CentralD2TextBox, ParametersName.CentralRingDiameter2),
                     (CentralHTextBox, ParametersName.CentralRingHeight),
                     (MiddleD1TextBox, ParametersName.MiddleRingDiameter1),
                     (MiddleD2TextBox, ParametersName.MiddleRingDiameter2),
                     (MiddleHTextBox, ParametersName.MiddleRingHeight),
                     (OuterD1TextBox,ParametersName.OuterRingDiameter1),
                     (OuterD2TextBox,ParametersName.OuterRingDiameter2),
                     (OuterHTextBox,ParametersName.OuterRingHeight),
                     (JumperDTextBox,ParametersName.JumperDiameter),
                     (JumperLTextBox,ParametersName.JumperLenght),
                     (JumperNTextBox,ParametersName.JumperNumber),
                     (CommonHTextBox,ParametersName.SleeveHeight)
                    };
            //Перебор всех элементов картежа
            foreach (var element in elements)
            {
                //Добавление параметра в словарь элементов TextBox формы
                _formElements.Add(element.textBox, element.parameter);
            }
        }

        /// <summary>
        /// Присваивает параметру значение 
        /// из соответствующего элемента TextBox
        /// при изменении пользователем значения Text
        /// для TextBox
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Действие</param>
        private void TextBoxChanged(object sender, EventArgs e)
        {
            //Преобразуем из object в TextBox
            var textBox = (TextBox)sender;
            //Блок ожидания ошибки
            try
            {
                textBox.BackColor = Color.Salmon;
                //Получаем текст из элемента TextBox
                var value = double.Parse(textBox.Text);
                //Определяем имя параметра соответствующего
                //данному TextBox
                var parameterName = _formElements[textBox];
                //Присваиваем значение найденному параметру
                _modelParameters.Parameter(parameterName).Value = value;
                //При изменении внешнего диаметра центрального кольца
                //или изменении внутреннего диаметра среднего кольца
                if (parameterName == ParametersName.MiddleRingDiameter1 
                    && JumperLTextBox.BackColor != Color.Salmon)
                {
                    _modelParameters.CalculationJumperLenght();
                    CentralD2TextBox.BackColor = Color.Salmon;
                    JumperLTextBox.Text = 
                        _modelParameters.Parameter(ParametersName.JumperLenght).Value.ToString();
                    CentralD2TextBox.BackColor = Color.LightGreen;
                }
                if (parameterName == ParametersName.CentralRingDiameter2 
                    && JumperLTextBox.BackColor != Color.Salmon)
                {
                    _modelParameters.CalculationJumperLenght();
                    MiddleD1TextBox.BackColor = Color.Salmon;
                    JumperLTextBox.Text = 
                        _modelParameters.Parameter(ParametersName.JumperLenght).Value.ToString();
                    MiddleD1TextBox.BackColor = Color.LightGreen;
                }
                //При изменении внешнего диаметра среднего кольца
                if (parameterName == ParametersName.MiddleRingDiameter2)
                {
                    _modelParameters.CalculationOuterRingDiameter1();
                    OuterD1TextBox.Text = 
                        _modelParameters.Parameter(ParametersName.OuterRingDiameter1).
                        Value.ToString();
                }
                //При изменении внутреннего диаметра внешнего кольца
                if (parameterName == ParametersName.OuterRingDiameter1)
                {
                    _modelParameters.CalculationMiddleRingDiameter2();
                    MiddleD2TextBox.Text = 
                        _modelParameters.Parameter(ParametersName.MiddleRingDiameter2).
                        Value.ToString();
                }
                //При изменении длины перемычек
                if (parameterName == ParametersName.JumperLenght)
                {
                    if(CentralD2TextBox.BackColor == Color.LightGreen 
                        && MiddleD1TextBox.BackColor == Color.LightGreen)
                    {
                        _modelParameters.CalculationDiameterMidelAndCenter();
                        MiddleD1TextBox.Text = 
                            _modelParameters.Parameter(ParametersName.MiddleRingDiameter1).
                            Value.ToString();
                        CentralD2TextBox.Text = 
                            _modelParameters.Parameter(ParametersName.CentralRingDiameter2).
                            Value.ToString();
                    }
                }
                //При изменении общей высоты втулки
                if (parameterName == ParametersName.SleeveHeight)
                {
                    _modelParameters.CalculationHeightSleeve();
                    MiddleHTextBox.Text = 
                        _modelParameters.Parameter(ParametersName.MiddleRingHeight).Value.ToString();
                    CentralHTextBox.Text = 
                        _modelParameters.Parameter(ParametersName.CentralRingHeight).Value.ToString();
                    OuterHTextBox.Text = 
                        _modelParameters.Parameter(ParametersName.OuterRingHeight).Value.ToString();
                    DisplayInterval(ParametersName.MiddleRingHeight, MiddleHLabel);
                    DisplayInterval(ParametersName.OuterRingHeight, OuterHLabel);
                    DisplayInterval(ParametersName.CentralRingHeight, CentralHLabel);
                }
                textBox.BackColor = Color.LightGreen;
            }
            //Выполняется в случае выявления ошибки в try
            catch
            {
                //Окрашиваем поле в красный цвет
                textBox.BackColor = Color.Salmon;
            }
        }

        /// <summary>
        /// Метод присваивает
        /// элементу TextBox последнее
        /// корректное значение
        /// если при потере фокуса 
        /// цвет элемента красный
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Действие</param>
        private void TextBoxLeave(object sender, EventArgs e)
        {
            //Преобразуем из object в TextBox
            var textBox = (TextBox)sender;
            //Если цвет поля красный
            if (textBox.BackColor == Color.Salmon)
            {
                //Определяем название параметра
                var name = _formElements[textBox];
                //Присваиваем значение в соответствующий параметру TextBox
                textBox.Text = _modelParameters.Parameter(name).Value.ToString();
            }
        }

        /// <summary>
        /// Методя для отображаения 
        /// интервала значений 
        /// параметра
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="label">Label соответствующий параметру</param>
        private void DisplayInterval(ParametersName name, Label label)
        {
            //Получаем параметер по имени
            var parameter = _modelParameters.Parameter(name);
            //Отображаем его интервал
            label.Text = "Интервал значений ( от " +
                        parameter.MinValue + " до " +
                        parameter.MaxValue + " ) мм";
        }

        /// <summary>
        /// Метод возвращает форму 
        /// к начальным параметра
        /// при нажатии на кнопку "Сбросить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            //Создание нового списка параметров
            _modelParameters = new ModelParameters();
            //Перебор всех TextBox формы
            foreach (var textBox in _formElements.Keys)
            {
                //Определение имени параметра по TextBox
                var parameterName = _formElements[textBox];
                //Окрашивание TextBox в зеленый цвет
                textBox.BackColor = Color.LightGreen;
                //Присваиваем текущее значение параметра в соответствующий TextBox
                textBox.Text =
                    string.Concat(_modelParameters.Parameter(parameterName).Value);
            }
            ThreadTypeСomboBox.Text = "Нет";
            //Отображение интервалов
            //
            DisplayInterval(ParametersName.CentralRingHeight, CentralHLabel);
            DisplayInterval(ParametersName.MiddleRingHeight, MiddleHLabel);
            DisplayInterval(ParametersName.OuterRingHeight, OuterHLabel);
        }

        /// <summary>
        /// Методя для создания нового
        /// экземпляра класса Manager
        /// при нажатии на кнопку "Создать"
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Действие</param>
        private void BuilderButton_Click(object sender, EventArgs e)
        {
            var manager = new Manager(_modelParameters);
        }

        /// <summary>
        /// Метод для изменения типа резьбы
        /// при изменения значения текста
        /// в элементе ComboBox
        /// </summary>
        /// <param name="sender">Объект</param>
        /// <param name="e">Действие</param>
        private void ThreadTypeСomboBox_TextChanged(object sender, EventArgs e)
        {
            if(ThreadTypeСomboBox.Text == "Нет")
            {
                _modelParameters.InstallThreadType(ThreadType.NoneThread);
            }
            if (ThreadTypeСomboBox.Text == "Метрическая")
            {
                _modelParameters.InstallThreadType(ThreadType.MetricThread);
            }
            if (ThreadTypeСomboBox.Text == "Упорная")
            {
                _modelParameters.InstallThreadType(ThreadType.ThrustThread);
            }
        }
    }
}
