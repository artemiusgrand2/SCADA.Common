using System;
namespace SCADA.Common.LogicalParse
{
	/// <summary>
	/// вычисляет логические простейшие операции
	/// </summary>
	public static class LogicalCalculator
	{
		/// <summary>
		/// операция И
		/// </summary>
		/// <param name="first">
		/// A <see cref="System.Boolean"/>
		/// первое значение
		/// </param>
		/// <param name="second">
		/// A <see cref="System.Boolean"/>
		/// второе значение
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// результат
		/// </returns>
		public static InfixNotation.infix_states And(InfixNotation.infix_states first, InfixNotation.infix_states second)
		{
			InfixNotation.infix_states res_state = InfixNotation.infix_states.UncontrolledState;
            if (first == InfixNotation.infix_states.UncontrolledState || second == InfixNotation.infix_states.UncontrolledState)
                return res_state;
            else
            {
                if ((first == InfixNotation.infix_states.ActiveState) && (second == InfixNotation.infix_states.ActiveState))
                    res_state = InfixNotation.infix_states.ActiveState;
                else res_state = InfixNotation.infix_states.PassiveState;
            }
			return res_state;
		}
		
		/// <summary>
		/// операция ИЛИ
		/// </summary>
		/// <param name="first">
		/// A <see cref="System.Boolean"/>
		/// первое значение
		/// </param>
		/// <param name="second">
		/// A <see cref="System.Boolean"/>
		/// второе значение
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// результат
		/// </returns>
		public static InfixNotation.infix_states Or(InfixNotation.infix_states first, InfixNotation.infix_states second)
		{
            if (first == InfixNotation.infix_states.UncontrolledState && second == InfixNotation.infix_states.UncontrolledState)
                return InfixNotation.infix_states.UncontrolledState;
            else
            {
                if (first == InfixNotation.infix_states.UncontrolledState || second == InfixNotation.infix_states.UncontrolledState)
                {
                    if (first == InfixNotation.infix_states.UncontrolledState)
                        return second;
                    else return first;
                }
                else
                {
                     if ((first == InfixNotation.infix_states.ActiveState) || (second == InfixNotation.infix_states.ActiveState))
                         return InfixNotation.infix_states.ActiveState;
                     else
                         return InfixNotation.infix_states.PassiveState;
                }
            }
		}
		
		/// <summary>
		/// операция отрицания
		/// </summary>
		/// <param name="operand">
		/// A <see cref="System.Boolean"/>
		/// входное значение
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// результат
		/// </returns>
		public static InfixNotation.infix_states Not(InfixNotation.infix_states operand)
		{
			InfixNotation.infix_states res = InfixNotation.infix_states.UncontrolledState;
            if (operand == InfixNotation.infix_states.UncontrolledState)
                return res;
            else
            {
                if (operand == InfixNotation.infix_states.PassiveState)
                    res = InfixNotation.infix_states.ActiveState;
                else res = InfixNotation.infix_states.PassiveState;
            }
			return res;
		}
		
		public static InfixNotation.infix_states Uncontrolled(InfixNotation.infix_states operand)
		{
            if (operand == InfixNotation.infix_states.ActiveState || operand == InfixNotation.infix_states.PassiveState)
                return InfixNotation.infix_states.ActiveState;
            else return InfixNotation.infix_states.UncontrolledState;
		}
	}
}

