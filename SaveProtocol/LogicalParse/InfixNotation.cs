using System;
using System.Collections.Generic;

namespace SCADA.Common.LogicalParse
{
    /// <summary>
    /// класс инфиксной нотации
    /// </summary>
    public class InfixNotation
    {
        /// <summary>
        /// входная строка для разбора
        /// </summary>
        private string _input = "";

        /// <summary>
        /// набор входных объектов
        /// </summary>
        private List<string> _inputList = new List<string>();
        /// <summary>
        /// исходный набор объектов польской нотации, вычисляемый при подготовке польской записи
        /// </summary>
        private List<string> _polishList = new List<string>();
        /// <summary>
        /// рабочий набор объектов польской нотации
        /// </summary>
        private List<string> _polishListWorkCopy = new List<string>();
        /// <summary>
        /// индексы операций в польской записи
        /// </summary>
        private List<int> _operationIndexes = new List<int>();
        /// <summary>
        /// стек для промежуточного хранения при подготовке польской записи
        /// </summary>
        private Stack<string> _stack = new Stack<string>();
        /// <summary>
        /// значения импульсов
        /// </summary>
        public Dictionary<string, infix_states> _impulsesValues = new Dictionary<string, infix_states>();

        /// <summary>
        /// имена используемых импульсов
        /// </summary>
        public List<string> _impulsesNames = new List<string>();

        /// <summary>
        /// логическое и
        /// </summary>
        public  const string _and = "*";
        /// <summary>
        /// логическое или
        /// </summary>
        public const string _or = "+";
        /// <summary>
        /// логическое отрицание
        /// </summary>
        public const string _not = "!";
        /// <summary>
        /// левая скобка
        /// </summary>
        public  const string _lParen = "(";
        /// <summary>
        /// правая скобка
        /// </summary>
        public  const string _rParen = ")";
        /// <summary>
        /// Неопределенный импульс
        /// </summary>
        public  const string _uncontr = "~";

        public enum infix_states
        {
            /// <summary>
            /// Импульс и поле контроля имеют пассивное состояние
            /// </summary>
            PassiveState = 0,
            /// <summary>
            /// Импульс и поле контроля имеют активное состояние
            /// </summary>
            ActiveState = 1,
            /// <summary>
            /// Импульс не приходит, поле контроля не определено
            /// </summary>
            UncontrolledState = 2

        }


        // смещение индекса при И или ИЛИ
        int _and_or_offset = 2;
        // смещение индекса при отрицании
        int _not_offset = 1;
        //смещение индекса при неонтролируемом импульсе
        int _unc_offset = 1;



        /// <summary>
        /// состояние вычисления:
        /// 0 - создан объект
        /// 1 - строка разбита на составляющие идентификаторы и операторы,
        /// создана польская запись
        /// </summary>
        private int _state = 0;


        /// <summary>
        /// Конструктор, инициализирующий входные данные для нотации
        /// </summary>
        /// <param name="input">
        /// A <see cref="System.String"/>
        /// </param>
        public InfixNotation(string input)
        {
            _input = input;
            Prepare();
            _polishListWorkCopy = getPolishListCopy();
        }



