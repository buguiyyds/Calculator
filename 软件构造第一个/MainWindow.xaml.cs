using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SimpleCalculator.Services;

namespace SimpleCalculator
{
    public partial class MainWindow : Window
    {
        private readonly CalculatorEngine _calculatorEngine = new();

        private double? _firstNumber = null;
        private string? _currentOperator = null;
        private bool _isNewInput = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                AppendDigit(button.Content.ToString()!);
            }
        }

        private void DecimalButton_Click(object sender, RoutedEventArgs e)
        {
            AppendDecimalPoint();
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SetOperator(button.Content.ToString()!);
            }
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateResult();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void AppendDigit(string digit)
        {
            if (Display.Text == "0" || _isNewInput)
            {
                Display.Text = digit;
                _isNewInput = false;
            }
            else
            {
                Display.Text += digit;
            }
        }

        private void AppendDecimalPoint()
        {
            if (_isNewInput)
            {
                Display.Text = "0.";
                _isNewInput = false;
                return;
            }

            if (!Display.Text.Contains("."))
            {
                Display.Text += ".";
            }
        }

        private void SetOperator(string operation)
        {
            if (double.TryParse(Display.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double currentValue))
            {
                if (_firstNumber.HasValue && !_isNewInput && _currentOperator != null)
                {
                    try
                    {
                        double result = _calculatorEngine.Calculate(_firstNumber.Value, currentValue, _currentOperator);
                        Display.Text = result.ToString(CultureInfo.InvariantCulture);
                        _firstNumber = result;
                    }
                    catch (DivideByZeroException)
                    {
                        MessageBox.Show("Cannot divide by zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        ClearAll();
                        return;
                    }
                }
                else
                {
                    _firstNumber = currentValue;
                }

                _currentOperator = operation;
                _isNewInput = true;
            }
        }

        private void CalculateResult()
        {
            if (!_firstNumber.HasValue || string.IsNullOrEmpty(_currentOperator))
            {
                return;
            }

            if (!double.TryParse(Display.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double secondNumber))
            {
                MessageBox.Show("Invalid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                double result = _calculatorEngine.Calculate(_firstNumber.Value, secondNumber, _currentOperator);
                Display.Text = result.ToString(CultureInfo.InvariantCulture);

                _firstNumber = result;
                _currentOperator = null;
                _isNewInput = true;
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Cannot divide by zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ClearAll();
            }
        }

        private void ClearAll()
        {
            Display.Text = "0";
            _firstNumber = null;
            _currentOperator = null;
            _isNewInput = false;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Numbers from main keyboard
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                string digit = (e.Key - Key.D0).ToString();
                AppendDigit(digit);
                return;
            }

            // Numbers from numpad
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                string digit = (e.Key - Key.NumPad0).ToString();
                AppendDigit(digit);
                return;
            }

            switch (e.Key)
            {
                case Key.Add:
                    SetOperator("+");
                    break;
                case Key.Subtract:
                    SetOperator("-");
                    break;
                case Key.Multiply:
                    SetOperator("*");
                    break;
                case Key.Divide:
                    SetOperator("/");
                    break;
                case Key.OemPlus:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        SetOperator("+");
                    else
                        CalculateResult();
                    break;
                case Key.OemMinus:
                    SetOperator("-");
                    break;
                case Key.Oem2:
                    SetOperator("/");
                    break;
                case Key.D8:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        SetOperator("*");
                    break;
                case Key.Decimal:
                case Key.OemPeriod:
                    AppendDecimalPoint();
                    break;
                case Key.Enter:
                    CalculateResult();
                    break;
                case Key.Escape:
                    ClearAll();
                    break;
            }
        }
    }
}