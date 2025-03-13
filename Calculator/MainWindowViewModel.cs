using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Calculator
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private string _internalNumberString = "";
        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged(nameof(DisplayText));
                }
            }
        }

        private bool _isDigitGroupingEnabled = false;
        public bool IsDigitGroupingEnabled
        {
            get { return _isDigitGroupingEnabled; }
            set
            {
                if (_isDigitGroupingEnabled != value)
                {
                    _isDigitGroupingEnabled = value;
                    OnPropertyChanged(nameof(IsDigitGroupingEnabled));
                    Properties.Settings.Default.IsDigitGroupingEnabled = value;
                    Properties.Settings.Default.Save();
                    UpdateDisplayText();
                }
            }
        }

        private bool _isStandardMode;
        public bool IsStandardMode
        {
            get { return _isStandardMode; }
            set
            {
                if (_isStandardMode != value)
                {
                    _isStandardMode = value;
                    OnPropertyChanged(nameof(IsStandardMode));
                    OnPropertyChanged(nameof(IsProgrammerMode));
                    Properties.Settings.Default.StandardMode = value;
                    Properties.Settings.Default.Save();
                    UpdateVisibility();
                }
            }
        }

        private bool _isProgrammerMode = false;
        public bool IsProgrammerMode
        {
            get { return _isProgrammerMode; }
            set
            {
                if (_isProgrammerMode != value)
                {
                    _isProgrammerMode = value;
                    OnPropertyChanged(nameof(IsProgrammerMode));
                    OnPropertyChanged(nameof(IsStandardMode));
                    Properties.Settings.Default.StandardMode = !value;
                    Properties.Settings.Default.Save();
                    UpdateVisibility();
                }
            }
        }

        public string HexValue { get; private set; }
        public string DecValue { get; private set; }
        public string OctValue { get; private set; }
        public string BinValue { get; private set; }

        private int _selectedBase;
        public int SelectedBase
        {
            get { return _selectedBase; }
            set
            {
                if (_selectedBase != value)
                {
                    _selectedBase = value;
                    OnPropertyChanged(nameof(SelectedBase));
                    Properties.Settings.Default.SelectedBase = value;
                    Properties.Settings.Default.Save();
                    UpdateBaseDisplayText();
                }
            }
        }

        private string _baseDisplayText = string.Empty;
        public string BaseDisplayText
        {
            get { return _baseDisplayText; }
            set
            {
                if (_baseDisplayText != value)
                {
                    _baseDisplayText = value;
                    OnPropertyChanged(nameof(BaseDisplayText));
                }
            }
        }

        //private bool _isHexSelected;
        //public bool IsHexSelected
        //{
        //    get { return _isHexSelected; }
        //    set
        //    {
        //        if (_isHexSelected != value)
        //        {
        //            _isHexSelected = value;
        //            OnPropertyChanged(nameof(IsHexSelected));
        //            if (value) SelectedBase = 16; //Setam valoarea
        //            UpdateBaseDisplayText();

        //        }
        //    }
        //}

        //private bool _isDecSelected = true; //Initial selectat
        //public bool IsDecSelected
        //{
        //    get { return _isDecSelected; }
        //    set
        //    {
        //        if (_isDecSelected != value)
        //        {
        //            _isDecSelected = value;
        //            OnPropertyChanged(nameof(IsDecSelected));
        //            if (value) SelectedBase = 10; //Setam valoarea
        //            UpdateBaseDisplayText();
        //        }
        //    }
        //}

        //private bool _isOctSelected;
        //public bool IsOctSelected
        //{
        //    get { return _isOctSelected; }
        //    set
        //    {
        //        if (_isOctSelected != value)
        //        {
        //            _isOctSelected = value;
        //            OnPropertyChanged(nameof(IsOctSelected));
        //            if (value) SelectedBase = 8; //Setam valoarea
        //            UpdateBaseDisplayText();
        //        }
        //    }
        //}

        //private bool _isBinSelected;
        //public bool IsBinSelected
        //{
        //    get { return _isBinSelected; }
        //    set
        //    {
        //        if (_isBinSelected != value)
        //        {
        //            _isBinSelected = value;
        //            OnPropertyChanged(nameof(IsBinSelected));
        //            if (value) SelectedBase = 2; //Setam valoarea
        //            UpdateBaseDisplayText();
        //            UpdateEnabledButtons();
        //        }
        //    }
        //}

        private bool _isHexSelected;
        public bool IsHexSelected
        {
            get { return _isHexSelected; }
            set
            {
                if (_isHexSelected != value)
                {
                    _isHexSelected = value;
                    OnPropertyChanged(nameof(IsHexSelected));

                    if (value) //Daca este selectata hexa, celelalte trebuie sa fie false.
                    {
                        IsDecSelected = false;
                        OnPropertyChanged(nameof(IsDecSelected));
                        IsOctSelected = false;
                        OnPropertyChanged(nameof(IsOctSelected));
                        IsBinSelected = false;
                        OnPropertyChanged(nameof(IsBinSelected));
                        UpdateEnabledButtons();
                        UpdateBaseDisplayText(); // Re-afiseaza in baza selectata
                    }
                }
            }
        }

        private bool _isDecSelected = true; //Initial selectat
        public bool IsDecSelected
        {
            get { return _isDecSelected; }
            set
            {
                if (_isDecSelected != value)
                {
                    _isDecSelected = value;
                    OnPropertyChanged(nameof(IsDecSelected));
                    if (value)
                    {
                        IsHexSelected = false;
                        OnPropertyChanged(nameof(IsHexSelected));
                        IsOctSelected = false;
                        OnPropertyChanged(nameof(IsOctSelected));
                        IsBinSelected = false;
                        OnPropertyChanged(nameof(IsBinSelected));
                        UpdateEnabledButtons();
                        UpdateBaseDisplayText();
                    }
                }
            }
        }

        private bool _isOctSelected;
        public bool IsOctSelected
        {
            get { return _isOctSelected; }
            set
            {
                if (_isOctSelected != value)
                {
                    _isOctSelected = value;
                    OnPropertyChanged(nameof(IsOctSelected));
                    if (value)
                    {
                        IsHexSelected = false;
                        OnPropertyChanged(nameof(IsHexSelected));
                        IsDecSelected = false;
                        OnPropertyChanged(nameof(IsDecSelected));
                        IsBinSelected = false;
                        OnPropertyChanged(nameof(IsBinSelected));
                        UpdateEnabledButtons();
                        UpdateBaseDisplayText();
                    }
                }
            }
        }

        private bool _isBinSelected;
        public bool IsBinSelected
        {
            get { return _isBinSelected; }
            set
            {
                if (_isBinSelected != value)
                {
                    _isBinSelected = value;
                    OnPropertyChanged(nameof(IsBinSelected));
                    if (value)
                    {
                        IsHexSelected = false;
                        OnPropertyChanged(nameof(IsHexSelected));
                        IsDecSelected = false;
                        OnPropertyChanged(nameof(IsDecSelected));
                        IsOctSelected = false;
                        OnPropertyChanged(nameof(IsOctSelected));

                        UpdateEnabledButtons();
                        UpdateBaseDisplayText();
                    }
                }
            }
        }

        private Visibility _programmerModeVisibility = Visibility.Collapsed;
        public Visibility ProgrammerModeVisibility
        {
            get { return _programmerModeVisibility; }
            set
            {
                if (_programmerModeVisibility != value)
                {
                    _programmerModeVisibility = value;
                    OnPropertyChanged(nameof(ProgrammerModeVisibility));
                }
            }
        }

        public ObservableCollection<bool> IsDigitEnabled { get; private set; } = new ObservableCollection<bool>();
        public ObservableCollection<decimal> MemoryStack { get; private set; } = new ObservableCollection<decimal>();
        public ICommand NumberCommand { get; private set; }
        public ICommand OperatorCommand { get; private set; }
        public ICommand CalculateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand ClearEntryCommand { get; private set; }
        public ICommand BackspaceCommand { get; private set; }
        public ICommand NegateCommand { get; private set; }
        public ICommand DecimalCommand { get; private set; }
        public ICommand MemoryClearCommand { get; private set; }
        public ICommand MemoryRecallCommand { get; private set; }
        public ICommand MemoryAddCommand { get; private set; }
        public ICommand MemorySubtractCommand { get; private set; }
        public ICommand MemoryStoreCommand { get; private set; }
        public ICommand MemoryAddFromMemoryCommand { get; private set; }
        public ICommand MemorySubtractFromMemoryCommand { get; private set; }
        public ICommand MemoryRemoveCommand { get; private set; }
        public ICommand ShowMemoryStackCommand { get; private set; }
        public ICommand SetStandardModeCommand { get; private set; }
        public ICommand SetProgrammerModeCommand { get; private set; }
        public ICommand ProgrammerOperatorCommand { get; private set; }
        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }

        // Câmpuri private
        private decimal _firstOperand;
        private string _currentOperator = string.Empty;
        private bool _isNewNumber = true;
        private string _clipboardContent = string.Empty;
        private decimal _memoryValue; // Simplified single memory slot
        private bool _isOperationInCascade = false;

        // Constructor
        public MainWindowViewModel()
        {
            NumberCommand = new RelayCommand<string>(ExecuteNumberCommand);
            OperatorCommand = new RelayCommand<string>(ExecuteOperatorCommand);
            CalculateCommand = new RelayCommand<string>(ExecuteCalculateCommand);
            ClearCommand = new RelayCommand<string>(ExecuteClearCommand);
            ClearEntryCommand = new RelayCommand<string>(ExecuteClearEntryCommand);
            BackspaceCommand = new RelayCommand<string>(ExecuteBackspaceCommand);
            NegateCommand = new RelayCommand<string>(ExecuteNegateCommand);
            DecimalCommand = new RelayCommand<string>(ExecuteDecimalCommand);
            MemoryClearCommand = new RelayCommand<string>(ExecuteMemoryClearCommand);
            MemoryRecallCommand = new RelayCommand<string>(ExecuteMemoryRecallCommand);
            MemoryAddCommand = new RelayCommand<string>(ExecuteMemoryAddCommand);
            MemorySubtractCommand = new RelayCommand<string>(ExecuteMemorySubtractCommand);
            //MemoryAddFromMemoryCommand = new RelayCommand<object>(ExecuteMemoryAddFromMemoryCommand);
            //MemorySubtractFromMemoryCommand = new RelayCommand<object>(ExecuteMemorySubtractFromMemoryCommand);
            MemoryStoreCommand = new RelayCommand<string>(ExecuteMemoryStoreCommand);
            ShowMemoryStackCommand = new RelayCommand(ExecuteShowMemoryStackCommand);
            SetStandardModeCommand = new RelayCommand<string>(ExecuteSetStandardModeCommand);
            SetProgrammerModeCommand = new RelayCommand<string>(ExecuteSetProgrammerModeCommand);
            ProgrammerOperatorCommand = new RelayCommand<string>(ExecuteProgrammerOperatorCommand);
            CutCommand = new RelayCommand<string>(ExecuteCutCommand);
            CopyCommand = new RelayCommand<string>(ExecuteCopyCommand);
            PasteCommand = new RelayCommand<string>(ExecutePasteCommand);
            AboutCommand = new RelayCommand<string>(ExecuteAboutCommand);

            IsDigitGroupingEnabled = Properties.Settings.Default.IsDigitGroupingEnabled;
            IsStandardMode = Properties.Settings.Default.StandardMode;
            IsProgrammerMode = !IsStandardMode;
            SelectedBase = Properties.Settings.Default.SelectedBase;

            IsDecSelected = true; //Initial selectat
            UpdateBaseDisplayText();

            for(int i = 0; i < 16; i++)
            { 
                IsDigitEnabled.Add(false);
            }
            UpdateEnabledButtons();
        }

        // Implementare metode
        private void ExecuteNumberCommand(string number)
        {
            if (number != null)
            {
                // Dacă suntem la început, înlocuim _internalNumberString cu cifra introdusă,
                // altfel adăugăm cifra la sfârșit.
                if (_isNewNumber)
                {
                    _internalNumberString = number;
                    _isNewNumber = false;
                }
                else
                {
                    _internalNumberString += number;
                }
                // Actualizează DisplayText pe baza _internalNumberString fără separatorii de mii,
                // iar UpdateDisplayText se va ocupa de formatarea lor dacă e necesar.
                UpdateDisplayText();
            }
        }

        private void ExecuteOperatorCommand(object parameter)
        {
            string operatorSymbol = parameter as string;
            _internalNumberString = DisplayText;
            if (operatorSymbol != null)
            {
                if (!_isNewNumber && _isOperationInCascade)  // Calculate intermediate result for cascading operations
                {
                    ExecuteCalculateCommand(null);
                }

                if (decimal.TryParse(_internalNumberString, out decimal currentNumber))
                {
                    switch (operatorSymbol)
                    {
                        case "√":
                            if (decimal.TryParse(_internalNumberString, out decimal firstOperand))
                            {
                                if (firstOperand >= 0)
                                {
                                    decimal result = (decimal)Math.Sqrt((double)firstOperand);
                                    _internalNumberString = result.ToString();
                                    DisplayText = _internalNumberString;
                                    UpdateDisplayText();
                                    _isNewNumber = true;
                                    _isOperationInCascade = false;

                                }
                                else
                                {
                                    DisplayText = "Error: Invalid input for square root";
                                    return;
                                }
                            }
                            break;
                        case "x²":
                            if (decimal.TryParse(_internalNumberString, out decimal number))
                            {
                                decimal result = number * number;
                                _internalNumberString = result.ToString();
                                DisplayText = _internalNumberString;
                                UpdateDisplayText();
                                _isNewNumber = true;
                                _isOperationInCascade = false;

                            }
                            break;
                        case "1/x":
                            if (decimal.TryParse(_internalNumberString, out decimal divider))
                            {
                                if (divider != 0)
                                {
                                    decimal result = 1 / divider;
                                    _internalNumberString = result.ToString();
                                    DisplayText = _internalNumberString;
                                    UpdateDisplayText();
                                    _isNewNumber = true;
                                    _isOperationInCascade = false;

                                }
                                else
                                {
                                    DisplayText = "Error: Division by zero";
                                    return;
                                }
                            }
                            break;

                        case "%":

                            if (decimal.TryParse(_internalNumberString, out decimal percentNumber))
                            {
                                decimal result = percentNumber / 100;
                                _internalNumberString = result.ToString();
                                DisplayText = _internalNumberString;
                                UpdateDisplayText();
                                _isNewNumber = true;
                                _isOperationInCascade = false;

                            }
                            break;

                        default:
                            _firstOperand = currentNumber;
                            _currentOperator = operatorSymbol;
                            _isNewNumber = true;
                            _isOperationInCascade = true;
                            break;
                    }
                }


                //if (decimal.TryParse(DisplayText, out decimal currentNumber))
                //{
                //    _firstOperand = currentNumber;
                //    _currentOperator = operatorSymbol;
                //    _isNewNumber = true;
                //    _isOperationInCascade = true;
                //}
            }
        }

        private void ExecuteCalculateCommand(object parameter)
        {
            if (_currentOperator != null && !_isNewNumber)
            {
                if (!decimal.TryParse(DisplayText, out decimal secondOperand))
                {
                    DisplayText = "Error: Invalid Input";
                    return;
                }

                decimal result = 0;

                try
                {
                    switch (_currentOperator)
                    {
                        case "+":
                            result = _firstOperand + secondOperand;
                            break;
                        case "-":
                            result = _firstOperand - secondOperand;
                            break;
                        case "*":
                            result = _firstOperand * secondOperand;
                            break;
                        case "/":
                            if (secondOperand != 0)
                            {
                                result = _firstOperand / secondOperand;
                            }
                            else
                            {
                                DisplayText = "Error: Division by zero";
                                return;
                            }
                            break;

                    }
                }
                catch (Exception ex)
                {
                    DisplayText = "Error: Calculation error";
                    return;
                }

                DisplayText = result.ToString();
                _internalNumberString = DisplayText;
                UpdateDisplayText();
                _firstOperand = result;
                _currentOperator = null;
                _isNewNumber = true;
                _isOperationInCascade = false;
            }
        }

        private void ExecuteClearCommand(object parameter)
        {
            DisplayText = "0";
            _internalNumberString = DisplayText;
            _firstOperand = 0;
            _currentOperator = null;
            _isNewNumber = true;
            _isOperationInCascade = false;
            UpdateDisplayText();
        }

        private void ExecuteClearEntryCommand(object parameter)
        {
            DisplayText = "0";
            _isNewNumber = true;
            UpdateDisplayText();
        }

        private void ExecuteBackspaceCommand(object parameter)
        {
            if (!string.IsNullOrEmpty(_internalNumberString))
            {
                _internalNumberString = _internalNumberString.Length == 1 ? "0" : _internalNumberString.Substring(0, _internalNumberString.Length - 1);
                // Dacă s-a șters tot, considerăm că avem "0"
                if (string.IsNullOrEmpty(_internalNumberString))
                {
                    _internalNumberString = "0";
                }
                UpdateDisplayText();
            }
        }

        private void ExecuteNegateCommand(object parameter)
        {
            if (decimal.TryParse(DisplayText, out decimal number))
            {
                DisplayText = (-number).ToString();
                UpdateDisplayText();
            }
        }

        private void ExecuteDecimalCommand(object parameter)
        {
            if (!_internalNumberString.Contains("."))
            {
                _internalNumberString += ".";
                UpdateDisplayText();
            }
        }

        private void ExecuteMemoryClearCommand(object parameter)
        {
            _memoryValue = 0;
            MemoryStack.Clear();
        }

        private void ExecuteMemoryRecallCommand(object parameter)
        {
            DisplayText = _memoryValue.ToString();            
            //UpdateDisplayText();
            if (MemoryStack.Count > 0)
            {
                //DisplayText = MemoryStack[MemoryStack.Count - 1].ToString();
                UpdateDisplayText();
            }
        }

        private void ExecuteMemoryAddCommand(object parameter)
        {
            if (decimal.TryParse(DisplayText, out decimal number))
            {
                _memoryValue += number;
                MemoryStack.Add(number);
            }
        }

        private void ExecuteMemorySubtractCommand(object parameter)
        {
            if (decimal.TryParse(DisplayText, out decimal number))
            {
                _memoryValue -= number;
                MemoryStack.Add(-number);
            }
        }

        private void ExecuteMemoryStoreCommand(object parameter)
        {
            if (decimal.TryParse(DisplayText, out decimal number))
            {
                _memoryValue = number;
                MemoryStack.Add(number);
            }
        }

        private void ExecuteShowMemoryStackCommand()
        {
            // Afișează stiva de valori. Puteți afișa folosind un MessageBox sau deschideți un Popup personalizat.
            string message = "Memorie:\n";
            int index = 0;
            foreach (var val in MemoryStack)
            {
                message += $"{index++}: {val.ToString("N", CultureInfo.CurrentCulture)}\n";
            }
            MessageBox.Show(message, "Stiva de Memorie (M>)");

            // Dacă doriți ca utilizatorul să poată selecta o valoare, ar fi ideal să afișați un control (ex. ListBox) care permite
            // interacțiunea și apoi, în urma selecției, să setați DisplayText sau _internalNumberString cu valoarea aleasă.
        }


        private void ExecuteSetStandardModeCommand(object parameter)
        {
            IsStandardMode = true;
            IsProgrammerMode = false;
        }

        private void ExecuteSetProgrammerModeCommand(object parameter)
        {
            IsStandardMode = false;
            IsProgrammerMode = true;
        }

        private void ExecuteProgrammerOperatorCommand(object parameter)
        {
            string programmerOperator = parameter as string;

            if (programmerOperator != null)
            {
                long currentValue;

                if (!long.TryParse(DisplayText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out currentValue))
                {
                    DisplayText = "Error: Invalid Input";
                    return;
                }
                long result = currentValue;

                switch (programmerOperator)
                {
                    case "AND":
                        // TODO: Implement AND
                        break;
                    case "OR":
                        // TODO: Implement OR
                        break;
                    case "XOR":
                        // TODO: Implement XOR
                        break;
                    case "NOT":
                        // TODO: Implement NOT
                        break;
                }
                DisplayText = result.ToString("X");
                UpdateDisplayText();
            }
        }

        private void ExecuteCutCommand(object parameter)
        {
            _clipboardContent = DisplayText;
            DisplayText = "0";
            UpdateDisplayText();
        }

        private void ExecuteCopyCommand(object parameter)
        {
            _clipboardContent = DisplayText;
        }

        private void ExecutePasteCommand(object parameter)
        {
            DisplayText = _clipboardContent;
            UpdateDisplayText();
        }

        private void ExecuteAboutCommand(object parameter)
        {
            MessageBox.Show("Calculator WPF \nTișcă Laurențiu-Ștefan - 10LF333"); // Replace with your group information
        }
        // Helper methods
        private void UpdateDisplayText()
        {
            if (IsDigitGroupingEnabled)
            {
                // Folosim _internalNumberString, care nu conține separatorii de mii.
                if (decimal.TryParse(_internalNumberString,  out decimal number))
                {
                    CultureInfo culture = CultureInfo.CurrentCulture;
                    string formattedNumber = number.ToString("N0", culture); 
                    DisplayText = formattedNumber;
                }
                else
                {
                    // Dacă parsarea eșuează, afișăm șirul neformatat
                    DisplayText = _internalNumberString;
                }
            }
            else
            {
                // Când digit grouping nu e activ, afișează exact _internalNumberString
                DisplayText = _internalNumberString;
            }
            UpdateBaseDisplayText();
        }

        private void UpdateBaseDisplayText()
        {
            // Se presupune că _internalNumberString conține numărul "raw", fără formatare.
            if (decimal.TryParse(_internalNumberString, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out decimal number))
            {
                HexValue = Convert.ToString((long)number, 16).ToUpper();
                DecValue = number.ToString();
                OctValue = Convert.ToString((long)number, 8);
                BinValue = Convert.ToString((long)number, 2);

                OnPropertyChanged(nameof(HexValue));
                OnPropertyChanged(nameof(DecValue));
                OnPropertyChanged(nameof(OctValue));
                OnPropertyChanged(nameof(BinValue));
            }
        }

        //public Dictionary<int, bool> IsDigitEnabled { get; private set; } = new Dictionary<int, bool>(16);

        private void UpdateEnabledButtons()
        {
            DisplayText = "0";
            _internalNumberString = DisplayText;

            bool isHex = IsHexSelected;
            bool isOctal = IsOctSelected;
            bool isBinary = IsBinSelected;
            bool isDec = IsDecSelected; 
            if (isHex) 
                SelectedBase = 16;
            else if (isDec)
                SelectedBase = 10;
            else if (isOctal)
                SelectedBase = 8;
            else if (isBinary)
                SelectedBase = 2;
            Properties.Settings.Default.SelectedBase = SelectedBase;
            Properties.Settings.Default.Save();

            //Initialize IsDigitEnabled array to false
            for (int i = 0; i < 16; i++)
            {
                IsDigitEnabled[i] = false;
            }

            //Digit buttons
            IsDigitEnabled[0] = true; // 0 is always enabled

            if (isBinary)
            {
                IsDigitEnabled[1] = true;

            }
            else
            if (isOctal)
            {
                IsDigitEnabled[1] = true;
                IsDigitEnabled[2] = true;
                IsDigitEnabled[3] = true;
                IsDigitEnabled[4] = true;
                IsDigitEnabled[5] = true;
                IsDigitEnabled[6] = true;
                IsDigitEnabled[7] = true;
            }
            else
            if (isDec)
            {
                IsDigitEnabled[1] = true;
                IsDigitEnabled[2] = true;
                IsDigitEnabled[3] = true;
                IsDigitEnabled[4] = true;
                IsDigitEnabled[5] = true;
                IsDigitEnabled[6] = true;
                IsDigitEnabled[7] = true;
                IsDigitEnabled[8] = true;
                IsDigitEnabled[9] = true;
            }
            else
            if (isHex)
            {
                IsDigitEnabled[1] = true;
                IsDigitEnabled[2] = true;
                IsDigitEnabled[3] = true;
                IsDigitEnabled[4] = true;
                IsDigitEnabled[5] = true;
                IsDigitEnabled[6] = true;
                IsDigitEnabled[7] = true;
                IsDigitEnabled[8] = true;
                IsDigitEnabled[9] = true;
                IsDigitEnabled[10] = true;
                IsDigitEnabled[11] = true;
                IsDigitEnabled[12] = true;
                IsDigitEnabled[13] = true;
                IsDigitEnabled[14] = true;
                IsDigitEnabled[15] = true;
            }
            // Notify property changes for all digit enabled states
            for (int i = 0; i < 16; i++)
            {
                OnPropertyChanged($"IsDigitEnabled[{i}]");
            }

            //OnPropertyChanged(nameof(IsHexEnabled));

        }

        //private void UpdateEnabledButtons()
        //{
        //    bool isHex = IsHexSelected;
        //    bool isOctal = IsOctSelected;
        //    bool isBinary = IsBinSelected;
        //    bool isDecimal = IsDecSelected; // Check if Decimal base is selected

        //    //Initialize IsDigitEnabled array to false
        //    for (int i = 0; i <= 9; i++)
        //    {
        //        IsDigitEnabled[i] = false;
        //    }
        //    OnPropertyChanged("IsDigitEnabled");

        //    //Digit buttons
        //    IsDigitEnabled[0] = true; // 0 is always enabled

        //    if (isBinary)
        //    {
        //        IsDigitEnabled[1] = true;
        //        IsHexEnabled = false;
        //        OnPropertyChanged(nameof(IsHexEnabled));
        //    }

        //    else if (isOctal)
        //    {
        //        IsDigitEnabled[1] = true;
        //        IsDigitEnabled[2] = true;
        //        IsDigitEnabled[3] = true;
        //        IsDigitEnabled[4] = true;
        //        IsDigitEnabled[5] = true;
        //        IsDigitEnabled[6] = true;
        //        IsDigitEnabled[7] = true;
        //        IsHexEnabled = false;
        //        OnPropertyChanged(nameof(IsHexEnabled));
        //    }

        //    else if (isDecimal || isHex)
        //    {
        //        for (int i = 1; i < 10; i++)
        //            IsDigitEnabled[i] = true;
        //        IsHexEnabled = true;
        //        OnPropertyChanged(nameof(IsHexEnabled));
        //    }

        //    // Notify property changes for all digit enabled states
        //    for (int i = 0; i <= 9; i++)
        //    {
        //        OnPropertyChanged($"IsDigitEnabled[{i}]");
        //    }

        //    IsHexEnabled = isHex;
        //    OnPropertyChanged(nameof(IsHexEnabled));
        //}

        private void UpdateVisibility()
        {
            ProgrammerModeVisibility = IsProgrammerMode ? Visibility.Visible : Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}