        /// <summary>
        /// разбирает входные данные для дальнейшей обработки
        /// </summary>
        private void Prepare()
        {
            // входная строка посимвольно
            char[] mas = _input.ToCharArray();
            // позиция начала идентификатора
            int firstIndexOfID = -2;
            // был ли оператор перед левой скобкой
            bool op_before = false;
            _inputList = new List<string>();

            // чистим входную строку
            for (int i = 0; i < mas.Length; i++)
            {
                switch (mas[i])
                {
                    case ' ':
                        addID(ref firstIndexOfID, i);
                        break;
                    case '*':
                        addID(ref firstIndexOfID, i);
                        _inputList.Add(_and);
                        op_before = true;
                        break;
                    case '+':
                        addID(ref firstIndexOfID, i);
                        _inputList.Add(_or);
                        op_before = true;
                        break;
                    case '!':
                        addID(ref firstIndexOfID, i);
                        _inputList.Add(_not);
                        op_before = true;
                        break;
                    case '(':
                        addID(ref firstIndexOfID, i);
                        //if (op_before == false && i != 0)
                        //    _inputList.Add(_and);
                        _inputList.Add(_lParen);
                        break;
                    case ')':
                        addID(ref firstIndexOfID, i);
                        _inputList.Add(_rParen);
                        break;
                    case '~':
                        addID(ref firstIndexOfID, i);
                        _inputList.Add(_uncontr);
                        op_before = true;
                        break;
                    // это переменная
                    default:
                        if (firstIndexOfID < 0)
                            firstIndexOfID = i;
                        op_before = false;
                        break;
                }
            }

            // добавляем последний идентификатор
            if (firstIndexOfID >= 0)
                addID(ref firstIndexOfID, mas.Length);
            computePolishNotation();
            _state = 1;
        }

        /// <summary>
        /// добавляет идентификатор в выходные данные
        /// </summary>
        /// <param name="begin">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="end">
        /// A <see cref="System.Int32"/>
        /// </param>
        private void addID(ref int begin, int end)
        {
            // название идентификатора
            string id = "";
            // длина выделения
            int res = end - begin;
            // выделить нечего
            if (res < 1 || begin < 0)
            {
                // делаем begin < 0 , чтоб показать, что процес получения ID надо начать сначала
                begin = -2;
                return;
            }

            id = _input.Substring(begin, res);
            _inputList.Add(id);
            // подготавливаем хранилище для значения импульса
            if (!_impulsesValues.ContainsKey(id))
            {
                _impulsesValues.Add(id, infix_states.UncontrolledState);
                _impulsesNames.Add(id);
            }

            // делаем begin < 0 , чтоб показать, что процес получения ID надо начать сначала
            begin = -2;
        }

        /// <summary>
        /// получает польскую нотацию
        /// </summary>
        private void computePolishNotation()
        {
            // верхнее значение в стеке
            string topValue = "";
            _operationIndexes = new List<int>();
            _polishList = new List<string>();
            _stack.Clear();

            // проходим по входным объектам для их преобразования в польскую нотацию
            foreach (string input in _inputList)
            {
                switch (input)
                {
                    // и и или имеют одинаковый приоритет, а таже являются левоассоциативными,
                    // поэтому из стека они могут вывести в выходную строку лишь и или или
                    case _and:
                        while (_stack.Count != 0 && _stack.Peek() != _rParen && _stack.Peek() != _lParen && _stack.Peek() != _or)
                            addFromStack();
                        _stack.Push(input);
                        break;
                    case _or:
                        while (_stack.Count != 0 && _stack.Peek() != _rParen && _stack.Peek() != _lParen)
                            addFromStack();
                        _stack.Push(input);
                        break;
                    // открывающуюся скобку загоняем в стек
                    case _lParen:
                        _stack.Push(_lParen);
                        break;
                    // при закрывающеся скобке необходимо проанализировать стек
                    case _rParen:
                        try
                        {
                            // встретили открывающуюся скобку в стеке
                            bool findLParen = false;

                            // проводим разбор, пока не найдём открывающуюся скобку
                            while (!findLParen)
                            {
                                topValue = addFromStack();
                                // если нашли открывающуюмся скобку, то эту часть разбора завершаем
                                if (topValue == _lParen)
                                    findLParen = true;
                                // иначе добавляем элемент стека в польскую запись
                                //else
                                //	_polishList.Add(topValue);
                            }
                        }
                        catch (Exception)
                        {
                            throw new Exception(String.Format("В анализируемой строке либо не согласованы скобки, либо неверно поставлен разделитель.{0}Входная строка:{1}",
                                                          Environment.NewLine, _input));
                        }
                        break;
                    // так так оператор отрицания правоассоциированный и обладает высшим приоритетом
                    // то просто пихаем его в стек
                    case _not:
                        _stack.Push(input);
                        break;
                    case _uncontr:
                        _stack.Push(input);
                        break;
                    // это название импульса, его добавляем к выходной строке
                    default:
                        _polishList.Add(input);
                        break;
                }
            }

            // входная строка закончилась - необходимо вытолкнуть все элементы из стека в выходную строку
            while (_stack.Count != 0)
            {
                topValue = addFromStack();
                // в стеке не должны были остаться скобки
                if (topValue == _rParen || topValue == _lParen)
                    throw new Exception(String.Format("В анализируемой строке не согласованы скобки.{0}Входная строка:{1}",
                                                          Environment.NewLine, _input));
            }
        }

