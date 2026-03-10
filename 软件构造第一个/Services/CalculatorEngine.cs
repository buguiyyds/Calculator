using System;

namespace SimpleCalculator.Services
{
    public class CalculatorEngine
    {
        public double Calculate(double firstNumber, double secondNumber, string operation)
        {
            return operation switch
            {
                "+" => firstNumber + secondNumber,
                "-" => firstNumber - secondNumber,
                "*" => firstNumber * secondNumber,
                "/" => secondNumber == 0
                    ? throw new DivideByZeroException("Cannot divide by zero.")
                    : firstNumber / secondNumber,
                _ => throw new InvalidOperationException("Unknown operation.")
            };
        }
    }
}