        /// <summary>
        /// добавление в польскую запись значение с верхушки стека
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// значение с верхушки стека
        /// </returns>
        private string addFromStack()
        {
            // значение с верхушки стека
            string val = _stack.Pop();
            // если это оператор, то запоминаем его индекс для дальнейшего удобства
            if (val == _and || val == _or || val == _not || val == _uncontr)
                _operationIndexes.Add(_polishList.Count);
            // скобки в польскую записб не заносятся
            if (val != _rParen && val != _lParen)
                _polishList.Add(val);

            return val;
        }

        /// <summary>
        /// вычисляет результат выражения
        /// </summary>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// результат выражения
        /// </returns>
        public infix_states Compute()
        {
            // смещение индекса
            int offset = 0;
            // оператор
            string op = "";
            // первый операнд
            infix_states first = infix_states.PassiveState;
            // второй операнд
            infix_states second = infix_states.PassiveState;
            // результат операции
            infix_states rez = infix_states.PassiveState;
            int new_index = 0;

            // результатом является просто значение импульса
            if (_operationIndexes.Count == 0 && _polishListWorkCopy.Count == 1)
                return _impulsesValues[_polishListWorkCopy[0]];

            // просматриваем операторы в польской записи
            foreach (int index in _operationIndexes)
            {
                // вычисление нового индекса согласно смещению
                new_index = index - offset;
                op = _polishListWorkCopy[new_index];

                /// выполнение указанных операций и занесение результатов в польскую запись
                ///
                switch (op)
                {
                    case _and:
                        try
                        {
                            first = _impulsesValues[_polishListWorkCopy[new_index - 1]];
                            second = _impulsesValues[_polishListWorkCopy[new_index - 2]];
                            rez = LogicalCalculator.And(first, second);
                            insertValue(new_index, _and_or_offset, rez);
                            // изменяем смещение
                            offset = offset + _and_or_offset;
                        }
                        catch (Exception)
                        {
                            throw new Exception(String.Format("Ошибка употребления оператора {0} входная строка {1} .", _and, _input));
                        }
                        break;
                    case _or:
                        try
                        {
                            first = _impulsesValues[_polishListWorkCopy[new_index - 1]];
                            second = _impulsesValues[_polishListWorkCopy[new_index - 2]];
                            rez = LogicalCalculator.Or(first, second);
                            insertValue(new_index, _and_or_offset, rez);
                            // изменяем смещение
                            offset = offset + _and_or_offset;
                        }
                        catch (Exception)
                        {
                            throw new Exception(String.Format("Ошибка употребления оператора {0} входная строка {1} .", _or, _input));
                        }
                        break;
                    case _not:
                        try
                        {
                            first = _impulsesValues[_polishListWorkCopy[new_index - 1]];
                            rez = LogicalCalculator.Not(first);
                            insertValue(new_index, _not_offset, rez);
                            // изменяем смещение
                            offset = offset + _not_offset;
                        }
                        catch (Exception)
                        {
                            throw new Exception(String.Format("Ошибка употребления оператора {0} входная строка {1} .", _not, _input));
                        }
                        break;
                    case _uncontr:
                        try
                        {
                            first = _impulsesValues[_polishListWorkCopy[new_index - 1]];
                            rez = LogicalCalculator.Uncontrolled(first);
                            insertValue(new_index, _unc_offset, rez);
                            // изменяем смещение
                            offset = offset + _unc_offset;
                        }
                        catch
                        {
                            throw new Exception(String.Format("Ошибка употребления оператора {0} входная строка {1} .", _not, _input));
                        }
                        break;
                    default:
                        throw new Exception(String.Format("В польской записи обнаружен неизвестный оператор:{0}. Входная строка {1}", op, _input));
                }
            }

            if (_polishListWorkCopy.Count != 1)
                throw new Exception(String.Format("Ошибка во входных данных. Входная строка {0}", _input));

            _state = 3;
            return rez;
        }

        public bool translate_res_to_bool()
        {
            infix_states state = Compute();
            if (state == InfixNotation.infix_states.ActiveState)
                return true;
            else return false;
        }

        /// <summary>
        /// Загружает в память значения импульсов, необходимых для вычисления результата
        /// </summary>
        public void LoadImpulsesValues()
        {
            if (_state < 1)
                throw new Exception("Необходимо вызвать Prepare");

            ////foreach(string name in _impulsesNames)
            ////    _impulsesValues[name] = _impulsesInformator.GetState(name);

            _polishListWorkCopy = getPolishListCopy();

            _state = 2;

        }
        /// <summary>
        /// возвращает копию объектов польской нотации
        /// </summary>
        /// <returns>
        /// A <see cref="List<System.String>"/>
        /// копия объектов польской нотации
        /// </returns>
        private List<string> getPolishListCopy()
        {
            List<string> copy = new List<string>();
            foreach (string ob in _polishList)
                copy.Add(ob);
            return copy;
        }

        /// <summary>
        /// делает вставку результата в польскую запись
        /// </summary>
        /// <param name="operatorIndex">
        /// A <see cref="System.Int32"/>
        /// индекс оператора в польской записи
        /// </param>
        /// <param name="offset">
        /// A <see cref="System.Int32"/>
        /// смещение согласно типу оператора
        /// </param>
        /// <param name="operationValue">
        /// A <see cref="System.Boolean"/>
        /// новое значение
        /// </param>
        private void insertValue(int operatorIndex, int offset, infix_states operationValue)
        {
            // тип оператора И
            if (offset == _and_or_offset)
            {
                _polishListWorkCopy.RemoveAt(operatorIndex);
                _polishListWorkCopy.RemoveAt(operatorIndex - 1);
                updatePilishList(operatorIndex - 2, operationValue);
            }
            else
            {
                // тип оператора ИЛИ
                if (offset == _not_offset)
                {
                    _polishListWorkCopy.RemoveAt(operatorIndex);
                    updatePilishList(operatorIndex - 1, operationValue);
                }
                else
                    throw new Exception(String.Format("Подано неизвестное смещение. Входная строка {0}", _input));
            }

        }

        /// <summary>
        /// обновляет значение для польской записи. При этом создаёт временные результаты и хранит их вместе со значениями импульсов
        /// </summary>
        /// <param name="index">
        /// A <see cref="System.Int32"/>
        /// куда в польской записи вставить значение
        /// </param>
        /// <param name="new_value">
        /// A <see cref="System.Boolean"/>
        /// новое значение
        /// </param>
        private void updatePilishList(int index, infix_states new_value)
        {
            // название временного результата
            string name = " " + index;

            // обновляем
            if (_impulsesValues.ContainsKey(name))
                _impulsesValues[name] = new_value;
            // иначе создаём
            else
                _impulsesValues.Add(name, new_value);

            // корректируем имя
            _polishListWorkCopy[index] = name;
        }
    }
}

